using System;

namespace Domain;

public sealed class Trip
{
    public int Id { get; }
    public int DriverId { get; }
    public decimal DistanceKm { get; private set; }
    public bool IsNight { get; }
    public bool IsCompleted { get; private set; }
    public bool IsConfirmed { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }

    public Trip(int id, int driverId,
        decimal distanceKm, bool isNight)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
        if (driverId <= 0) throw new ArgumentOutOfRangeException(nameof(driverId));
        Id = id;
        DriverId = driverId;
        IsNight = isNight;
        SetDistance(distanceKm);
    }

    public void Complete()
        => Complete(DateTimeOffset.UtcNow);

    public void Complete(DateTimeOffset completedAt)
    {
        if (IsCompleted) return;
        CompletedAt = completedAt;
        IsCompleted = true;
    }

    public void Confirm()
    {
        if (!IsCompleted)
            throw new InvalidOperationException(
                "Trip must be completed.");

        IsConfirmed = true;
    }

    public void SetDistance(decimal km)
    {
        if (IsConfirmed)
            throw new InvalidOperationException(
                "Confirmed trip is locked.");

        if (km < 0)
            throw new ArgumentOutOfRangeException(
                nameof(km));

        DistanceKm = km;
    }
}
