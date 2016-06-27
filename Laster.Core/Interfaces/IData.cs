using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Laster.Core.Helpers;
using System.Text;

namespace Laster.Core.Interfaces
{
    public class IData : IDisposable, IEnumerable<object>
    {
        bool _IsCached;
        ITopologyItem _Source;
        bool _HandleedDispose = false;
        DateTime _CreateUtc = DateTime.UtcNow;

        /// <summary>
        /// Origen
        /// </summary>
        internal ITopologyItem Source { get { return _Source; } }
        /// <summary>
        /// Se han liberado los datos
        /// </summary>
        internal bool HandledDispose { get { return _HandleedDispose; } set { _HandleedDispose = value; } }

        /// <summary>
        /// Devuelve o establece si está cacheada
        /// </summary>
        public bool IsCached { get { return _IsCached; } }
        /// <summary>
        /// Fecha de creación en UTC
        /// </summary>
        public DateTime CreateUtc { get { return _CreateUtc; } }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">Origen</param>
        internal protected IData(ITopologyItem source)
        {
            _IsCached = false;
            _Source = source;
        }
        /// <summary>
        /// Lo marca como cacheado
        /// </summary>
        internal void MarkAsCached() { _IsCached = true; }
        /// <summary>
        /// Liberación de recursos
        /// </summary>
        public virtual void Dispose()
        {
            _HandleedDispose = true;
        }
        /// <summary>
        /// Obtiene el objeto interno
        /// </summary>
        public virtual object GetInternalObject() { return null; }
        public override string ToString()
        {
            if (_Source == null) return base.ToString();
            return _Source.Name;
        }

        public virtual IEnumerator<object> GetEnumerator()
        {
            yield break;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        /// <summary>
        /// Convierte el stream a un buffer de memoria
        /// </summary>
        /// <param name="stringEncoding">Codificación para los string</param>
        public MemoryStream ToStream(SerializationHelper.EEncoding stringEncoding)
        {
            MemoryStream stream = new MemoryStream();

            Encoding codec = SerializationHelper.GetEncoding(stringEncoding);
            foreach (object d in this)
            {
                if (d == null) continue;

                if (d is byte[])
                {
                    byte[] cad = (byte[])d;
                    stream.Write(cad, 0, cad.Length);
                }
                else
                {
                    byte[] cad = codec.GetBytes(d.ToString());
                    stream.Write(cad, 0, cad.Length);
                }
            }

            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
}