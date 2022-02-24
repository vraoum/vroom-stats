namespace VroomStats.Models;

/// <summary>
/// Represents the settings model of a car.
/// </summary>
/// <param name="Settings">Settings of the car.</param>
public record CarSettingsModel(Dictionary<string, string> Settings);