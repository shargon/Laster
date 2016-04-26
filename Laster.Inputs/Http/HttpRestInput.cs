using Laster.Core.Classes.RaiseMode;
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
        public NetworkCredential Credentials { get { return _Credentials; } set { _Credentials = value; } }

        /// <summary>
        /// Constructor
        /// </summary>
        public HttpRestInput() : base(new DataInputInterval())
        {

        }

        protected override IData OnGetData()
        {
            return base.OnGetData();
        }
    }
}