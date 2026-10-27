using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain;

public sealed class Payroll
{
    private readonly List<PayrollEntry> _entries = new();

    public int Id { get; }
    public int DriverId { get; }
    public bool IsApproved { get; private set; }

    public IReadOnlyList<PayrollEntry> Entries
        => _entries.AsReadOnly();

    public Payroll(int id, int driverId)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
        if (driverId <= 0) throw new ArgumentOutOfRangeException(nameof(driverId));
        Id = id;
        DriverId = driverId;
    }

    public void AddEntry(int tripId, decimal amount)
    {
        EnsureDraft();

        if (_entries.Any(e => e.TripId == tripId))
            throw new InvalidOperationException(
                "Trip is already in this payroll.");

        _entries.Add(new PayrollEntry(tripId, amount));
    }

    public void ChangeAmount(int tripId, decimal amount)
    {
        EnsureDraft();

        int index = _entries.FindIndex(e => e.TripId == tripId);
        if (index < 0)
            throw new InvalidOperationException(
                "Entry was not found.");

        _entries[index] = new PayrollEntry(tripId, amount);
    }

    public void Approve()
    {
        IsApproved = true;
    }

    private void EnsureDraft()
    {
        if (IsApproved)
            throw new InvalidOperationException(
                "Approved payroll is locked.");
    }
}
