using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Json;


namespace CoolKids.HomeAutomation
{

    public static class JSONSerializationHelper
    {

        public static T Deserialize<T>(string jsonData) where T : class
        {
            try
            {
                T result = null;
                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonData)))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                    result = serializer.ReadObject(stream) as T;
                }

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error in JSONSerializationHelper.Deserialize(string): {0}", ex.Message));
                return default(T);
            }
        }


        public static T Deserialize<T>(Stream jsonDataStream) where T : class
        {
            try
            {
                T result = null;
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                result = serializer.ReadObject(jsonDataStream) as T;
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error in JSONSerializationHelper.Deserialize(Stream): {0}", ex.Message));
                return default(T);
            }
        }


        public static string Serialize<T>(T entity) where T : class
        {
            try
            {
                string result = null;
                using (MemoryStream stream = new MemoryStream())
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                    serializer.WriteObject(stream, entity);
                    StreamReader sr = new StreamReader(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    result = sr.ReadToEnd();
                }

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error in JSONSerializationHelper.Deserialize(string): {0}", ex.Message));
                return null;
            }
        }
    }

}
