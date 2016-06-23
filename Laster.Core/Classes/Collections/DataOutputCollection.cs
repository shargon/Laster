using Laster.Core.Interfaces;

namespace Laster.Core.Classes.Collections
{
    public class DataOutputCollection : IDataCollection<IDataOutput>
    {
        bool _UseParallel;
        /// <summary>
        /// Usar procesamiento en paralelo si o no
        /// </summary>
        public bool UseParallel { get { return _UseParallel; } set { _UseParallel = value; } }
        /// <summary>
        /// Constructor
        /// </summary>
        public DataOutputCollection() { }
        /// <summary>
        /// Lanza el evento de creación
        /// </summary>
        public void RaiseOnCreate()
        {
            foreach (IDataOutput process in this) process.OnCreate();
        }
    }
}