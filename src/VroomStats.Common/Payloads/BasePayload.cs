namespace VroomStats.Payloads;

public record BasePayload(OpCode OpCode, Dictionary<string, string> Data);