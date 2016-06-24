using Laster.Core.Enums;
using Laster.Core.Helpers;
using Laster.Core.Interfaces;
using System;
using System.Net;

namespace Laster.Outputs
{
    //https://msdn.microsoft.com/es-es/library/system.net.httplistener(v=vs.110).aspx
    public class HttpRestOutput : IDataOutput
    {
        //Dictionary<ushort, HttpListener> _Listeners = new Dictionary<ushort, HttpListener>();

        /// <summary>
        /// Prefixes "http://contoso.com:8080/index/" - "http://127.0.0.1:8080/index/"
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

        /// <summary>
        /// Formato
        /// </summary>
        public SerializationHelper.EFormat Format { get; set; }
        /// <summary>
        /// Codificación
        /// </summary>
        public SerializationHelper.EEncoding Encoding { get; set; }

        HttpListener _Listener;
        byte[] _CacheData;

        public HttpRestOutput()
        {
            Format = SerializationHelper.EFormat.Json;
            Encoding = SerializationHelper.EEncoding.UTF8;
        }
        public override void OnCreate()
        {
            if (_Listener != null) return;

            _Listener = new HttpListener();
            foreach (string p in Prefixes)
                _Listener.Prefixes.Add(p);
            _Listener.Start();

            _Listener.BeginGetContext(callContext, null);
        }
        void callContext(IAsyncResult ar)
        {
            HttpListenerContext cn = _Listener.EndGetContext(ar);

            if (_CacheData == null)
            {
                cn.Response.Abort();
            }
            else
            {
                cn.Response.ContentType = SerializationHelper.GetMimeType(Format);
                cn.Response.ContentEncoding = SerializationHelper.GetEncoding(Encoding);
                cn.Response.OutputStream.Write(_CacheData, 0, _CacheData.Length);
                cn.Response.Close();
            }

            _Listener.BeginGetContext(callContext, null);
        }

        /// <summary>
        /// Liberación de recursos
        /// </summary>
        public override void Dispose()
        {
            if (_Listener != null)
                _Listener.Stop();

            base.Dispose();
        }
        /// <summary>
        /// Saca el contenido de los datos a un Rest
        /// </summary>
        /// <param name="data">Datos</param>
        /// <param name="state">Estado de la enumeración</param>
        protected override void OnProcessData(IData data, EEnumerableDataState state)
        {
            // Cacheamos la respuesta
            _CacheData = SerializationHelper.Serialize(data.GetInternalObject(), Encoding, Format);
        }
    }
}