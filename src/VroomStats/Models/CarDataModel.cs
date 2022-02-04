namespace VroomStats.Models;

public record CarDataModel(DateTimeOffset PulledAt, Dictionary<string, object> Data);