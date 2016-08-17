using Laster.Core.Enums;
using Laster.Core.Helpers;
using Laster.Core.Interfaces;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;

namespace Laster.Process.Files
{
    public class WriteFileProcess : IDataProcess
    {
        /// <summary>
        /// Archivo de salida
        /// </summary>
        [DefaultValue("")]
        public string FileName { get; set; }
        /// <summary>
        /// Codificación
        /// </summary>
        [DefaultValue(SerializationHelper.EEncoding.UTF8)]
        public SerializationHelper.EEncoding StringEncoding { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public WriteFileProcess()
        {
            StringEncoding = SerializationHelper.EEncoding.UTF8;
            DesignBackColor = Color.Brown;
        }

        public override string Title { get { return "Files - Write file"; } }

        /// <summary>
        /// Saca el contenido de los datos a un archivo
        /// </summary>
        /// <param name="data">Datos</param>
        /// <param name="state">Estado de la enumeración</param>
        protected override IData OnProcessData(IData data, EEnumerableDataState state)
        {
            // Formato del archivo

            using (FileStream stream = new FileStream(Environment.ExpandEnvironmentVariables(FileName),
                state == EEnumerableDataState.Middle ||
                state == EEnumerableDataState.End ? FileMode.OpenOrCreate : FileMode.Create,
                FileAccess.Write, FileShare.None))
            {
                using (MemoryStream ms = data.ToStream(StringEncoding))
                    ms.CopyTo(stream);
            }

            return data;
        }
    }
}