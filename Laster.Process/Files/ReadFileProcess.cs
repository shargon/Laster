using Laster.Core.Enums;
using Laster.Core.Helpers;
using Laster.Core.Interfaces;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;

namespace Laster.Process.Files
{
    public class ReadFileProcess : IDataProcess
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
        /// Convertir a string la cadena
        /// </summary>
        [DefaultValue(true)]
        public bool ConvertToString { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ReadFileProcess()
        {
            StringEncoding = SerializationHelper.EEncoding.UTF8;
            DesignBackColor = Color.Brown;
            ConvertToString = true;
        }

        public override string Title { get { return "Files - Read file"; } }

        /// <summary>
        /// Saca el contenido de los datos a un archivo
        /// </summary>
        /// <param name="data">Datos</param>
        /// <param name="state">Estado de la enumeración</param>
        protected override IData OnProcessData(IData data, EEnumerableDataState state)
        {
            if (FileName == null) return DataBreak();

            string file = Environment.ExpandEnvironmentVariables(FileName);
            if (string.IsNullOrEmpty(file) || !File.Exists(file)) return DataBreak();

            using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                byte[] d = new byte[stream.Length];
                StreamHelper.ReadFull(stream, d, 0, d.Length);

                if (ConvertToString)
                {
                    Encoding codec = SerializationHelper.GetEncoding(StringEncoding);
                    return DataObject(codec.GetString(d));
                }

                return DataObject(d);
            }
        }
    }
}