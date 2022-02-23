using System.Text;

namespace VroomStats.Obd;

public class VehicleIdentificationNumber
{
    public byte[] RawVin { get; }

    public string Vin => Encoding.ASCII.GetString(RawVin);

    public VehicleIdentificationNumber(byte[] rawVin)
    {
        RawVin = rawVin;
    }
    
    public override string ToString()
    {
        return Encoding.ASCII.GetString(RawVin);
    }
}