using Laster.Core.Data;
using Laster.Core.Enums;
using Laster.Core.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Web;

namespace Laster.Process.Converters
{
    // Url = http://feeds.feedburner.com/cuantarazon?format=xml
    public class StringSatenization : IDataProcess
    {
        public enum ESanetizationType
        {
            None,
            UrlEncode,
            HtmlEncode,
            UrlDecode,
            HtmlDecode,
        }

        [DefaultValue(true)]
        public bool OnlyReturnItems { get; set; }

        [DefaultValue(ESanetizationType.None)]
        public ESanetizationType Sanetization { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public StringSatenization() : base()
        {
            DesignBackColor = Color.BlueViolet;
            Sanetization = ESanetizationType.None;
        }

        public override string Title { get { return "Converters - String sanetization"; } }

        protected override IData OnProcessData(IData data, EEnumerableDataState state)
        {
            if (data == null || data is DataEmpty)
                return data;

            List<string> ls = new List<string>();
            foreach (object o in data)
            {
                if (o == null) continue;
                ls.Add(Format(o, Sanetization));
            }

            return Reduce(EReduceZeroEntries.Break, ls.ToArray());
        }

        public static string Format(object o, ESanetizationType type)
        {
            if (o == null) return "";

            switch (type)
            {
                case ESanetizationType.UrlEncode: return HttpUtility.UrlEncode(o.ToString());
                case ESanetizationType.HtmlEncode: return HttpUtility.HtmlEncode(o.ToString());
                case ESanetizationType.UrlDecode: return HttpUtility.UrlDecode(o.ToString());
                case ESanetizationType.HtmlDecode: return HttpUtility.HtmlDecode(o.ToString());
            }

            return o.ToString();
        }
    }
}