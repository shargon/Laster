﻿using Laster.Core.Data;
using Laster.Core.Interfaces;
using System;
using System.Net;

namespace Laster.Inputs.Http
{
    public class HttpRestInput : IDataInput
    {
        Uri _Url;
        NetworkCredential _Credentials;

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

        /// <summary>
        /// Constructor
        /// </summary>
        public HttpRestInput() : base() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="raiseMode">Modo de lanzamiento</param>
        public HttpRestInput(IDataInputRaiseMode raiseMode) : base(raiseMode) { }

        protected override IData OnGetData()
        {
            using (WebClient c = new WebClient())
            {
                if (!string.IsNullOrEmpty(UserName) || !string.IsNullOrEmpty(Password))
                    c.Credentials = new NetworkCredential(UserName, Password);

                string u = c.DownloadString(_Url);

                if (string.IsNullOrEmpty(u)) return new DataEmpty(this);
                return new DataObject(this, u);
            }
        }
    }
}