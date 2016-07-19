using Laster.Core.Interfaces;
using System;

namespace Laster.Core.Classes.Collections
{
    public class DataCollection : IDataCollection<ITopologyItem>, IDisposable
    {
        IData[] _InternalItems = null;
        int _Filled = 0;

        /// <summary>
        /// Array de información
        /// </summary>
        public new IData[] Items { get { return _InternalItems; } }
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
            _Filled = 0;
        }
        public void ClearData()
        {
            if (_InternalItems != null)
            {
                // Liberar la memoria de todos
                for (int x = _InternalItems.Length - 1; x >= 0; x--)
                {
                    if (_InternalItems[x] != null)
                    {
                        _InternalItems[x].Dispose();
                        _InternalItems[x] = null;
                    }
                }

                _Filled = 0;
            }
        }
        /// <summary>
        /// Establece los datos y devuelve si está lleno
        /// </summary>
        /// <param name="data">Datos a establecer</param>
        public bool SetData(IData data)
        {
            // Controlamos aquí la liberación de los recursos
            data.HandledDispose = true;

            for (int x = 0, m = this.Count; x < m; x++)
            {
                ITopologyItem ds = this[x];
                if (ds == data.Source)
                {
                    if (_InternalItems[x] == null)
                    {
                        _InternalItems[x] = data;
                        _Filled++;
                    }
                    else
                    {
                        _InternalItems[x].Dispose();
                        _InternalItems[x] = data;
                    }
                }
                else
                {
                    // Si no es el dato actual que vamos a leer damos por hecho
                    // Que los datos de otras fuentes están cacheados
                    if (_InternalItems[x] != null)
                        _InternalItems[x].MarkAsCached();
                }
            }

            return _Filled == Count;
        }
        public void Dispose()
        {
            Clear();
        }
    }
}