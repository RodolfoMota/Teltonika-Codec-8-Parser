using Teltonika.Models;
using Teltonika.Parse;

namespace Teltonika
{

    internal class Program
    {

        static byte[] StringToByteArray(string hex)
        {
            int length = hex.Length;
            byte[] bytes = new byte[length / 2];
            for (int i = 0; i < length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        static void Main(string[] args)
        {
       
            try
            {
                string hexData = "000000000000004308020000016B40D57B480100000000000000000000000000000001010101000000000000016B40D5C198010000000000000000000000000000000101010101000000020000252C";
                byte[] data = StringToByteArray(hexData);
                Codec8Packet packet = Codec8Parser.Parse(data);
                LogData(packet);

                Console.WriteLine("----------------------------------------------------------------------");


                string hexData2 = "000000000000002808010000016B40D9AD80010000000000000000000000000000000103021503010101425E100000010000F22A";
                byte[] data2 = StringToByteArray(hexData2);
                Codec8Packet packet2 = Codec8Parser.Parse(data2);
                LogData(packet2);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            Console.ReadKey();
        }

        private static void LogData(Codec8Packet packet)
        {
            Console.WriteLine("Packet Length: " + packet.PacketLength);
            Console.WriteLine("Codec ID: " + packet.CodecId);
            Console.WriteLine("Number of Data Elements: " + packet.NumberOfDataElements);
            Console.WriteLine("CRC: " + packet.Crc);

            for (int i = 0; i < packet.DataElements.Count; i++)
            {
                var element = packet.DataElements[i];
                Console.WriteLine($"Data Element {i + 1}:");
                Console.WriteLine($"\tTimestamp: {element.Timestamp}");
                Console.WriteLine($"\tPriority: {element.Priority}");
                Console.WriteLine($"\tLatitude: {element.Latitude}");
                Console.WriteLine($"\tLongitude: {element.Longitude}");
                Console.WriteLine($"\tAltitude: {element.Altitude}");
                Console.WriteLine($"\tAngle: {element.Angle}");
                Console.WriteLine($"\tSatellites: {element.Satellites}");
                Console.WriteLine($"\tSpeed: {element.Speed}");
                Console.WriteLine($"\t\tIoData:");

                foreach (KeyValuePair<byte, long> entry in element.IOData)
                {
                    Console.WriteLine("\t\t\t IO ID = {0}, IO VALUE = {1}", entry.Key, entry.Value);
                }
            }
        }
    }
}