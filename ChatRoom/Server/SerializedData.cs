using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Server
{
    public static class SerializedData
    {
        /*
        public static void SerializeDataDaily(TrackedData data, int playerNum, int numDay)
        {
            string dir = @"c:\temp";
            string serializationFile = Path.Combine(dir, "player" + playerNum + "day" + numDay + ".bin");

            try
            {
                using (Stream stream = File.Open(serializationFile, FileMode.Create))
                {
                    BinaryFormatter bformatter = new BinaryFormatter();

                    bformatter.Serialize(stream, data);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public static TrackedData DeserializeDailyData(int playerNum, int numDay)
        {
            string dir = @"c:\temp";
            string serializationFile = Path.Combine(dir, "player" + playerNum + "day" + numDay + ".bin");
            TrackedData data;

            using (Stream stream = File.Open(serializationFile, FileMode.Open))
            {
                BinaryFormatter bformatter = new BinaryFormatter();

                data = (TrackedData)bformatter.Deserialize(stream);
            }

            return data;
        }
        */

    }
}
