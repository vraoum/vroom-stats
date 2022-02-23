using MongoDB.Bson;
using MongoDB.Driver;
using VroomStats.Models;
using VroomStats.Payloads;

namespace VroomStats.Services;

/// <inheritdoc cref="IDatabaseService"/>
public class DatabaseService : IDatabaseService
{
    private readonly IMongoCollection<CarModel> _collection;
    private readonly SemaphoreSlim _semaphore;

    public DatabaseService(IMongoDatabase database)
    {
        _semaphore = new SemaphoreSlim(1);
        _collection = database.GetCollection<CarModel>("car-data");
    }

    /// <inheritdoc cref="IDatabaseService.GetCarsAsync"/>
    public async Task<IReadOnlyCollection<CarOutModel>> GetCarsAsync()
    {
        var cars = await _collection
            .Find(new BsonDocument())
            .Project(x => new CarOutModel(x.Id, x.Settings))
            .ToListAsync();

        if (cars is null)
        {
            throw new InvalidOperationException("Database query failed");
        }

        return cars.AsReadOnly();
    }
    
    /// <inheritdoc cref="IDatabaseService.GetCarAsync"/>
    public async Task<CarModel?> GetCarAsync(string carId, int amount)
    {
        var carData = await _collection
            .Find(x => x.Id == carId)
            .FirstOrDefaultAsync();

        if (carData is null)
        {
            return null;
        }

        return new CarModel(carData.Id, carData.Settings, carData.Data.Take(amount).ToList());
    }
    
    /// <inheritdoc cref="IDatabaseService.AppendDataAsync"/>
    public async Task AppendDataAsync(string carId, BasePayload payload)
    {
        await _semaphore.WaitAsync();

        try
        {
            var carData = await _collection
                .Find(x => x.Id == carId)
                .FirstOrDefaultAsync();

            if (carData is null)
            {
                await RegisterCarAsync(carId, new CarSettingsModel(new Dictionary<string, string>()));
                carData = await _collection
                    .Find(x => x.Id == carId)
                    .FirstAsync();

                if (carData is null)
                {
                    throw new InvalidOperationException("Car not found.");
                }
            }

            carData.Data.Add(new CarDataModel(DateTimeOffset.Now, payload.Data));
            await _collection.ReplaceOneAsync(x => x.Id == carId, carData);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <inheritdoc cref="IDatabaseService.UpdateSettingsAsync"/>
    public async Task<CarOutModel?> UpdateSettingsAsync(string carId, CarSettingsModel settings)
    {
        await _semaphore.WaitAsync();
        
        try
        {
            var carData = await _collection
                .Find(x => x.Id == carId)
                .FirstOrDefaultAsync();

            if (carData is null)
            {
                return null;
            }

            carData.Settings.Clear();
            foreach (var (key, value) in settings.Settings)
            {
                carData.Settings.Add(key, value);
            }
        
            await _collection.ReplaceOneAsync(x => x.Id == carId, carData);
            return new CarOutModel(carData.Id, carData.Settings);
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    /// <inheritdoc cref="IDatabaseService.RegisterCarAsync"/>
    public async Task<bool> RegisterCarAsync(string carId, CarSettingsModel settings)
    {
        await _semaphore.WaitAsync();
        
        try
        {
            if (await _collection.Find(x => x.Id == carId).AnyAsync())
            {
                return false;
            }

            await _collection.InsertOneAsync(new CarModel(carId, settings.Settings, new List<CarDataModel>()));
            return true;
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    /// <inheritdoc cref="IDatabaseService.GetLatestDataAsync"/>
    public async Task<CarDataModel?> GetLatestDataAsync(string carId)
    {
        var carData = await _collection
            .Find(x => x.Id == carId)
            .FirstOrDefaultAsync();

        if (carData is null)
        {
            return null;
        }

        var last = carData.Data.LastOrDefault();
        if (last is null)
        {
            return new CarDataModel(DateTimeOffset.Now, new Dictionary<string, string>());
        }

        var model = new CarDataModel(last.PulledAt, last.Data);
        foreach (var data in carData.Data.OrderByDescending(x => x.PulledAt))
        {
            foreach (var (key, s) in data.Data)
            {
                if (!model.Data.TryGetValue(key, out _))
                {
                    model.Data.Add(key, s);
                }
            }
        }
        
        return model;
    }
}