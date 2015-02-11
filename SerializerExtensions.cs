using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace System
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



        public static bool Serialze_Xml<T>(this IEnumerable<T> ls, string fileName)
        {
            try
            {
                using (Stream str = File.OpenWrite(fileName))
                {
                    XmlSerializer ser = new XmlSerializer(ls.GetType());
                    ser.Serialize(str, ls);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static byte[] Serialze_Xml<T>(this IEnumerable<T> ls)
        {
            byte[] retVal = null;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    XmlSerializer ser = new XmlSerializer(typeof(T));
                    ser.Serialize(ms, ls);
                    ms.Position = 0;
                    retVal = ms.ToArray();
                }
            }
            catch
            {
            }
            return retVal;
        }
        public static T DeSerialze_Xml<T>(this string fileName) where T : class
        {
            T retVal = default(T);
            try
            {
                using (Stream str = File.OpenRead(fileName))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(T));
                    var a = ser.Deserialize(str);
                    retVal = a as T;
                }
            }
            catch
            {
            }
            return retVal;
        }
        public static T DeSerialze_Xml<T>(this byte[] obj) where T : class
        {
            T retVal = default(T);
            try
            {
                using (MemoryStream ms = new MemoryStream(obj))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(T));
                    ms.Position = 0;
                    var a = ser.Deserialize(ms);
                    retVal = a as T;
                }
            }
            catch
            {
            }
            return retVal;
        }



        public static bool Serialze_JSON<T>(this IEnumerable<T> ls, string fileName)
        {
            try
            {
                using (Stream str = File.OpenWrite(fileName))
                {
                    using (StreamWriter writer = new StreamWriter(str))
                    {
                        writer.Write(Serialze_JSON<T>(ls));
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static string Serialze_JSON<T>(this IEnumerable<T> ls)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(ls);
        }
        public static T DeSerialze_JSON_File<T>(this string fileName) where T : class
        {
            T retVal = default(T);
            try
            {
                using (Stream str = File.OpenRead(fileName))
                {
                    string a = "";
                    using (StreamReader reader = new StreamReader(str))
                    {
                        a = reader.ReadToEnd();
                    }
                    if(!string.IsNullOrEmpty(a))
                    {
                        return DeSerialze_JSON<T>(a);
                    }
                }
            }
            catch
            {
            }
            return retVal;
        }
        public static T DeSerialze_JSON<T>(this string obj) where T : class
        {
            T retVal = default(T);
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(obj, new Newtonsoft.Json.JsonSerializerSettings
                {
                    Error = delegate(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
                    {
                        Logger.Warn(args.ErrorContext.Error.Message);
                        args.ErrorContext.Handled = true;
                    }
                }) as T;
            }
            catch
            {
            }
            return retVal;
        }
    }
}
