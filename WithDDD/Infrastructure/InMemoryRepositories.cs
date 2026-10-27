using Domain;
using Application;

namespace Infrastructure;

public sealed class InMemoryConfirmedTrips(IEnumerable<Trip> source) : IConfirmedTrips
{
    private readonly Trip[] all = source.ToArray();

    public IReadOnlyList<Trip> GetConfirmed(int driverId, PayrollPeriod period)
        => all.Where(t => t.DriverId == driverId && t.IsConfirmed &&
                         t.CompletedAt is { } at && period.Contains(at))
              .OrderBy(t => t.CompletedAt).ThenBy(t => t.Id).ToArray();
}

public sealed record TariffVersion(
    int DriverId, DateTimeOffset EffectiveFrom, DriverTariff Tariff);

// Immutable history snapshot. A version remains valid until the next version.
// Missing history is an error: never silently substitute today's tariff.
public sealed class InMemoryTariffHistory : ITariffHistory
{
    private readonly TariffVersion[] versions;

    public InMemoryTariffHistory(IEnumerable<TariffVersion> source)
    {
        versions = source.ToArray();
        if (versions.Any(v => v.Tariff is null))
            throw new ArgumentException("A tariff version cannot be null.");
        if (versions.GroupBy(v => (v.DriverId, v.EffectiveFrom)).Any(g => g.Count() > 1))
            throw new ArgumentException("Ambiguous tariff effective date.");
    }

    public DriverTariff GetAt(int driverId, DateTimeOffset completedAt)
        => versions.Where(v => v.DriverId == driverId && v.EffectiveFrom <= completedAt)
                   .OrderByDescending(v => v.EffectiveFrom)
                   .Select(v => v.Tariff)
                   .FirstOrDefault()
           ?? throw new InvalidOperationException("No tariff for this completion time.");
}
