using System.Net;
using Teltonika.Models;

namespace Teltonika.Parse
{
    public static class Codec8Parser
    {

        public static Codec8Packet Parse(byte[] data)
        {
            if (data == null || data.Length < 16)
            {
                throw new ArgumentException("Invalid data array.");
            }

            using (var memoryStream = new MemoryStream(data))
            using (var binaryReader = new BinaryReader(memoryStream))
            {
                var packet = new Codec8Packet();

                // Read and discard the first 4 bytes (zeroes)
                int zero = IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());

                // Read packet length
                packet.PacketLength = IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());

                // Read codec ID
                packet.CodecId = binaryReader.ReadByte();

                // Read the number of data elements
                packet.NumberOfDataElements = binaryReader.ReadByte();

                // Calculate the size of data elements and read the raw data elements
                int dataElementsSize = packet.PacketLength;
                var rawDataElements = binaryReader.ReadBytes(dataElementsSize);

                // Parse the data elements
                packet.DataElements = ParseDataElements(rawDataElements, packet.NumberOfDataElements);

                // Read the second number of data elements (should match the first one)
                packet.NumberOfDataElements2 = binaryReader.ReadByte();

                //NOT WORKING
                #region NOT WORKING
                //if (packet.NumberOfDataElements != packet.NumberOfDataElements2)
                //{
                //    throw new InvalidOperationException($"Mismatched number of data elements. Expected: {packet.NumberOfDataElements}, received: {packet.NumberOfDataElements2}");
                //}

                //// Read CRC value
                //packet.Crc = IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());
                #endregion
                return packet;
            }
        }

        private static List<DataElement> ParseDataElements(byte[] data, int numberOfDataElements)
        {
            List<DataElement> dataElements = new List<DataElement>();
            int currentPosition = 0;

            for (int i = 0; i < numberOfDataElements; i++)
            {
                DataElement element = new DataElement();

                // Read timestamp bytes, convert to Int64, and set to DataElement
                byte[] timestampBytes = new byte[8];
                Array.Copy(data, currentPosition, timestampBytes, 0, 8);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(timestampBytes);
                }
                long timestamp = BitConverter.ToInt64(timestampBytes, 0);
                element.Timestamp = timestamp;
                currentPosition += 8;

                // Read other DataElement properties
                element.Priority = data[currentPosition++];
                element.Latitude = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(data, currentPosition)) * 0.0000001f;
                currentPosition += 4;
                element.Longitude = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(data, currentPosition)) * 0.0000001f;
                currentPosition += 4;
                element.Altitude = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, currentPosition));
                currentPosition += 2;
                element.Angle = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, currentPosition));
                currentPosition += 2;
                element.Satellites = data[currentPosition++];
                element.Speed = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, currentPosition));
                currentPosition += 2;
                element.EventIOID = data[currentPosition++];
                element.TotalIOCount = data[currentPosition++];

                // Read and parse IO data
                for (int ioSize = 0; ioSize < element.TotalIOCount; ioSize++)
                {
                    // Check if currentPosition is within data bounds
                    if (currentPosition >= data.Length)
                    {
                        break;
                    }

                    byte ioCount = data[currentPosition++];

                    for (int j = 0; j < ioCount; j++)
                    {
                        // Check if currentPosition is within data bounds
                        if (currentPosition >= data.Length)
                        {
                            break;
                        }

                        byte ioID = data[currentPosition++];
                        long ioValue;

                        // Determine IO value based on IO size
                        switch (ioSize)
                        {
                            case 0: // One-byte IO
                                ioValue = data[currentPosition++];
                                break;
                            case 1: // Two-bytes IO
                                ioValue = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, currentPosition));
                                currentPosition += 2;
                                break;
                            case 2: // Four-bytes IO
                                ioValue = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(data, currentPosition));
                                currentPosition += 4;
                                break;
                            case 3: // Eight-bytes IO
                                ioValue = IPAddress.NetworkToHostOrder(BitConverter.ToInt64(data, currentPosition));
                                currentPosition += 8;
                                break;
                            default:
                                throw new InvalidOperationException("Invalid IO size.");
                        }

                        // Store IO value in DataElement's IOData dictionary
                        element.IOData[ioID] = ioValue;
                    }
                }

                if (element.TotalIOCount < 4)// Need the space of N*
                {
                    var result = 4 - element.TotalIOCount;
                    currentPosition += result;
                }

                // Add the DataElement to the list of data elements
                dataElements.Add(element);

                if (dataElements.Count == numberOfDataElements)
                    break;
            }

            return dataElements;
        }
    }

}
