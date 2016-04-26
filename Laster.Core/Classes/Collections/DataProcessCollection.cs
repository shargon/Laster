using Laster.Core.Interfaces;

namespace Laster.Core.Classes.Collections
{
    public class DataProcessCollection : IDataCollection<IDataProcess>
    {
        IDataSource _Parent;

        /// <summary>
        /// Origen de datos
        /// </summary>
        public IDataSource Parent { get { return _Parent; } }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">Padre</param>
        public DataProcessCollection(IDataSource parent)
        {
            _Parent = parent;
        }

        // Cuando se añade o elimina un Origen a esta colección se le añade como origen esperado
        protected override void OnItemAdd(IDataProcess item) { item.Data.Add(_Parent); }
        protected override void OnItemRemove(IDataProcess item) { item.Data.Remove(_Parent); }

        /// <summary>
        /// Procesa los datos de entrada
        /// </summary>
        /// <param name="data">Datos</param>
        public void ProcessData(IData data)
        {
            foreach (IDataProcess process in this) process.ProcessData(data);
        }
    }
}