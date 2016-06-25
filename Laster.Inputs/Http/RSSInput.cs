using Laster.Core.Data;
using Laster.Core.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Laster.Inputs.Http
{
    // Url = http://feeds.feedburner.com/cuantarazon?format=xml
    public class RSSInput : HttpDownloadInput
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public RSSInput() : base() { }

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

        public override string Title { get { return "RSS"; } }

        protected override IData OnGetData()
        {
            IData ret = base.OnGetData();

            if (ret == null || ret is DataEmpty)
                return ret;

            if (ret is DataObject)
            {
                DataObject d = (DataObject)ret;

                byte[] data = null;

                if (d.Data != null && d.Data is string)
                    data = Encoding.UTF8.GetBytes(d.Data.ToString());

                if (data != null)
                {
                    // Ya tenemos la página web, vamos a tratarla
                    using (MemoryStream ms = new MemoryStream(data))
                    using (StreamReader te = new StreamReader(ms, Encoding.UTF8))
                    {
                        Channel[] ar = getChannelQuery(XDocument.Load(te)).ToArray();

                        if (ar == null || ar.Length <= 0) return new DataEmpty(this);

                        if (ar.Length == 1) return new DataObject(this, ar[0]);
                        return new DataArray(this, ar);
                    }
                }
            }

            return new DataEmpty(this);
        }
    }
}