namespace Domain;

// A separate illustrative payment scheme, not an extra bonus on top of a trip.
// Different inputs need not be forced into a common interface.
public sealed class HourlyPaymentPolicy
{
    public decimal Calculate(decimal hours, decimal hourlyRate)
    {
        if (hours < 0) throw new ArgumentOutOfRangeException(nameof(hours));
        if (hourlyRate < 0) throw new ArgumentOutOfRangeException(nameof(hourlyRate));
        return hours * hourlyRate;
    }
}
