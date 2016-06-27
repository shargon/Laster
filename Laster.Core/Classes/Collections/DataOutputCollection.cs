using Laster.Core.Interfaces;

namespace Laster.Core.Classes.Collections
{
    public class DataOutputCollection : IDataCollection<IDataOutput>
    {
        ITopologyItem _Parent;
        /// <summary>
        /// Origen de datos
        /// </summary>
        public ITopologyItem Parent { get { return _Parent; } }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Parent">Padre</param>
        public DataOutputCollection(ITopologyItem parent)
        {
            _Parent = parent;
        }
        // Cuando se añade o elimina un Origen a esta colección se le añade como origen esperado
        protected override void OnItemAdd(IDataOutput item)
        {
            item.Data.Add(_Parent);
        }
        protected override void OnItemRemove(IDataOutput item)
        {
            item.Data.Remove(_Parent);
        }
        /// <summary>
        /// Lanza el evento de creación
        /// </summary>
        public void RaiseOnCreate()
        {
            foreach (IDataOutput process in this) process.OnCreate();
        }
    }
}