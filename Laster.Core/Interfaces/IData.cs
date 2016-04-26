using System;

namespace Laster.Core.Interfaces
{
    public class IData : IName, IDisposable
    {
        bool _IsCached;
        IDataSource _Origin;
        bool _HandleedDispose = false;

        /// <summary>
        /// Origen
        /// </summary>
        internal IDataSource Origin { get { return _Origin; } }
        public bool HandledDispose { get { return _HandleedDispose; } set { _HandleedDispose = value; } }
        /// <summary>
        /// Devuelve o establece si está cacheada
        /// </summary>
        public bool IsCached { get { return _IsCached; } set { _IsCached = value; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="origin">Origen</param>
        protected IData(IDataSource origin)
        {
            _IsCached = false;
            _Origin = origin;

            if (origin != null) Name = origin.Name;
            else Name = "";
        }
        /// <summary>
        /// Liberación de recursos
        /// </summary>
        public virtual void Dispose()
        {
            _HandleedDispose = true;
        }
    }
}