using Laster.Core.Classes;
using Laster.Core.Classes.Collections;
using Laster.Core.Data;
using Laster.Core.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Threading;
using System.Linq;

namespace Laster.Core.Interfaces
{
    public class ITopologyItem : NameClass, IDisposable
    {
        DataProcessCollection _Process;
        static int _CurrentId = 0;
        protected bool _IsBusy;
        Semaphore _Wait = new Semaphore(0, 1);
        bool _UseParallel;
        int _Id = 0;

        public delegate void delOnProcess(ITopologyItem sender, EProcessState state);
        public delegate void delOnException(ITopologyItem sender, Exception e);

        public static event delOnException OnException = null;

        [Category("Design")]
        [DefaultValue("")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Comment { get; set; }
        /// <summary>
        /// Usar procesamiento en paralelo
        /// </summary>
        [Category("Process-Mode")]
        [DefaultValue(true)]
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
            _UseParallel = true;
            Id = Interlocked.Increment(ref _CurrentId);

            Name = Title.Split('-').LastOrDefault().Trim();
            _Wait.Release();
        }
        /// <summary>
        /// Lanza el evento de procesado
        /// </summary>
        /// <param name="state">Estado</param>
        protected void RaiseOnProcess(EProcessState state)
        {
            OnProcess?.Invoke(this, state);
        }
        public void OnError(Exception e)
        {
            OnException?.Invoke(this, e);
        }
        /// <summary>
        /// Evento de que va comenzar todo el proceso
        /// </summary>
        public void Start()
        {
            _Wait.WaitOne();

            try
            {
                lock (this)
                {
                    OnStart();
                    _Process.RaiseOnStart();
                }
            }
            catch (Exception e)
            {
                _Wait.Release();
                throw (e);
            }

            _Wait.Release();
        }
        /// <summary>
        /// Evento de que va a parar todo el proceso
        /// </summary>
        public void Stop()
        {
            _Wait.WaitOne();

            try
            {
                lock (this)
                {
                    OnStop();
                    _Process.RaiseOnStop();
                }
            }
            catch (Exception e)
            {
                _Wait.Release();
                throw (e);
            }

            _Wait.Release();
        }
        /// <summary>
        /// Liberación de recursos
        /// </summary>
        public virtual void Dispose() { }
        /// <summary>
        /// Evento de que va comenzar todo el proceso
        /// </summary>
        protected virtual void OnStart() { }
        /// <summary>
        /// Evento de que va a parar todo el proceso
        /// </summary>
        protected virtual void OnStop() { }

        #region Helpers
        public DataObject DataObject(object data) { return new DataObject(this, data); }
        public DataEmpty DataEmpty() { return new DataEmpty(this); }
        public DataBreak DataBreak() { return new DataBreak(this); }
        public DataArray DataArray(object[] items) { return new DataArray(this, items); }
        public DataArray DataArray(List<object> items) { return new DataArray(this, items); }
        public DataEnumerable DataEnumerable(IEnumerable<object> items) { return new DataEnumerable(this, items); }
        public IData Reduce(bool useBreak, List<object> v)
        {
            if (v == null)
                return useBreak ? new DataBreak(this) : null;

            switch (v.Count)
            {
                case 0: return useBreak ? new DataBreak(this) : null;
                case 1: return new DataObject(this, v[0]);
                default: return new DataArray(this, v);
            }
        }
        public IData Reduce(bool useBreak, object[] v)
        {
            if (v == null)
                return useBreak ? new DataBreak(this) : null;

            switch (v.Length)
            {
                case 0: return useBreak ? new DataBreak(this) : null;
                case 1: return new DataObject(this, v[0]);
                default: return new DataArray(this, v);
            }
        }
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