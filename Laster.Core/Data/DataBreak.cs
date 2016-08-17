using Laster.Core.Interfaces;

namespace Laster.Core.Data
{
    /// <summary>
    /// Cortar flujo de datos
    /// </summary>
    public class DataBreak : IData
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">Origen de datos</param>
        internal DataBreak(ITopologyItem source) : base(source) { }
    }
}