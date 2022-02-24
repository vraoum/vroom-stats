using OBD.NET.Common.DataTypes;
using OBD.NET.Common.OBDData;

namespace VroomStats.Obd;

public class Odometer : AbstractOBDData
{
    public Kilometre Odom => new((D + C << 8) + (B << 16) + (A << 24), 0, 429496729.5);
    
    public Odometer() : base(0xA6, 4)
    {
    }
}