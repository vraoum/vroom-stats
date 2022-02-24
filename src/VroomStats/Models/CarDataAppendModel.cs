namespace VroomStats.Models;

/// <summary>
/// Represents a model related to appending data to a car.
/// </summary>
/// <param name="Data">Data to append to existing data.</param>
public record CarDataAppendModel(Dictionary<string, string?> Data);