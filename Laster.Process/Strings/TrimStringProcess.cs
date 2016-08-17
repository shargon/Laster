using Laster.Core.Enums;
using Laster.Core.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace Laster.Process.Strings
{
    public class TrimStringProcess : IDataProcess
    {
        public enum ETrim : byte
        {
            None = 0,
            All = 1,
            Start = 2,
            End = 3
        }

        /// <summary>
        /// Realizar un trim antes de la comprobación
        /// </summary>
        [DefaultValue(ETrim.All)]
        public ETrim Trim { get; set; }
        [DefaultValue(false)]
        public bool ReturnEmpty { get; set; }

        public override string Title { get { return "Strings - Trim"; } }

        public TrimStringProcess()
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

                    string cad = o.ToString();
                    switch (Trim)
                    {
                        case ETrim.All: cad = cad.Trim(); break;
                        case ETrim.End: cad = cad.TrimEnd(); break;
                        case ETrim.Start: cad = cad.TrimStart(); break;
                    }

                    // Control de vacios
                    if (string.IsNullOrEmpty(cad))
                    {
                        if (!ReturnEmpty) continue;
                        cad = "";
                    }

                    ls.Add(cad);
                }

            return Reduce(false, ls);
        }
    }
}