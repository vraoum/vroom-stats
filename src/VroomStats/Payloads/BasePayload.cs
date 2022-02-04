namespace VroomStats.Payloads;

public record BasePayload(OpCode OpCode, Dictionary<string, object> Data);