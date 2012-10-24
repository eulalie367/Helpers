using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Extensions
{
    public static class SerializerExtensions
    {
        public static bool Serialze_Binary(this object ls, string fileName)
        {
            try
            {
                using (Stream str = File.OpenWrite(fileName))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(str, ls);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static byte[] Serialze_Binary(this object ls)
        {
            byte[] retVal = null;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(ms, ls);
                    ms.Position = 0;
                    retVal = ms.ToArray();
                }
            }
            catch
            {
            }
            return retVal;
        }
        public static T DeSerialze_Binary<T>(this string fileName) where T : class
        {
            T retVal = default(T);
            try
            {
                using (Stream str = File.OpenRead(fileName))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    var a = formatter.Deserialize(str);
                    retVal = a as T;
                }
            }
            catch
            {
            }
            return retVal;
        }
        public static T DeSerialze_Binary<T>(this byte[] obj) where T : class
        {
            T retVal = default(T);
            try
            {
                using (MemoryStream ms = new MemoryStream(obj))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    ms.Position = 0;
                    var a = formatter.Deserialize(ms);
                    retVal = a as T;
                }
            }
            catch
            {
            }
            return retVal;
        }
    }
}
