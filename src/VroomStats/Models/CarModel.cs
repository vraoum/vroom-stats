namespace VroomStats.Models;

/// <summary>
/// Represents a car model.
/// </summary>
/// <param name="Id">Id of the car.</param>
/// <param name="Settings">Settings of the car.</param>
/// <param name="Data">Data of the car.</param>
public record CarModel(string Id, Dictionary<string, string> Settings, List<CarDataModel> Data);