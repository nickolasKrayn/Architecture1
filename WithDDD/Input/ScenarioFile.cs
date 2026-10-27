using System.Text.Json;
using System.Text.Json.Serialization;

namespace Input;

// Transport data, not a domain model. The same JSON contract is used in both projects.
public sealed class ScenarioData
{
    public required int PayrollId { get; init; }
    public required int DriverId { get; init; }
    public required DateTimeOffset From { get; init; }
    public required DateTimeOffset ToExclusive { get; init; }
    public required TripInput[] Trips { get; init; }
    public required TariffInput[] Tariffs { get; init; }
}

public sealed class TripInput
{
    public required int Id { get; init; }
    public required int DriverId { get; init; }
    public required decimal DistanceKm { get; init; }
    public required bool IsNight { get; init; }
    public required bool IsCompleted { get; init; }
    public required bool IsConfirmed { get; init; }
    public required DateTimeOffset? CompletedAt { get; init; }
}

public sealed class TariffInput
{
    public required int DriverId { get; init; }
    public required DateTimeOffset EffectiveFrom { get; init; }
    public required decimal PerTrip { get; init; }
    public required decimal PerKm { get; init; }
    public required decimal NightBonusPercent { get; init; }
}

public static class ScenarioFile
{
    public static string DefaultPath => Path.Combine(AppContext.BaseDirectory, "Data", "sample.json");

    public static ScenarioData Load(string path) => Parse(File.ReadAllText(path));

    public static ScenarioData Parse(string json)
    {
        var data = JsonSerializer.Deserialize<ScenarioData>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
        }) ?? throw new ArgumentException("Сценарий не может быть null.");
        Validate(data);
        return data;
    }

    public static void Validate(ScenarioData data)
    {
        if (data.PayrollId <= 0 || data.DriverId <= 0)
            throw new ArgumentException("Идентификаторы ведомости и водителя должны быть положительными.");
        if (data.ToExclusive <= data.From)
            throw new ArgumentException("Конец периода должен быть позже начала.");
        if (data.Trips is null || data.Tariffs is null ||
            data.Trips.Any(t => t is null) || data.Tariffs.Any(t => t is null))
            throw new ArgumentException("Массивы рейсов и тарифов не могут содержать null.");
        if (data.Trips.GroupBy(t => t.Id).Any(g => g.Count() > 1))
            throw new ArgumentException("В исходных данных повторяется идентификатор рейса.");
        foreach (var trip in data.Trips)
        {
            if (trip.Id <= 0 || trip.DriverId <= 0 || trip.DistanceKm < 0)
                throw new ArgumentException("Рейс: положительные идентификаторы и неотрицательный пробег обязательны.");
            if (trip.IsConfirmed && !trip.IsCompleted)
                throw new ArgumentException("Нельзя подтвердить незавершённый рейс.");
            if (trip.IsCompleted != trip.CompletedAt.HasValue)
                throw new ArgumentException("Дата завершения должна быть указана ровно у завершённых рейсов.");
        }
        foreach (var tariff in data.Tariffs)
        {
            if (tariff.DriverId <= 0 || tariff.PerTrip < 0 || tariff.PerKm < 0 || tariff.NightBonusPercent < 0)
                throw new ArgumentException("Тариф: положительный водитель и неотрицательные ставки обязательны.");
        }
        if (data.Tariffs.GroupBy(t => (t.DriverId, t.EffectiveFrom)).Any(g => g.Count() > 1))
            throw new ArgumentException("Две версии тарифа одного водителя начинают действовать одновременно.");
    }
}
