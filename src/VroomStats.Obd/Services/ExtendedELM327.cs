using System.Globalization;
using OBD.NET.Common.Communication;
using OBD.NET.Common.Devices;
using OBD.NET.Common.Enums;

namespace VroomStats.Obd.Services;

public class ExtendedElm327 : ELM327
{
    private bool _isProcessingVin;

    public ExtendedElm327(ISerialConnection connection) : base(connection)
    {
    }

    public async Task<VehicleIdentificationNumber> RequestVinAsync()
    {
        const byte pid = 2;

        var cmd = ((byte) Mode.RequestVehicleInformation).ToString("X2") + pid.ToString("X2");

        var result = SendCommand(cmd);
        await result.WaitHandle.WaitAsync();

        return (result.Result as VehicleIdentificationNumber)!;
    }

    protected override object? ProcessMessage(string message)
    {
        if (message == "014") // VIN request
        {
            _isProcessingVin = true;
            return message;
        }

        if (!_isProcessingVin)
        {
            return base.ProcessMessage(message);
        }

        if (message[0] == '2' && message[1] == ':')
        {
            // last VIN payload response received.

            _isProcessingVin = false;

            MessageChunk += message;
            
            var parsedVin = 
                // joins the different payloads
                string.Join("", MessageChunk.Split('\n').Select(x => x[2..]))
                // removes useless spaces and removes 6 first unused chars
                .Replace(" ", "")[6..]
                // makes chunks of two chars (cause bytes)
                .Chunk(2)
                // joins the chars to make them strings and parse them more easily
                .Select(x => string.Join("", x))
                // converts to byte by parsing them as hex numbers
                .Select(x => byte.Parse(x, NumberStyles.HexNumber))
                // makes it a byte array
                .ToArray();

            var payload = new VehicleIdentificationNumber(parsedVin);
            MessageChunk = null;

            return payload;
        }
        
        MessageChunk += message + "\n";

        return null;
    }
}