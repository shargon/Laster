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
        ECallMode _CallMethod;
        DataCollection _Data;

        public enum ECallMode : byte
        {
            /// <summary>
            /// Espera a que se reciba un dato de cada origen
            /// </summary>
            WaitAll = 0,
            /// <summary>
            /// Se le llama siempre que se recibe una llamada
            /// </summary>
            Quickly = 1,
            /// <summary>
            /// Se llaman de manera independiente cada llamada
            /// </summary>
            PeerCall = 2
        }

        /// <summary>
        /// Colección de Información
        /// </summary>
        [Browsable(false)]
        internal DataCollection Data { get { return _Data; } }
        /// <summary>
        /// Esperar a que todos los conjuntos de datos esten disponibles para su procesado
        /// </summary>
        [DefaultValue(ECallMode.WaitAll)]
        [Description(
            @"WaitAll=Espera a que todos los elementos estén disponibles (Enumerador - Sin nulos)
Quickly=Siempre que recibe datos los envia (Enumerador - Puede haber nulos)
PeerCall=Envia de manera independiente cada llamada (Se pueden pisar)")]
        [Category("Process-Mode")]
        public ECallMode CallMethod { get { return _CallMethod; } set { _CallMethod = value; } }
        /// <summary>
        /// Constructor privado
        /// </summary>
        protected IDataProcess() : base()
        {
            _CallMethod = ECallMode.WaitAll;
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
        /// <param name="caller">Item que lo llama</param>
        /// <param name="state">Estado de la enumeración</param>
        public void ProcessData(IData data, ITopologyItem caller, EEnumerableDataState state)
        {
            if (data == null) return;

            // Si tiene varios origenes de datos, se tiene que esperar a estan todos llenos
            IData jdata;
            if (_Data.Count > 1 && CallMethod != ECallMode.PeerCall)
            {
                // Esperamos a que el conjunto esperado esté disponible
                if (!_Data.SetData(data, caller) && CallMethod == ECallMode.WaitAll)
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
            if (ret != null && !(ret is DataBreak))
            {
                // Se los envia a otros procesadores
                Process.ProcessData(this, ret, UseParallel);

                // Liberación de recursos
                if (ret != data && !ret.HandledDispose)
                    ret.Dispose();
            }

            _IsBusy = false;
        }
        protected override void OnStop()
        {
            _Data.ClearData();
        }
        /// <summary>
        /// Liberación de recursos
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            OnStop();
            _Data.Dispose();
        }
    }
}