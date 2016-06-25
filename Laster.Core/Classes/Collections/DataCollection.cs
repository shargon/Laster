﻿using Laster.Core.Interfaces;
using System;

namespace Laster.Core.Classes.Collections
{
    public class DataCollection : IDataCollection<IDataSource>, IDisposable
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
        /// <summary>
        /// Establece los datos
        /// </summary>
        /// <param name="data">Datos a establecer</param>
        public int SetData(IData data)
        {
            // Controlamos aquí la liberación de los recursos
            data.HandledDispose = true;

            int ret = -1;
            for (int x = 0, m = this.Count; x < m; x++)
            {
                IDataSource ds = this[x];
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
                    ret = x;
                }
                else
                {
                    // Si no es el dato actual que vamos a leer damos por hecho
                    // Que los datos de otras fuentes están cacheados
                    if (_InternalItems[x] != null)
                        _InternalItems[x].MarkAsCached();
                }
            }
            return ret;
        }

        public void Dispose()
        {
            if (_InternalItems != null)
            {
                // Liberar la memoria de todos
                foreach (IData o in _InternalItems)
                    if (o != null) o.Dispose();

                _InternalItems = null;
            }
        }
    }
}