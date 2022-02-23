namespace VroomStats.Models;

public record CarModel(string Id, Dictionary<string, string> Settings, List<CarDataModel> Data);