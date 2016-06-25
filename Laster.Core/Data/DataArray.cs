﻿using Laster.Core.Interfaces;
using System.Collections.Generic;

namespace Laster.Core.Data
{
    /// <summary>
    /// Array de objetos
    /// </summary>
    public class DataArray : IData
    {
        object[] _Items;

        /// <summary>
        /// Items
        /// </summary>
        public object[] Items { get { return _Items; } }
        /// <summary>
        /// Tamaño de items
        /// </summary>
        public int Count { get { return _Items == null ? 0 : _Items.Length; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">Origen de datos</param>
        /// <param name="items">Items</param>
        public DataArray(IDataSource source, params object[] items) : base(source) { _Items = items; }

        public override object GetInternalObject() { return _Items; }
        IEnumerator<object> GetEmpty() { yield break; }

        public override IEnumerator<object> GetEnumerator()
        {
            if (_Items != null)
            {
                foreach (object o in _Items)
                    yield return o;
            }
        }
    }
}