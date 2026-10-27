namespace Models;

public sealed class Payroll
{
    public int Id { get; set; }
    public int DriverId { get; set; }
    public bool IsApproved { get; set; }
    public List<PayrollEntry> Entries { get; set; } = new();
}

public sealed class PayrollEntry
{
    public int TripId { get; set; }
    public decimal Amount { get; set; }
}

public sealed class PayrollPeriod
{
    public DateTimeOffset From { get; set; }
    public DateTimeOffset ToExclusive { get; set; }
}
