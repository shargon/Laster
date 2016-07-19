using System;
using System.Collections;
using System.Collections.Generic;

namespace Laster.Core.Interfaces
{
    public class IDataCollection<T> : IEnumerable<T>
    {
        T[] _Items;

        /// <summary>
        /// Items
        /// </summary>
        public T[] Items { get { return _Items; }/* set { _Items = value; } */}
        /// <summary>
        /// Número de items
        /// </summary>
        public int Count { get { return _Items == null ? 0 : _Items.Length; } }
        /// <summary>
        /// Índice
        /// </summary>
        /// <param name="index">Posición</param>
        /// <returns>Devuelve la clase</returns>
        public T this[int index] { get { return _Items[index]; } }

        public IDataCollection() { }
        public IDataCollection(params T[] values) { AddAll(values); }

        /// <summary>
        /// Vacia la lista
        /// </summary>
        public virtual void Clear()
        {
            if (_Items != null)
            {
                lock (this)
                {
                    foreach (T i in _Items)
                        OnItemRemove(i);

                    _Items = null;
                }
                OnItemsChange(0);
            }
        }

        public void Remove(T item)
        {
            if (_Items == null) return;

            List<T> ls = new List<T>();
            ls.AddRange(_Items);
            if (!ls.Remove(item))
                return;

            lock (this)
            {
                _Items = ls.ToArray();
            }

            OnItemRemove(item);
            OnItemsChange(_Items.Length);
        }

        /// <summary>
        /// Se lanza al modificarse los items
        /// </summary>
        /// <param name="size">Tamaño</param>
        protected virtual void OnItemsChange(int size) { }
        /// <summary>
        /// Se lanza al modificarse los items
        /// </summary>
        /// <param name="item">Item</param>
        protected virtual void OnItemAdd(T item) { }
        /// <summary>
        /// Se lanza al modificarse los items
        /// </summary>
        /// <param name="item">Item</param>
        protected virtual void OnItemRemove(T item) { }
        /// <summary>
        /// Añade todos los elementos
        /// </summary>
        /// <param name="items">Items</param>
        /// <returns>Devuelve el número de elementos añadidos</returns>
        public int AddAll(IEnumerable<T> items)
        {
            if (items == null) return 0;

            int x = 0;
            foreach (T item in items)
            {
                Add(item);
                x++;
            }
            return x;
        }
        /// <summary>
        /// Devuelve si ya está el elemento en la colección
        /// </summary>
        /// <param name="item">Item</param>
        public bool Contains(T item)
        {
            if (_Items == null) return false;
            foreach (T t in _Items) if ((object)t == (object)item) return true;
            return false;
        }
        /// <summary>
        /// Añade varios items a la colección
        /// </summary>
        /// <param name="items">Items</param>
        /// <returns>Devuelve la posición desde donde se añadió</returns>
        public int Add(params T[] items)
        {
            if (items == null) return -1;

            lock (this)
            {
                if (_Items == null)
                {
                    _Items = items;
                    foreach (T i in items) OnItemAdd(i);
                    OnItemsChange(_Items.Length);
                    return 0;
                }

                int l = _Items.Length;
                Array.Resize(ref _Items, l + items.Length);
                Array.Copy(items, 0, _Items, l, items.Length);

                foreach (T i in items) OnItemAdd(i);
                OnItemsChange(_Items.Length);
                return l;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (_Items != null) foreach (T i in _Items) yield return i;
        }
        public IEnumerator<T> GetEnumerator()
        {
            if (_Items != null) foreach (T i in _Items) yield return i;
        }
    }
}