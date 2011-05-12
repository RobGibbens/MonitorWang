using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace MonitorWang.Core
{
    public class SerialisationHelper<T>
    {
        /// <summary>
        /// Serialize the object using the DataContractSerializer
        /// </summary>
        /// <param name="encoding">The encoding to use when serializing.</param>
        /// <param name="entity">The entity to serialize.</param>
        /// <returns></returns>
        public static string DataContractSerialize(Encoding encoding, T entity)
        {
            var serializer = new DataContractSerializer(typeof(T));

            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, entity);
                return encoding.GetString(ms.ToArray());
            }
        }

        /// <summary>
        /// Serialize the object using the DataContractSerializer, defaults to using UTF8 encoding
        /// </summary>
        /// <param name="entity">The entity to serialize.</param>
        /// <returns></returns>
        public static string DataContractSerialize(T entity)
        {
            return DataContractSerialize(Encoding.UTF8, entity);
        }

        /// <summary>
        /// Deserialize the object using the DataContractSerializer
        /// </summary>
        /// <param name="encoding">The encoding to use when deserializing.</param>
        /// <param name="entity">The entity to deserialize.</param>
        /// <returns></returns>
        public static T DataContractDeserialize(Encoding encoding, string entity)
        {
            var serializer = new DataContractSerializer(typeof(T));
            
            using (var ms = new MemoryStream(encoding.GetBytes(entity)))
            {
                return (T)serializer.ReadObject(ms);
            }
        }

        /// <summary>
        /// Deserialize the object using the DataContractSerializer, defaults to using UTF8 encoding
        /// </summary>
        /// <param name="entity">The entity to deserialize.</param>
        /// <returns></returns>
        public static T DataContractDeserialize(string entity)
        {
            return DataContractDeserialize(Encoding.UTF8, entity);
        }        
    }
}