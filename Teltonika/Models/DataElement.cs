namespace Teltonika.Models
{
    public class DataElement
    {
        public long Timestamp { get; set; }
        public byte Priority { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public ushort Altitude { get; set; }
        public ushort Angle { get; set; }
        public byte Satellites { get; set; }
        public ushort Speed { get; set; }
        public byte EventIOID { get; set; }
        public byte TotalIOCount { get; set; }
        public Dictionary<byte, long> IOData { get; set; } = new Dictionary<byte, long>();
    }
}
