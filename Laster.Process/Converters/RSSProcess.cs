using Laster.Core.Data;
using Laster.Core.Enums;
using Laster.Core.Interfaces;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Laster.Process.Converters
{
    // Url = http://feeds.feedburner.com/cuantarazon?format=xml
    public class RSSProcess : IDataProcess
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public RSSProcess() : base()
        {
            DesignBackColor = Color.BlueViolet;
        }

        #region Clases
        public class Channel
        {
            public string Title { get; set; }
            public string Link { get; set; }
            public string Description { get; set; }
            public string Generator { get; set; }
            public Item[] Items { get; set; }

            public override string ToString() { return Title; }
        }
        public class Item
        {
            public string Title { get; set; }
            public string Link { get; set; }
            public string Description { get; set; }
            public string PublishDate { get; set; }
            public string Guid { get; set; }

            public override string ToString() { return Title; }
        }
        #endregion

        static IEnumerable<Channel> getChannelQuery(XDocument xdoc)
        {
            return from channels in xdoc.Descendants("channel")
                   select new Channel
                   {
                       Title = channels.Element("title") != null ? channels.Element("title").Value : "",
                       Link = channels.Element("link") != null ? channels.Element("link").Value : "",
                       Generator = channels.Element("generator") != null ? channels.Element("generator").Value : "",
                       Description = channels.Element("description") != null ? channels.Element("description").Value : "",
                       Items = (from items in channels.Descendants("item")
                                select new Item
                                {
                                    Title = items.Element("title") != null ? items.Element("title").Value : "",
                                    Link = items.Element("link") != null ? items.Element("link").Value : "",
                                    Description = items.Element("description") != null ? items.Element("description").Value : "",
                                    PublishDate = items.Element("pubDate") != null ? items.Element("pubDate").Value : "",
                                    Guid = (items.Element("guid") != null ? items.Element("guid").Value : "")
                                }
                               ).ToArray()
                   };
        }

        public override string Title { get { return "Converters - Parse RSS"; } }

        protected override IData OnProcessData(IData data, EEnumerableDataState state)
        {
            if (data == null || data is DataEmpty)
                return data;

            if (data is DataObject)
            {
                DataObject d = (DataObject)data;

                byte[] buff = null;

                if (d.Data != null && d.Data is string)
                    buff = Encoding.UTF8.GetBytes(d.Data.ToString());

                if (buff != null)
                {
                    // Ya tenemos la página web, vamos a tratarla
                    using (MemoryStream ms = new MemoryStream(buff))
                    using (StreamReader te = new StreamReader(ms, Encoding.UTF8))
                    {
                        Channel[] ar = getChannelQuery(XDocument.Load(te)).ToArray();

                        if (ar == null || ar.Length <= 0) return DataEmpty();

                        if (ar.Length == 1) return DataObject(ar[0]);
                        return DataArray(ar);
                    }
                }
            }

            return DataEmpty();
        }
    }
}