using Laster.Core.Converters;
using Laster.Core.Designer;
using Laster.Core.Enums;
using Laster.Core.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Text.RegularExpressions;

namespace Laster.Process.Strings
{
    /// <summary>
    /// Filtro de regex
    /// </summary>
    public class RegexFilterProcess : IDataProcess
    {
        [Category("Filter")]
        [JsonConverter(typeof(ImprovedRegexConverter))]
        [DefaultValue(null)]
        [TypeConverter(typeof(RegexConverter))]
        [Editor(typeof(RegexEditor), typeof(UITypeEditor))]
        public Regex Regex { get; set; }

        /// <summary>
        /// Define si lo que se espera es que lo cumpla
        /// </summary>
        [DefaultValue(true)]
        public bool Expected { get; set; }

        public override string Title { get { return "Strings - Regex"; } }

        public RegexFilterProcess()
        {
            DesignBackColor = Color.Blue;
            Expected = true;
        }

        /// <summary>
        /// Saca el contenido de los datos a un archivo
        /// </summary>
        /// <param name="data">Datos</param>
        /// <param name="state">Estado de la enumeración</param>
        protected override IData OnProcessData(IData data, EEnumerableDataState state)
        {
            if (Regex == null) return DataEmpty();

            List<object> l = new List<object>();
            foreach (object d in data)
            {
                Match m = Regex.Match(d.ToString());

                if (Expected)
                {
                    if (m.Success) l.Add(m.Value);
                }
                else
                {
                    if (!m.Success) l.Add(m.Value);
                }
            }

            return Reduce(EReduceZeroEntries.Empty, l);
        }
    }
}