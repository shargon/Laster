using Laster.Core.Classes.Collections;
using Laster.Core.Data;
using Laster.Core.Enums;
using System;
using System.ComponentModel;
using System.Drawing;

namespace Laster.Core.Interfaces
{
    public class IDataProcess : ITopologyItem
    {
        bool _WaitForFull;
        DataCollection _Data;

        /// <summary>
        /// Colección de Información
        /// </summary>
        [Browsable(false)]
        internal DataCollection Data { get { return _Data; } }
        /// <summary>
        /// Esperar a que todos los conjuntos de datos esten disponibles para su procesado
        /// </summary>
        [DefaultValue(true)]
        [Category("Process-Mode")]
        public bool WaitForFull { get { return _WaitForFull; } set { _WaitForFull = value; } }
        /// <summary>
        /// Constructor privado
        /// </summary>
        protected IDataProcess() : base()
        {
            _WaitForFull = true;
            _Data = new DataCollection();

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
            RaiseOnProcess(EProcessState.PreProcess);

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

            RaiseOnProcess(EProcessState.PostProcess);

            // Siempre que no sea null se reenvia a otros nodos
            if (ret != null)
            {
                // Se los envia a otros procesadores
                Process.ProcessData(ret, UseParallel);

                // Liberación de recursos
                if (ret != data && !ret.HandledDispose)
                    ret.Dispose();
            }

            _IsBusy = false;
        }
        public override void OnStop()
        {
            _Data.ClearData();
            base.OnStop();
        }
        /// <summary>
        /// Liberación de recursos
        /// </summary>
        public override void Dispose() { base.Dispose(); OnStop(); _Data.Dispose(); }
    }
}