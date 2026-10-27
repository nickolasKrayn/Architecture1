using System;

namespace Domain;

public sealed class PayrollEntry
{
    public int TripId { get; }
    public decimal Amount { get; }

    public PayrollEntry(int tripId, decimal amount)
    {
        if (tripId <= 0) throw new ArgumentOutOfRangeException(nameof(tripId));
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        TripId = tripId;
        Amount = amount;
    }
}
