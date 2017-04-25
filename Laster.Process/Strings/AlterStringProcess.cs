using Laster.Core.Enums;
using Laster.Core.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace Laster.Process.Strings
{
    public class AlterStringProcess : IDataProcess
    {
        public enum ETrim : byte
        {
            None = 0,
            All = 1,
            Start = 2,
            End = 3
        }

        public enum ECase : byte
        {
            None = 0,
            ToLower = 1,
            ToUpper = 2,
            ToLowerInvariant = 3,
            ToUpperInvariant = 4,
        }

        /// <summary>
        /// Realizar un trim antes de la comprobación
        /// </summary>
        [Category("String modification")]
        [DefaultValue(ETrim.All)]
        public ETrim Trim { get; set; }

        /// <summary>
        /// Realizar un trim antes de la comprobación
        /// </summary>
        [Category("String modification")]
        [DefaultValue(ECase.None)]
        public ECase Case { get; set; }

        [DefaultValue(false)]
        public bool ReturnEmpty { get; set; }

        public override string Title { get { return "Strings - Alter"; } }

        public AlterStringProcess()
        {
            DesignBackColor = Color.Blue;
            Trim = ETrim.All;
            ReturnEmpty = false;
        }

        /// <summary>
        /// Saca el contenido de los datos a un archivo
        /// </summary>
        /// <param name="data">Datos</param>
        /// <param name="state">Estado de la enumeración</param>
        protected override IData OnProcessData(IData data, EEnumerableDataState state)
        {
            List<object> ls = new List<object>();

            if (data != null)
                foreach (object o in data)
                {
                    if (o == null) continue;

                    string cad = Alter(o, Trim, Case);

                    // Control de vacios
                    if (string.IsNullOrEmpty(cad))
                    {
                        if (!ReturnEmpty) continue;
                        cad = "";
                    }

                    ls.Add(cad);
                }

            return Reduce(EReduceZeroEntries.Empty, ls);
        }

        public static string Alter(object o, ETrim trim, ECase cas)
        {
            if (o == null) return null;

            string cad = o.ToString();
            switch (trim)
            {
                case ETrim.All: cad = cad.Trim(new char[] { '\0', ' ', '\n', '\r' }); break;
                case ETrim.End: cad = cad.TrimEnd(new char[] { '\0', ' ', '\n', '\r' }); break;
                case ETrim.Start: cad = cad.TrimStart(new char[] { '\0', ' ', '\n', '\r' }); break;
            }

            switch (cas)
            {
                case ECase.ToUpperInvariant: cad = cad.ToUpperInvariant(); break;
                case ECase.ToUpper: cad = cad.ToUpper(); break;
                case ECase.ToLowerInvariant: cad = cad.ToLowerInvariant(); break;
                case ECase.ToLower: cad = cad.ToLower(); break;
            }

            return cad;
        }
    }
}