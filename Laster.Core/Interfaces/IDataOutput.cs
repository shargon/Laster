using Laster.Core.Classes.Collections;
using Laster.Core.Enums;
using System;
using System.ComponentModel;

namespace Laster.Core.Interfaces
{
    public class IDataOutput : ITopologyItem, IDisposable
    {
        DataVariableCollection _Variables;

        /// <summary>
        /// Variables
        /// </summary>
        [Browsable(false)]
        public DataVariableCollection Variables { get { return _Variables; } }
        /// <summary>
        /// Constructor privado
        /// </summary>
        protected IDataOutput()
        {
            _Variables = new DataVariableCollection();
        }
        /// <summary>
        /// Procesa los datos
        /// </summary>
        /// <param name="data">Datos</param>
        /// <param name="state">Estado de la enumeración</param>
        public void ProcessData(IData data, EEnumerableDataState state)
        {
            // Procesa los datos
            OnProcessData(data, state);
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
        /// <summary>
        /// Liberación de los recursos
        /// </summary>
        public virtual void Dispose() { }
    }
}