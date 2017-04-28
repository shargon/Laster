using Laster.Core.Enums;
using Laster.Core.Interfaces;
using Laster.Process.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;

namespace Laster.Process.Strings
{
    /// <summary>
    /// Formatea unos datos
    /// </summary>
    public class StringBuilderProcess : IDataProcess
    {
        [DefaultValue(StringSatenization.ESanetizationType.None)]
        public StringSatenization.ESanetizationType Sanetization { get; set; }

        /// <summary>
        /// Devuelve
        /// </summary>
        [DefaultValue("{dd}/{MM}/{yyyy} - {Data}")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Return { get; set; }

        /// <summary>
        /// Remplaza los comodines de las propiedades
        /// </summary>
        [DefaultValue(false)]
        public bool ReplaceProperties { get; set; }
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
            ReplaceProperties = false;
            Return = "{dd}/{MM}/{yyyy} - {Data}";
        }
        protected override IData OnProcessData(IData data, EEnumerableDataState state)
        {
            List<object> l = new List<object>();

            foreach (object o in data)
            {
                string s = FormatStr(o);
                if (string.IsNullOrEmpty(s)) continue;

                l.Add(s);
            }

            return Reduce(EReduceZeroEntries.Break, l);
        }
        public string FormatStr(object o)
        {
            if (o == null) return "";

            string s = o.ToString();
            if (string.IsNullOrEmpty(s)) return "";

            s = Return.Replace("{Data}", s);

            if (ReplaceDateFormat)
                foreach (string pic in new string[] { "yyyy", "MM", "dd", "HH", "hh", "mm", "ss" })
                    s = s.Replace("{" + pic + "}", DateTime.Now.ToString(pic));

            if (ExpandEnvironmentVariables)
                s = Environment.ExpandEnvironmentVariables(s);

            if (ReplaceProperties)
            {
                foreach (PropertyInfo pi in o.GetType().GetProperties())
                    s = s.Replace("{" + pi.Name + "}", StringSatenization.Format(pi.GetValue(o, null), Sanetization));
            }

            return s;
        }
    }
}