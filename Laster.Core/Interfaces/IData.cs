using System;
using System.Collections;
using System.Collections.Generic;

namespace Laster.Core.Interfaces
{
    public class IData : IDisposable, IEnumerable<object>
    {
        bool _IsCached;
        IDataSource _Source;
        bool _HandleedDispose = false;
        DateTime _CreateUtc = DateTime.UtcNow;

        /// <summary>
        /// Origen
        /// </summary>
        internal IDataSource Source { get { return _Source; } }
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
        internal protected IData(IDataSource source)
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
    }
}