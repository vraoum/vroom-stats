namespace VroomStats.Models;

public record CarDataModel(DateTimeOffset PulledAt, Dictionary<string, string> Data);