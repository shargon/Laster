using Laster.Core.Enums;
using Laster.Core.Helpers;
using Laster.Core.Interfaces;
using System.IO;

namespace Laster.Outputs
{
    public class FileOutput : IDataOutput
    {
        string _FileName;

        /// <summary>
        /// Archivo de salida
        /// </summary>
        public string FileName { get { return _FileName; } set { _FileName = value; } }
        /// <summary>
        /// Formato
        /// </summary>
        public SerializationHelper.EFormat Format { get; set; }
        /// <summary>
        /// Codificación
        /// </summary>
        public SerializationHelper.EEncoding Encoding { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public FileOutput()
        {
            Format = SerializationHelper.EFormat.Json;
            Encoding = SerializationHelper.EEncoding.UTF8;
        }
        /// <summary>
        /// Saca el contenido de los datos a un archivo
        /// </summary>
        /// <param name="data">Datos</param>
        /// <param name="state">Estado de la enumeración</param>
        protected override void OnProcessData(IData data, EEnumerableDataState state)
        {
            // Formato del archivo
            byte[] cad = SerializationHelper.Serialize(data.GetInternalObject(), Encoding, Format);

            // Guardado del archivo
            switch (state)
            {
                case EEnumerableDataState.OnlyOne:
                case EEnumerableDataState.NonEnumerable:
                case EEnumerableDataState.Start:
                    {
                        File.WriteAllBytes(FileName, cad);
                        break;
                    }
                case EEnumerableDataState.Middle:
                case EEnumerableDataState.End:
                    {
                        FileStream fs = File.OpenWrite(FileName);
                        fs.Write(cad, 0, cad.Length);
                        fs.Close();
                        break;
                    }
            }
        }
    }
}