using Domain;

namespace Application;

public interface IConfirmedTrips
{
    IReadOnlyList<Trip> GetConfirmed(int driverId, PayrollPeriod period);
}

public interface ITariffHistory
{
    DriverTariff GetAt(int driverId, DateTimeOffset completedAt);
}

// The caller supplies an already allocated, unique payroll identifier.
// Loading and saving are application concerns; approval is a later action.
public sealed class PayrollService
{
    private readonly IConfirmedTrips trips;
    private readonly ITariffHistory tariffs;
    private readonly TripPaymentPolicy paymentPolicy;

    public PayrollService(IConfirmedTrips trips, ITariffHistory tariffs,
        TripPaymentPolicy paymentPolicy)
    {
        this.trips = trips ?? throw new ArgumentNullException(nameof(trips));
        this.tariffs = tariffs ?? throw new ArgumentNullException(nameof(tariffs));
        this.paymentPolicy = paymentPolicy
            ?? throw new ArgumentNullException(nameof(paymentPolicy));
    }

    public Payroll CalculateSalary(
        int payrollId, int driverId, PayrollPeriod period)
    {
        ArgumentNullException.ThrowIfNull(period);
        var payroll = new Payroll(payrollId, driverId);
        var confirmedTrips = trips.GetConfirmed(driverId, period);

        foreach (var trip in confirmedTrips)
        {
            var completedAt = trip.CompletedAt
                ?? throw new InvalidOperationException("Completion time is missing.");

            if (trip.DriverId != driverId || !trip.IsConfirmed ||
                !period.Contains(completedAt))
                throw new InvalidOperationException("The trip query returned invalid data.");

            var tariff = tariffs.GetAt(driverId, completedAt);
            var amount = paymentPolicy.Calculate(trip, tariff);
            payroll.AddEntry(trip.Id, amount);
        }

        return payroll;
    }
}
