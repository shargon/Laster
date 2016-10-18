using Laster.Core.Enums;
using Laster.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;

namespace Laster.Process.Strings
{
    /// <summary>
    /// Formatea unos datos
    /// </summary>
    public class StringBuilderProcess : IDataProcess
    {
        /// <summary>
        /// Devuelve
        /// </summary>
        [DefaultValue("{dd}/{MM}/{yyyy} - {Data}")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Return { get; set; }

        /// <summary>
        /// Remplaza los comodines de fecha
        /// </summary>
        [DefaultValue(true)]
        public bool ReplaceDateFormat { get; set; }
        /// <summary>
        /// Reemplaza las variables de entorno
        /// </summary>
        [DefaultValue(true)]
        public bool ExpandEnvironmentVariables { get; set; }

        public override string Title { get { return "Strings - Builder"; } }

        public StringBuilderProcess()
        {
            DesignBackColor = Color.Blue;

            ExpandEnvironmentVariables = true;
            ReplaceDateFormat = true;
            Return = "{dd}/{MM}/{yyyy} - {Data}";
        }
        protected override IData OnProcessData(IData data, EEnumerableDataState state)
        {
            List<object> l = new List<object>();

            foreach (object o in data)
            {
                if (o == null) continue;

                string s = o.ToString();
                if (string.IsNullOrEmpty(s)) continue;

                l.Add(FormatStr(s));
            }

            return Reduce(EReduceZeroEntries.Empty, l);
        }
        public string FormatStr(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";

            s = Return.Replace("{Data}", s);

            if (ReplaceDateFormat)
                foreach (string pic in new string[] { "yyyy", "MM", "dd", "hh", "mm", "ss" })
                    s = s.Replace("{" + pic + "}", DateTime.Now.ToString(pic));

            if (ExpandEnvironmentVariables)
                s = Environment.ExpandEnvironmentVariables(s);

            return s;
        }
    }
}