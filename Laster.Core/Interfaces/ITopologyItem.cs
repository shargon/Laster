using Laster.Core.Classes;
using Laster.Core.Classes.Collections;
using Laster.Core.Data;
using Laster.Core.Enums;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;

namespace Laster.Core.Interfaces
{
    public class ITopologyItem : NameClass, IDisposable
    {
        DataProcessCollection _Process;
        static int _CurrentId = 0;
        protected bool _IsBusy;
        bool _UseParallel;
        int _Id = 0;

        public delegate void delOnProcess(ITopologyItem sender, EProcessState state);
        public delegate void delOnException(ITopologyItem sender, Exception e);

        public static event delOnException OnException = null;

        /// <summary>
        /// Usar procesamiento en paralelo
        /// </summary>
        [Category("Process-Mode")]
        [DefaultValue(false)]
        public bool UseParallel { get { return _UseParallel; } set { _UseParallel = value; } }
        [Category("Design")]
        public Color DesignBackColor { get; set; }
        [Category("Design")]
        public Color DesignForeColor { get; set; }
        /// <summary>
        /// Procesado de la información
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public DataProcessCollection Process { get { return _Process; } }

        /// <summary>
        /// Eventos de pre-procesado y post-procesado
        /// </summary>
        public event delOnProcess OnProcess;

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
        [JsonIgnore]
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
        [JsonIgnore]
        public virtual string Title { get { return GetType().Name; } }
        /// <summary>
        /// Constructor protegido
        /// </summary>
        protected ITopologyItem()
        {
            _Process = new DataProcessCollection(this);
            _IsBusy = false;
            _UseParallel = false;
            Id = Interlocked.Increment(ref _CurrentId);
            Name = Title;
        }
        /// <summary>
        /// Lanza el evento de procesado
        /// </summary>
        /// <param name="state">Estado</param>
        protected void RaiseOnProcess(EProcessState state)
        {
            if (OnProcess != null) OnProcess(this, state);
        }
        /// <summary>
        /// Liberación de recursos
        /// </summary>
        public virtual void Dispose() { }
        public void OnError(Exception e)
        {
            if (OnException != null) OnException(this, e);
        }
        /// <summary>
        /// Evento de que va comenzar todo el proceso
        /// </summary>
        public virtual void OnStart() { _Process.RaiseOnStart(); }
        /// <summary>
        /// Evento de que va a parar todo el proceso
        /// </summary>
        public virtual void OnStop() { _Process.RaiseOnStop(); }

        #region Helpers
        public DataObject DataObject(object data) { return new DataObject(this, data); }
        public DataEmpty DataEmpty() { return new DataEmpty(this); }
        public DataArray DataArray(params object[] items) { return new DataArray(this, items); }
        #endregion

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