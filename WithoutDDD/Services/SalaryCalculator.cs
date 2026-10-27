using Models;

namespace Services;

// Transaction Script: selection, historical tariff lookup and orchestration
// live in a procedural service. Data objects do not protect themselves.
public static class SalaryCalculator
{
    public static Payroll CalculateSalary(int payrollId, int driverId, PayrollPeriod period,
        IEnumerable<Trip> trips, IEnumerable<TariffVersion> tariffHistory)
    {
        ArgumentNullException.ThrowIfNull(period);
        if (period.ToExclusive <= period.From)
            throw new ArgumentException("Period end must be after its start.");
        var versions = tariffHistory.ToArray();
        if (versions.Any(v => v.Tariff is null))
            throw new ArgumentException("A tariff version cannot be null.");
        if (versions.GroupBy(v => (v.DriverId, v.EffectiveFrom)).Any(g => g.Count() > 1))
            throw new ArgumentException("Ambiguous tariff effective date.");
        var payroll = PayrollService.Create(payrollId, driverId);
        var eligible = trips.Where(t => t.DriverId == driverId && t.IsConfirmed &&
                t.CompletedAt is { } at && period.From <= at && at < period.ToExclusive)
            .OrderBy(t => t.CompletedAt).ThenBy(t => t.Id);
        foreach (var trip in eligible)
        {
            var tariff = versions.Where(v => v.DriverId == driverId && v.EffectiveFrom <= trip.CompletedAt)
                .OrderByDescending(v => v.EffectiveFrom).Select(v => v.Tariff).FirstOrDefault()
                ?? throw new InvalidOperationException("No tariff for this completion time.");
            PayrollService.AddEntry(payroll, trip.Id, PaymentCalculator.Calculate(trip, tariff));
        }
        return payroll;
    }
}
