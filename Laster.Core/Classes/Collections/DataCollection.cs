using Laster.Core.Data;
using Laster.Core.Interfaces;

namespace Laster.Core.Classes.Collections
{
    public class DataCollection : IDataCollection<IDataSource>
    {
        IData[] _InternalItems = null;
        ArrayData _ArrayData = null;
        int _Filled = 0;

        /// <summary>
        /// Array de información
        /// </summary>
        public ArrayData ArrayData { get { return _ArrayData; } }
        /// <summary>
        /// Está o no lleno
        /// </summary>
        public bool IsFull { get { return _Filled == Count; } }
        /// <summary>
        /// Se modifica el tamaño de la colección actual
        /// </summary>
        /// <param name="size">Tamaño nuevo</param>
        protected override void OnItemsChange(int size)
        {
            _InternalItems = new IData[size];
            _ArrayData = new ArrayData(_InternalItems);
            _Filled = 0;
        }
        /// <summary>
        /// Establece los datos
        /// </summary>
        /// <param name="data">Datos a establecer</param>
        public int SetData(IData data)
        {
            int ret = -1;
            for (int x = 0, m = this.Count; x < m; x++)
            {
                IDataSource ds = this[x];
                if (ds == data.Origin)
                {
                    if (_InternalItems[x] == null)
                    {
                        _InternalItems[x] = data;
                        _Filled++;
                    }
                    else
                    {
                        //_InternalItems[x].Dispose();
                        _InternalItems[x] = data;
                    }
                    ret = x;
                }
                else
                {
                    // Si no es el dato actual que vamos a leer damos por hecho
                    // Que los datos de otras fuentes están cacheados
                    if (_InternalItems[x] != null)
                        _InternalItems[x].IsCached = true;
                }
            }
            return ret;
        }
    }
}