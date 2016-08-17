using Laster.Core.Interfaces;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Net;

namespace Laster.Inputs.Http
{
    public class HttpDownloadInput : IDataInput
    {
        Uri _Url;

        /// <summary>
        /// Url
        /// </summary>
        [DefaultValue(null)]
        public Uri Url { get { return _Url; } set { _Url = value; } }
        /// <summary>
        /// Credenciales
        /// </summary>
        [DefaultValue("")]
        public string UserName { get; set; }
        /// <summary>
        /// Contraseña
        /// </summary>
        [DefaultValue("")]
        public string Password { get; set; }

        public override string Title { get { return "Remote - Http download"; } }

        public HttpDownloadInput()
        {
            DesignBackColor = Color.DeepPink;
        }

        protected override IData OnGetData()
        {
            using (WebClient c = new WebClient())
            {
                if (!string.IsNullOrEmpty(UserName) || !string.IsNullOrEmpty(Password))
                    c.Credentials = new NetworkCredential(UserName, Password);

                string u = c.DownloadString(_Url);

                if (string.IsNullOrEmpty(u)) return DataEmpty();
                return DataObject(u);
            }
        }
    }
}