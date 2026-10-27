namespace Models;

// Anemic data model: code can modify fields without going through a service.
public sealed class Trip
{
    public int Id { get; set; }
    public int DriverId { get; set; }
    public decimal DistanceKm { get; set; }
    public bool IsNight { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsConfirmed { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
}
