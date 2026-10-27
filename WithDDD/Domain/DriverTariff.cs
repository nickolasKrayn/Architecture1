using System;

namespace Domain;

public sealed record DriverTariff
{
    public decimal PerTrip { get; }
    public decimal PerKm { get; }
    public decimal NightBonusPercent { get; }

    public DriverTariff(decimal perTrip, decimal perKm,
        decimal nightBonusPercent)
    {
        if (perTrip < 0)
            throw new ArgumentOutOfRangeException(nameof(perTrip));
        if (perKm < 0)
            throw new ArgumentOutOfRangeException(nameof(perKm));
        if (nightBonusPercent < 0)
            throw new ArgumentOutOfRangeException(nameof(nightBonusPercent));

        PerTrip = perTrip;
        PerKm = perKm;
        NightBonusPercent = nightBonusPercent;
    }
}
