using Laster.Core.Interfaces;
using System;
using System.Net;

namespace Laster.Inputs.Http
{
    public class HttpDownloadInput : IDataInput
    {
        Uri _Url;

        /// <summary>
        /// Url
        /// </summary>
        public Uri Url { get { return _Url; } set { _Url = value; } }
        /// <summary>
        /// Credenciales
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Contraseña
        /// </summary>
        public string Password { get; set; }

        public override string Title { get { return "Http - Download"; } }

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