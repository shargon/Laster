using Laster.Core.Classes.Collections;
using Laster.Core.Classes.RaiseMode;
using Laster.Core.Data;
using System;

namespace Laster.Core.Interfaces
{
    /// <summary>
    /// Entrada de información -> Procesado -> Procesado -> Salida de información
    /// </summary>
    public class IDataInput : IDataSource, IDisposable
    {
        bool _IsBusy;
        DataProcessCollection _Process;
        IDataInputRaiseMode _RaiseMode;
        DataVariableCollection _Variables;

        /// <summary>
        /// Devuelve si está ocupado
        /// </summary>
        public bool IsBusy { get { return _IsBusy; } }
        /// <summary>
        /// Procesado de la información
        /// </summary>
        public DataProcessCollection Process { get { return _Process; } }
        /// <summary>
        /// Modo de lanzamiento de la fuente
        /// </summary>
        public IDataInputRaiseMode RaiseMode { get { return _RaiseMode; } }
        /// <summary>
        /// Variables
        /// </summary>
        public DataVariableCollection Variables { get { return _Variables; } }

        /// <summary>
        /// Constructor privado
        /// </summary>
        protected IDataInput(IDataInputRaiseMode raiseMode)
        {
            _IsBusy = false;
            _RaiseMode = raiseMode;

            _Process = new DataProcessCollection(this);
            _Variables = new DataVariableCollection();
        }

        /// <summary>
        /// Devuelve una información
        /// </summary>
        protected virtual IData OnGetData()
        {
            // Obtiene el dato y se pasa al procesador
            return new EmptyData(this);
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

            if (data == null) data = new EmptyData(this);
            data.IsCached = false;

            _Process.ProcessData(data);
            if (!data.HandledDispose)
                data.Dispose();

            _IsBusy = false;
        }
        /// <summary>
        /// Liberación de recursos
        /// </summary>
        public virtual void Dispose()
        {

        }
    }
}