using Models;
using Services;

namespace Input;

public static class ScenarioRunner
{
    public static Payroll Calculate(ScenarioData data)
    {
        ScenarioFile.Validate(data);
        var trips = data.Trips.Select(t =>
        {
            var trip = TripService.Create(t.Id, t.DriverId, t.DistanceKm, t.IsNight);
            if (t.IsCompleted) TripService.Complete(trip, t.CompletedAt!.Value);
            if (t.IsConfirmed) TripService.Confirm(trip);
            return trip;
        }).ToArray();

        var history = data.Tariffs.Select(t => new TariffVersion {
            DriverId = t.DriverId, EffectiveFrom = t.EffectiveFrom,
            Tariff = new DriverTariff { PerTrip = t.PerTrip, PerKm = t.PerKm, NightBonusPercent = t.NightBonusPercent }
        });
        return SalaryCalculator.CalculateSalary(data.PayrollId, data.DriverId,
            new PayrollPeriod { From = data.From, ToExclusive = data.ToExclusive }, trips, history);

    }
}
