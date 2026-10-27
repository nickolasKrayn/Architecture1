using Models;

namespace Services;

public static class PaymentCalculator
{
    public static decimal Calculate(Trip trip, DriverTariff tariff)
    {
        if (!trip.IsConfirmed || !trip.IsCompleted)
            throw new InvalidOperationException("Trip must be confirmed.");
        ValidateTariff(tariff);
        if (trip.DistanceKm < 0) throw new ArgumentOutOfRangeException(nameof(trip));
        decimal amount = tariff.PerTrip + trip.DistanceKm * tariff.PerKm;
        if (trip.IsNight) amount *= 1m + tariff.NightBonusPercent / 100m;
        return amount;
    }

    public static decimal CalculateHourly(decimal hours, decimal hourlyRate)
    {
        if (hours < 0) throw new ArgumentOutOfRangeException(nameof(hours));
        if (hourlyRate < 0) throw new ArgumentOutOfRangeException(nameof(hourlyRate));
        return hours * hourlyRate;
    }

    public static void ValidateTariff(DriverTariff tariff)
    {
        ArgumentNullException.ThrowIfNull(tariff);
        if (tariff.PerTrip < 0 || tariff.PerKm < 0 || tariff.NightBonusPercent < 0)
            throw new ArgumentOutOfRangeException(nameof(tariff));
    }
}
