using Laster.Core.Helpers;
using Laster.Core.Interfaces;
using System.Collections.Generic;
using System.Net;

namespace Laster.Outputs
{
    //https://msdn.microsoft.com/es-es/library/system.net.httplistener(v=vs.110).aspx
    public class HttpRestOutput : IDataOutput
    {
        //Dictionary<ushort, HttpListener> _Listeners = new Dictionary<ushort, HttpListener>();

        /// <summary>
        /// Prefixes "http://contoso.com:8080/index/".
        /// </summary>
        public string[] Prefixes { get; set; }
        /// <summary>
        /// Resuorce name (Default: /)
        /// </summary>
        public string Resource { get; set; }
        /// <summary>
        /// User name
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        HttpListener _Listener;
        string _CacheData;

        public HttpRestOutput()
        {
            _Listener = new HttpListener();
            _Listener.Start();
            //_Listener.BeginGetContext();
        }

        public override void Dispose()
        {
            base.Dispose();

            if (_Listener != null)
                _Listener.Stop();
        }

        protected override void OnProcessData(IData data)
        {
            // Cacheamos la respuesta
            _CacheData = LasterHelper.Data2Json(data);


            base.OnProcessData(data);
        }
    }
}