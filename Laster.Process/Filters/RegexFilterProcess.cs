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

namespace Laster.Process.Filters
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

        public override string Title { get { return "Filters - Regex"; } }

        public RegexFilterProcess()
        {
            DesignBackColor = Color.Blue;
        }

        /// <summary>
        /// Saca el contenido de los datos a un archivo
        /// </summary>
        /// <param name="data">Datos</param>
        /// <param name="state">Estado de la enumeración</param>
        protected override IData OnProcessData(IData data, EEnumerableDataState state)
        {
            if (Regex == null) return DataEmpty();

            List<string> l = new List<string>();
            foreach (object d in data)
            {
                Match m = Regex.Match(d.ToString());
                if (m.Success) l.Add(m.Value);
            }

            if (l.Count == 0) return DataEmpty();
            if (l.Count == 1) return DataObject(l[0]);
            return DataArray(l.ToArray());
        }
    }
}