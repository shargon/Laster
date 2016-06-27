using Laster.Core.Classes.Collections;
using Laster.Core.Data;
using Laster.Core.Enums;
using System;
using System.ComponentModel;

namespace Laster.Core.Interfaces
{
    public class IDataOutput : ITopologyItem
    {
        DataCollection _Data;

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
        /// Constructor privado
        /// </summary>
        protected IDataOutput() : base()
        {
            _Data = new DataCollection();
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

            try
            {
                OnProcessData(jdata, state);
            }
            catch (Exception e)
            {
                OnError(e);
            }

            // Liberación de recursos
            if (jdata != data && !jdata.HandledDispose)
                jdata.Dispose();

            RaiseOnPostProcess();
            _IsBusy = false;
        }
        /// <summary>
        /// Obtiene el dato y se pasa al procesador
        /// </summary>
        /// <param name="data">Información</param>
        protected virtual void OnProcessData(IData data, EEnumerableDataState state) { }
        /// <summary>
        /// Evento de que va comenzar todo el proceso
        /// </summary>
        public virtual void OnCreate() { }
    }
}