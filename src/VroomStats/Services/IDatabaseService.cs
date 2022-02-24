using VroomStats.Models;
using VroomStats.Payloads;

namespace VroomStats.Services;

/// <summary>
/// Defines a service that interfaces the mongo database with the REST api.
/// </summary>
public interface IDatabaseService
{
    /// <summary>
    /// Gets the different available cars with their settings.
    /// </summary>
    /// <returns>Returns a collection of cars.</returns>
    Task<IReadOnlyCollection<CarOutModel>> GetCarsAsync();

    /// <summary>
    /// Gets everything known about a car.
    /// </summary>
    /// <param name="carId">Id of the car.</param>
    /// <param name="amount">Amount of data to return.</param>
    /// <returns>Returns a nullable model depending on if the car exists or not.</returns>
    Task<CarModel?> GetCarAsync(string carId, int amount);
    
    /// <summary>
    /// Creates a new car from its id and optional settings.
    /// </summary>
    /// <param name="carId">Id of the car.</param>
    /// <param name="settings">Settings of the car.</param>
    /// <returns>Returns whether the registration has succeeded or not.</returns>
    Task<bool> RegisterCarAsync(string carId, CarSettingsModel settings);
    
    /// <summary>
    /// Updates the settings associated with a car.
    /// </summary>
    /// <param name="carId">Id of the car.</param>
    /// <param name="settings">Settings of the car.</param>
    /// <returns>Returns a nullable model depending whether the car exists or not with its new settings.</returns>
    Task<CarOutModel?> UpdateSettingsAsync(string carId, CarSettingsModel settings);
    
    /// <summary>
    /// Gets the latest known available data of a car.
    /// </summary>
    /// <param name="carId">Id of the car.</param>
    /// <returns>Returns a nullable model depending whether the car exists or not with the pulled data.</returns>
    Task<CarDataModel?> GetLatestDataAsync(string carId);
    
    /// <summary>
    /// Adds data to an existing car.
    /// </summary>
    /// <param name="carId">Id of the car.</param>
    /// <param name="payload">Data to add.</param>
    Task AppendDataAsync(string carId, BasePayload payload);
}