namespace Models;

public sealed class DriverTariff
{
    public decimal PerTrip { get; set; }
    public decimal PerKm { get; set; }
    public decimal NightBonusPercent { get; set; }
}

public sealed class TariffVersion
{
    public int DriverId { get; set; }
    public DateTimeOffset EffectiveFrom { get; set; }
    public DriverTariff Tariff { get; set; } = new();
}
