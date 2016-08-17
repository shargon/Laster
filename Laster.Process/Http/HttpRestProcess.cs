using Laster.Core.Enums;
using Laster.Core.Helpers;
using Laster.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;

namespace Laster.Process.Http
{
    //https://msdn.microsoft.com/es-es/library/system.net.httplistener(v=vs.110).aspx
    public class HttpRestProcess : IDataProcess
    {
        string[] _Prefixes;

        /// <summary>
        /// Prefixes "http://contoso.com:8080/index/" - "http://127.0.0.1:8080/index/"
        /// </summary>
        [DefaultValue(null)]
        public string[] Prefixes
        {
            get { return _Prefixes; }
            set
            {
                _Prefixes = value;
            }
        }
        /// <summary>
        /// User name
        /// </summary>
        [DefaultValue("")]
        public string UserName { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        [DefaultValue("")]
        public string Password { get; set; }
        /// <summary>
        /// Formato
        /// </summary>
        [DefaultValue("application/json")]
        public string ContentType { get; set; }
        /// <summary>
        /// Codificación
        /// </summary>
        [DefaultValue(SerializationHelper.EEncoding.UTF8)]
        public SerializationHelper.EEncoding StringEncoding { get; set; }

        static HttpListener _Listener;
        static Dictionary<string, delOnRequest> _Responses = new Dictionary<string, delOnRequest>();

        public delegate void delOnRequest(HttpListenerContext cn);

        List<string> _Registered;
        delOnRequest _OnRequest;
        byte[] _CacheData;

        public HttpRestProcess()
        {
            DesignBackColor = Color.Orange;
            _Registered = new List<string>();
            ContentType = SerializationHelper.GetMimeType(SerializationHelper.EFormat.Json);
            StringEncoding = SerializationHelper.EEncoding.UTF8;
            _OnRequest = new delOnRequest(onRequest);
        }
        public override void OnStart()
        {
            if (_Listener == null)
            {
                _Listener = new HttpListener();
            }

            foreach (string url in Prefixes)
            {
                if (_Registered.Contains(url)) continue;

                if (!_Listener.Prefixes.Contains(url))
                {
                    _Listener.Prefixes.Add(url);
                    _Responses.Add(url, _OnRequest);
                    _Registered.Add(url);
                }
                else
                {
                    throw (new Exception("Prefix already registered"));
                }
            }

            if (!_Listener.IsListening)
            {
                _Listener.Start();
                _Listener.BeginGetContext(callContext, null);
            }
        }
        // Escuchar peticiones global
        static void callContext(IAsyncResult ar)
        {
            try
            {
                HttpListenerContext cn = _Listener.EndGetContext(ar);

                // Identificar cual es el evento a llamar por su Prefijo

                bool resp = false;
                foreach (string url in _Responses.Keys)
                {
                    if (cn.Request.Url.OriginalString.StartsWith(url.TrimEnd('/')))
                    {
                        _Responses[url].Invoke(cn);
                        resp = true;
                        break;
                    }
                }

                if (!resp)
                {
                    // Si no llamamos nada, abortamos
                    cn.Response.Abort();
                }

                _Listener.BeginGetContext(callContext, null);
            }
            catch { }
        }

        // Evento local que captura la petición
        void onRequest(HttpListenerContext cn)
        {
            if (_CacheData == null)
            {
                cn.Response.Abort();
            }
            else
            {
                cn.Response.ContentType = ContentType;
                cn.Response.ContentEncoding = SerializationHelper.GetEncoding(StringEncoding);
                cn.Response.OutputStream.Write(_CacheData, 0, _CacheData.Length);
                cn.Response.Close();
            }
        }

        public override string Title { get { return "Http - Rest"; } }

        /// <summary>
        /// Liberación de recursos
        /// </summary>
        public override void OnStop()
        {
            // Desregistrar
            foreach (string p in _Registered)
            {
                _Responses.Remove(p);
                if (_Listener != null)
                {
                    _Listener.Prefixes.Remove(p);
                }
            }
            _Registered.Clear();

            // Cerrar escucha
            if (_Listener != null && _Responses.Count <= 0)
                _Listener.Stop();

            base.OnStop();
        }
        /// <summary>
        /// Saca el contenido de los datos a un Rest
        /// </summary>
        /// <param name="data">Datos</param>
        /// <param name="state">Estado de la enumeración</param>
        protected override IData OnProcessData(IData data, EEnumerableDataState state)
        {
            // Cacheamos la respuesta
            using (MemoryStream stream = data.ToStream(StringEncoding))
                _CacheData = stream.ToArray();

            return data;
        }
    }
}