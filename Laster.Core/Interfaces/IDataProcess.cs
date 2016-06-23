using Laster.Core.Classes.Collections;
using Laster.Core.Data;
using Laster.Core.Enums;
using System;
using System.ComponentModel;

namespace Laster.Core.Interfaces
{
    public class IDataProcess : ITopologyItem, IDataSource, IDisposable
    {
        bool _IsBusy;
        DataOutputCollection _Out;
        DataProcessCollection _Process;
        DataVariableCollection _Variables;
        DataCollection _Data;
        bool _WaitForFull;

        /// <summary>
        /// Devuelve si está ocupado
        /// </summary>
        [Browsable(false)]
        public bool IsBusy { get { return _IsBusy; } }
        /// <summary>
        /// Salidas de la información
        /// </summary>
        [Browsable(false)]
        public DataOutputCollection Out { get { return _Out; } }
        /// <summary>
        /// Procesado de la información
        /// </summary>
        [Browsable(false)]
        public DataProcessCollection Process { get { return _Process; } }
        /// <summary>
        /// Variables
        /// </summary>
        [Browsable(false)]
        public DataVariableCollection Variables { get { return _Variables; } }
        /// <summary>
        /// Colección de Información
        /// </summary>
        [Browsable(false)]
        internal DataCollection Data { get { return _Data; } }
        /// <summary>
        /// Esperar a que todos los conjuntos de datos esten disponibles para su procesado
        /// </summary>
        public bool WaitForFull { get { return _WaitForFull; } set { _WaitForFull = value; } }
        /// <summary>
        /// Constructor privado
        /// </summary>
        protected IDataProcess()
        {
            _IsBusy = false;
            _Out = new DataOutputCollection();
            _Process = new DataProcessCollection(this);
            _Variables = new DataVariableCollection();
            _Data = new DataCollection();
            _WaitForFull = true;
        }
        /// <summary>
        /// Recibe una información
        /// </summary>
        /// <param name="data">Información</param>
        /// <param name="state">Estado de la enumeración</param>
        /// <returns>Devuelve una información</returns>
        protected virtual IData OnProcessData(IData data, EEnumerableDataState state)
        {
            return data;
        }
        /// <summary>
        /// Procesa los datos
        /// </summary>
        /// <param name="data">Datos</param>
        /// <param name="state">Estado de la enumeración</param>
        public void ProcessData(IData data, EEnumerableDataState state)
        {
            // Si tiene varios origenes de datos, se tiene que esperar a estan todos llenos
            IData ret;
            if (_Data.Count > 1)
            {
                // Acoplamos los datos al array de información actual
                _Data.SetData(data);

                // Esperamos a que el conjunto esperado esté disponible
                if (_WaitForFull && !_Data.IsFull)
                    return;

                if (_IsBusy) return;
                _IsBusy = true;

                // Los datos a devolver tienen que ser los del array
                ret = OnProcessData(new DataArray(this, _Data.Items), state);
            }
            else
            {
                if (_IsBusy) return;

                _IsBusy = true;
                // Procesa los datos
                ret = OnProcessData(data, state);
            }

            if (ret == null) ret = new DataEmpty(this);

            // Se los envia a otros procesadores
            _Process.ProcessData(_Out, ret);

            // Liberación de recursos
            if (ret != null && ret != data && !ret.HandledDispose)
                ret.Dispose();

            _IsBusy = false;
        }
        /// <summary>
        /// Evento de que va comenzar todo el proceso
        /// </summary>
        public virtual void OnCreate()
        {
            _Process.RaiseOnCreate();
            _Out.RaiseOnCreate();
        }
        /// <summary>
        /// Liberación de recursos
        /// </summary>
        public virtual void Dispose() { }
    }
}