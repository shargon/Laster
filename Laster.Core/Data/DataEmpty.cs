using Laster.Core.Interfaces;

namespace Laster.Core.Data
{
    /// <summary>
    /// Datos vacíos
    /// </summary>
    public class DataEmpty : IData
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">Origen de datos</param>
        public DataEmpty(ITopologyItem source) : base(source) { }
    }
}