using Laster.Core.Classes;
using Newtonsoft.Json;
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

        protected bool _IsBusy;
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
        /// Devuelve si está ocupado
        /// </summary>
        [Browsable(false)]
        public bool IsBusy { get { return _IsBusy; } }
        /// <summary>
        /// Tag
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public object Tag { get; set; }
        /// <summary>
        /// Título a mostrar
        /// </summary>
        [Browsable(false)]
        public virtual string Title { get { return GetType().Name; } }
        /// <summary>
        /// Constructor protegido
        /// </summary>
        protected ITopologyItem()
        {
            Id = Interlocked.Increment(ref _CurrentId);
            Name = Title;
            _IsBusy = false;
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
        public virtual void Dispose() { }
        protected void OnError(Exception e)
        {
            throw e;
        }
        public override string ToString()
        {
            if (string.IsNullOrEmpty(Name))
                return Title;

            string cn = Title;

            if (Name == cn) return Name;
            return Name + " (" + cn + ")";
        }
    }
}