using VroomStats.Models;
using VroomStats.Payloads;

namespace VroomStats.Services;

public interface IDatabaseService
{
    Task<IReadOnlyCollection<CarOutModel>> GetCarsAsync();
    Task<bool> RegisterCarAsync(string carId, string displayName);
    Task<CarDataModel> GetLatestDataAsync(string carId);
    Task AppendDataAsync(string carId, BasePayload payload);
}