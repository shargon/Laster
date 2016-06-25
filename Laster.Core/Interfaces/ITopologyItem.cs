using Laster.Core.Classes;
using System;
using System.ComponentModel;
using System.Threading;

namespace Laster.Core.Interfaces
{
    public class ITopologyItem : NameClass, IDisposable
    {
        static int _CurrentId = 0;

        public delegate void delProcess(ITopologyItem sender);
        /// <summary>
        /// Eventos de pre-procesado y post-procesado
        /// </summary>
        public event delProcess OnPreProcess, OnPostProcess;

        int _Id = 0;
        /// <summary>
        /// Identificador para la generación de relaciones
        /// </summary>
        [Browsable(false)]
        public int Id
        {
            get { return _Id; }
            set
            {
                _Id = value;
                if (_Id > _CurrentId)
                    _CurrentId = _Id;
            }
        }
        /// <summary>
        /// Tag
        /// </summary>
        [Browsable(false)]
        public object Tag { get; set; }
        /// <summary>
        /// Constructor protegido
        /// </summary>
        protected ITopologyItem()
        {
            Id = Interlocked.Increment(ref _CurrentId);
        }
        /// <summary>
        /// Lanza el evento de pre-procesado
        /// </summary>
        protected void RaiseOnPreProcess()
        {
            if (OnPreProcess != null) OnPreProcess(this);
        }
        /// <summary>
        /// Lanza el evento de post-procesado
        /// </summary>
        protected void RaiseOnPostProcess()
        {
            if (OnPostProcess != null) OnPostProcess(this);
        }
        /// <summary>
        /// Liberación de recursos
        /// </summary>
        public virtual void Dispose()
        {
        }
    }
}