using Laster.Core.Classes.Collections;
using Laster.Core.Classes.RaiseMode;
using Laster.Core.Data;
using Laster.Core.Designer;
using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace Laster.Core.Interfaces
{
    /// <summary>
    /// Entrada de información -> Procesado -> Procesado -> Salida de información
    /// </summary>
    public class IDataInput : ITopologyItem, ITopologyReltem
    {
        bool _UseParallel;
        DataOutputCollection _Out;
        DataProcessCollection _Process;
        IRaiseMode _RaiseMode;

        /// <summary>
        /// Procesado de la información
        /// </summary>
        [Browsable(false)]
        public DataProcessCollection Process { get { return _Process; } }
        /// <summary>
        /// Salidas de la información
        /// </summary>
        [Browsable(false)]
        public DataOutputCollection Out { get { return _Out; } }
        /// <summary>
        /// Usar procesamiento en paralelo
        /// </summary>
        [Category("Process-Mode")]
        [DefaultValue(true)]
        public bool UseParallel { get { return _UseParallel; } set { _UseParallel = value; } }
        /// <summary>
        /// Modo de lanzamiento de la fuente
        /// </summary>
        [Category("General")]
        [Editor(typeof(DataInputRaiseEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public IRaiseMode RaiseMode
        {
            get { return _RaiseMode; }
            set
            {
                if (value == _RaiseMode) return;
                if (_RaiseMode != null && value != null)
                {
                    if (_RaiseMode.IsStarted)
                    {
                        _RaiseMode.Stop(this);
                        if (!value.IsStarted) value.Start(this);
                    }
                    else
                    {
                        if (value.IsStarted) value.Stop(this);
                    }
                }
                _RaiseMode = value;
            }
        }

        /// <summary>
        /// Constructor privado
        /// </summary>
        protected IDataInput() : base()
        {
            RaiseMode = new DataInputTimer();
            _Out = new DataOutputCollection(this);
            _Process = new DataProcessCollection(this);
            _UseParallel = true;
        }
        /// <summary>
        /// Procesa los datos
        /// </summary>
        public void ProcessData()
        {
            if (_IsBusy) return;
            _IsBusy = true;

            RaiseOnPreProcess();

            // Obtiene los datos del origen
            IData data;
            try
            {
                data = OnGetData();
            }
            catch (Exception e)
            {
                OnError(e);
                data = null;
            }

            if (data != null)
            {
                _Process.ProcessData(_Out, data, _UseParallel);

                // Liberación de recrusos
                if (!data.HandledDispose)
                    data.Dispose();

                RaiseOnPostProcess();
            }

            _IsBusy = false;
        }
        /// <summary>
        /// Devuelve una información
        /// </summary>
        protected virtual IData OnGetData()
        {
            // Obtiene el dato y se pasa al procesador
            return new DataEmpty(this);
        }
        /// <summary>
        /// Evento de que va comenzar todo el proceso
        /// </summary>
        public virtual void OnCreate()
        {
            _Process.RaiseOnCreate();
            _Out.RaiseOnCreate();
        }
    }
}