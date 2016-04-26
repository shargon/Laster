using Laster.Core.Interfaces;

namespace Laster.Core.Data
{
    public class EmptyData : IData
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">Padre</param>
        public EmptyData(IDataSource parent) : base(parent) { }
    }
}