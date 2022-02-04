using MongoDB.Bson;
using MongoDB.Driver;
using VroomStats.Models;
using VroomStats.Payloads;

namespace VroomStats.Services;

public class DatabaseService : IDatabaseService
{
    private readonly IMongoCollection<CarModel> _collection;
    
    public DatabaseService(IMongoDatabase database)
    {
        _collection = database.GetCollection<CarModel>("car-data");
    }

    public async Task<IReadOnlyCollection<CarModel>> GetCarsAsync()
    {
        var cars = await _collection
            .Find(new BsonDocument())
            .Project(x => new CarModel(x.Id, x.DisplayName, new List<CarDataModel>()))
            .ToListAsync();
        
        if (cars is null or {Count: <= 0})
        {
            throw new InvalidOperationException("Database query failed");
        }

        return cars.AsReadOnly();
    }

    public async Task AppendDataAsync(string carId, BasePayload payload)
    {
        var carData = await _collection
            .Find(x => x.Id == carId)
            .FirstAsync();

        if (carData is null)
        {
            throw new InvalidOperationException("Car not found.");
        }

        carData.Data.Add(new CarDataModel(DateTimeOffset.Now, payload.Data));
    }
    
    public async Task<bool> RegisterCarAsync(string carId, string displayName)
    {
        if (await _collection.Find(x => x.Id == carId).AnyAsync())
        {
            return false;
        }
        
        await _collection.InsertOneAsync(new CarModel(carId, displayName, new List<CarDataModel>()));
        return true;
    }

    public async Task<CarDataModel> GetLatestDataAsync(string carId)
    {
        var carData = await _collection
            .Find(x => x.Id == carId)
            .FirstAsync();

        return carData?.Data.Last()!;
    }
}