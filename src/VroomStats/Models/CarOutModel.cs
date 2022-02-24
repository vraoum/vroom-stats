namespace VroomStats.Models;

/// <summary>
/// Represents a car model without its data.
/// </summary>
/// <param name="Id">Id of the car.</param>
/// <param name="Settings">Settings of the car.</param>
public record CarOutModel(string Id, Dictionary<string, string> Settings);