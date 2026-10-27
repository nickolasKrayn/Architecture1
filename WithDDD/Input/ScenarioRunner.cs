using Domain;
using Application;
using Infrastructure;

namespace Input;

public static class ScenarioRunner
{
    public static Payroll Calculate(ScenarioData data)
    {
        ScenarioFile.Validate(data);
        var trips = data.Trips.Select(t =>
        {
            var trip = new Trip(t.Id, t.DriverId, t.DistanceKm, t.IsNight);
            if (t.IsCompleted) trip.Complete(t.CompletedAt!.Value);
            if (t.IsConfirmed) trip.Confirm();
            return trip;
        }).ToArray();

        var history = new InMemoryTariffHistory(data.Tariffs.Select(t =>
            new TariffVersion(t.DriverId, t.EffectiveFrom,
                new DriverTariff(t.PerTrip, t.PerKm, t.NightBonusPercent))));
        return new PayrollService(new InMemoryConfirmedTrips(trips), history, new TripPaymentPolicy())
            .CalculateSalary(data.PayrollId, data.DriverId, new PayrollPeriod(data.From, data.ToExclusive));

    }
}
