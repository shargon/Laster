using Laster.Core.Enums;
using Laster.Core.Helpers;
using Laster.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;

namespace Laster.Process.Files
{
    public class ReadFileProcess : IDataProcess
    {
        /// <summary>
        /// Archivo de salida
        /// </summary>
        [Category("Origin")]
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
        [Category("Processing")]
        [DefaultValue(true)]
        public bool ConvertToString { get; set; }
        /// <summary>
        /// Get File from input
        /// </summary>
        [Category("Origin")]
        [DefaultValue(false)]
        public bool GetFileNameFromInput { get; set; }
        /// <summary>
        /// Delete after read
        /// </summary>
        [Category("Processing")]
        [DefaultValue(false)]
        public bool DeleteAfterRead { get; set; }

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
            List<object> ret = new List<object>();

            if (GetFileNameFromInput)
            {
                if (data != null) foreach (object file in data)
                    {
                        if (file == null) continue;

                        object r = null;

                        if (file is byte[])
                        {
                            if (ConvertToString)
                            {
                                Encoding codec = SerializationHelper.GetEncoding(StringEncoding);
                                r = codec.GetString((byte[])file);
                            }
                        }
                        else
                        {
                            r = GetFile(file.ToString());
                        }
                        if (r != null) ret.Add(r);
                    }
            }
            else
            {
                object r = GetFile(FileName);
                if (r != null) ret.Add(r);
            }

            return Reduce(EReduceZeroEntries.Empty, ret);
        }

        object GetFile(string file)
        {
            if (file != null) file = Environment.ExpandEnvironmentVariables(file);
            object ret = null;
            if (!string.IsNullOrEmpty(file))
            {
                if (File.Exists(file))
                {
                    using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        byte[] d = new byte[stream.Length];
                        StreamHelper.ReadFull(stream, d, 0, d.Length);

                        if (ConvertToString)
                        {
                            Encoding codec = SerializationHelper.GetEncoding(StringEncoding);
                            ret = codec.GetString(d);
                        }
                        else ret = d;
                    }

                    if (DeleteAfterRead) File.Delete(file);
                }
                else
                {
                    Uri u;
                    if (Uri.TryCreate(file, UriKind.Absolute, out u))
                    {
                        using (WebClient c = new WebClient())
                            return c.DownloadString(u);
                    }
                }
            }
            return ret;
        }
    }
}