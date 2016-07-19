using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Linq;

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
            /// Salida en json escapada
            /// </summary>
            JsonIndented = 1,
            /// <summary>
            /// Salida convertida a string
            /// </summary>
            ToString = 2
        }

        static JsonSerializerSettings _Settings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };
        static JsonSerializerSettings _SettingsWithTypes = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,

            TypeNameAssemblyFormat = FormatterAssemblyStyle.Full,
            TypeNameHandling = TypeNameHandling.Auto,
            Binder = new TypeNameSerializationBinder()
        };

        public class TypeNameSerializationBinder : SerializationBinder
        {
            //public string TypeFormat { get; private set; }

            public TypeNameSerializationBinder()//string typeFormat)
            {
                //TypeFormat = typeFormat;
            }

            public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
            {
                assemblyName = serializedType.Assembly.GetName().Name;
                typeName = serializedType.FullName;
            }

            public override Type BindToType(string assemblyName, string typeName)
            {
                Assembly asm = AppDomain.CurrentDomain.GetAssemblies().
                        SingleOrDefault(assembly => assembly.GetName().Name == assemblyName);

                return asm.GetType(typeName, true);
            }
        }

        /// <summary>
        /// Serializa un objeto a un json
        /// </summary>
        /// <param name="data">Datos</param>
        /// <param name="withTypes">Exportar los tipos</param>
        public static string SerializeToJson(object data, bool withTypes = false, bool indented = false)
        {
            if (data == null) return null;

            return JsonConvert.SerializeObject(data, indented ? Formatting.Indented : Formatting.None, withTypes ? _SettingsWithTypes : _Settings);
        }
        /// <summary>
        /// Deserializa un json
        /// </summary>
        /// <typeparam name="T">Tipo</typeparam>
        /// <param name="json">Json</param>
        /// <param name="withTypes">Exporta los tipos</param>
        public static T DeserializeFromJson<T>(string json, bool withTypes = false)
        {
            if (json == null) return default(T);
            return JsonConvert.DeserializeObject<T>(json, withTypes ? _SettingsWithTypes : _Settings);
        }
        /// <summary>
        /// Serializa un objeto al tipo especificado
        /// </summary>
        /// <param name="o">Objeto</param>
        /// <param name="format">Formato</param>
        public static string Serialize(object o, EFormat format)
        {
            if (o == null) return "";

            switch (format)
            {
                case EFormat.Json: return SerializationHelper.SerializeToJson(o);
                case EFormat.JsonIndented: return SerializationHelper.SerializeToJson(o, false, true);
                case EFormat.ToString: return o.ToString();
                default: return "";
            }
        }

        public static string GetMimeType(EFormat format)
        {
            switch (format)
            {
                case EFormat.Json: return "application/json";
                case EFormat.ToString: return "text/html";
            }
            return "text / html";
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