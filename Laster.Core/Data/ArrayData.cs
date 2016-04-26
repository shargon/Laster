using Laster.Core.Interfaces;
using System.Collections.Generic;
using System;
using System.Collections;

namespace Laster.Core.Data
{
    public class ArrayData : IData, IEnumerable<IData>
    {
        IData[] _Items;

        /// <summary>
        /// Items
        /// </summary>
        public IData[] Items { get { return _Items; } }
        /// <summary>
        /// Tamaño de items
        /// </summary>
        public int Count { get { return _Items == null ? 0 : _Items.Length; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="items">Items</param>
        internal ArrayData(params IData[] items) : base(null)
        {
            _Items = items;
        }

        IEnumerator<IData> GetEmpty()
        {
            yield break;
        }
        public IEnumerator GetEnumerator()
        {
            if (_Items == null) return GetEmpty();
            return _Items.GetEnumerator();
        }
        IEnumerator<IData> IEnumerable<IData>.GetEnumerator()
        {
            if (_Items == null) return GetEmpty();
            return (IEnumerator<IData>)_Items.GetEnumerator();
        }
    }
}