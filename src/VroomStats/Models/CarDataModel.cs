namespace VroomStats.Models;

/// <summary>
/// Represents a car data model, with the date it was pulled at, and the data.
/// </summary>
/// <param name="PulledAt">Date the data was pulled at.</param>
/// <param name="Data">Data pulled.</param>
public record CarDataModel(DateTimeOffset PulledAt, Dictionary<string, string?> Data);