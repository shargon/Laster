using Laster.Core.Classes.Collections;
using Laster.Core.Classes.RaiseMode;
using Laster.Core.Data;
using System;
using System.ComponentModel;

namespace Laster.Core.Interfaces
{
    /// <summary>
    /// Entrada de información -> Procesado -> Procesado -> Salida de información
    /// </summary>
    public class IDataInput : ITopologyItem, IDataSource, IDisposable
    {
        bool _IsBusy;
        DataOutputCollection _Out;
        DataProcessCollection _Process;
        IDataInputRaiseMode _RaiseMode;
        DataVariableCollection _Variables;

        /// <summary>
        /// Devuelve si está ocupado
        /// </summary>
        [Browsable(false)]
        public bool IsBusy { get { return _IsBusy; } }
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
        /// Modo de lanzamiento de la fuente
        /// </summary>
        [Browsable(false)]
        public IDataInputRaiseMode RaiseMode { get { return _RaiseMode; } }
        /// <summary>
        /// Variables
        /// </summary>
        [Browsable(false)]
        public DataVariableCollection Variables { get { return _Variables; } }

        /// <summary>
        /// Constructor privado
        /// </summary>
        protected IDataInput() : this(new DataInputInterval()) { }
        /// <summary>
        /// Constructor privado
        /// </summary>
        /// <param name="raiseMode">Modo de venta</param>
        protected IDataInput(IDataInputRaiseMode raiseMode)
        {
            _IsBusy = false;
            _RaiseMode = raiseMode;

            _Out = new DataOutputCollection();
            _Process = new DataProcessCollection(this);
            _Variables = new DataVariableCollection();
        }
        /// <summary>
        /// Procesa los datos
        /// </summary>
        public void ProcessData()
        {
            if (_IsBusy) return;
            _IsBusy = true;

            // Obtiene los datos del origen
            IData data = OnGetData();

            if (data == null) data = new DataEmpty(this);
            _Process.ProcessData(_Out, data);

            // Liberación de recrusos
            if (!data.HandledDispose)
                data.Dispose();

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
        public virtual void OnCreate() { _Process.RaiseOnCreate(); }
        /// <summary>
        /// Liberación de recursos
        /// </summary>
        public virtual void Dispose() { }
    }
}