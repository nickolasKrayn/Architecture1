using Models;

namespace Services;

public static class TripService
{
    public static Trip Create(int id, int driverId, decimal km, bool isNight)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
        if (driverId <= 0) throw new ArgumentOutOfRangeException(nameof(driverId));
        var trip = new Trip { Id = id, DriverId = driverId, IsNight = isNight };
        SetDistance(trip, km);
        return trip;
    }

    public static void SetDistance(Trip trip, decimal km)
    {
        if (trip.IsConfirmed) throw new InvalidOperationException("Confirmed trip is locked.");
        if (km < 0) throw new ArgumentOutOfRangeException(nameof(km));
        trip.DistanceKm = km;
    }

    public static void Complete(Trip trip, DateTimeOffset at)
    {
        if (trip.IsCompleted) return;
        trip.CompletedAt = at;
        trip.IsCompleted = true;
    }

    public static void Confirm(Trip trip)
    {
        if (!trip.IsCompleted) throw new InvalidOperationException("Trip must be completed.");
        trip.IsConfirmed = true;
    }
}
