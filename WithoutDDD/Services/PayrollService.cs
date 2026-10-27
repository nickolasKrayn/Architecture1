using Models;

namespace Services;

// The service checks the same rules as the DDD aggregate.
// Bypassing this service bypasses its protection.
public static class PayrollService
{
    public static Payroll Create(int id, int driverId)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
        if (driverId <= 0) throw new ArgumentOutOfRangeException(nameof(driverId));
        return new Payroll { Id = id, DriverId = driverId };
    }

    public static void AddEntry(Payroll payroll, int tripId, decimal amount)
    {
        EnsureDraft(payroll);
        if (payroll.Entries.Any(e => e.TripId == tripId))
            throw new InvalidOperationException("Trip is already in this payroll.");
        ValidateEntry(tripId, amount);
        payroll.Entries.Add(new PayrollEntry { TripId = tripId, Amount = amount });
    }

    public static void ChangeAmount(Payroll payroll, int tripId, decimal amount)
    {
        EnsureDraft(payroll);
        var entry = payroll.Entries.SingleOrDefault(e => e.TripId == tripId)
            ?? throw new InvalidOperationException("Entry was not found.");
        ValidateEntry(tripId, amount);
        entry.Amount = amount;
    }

    public static void Approve(Payroll payroll) => payroll.IsApproved = true;

    private static void EnsureDraft(Payroll payroll)
    {
        if (payroll.IsApproved) throw new InvalidOperationException("Approved payroll is locked.");
    }

    private static void ValidateEntry(int tripId, decimal amount)
    {
        if (tripId <= 0) throw new ArgumentOutOfRangeException(nameof(tripId));
        if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
    }
}
