namespace Teltonika.Models
{
    public class Codec8Packet
    {
        public int PacketLength { get; set; }
        public byte CodecId { get; set; }
        public int NumberOfDataElements { get; set; }
        public List<DataElement> DataElements { get; set; }
        public byte NumberOfDataElements2 { get; set; }
        public int Crc { get; set; }
    }
}
