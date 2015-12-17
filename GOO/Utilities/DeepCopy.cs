using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GOO.Utilities
{
    public static class DeepCopy<T>
    {
        public static T CopyFrom(object objectToCopy)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, objectToCopy);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }
    }
}