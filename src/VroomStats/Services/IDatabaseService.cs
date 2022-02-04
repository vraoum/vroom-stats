using VroomStats.Models;

namespace VroomStats.Services;

public interface IDatabaseService
{
    Task<IReadOnlyCollection<CarModel>> GetCarsAsync();
    Task<bool> RegisterCarAsync(string carId, string displayName);
    Task<CarDataModel> GetLatestDataAsync(string carId);
}