using System;

namespace Domain;

public sealed class TripPaymentPolicy
{
    public decimal Calculate(
        Trip trip, DriverTariff tariff)
    {
        ArgumentNullException.ThrowIfNull(trip);
        ArgumentNullException.ThrowIfNull(tariff);

        if (!trip.IsConfirmed)
            throw new InvalidOperationException(
                "Trip must be confirmed.");

        decimal payment = tariff.PerTrip
            + trip.DistanceKm * tariff.PerKm;

        if (trip.IsNight)
            payment *= 1m
                + tariff.NightBonusPercent / 100m;

        return payment;
    }
}
