using Laster.Core.Enums;
using Laster.Core.Helpers;
using Laster.Core.Interfaces;
using System.IO;

namespace Laster.Outputs
{
    public class WriteFileOutput : IDataOutput
    {
        /// <summary>
        /// Archivo de salida
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Codificación
        /// </summary>
        public SerializationHelper.EEncoding StringEncoding { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public WriteFileOutput()
        {
            StringEncoding = SerializationHelper.EEncoding.UTF8;
        }

        public override string Title { get { return "Write file"; } }

        /// <summary>
        /// Saca el contenido de los datos a un archivo
        /// </summary>
        /// <param name="data">Datos</param>
        /// <param name="state">Estado de la enumeración</param>
        protected override void OnProcessData(IData data, EEnumerableDataState state)
        {
            // Formato del archivo

            using (FileStream stream = new FileStream(FileName,
                state == EEnumerableDataState.Middle ||
                state == EEnumerableDataState.End ? FileMode.OpenOrCreate : FileMode.Create,
                FileAccess.Write, FileShare.None))
            {
                using (MemoryStream ms = data.ToStream(StringEncoding))
                    ms.CopyTo(stream);
            }
        }
    }
}