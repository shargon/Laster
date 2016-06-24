using Laster.Core.Classes.Collections;

namespace Laster.Core.Interfaces
{
    public interface ITopologyRelationableItem
    {
        /// <summary>
        /// Devuelve si está ocupado
        /// </summary>
        bool IsBusy { get; }
        /// <summary>
        /// Procesado de la información
        /// </summary>
        DataProcessCollection Process { get; }
        /// <summary>
        /// Salidas de la información
        /// </summary>
        DataOutputCollection Out { get; }
    }
}