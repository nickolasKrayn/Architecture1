namespace Domain;

public sealed record PayrollPeriod
{
    public DateTimeOffset From { get; }
    public DateTimeOffset ToExclusive { get; }

    public PayrollPeriod(DateTimeOffset from, DateTimeOffset toExclusive)
    {
        if (toExclusive <= from)
            throw new ArgumentException("Period end must be after its start.");
        From = from;
        ToExclusive = toExclusive;
    }

    public bool Contains(DateTimeOffset instant)
        => From <= instant && instant < ToExclusive;
}
