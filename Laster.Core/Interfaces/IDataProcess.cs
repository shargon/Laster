using Laster.Core.Classes.Collections;
using Laster.Core.Data;
using Laster.Core.Enums;
using System;
using System.ComponentModel;
using System.Drawing;

namespace Laster.Core.Interfaces
{
    public class IDataProcess : ITopologyItem, ITopologyReltem
    {
        DataProcessCollection _Process;
        DataCollection _Data;
        bool _UseParallel;

        /// <summary>
        /// Procesado de la información
        /// </summary>
        [Browsable(false)]
        public DataProcessCollection Process { get { return _Process; } }
        /// <summary>
        /// Colección de Información
        /// </summary>
        [Browsable(false)]
        internal DataCollection Data { get { return _Data; } }
        /// <summary>
        /// Esperar a que todos los conjuntos de datos esten disponibles para su procesado
        /// </summary>
        [Browsable(false)]
        protected virtual bool WaitForFull { get { return true; } }
        /// <summary>
        /// Usar procesamiento en paralelo
        /// </summary>
        [Category("Process-Mode")]
        [DefaultValue(false)]
        public bool UseParallel { get { return _UseParallel; } set { _UseParallel = value; } }
        /// <summary>
        /// Constructor privado
        /// </summary>
        protected IDataProcess() : base()
        {
            _Process = new DataProcessCollection(this);
            _Data = new DataCollection();
            _UseParallel = false;

            DesignBackColor = Color.Blue;
            DesignForeColor = Color.White;
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
            if (data == null) return;

            // Si tiene varios origenes de datos, se tiene que esperar a estan todos llenos
            IData jdata;
            if (_Data.Count > 1)
            {
                // Esperamos a que el conjunto esperado esté disponible
                if (!_Data.SetData(data) && WaitForFull)
                    return;

                if (_IsBusy) return;
                _IsBusy = true;

                // Los datos a devolver tienen que ser los del array
                jdata = new DataJoin(this, _Data.Items) { HandledDispose = true };
            }
            else
            {
                if (_IsBusy) return;
                _IsBusy = true;

                jdata = data;
            }

            // Procesa los datos
            RaiseOnPreProcess();

            IData ret;
            try
            {
                ret = OnProcessData(jdata, state);
            }
            catch (Exception e)
            {
                OnError(e);
                ret = null;
            }

            // Siempre que no sea null se reenvia a otros nodos
            if (ret != null)
            {
                // Se los envia a otros procesadores
                _Process.ProcessData(ret, _UseParallel);

                // Liberación de recursos
                if (ret != data && !ret.HandledDispose)
                    ret.Dispose();

                RaiseOnPostProcess();
            }

            _IsBusy = false;
        }
        /// <summary>
        /// Evento de que va comenzar todo el proceso
        /// </summary>
        public virtual void OnStart()
        {
            _Process.RaiseOnStart();
        }
        /// <summary>
        /// Evento de que va comenzar todo el proceso
        /// </summary>
        public virtual void OnStop()
        {
            _Process.RaiseOnStop();
            _Data.ClearData();
        }
        /// <summary>
        /// Liberación de recursos
        /// </summary>
        public override void Dispose() { base.Dispose(); OnStop(); _Data.Dispose(); }
    }
}