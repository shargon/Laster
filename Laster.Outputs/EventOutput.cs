using Laster.Core.Classes.RaiseMode;
using Laster.Core.Enums;
using Laster.Core.Interfaces;

namespace Laster.Outputs
{
    public class EventOutput : IDataOutput
    {
        /// <summary>
        /// Nombre del evento
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public EventOutput() { }
        /// <summary>
        /// Saca el contenido de los datos a un archivo
        /// </summary>
        /// <param name="data">Datos</param>
        /// <param name="state">Estado de la enumeración</param>
        protected override void OnProcessData(IData data, EEnumerableDataState state)
        {
            DataInputEventListener.RaiseEvent(this, EventName);
        }
    }
}