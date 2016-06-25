using Laster.Core.Enums;

namespace Laster.Core.Interfaces
{
    public class IDataOutput : ITopologyItem
    {
        /// <summary>
        /// Constructor privado
        /// </summary>
        protected IDataOutput() : base() { }
        /// <summary>
        /// Procesa los datos
        /// </summary>
        /// <param name="data">Datos</param>
        /// <param name="state">Estado de la enumeración</param>
        public void ProcessData(IData data, EEnumerableDataState state)
        {
            // Procesa los datos
            RaiseOnPreProcess();

            OnProcessData(data, state);

            RaiseOnPostProcess();
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