using Laster.Core.Classes.Collections;
using Laster.Core.Data;
using System;

namespace Laster.Core.Interfaces
{
    public class IDataProcess : IDataSource, IDisposable
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
        public bool IsBusy { get { return _IsBusy; } }
        /// <summary>
        /// Salidas de la información
        /// </summary>
        public DataOutputCollection Out { get { return _Out; } }
        /// <summary>
        /// Procesado de la información
        /// </summary>
        public DataProcessCollection Process { get { return _Process; } }
        /// <summary>
        /// Variables
        /// </summary>
        public DataVariableCollection Variables { get { return _Variables; } }
        /// <summary>
        /// Colección de Información
        /// </summary>
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
            _Out = new DataOutputCollection(this);
            _Process = new DataProcessCollection(this);
            _Variables = new DataVariableCollection();
            _Data = new DataCollection();
            _WaitForFull = true;
        }
        /// <summary>
        /// Recibe una información
        /// </summary>
        /// <param name="data">Información</param>
        /// <returns>Devuelve una información</returns>
        protected virtual IData OnProcessData(IData data)
        {
            // Obtiene el dato y se pasa al procesador
            return new EmptyData(this);
        }
        /// <summary>
        /// Procesa los datos
        /// </summary>
        /// <param name="data">Datos</param>
        public void ProcessData(IData data)
        {
            if (_IsBusy) return;
            _IsBusy = true;

            // Si tiene varios origenes de datos, se tiene que esperar a estan todos llenos
            IData ret;
            if (_Data.Count > 1)
            {
                // Acoplamos los datos al array de información actual
                _Data.SetData(data);

                // Esperamos a que el conjunto esperado esté disponible
                if (!_Data.IsFull && _WaitForFull)
                {
                    _IsBusy = false;
                    return;
                }

                // Los datos a devolver tienen que ser los del array
                ret = OnProcessData(_Data.ArrayData);
            }
            else
            {
                // Procesa los datos
                ret = OnProcessData(data);
                //data.Dispose();
            }

            // Se los envia a otros procesadores
            _Process.ProcessData(ret);
            // Se los envia a la salidas de información
            _Out.ProcessData(ret);

            // Liberación de recrusos
            if (ret != data && !ret.HandledDispose)
                ret.Dispose();

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