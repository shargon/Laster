using Laster.Core.Interfaces;

namespace Laster.Core.Classes.Collections
{
    public class DataOutputCollection : IDataCollection<IDataOutput>
    {
        IDataProcess _Parent;
        /// <summary>
        /// Padre
        /// </summary>
        public IDataProcess Parent { get { return _Parent; } }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">Padre</param>
        public DataOutputCollection(IDataProcess parent)
        {
            _Parent = parent;
        }
        /// <summary>
        /// Procesa los datos de salida
        /// </summary>
        /// <param name="data">Datos</param>
        public void ProcessData(IData data)
        {
            foreach (IDataOutput outs in this) outs.ProcessData(data);
        }
    }
}