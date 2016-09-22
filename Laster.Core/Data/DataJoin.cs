using Laster.Core.Interfaces;
using System.Collections.Generic;

namespace Laster.Core.Data
{
    /// <summary>
    /// Array de Datas (debido a que contiene varias fuentes de información)
    /// </summary>
    public class DataJoin : IData
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
        /// Devuelve el Data por el name del Source
        /// </summary>
        /// <param name="name">Name</param>
        public IData this[string name]
        {
            get
            {
                foreach (IData d in _Items)
                    if (d.Source.Name == name) return d;

                return null;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">Origen de datos</param>
        /// <param name="items">Items</param>
        internal DataJoin(ITopologyItem source, params IData[] items) : base(source) { _Items = items; }

        public override object GetInternalObject() { return _Items; }
        IEnumerator<object> GetEmpty() { yield break; }

        /// <summary>
        /// Liberación de recursos
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            if (_Items != null)
            {
                foreach (IData i in _Items) i.Dispose();
            }
        }

        public override IEnumerator<object> GetEnumerator()
        {
            if (_Items != null)
            {
                foreach (IData o in _Items)
                {
                    if (o == null) continue;

                    object i = o.GetInternalObject();
                    if (i is IEnumerable<object>)
                    {
                        foreach (object i2 in (IEnumerable<object>)i)
                            yield return i2;
                    }
                    else
                    {
                        yield return i;
                    }
                }
            }
        }
    }
}