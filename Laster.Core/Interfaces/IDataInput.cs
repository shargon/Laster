using Laster.Core.Classes.RaiseMode;
using Laster.Core.Data;
using Laster.Core.Designer;
using Laster.Core.Enums;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace Laster.Core.Interfaces
{
    /// <summary>
    /// Entrada de información -> Procesado -> Procesado -> Salida de información
    /// </summary>
    public class IDataInput : ITopologyItem
    {
        IRaiseMode _RaiseMode;

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
            DesignBackColor = Color.Green;
            DesignForeColor = Color.White;
        }
        /// <summary>
        /// Procesa los datos
        /// </summary>
        public void ProcessData()
        {
            if (_IsBusy) return;
            _IsBusy = true;

            RaiseOnProcess(EProcessState.PreProcess);

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

            RaiseOnProcess(EProcessState.PostProcess);

            if (data != null)
            {
                Process.ProcessData(data, UseParallel);

                // Liberación de recrusos
                if (!data.HandledDispose)
                    data.Dispose();
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
    }
}