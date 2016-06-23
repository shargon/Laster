using Newtonsoft.Json;
using System.Text;

namespace Laster.Core.Helpers
{
    public class SerializationHelper
    {
        // encoding subset to implement
        public enum EEncoding
        {
            ASCII, UTF8, UTF7, UTF32, Unicode, BigEndianUnicode
        };

        public enum EFormat : byte
        {
            /// <summary>
            /// Salida en json
            /// </summary>
            Json = 0,
            /// <summary>
            /// Salida convertida a string
            /// </summary>
            ToString = 1
        }

        static JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        /// <summary>
        /// Serializa un objeto a un json
        /// </summary>
        /// <param name="data">Datos</param>
        public static string Serialize2Json(object data)
        {
            if (data == null) return null;
            return JsonConvert.SerializeObject(data, Formatting.None, settings);
        }
        /// <summary>
        /// Deserializa un json
        /// </summary>
        /// <typeparam name="T">Tipo</typeparam>
        /// <param name="json">Json</param>
        public static object DeserializeFromJson<T>(string json)
        {
            if (json == null) return default(T);
            return JsonConvert.DeserializeObject<T>(json);
        }
        /// <summary>
        /// Serializa un objeto al tipo especificado
        /// </summary>
        /// <param name="o">Objeto</param>
        /// <param name="encoding">Codificación</param>
        /// <param name="format">Formato</param>
        /// <returns></returns>
        public static byte[] Serialize(object o, EEncoding encoding, EFormat format)
        {
            if (o == null) return new byte[] { };

            string dv;
            switch (format)
            {
                case EFormat.Json: dv = SerializationHelper.Serialize2Json(o); break;
                case EFormat.ToString: dv = o.ToString(); break;
                default: return new byte[] { };
            }

            return GetEncoding(encoding).GetBytes(dv);
        }

        public static Encoding GetEncoding(EEncoding encoding)
        {
            switch (encoding)
            {
                case EEncoding.ASCII: return Encoding.ASCII;
                case EEncoding.UTF7: return Encoding.UTF7;
                case EEncoding.UTF8: return Encoding.UTF8;
                case EEncoding.UTF32: return Encoding.UTF32;
                case EEncoding.Unicode: return Encoding.Unicode;
                case EEncoding.BigEndianUnicode: return Encoding.BigEndianUnicode;
            }

            return Encoding.Default;
        }
    }
}