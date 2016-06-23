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
        public void Clear()
        {
            if (_Items != null)
            {
                foreach (T i in _Items)
                    OnItemRemove(i);

                _Items = null;
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

            _Items = ls.ToArray();

            OnItemRemove(item);
            OnItemsChange(_Items.Length);
        }

        /// <summary>
        /// Se lanza al modificarse los items
        /// </summary>
        /// <param name="size">Tamaño</param>
        protected virtual void OnItemsChange(int size)
        {

        }
        /// <summary>
        /// Se lanza al modificarse los items
        /// </summary>
        /// <param name="item">Item</param>
        protected virtual void OnItemAdd(T item)
        {

        }
        /// <summary>
        /// Se lanza al modificarse los items
        /// </summary>
        /// <param name="item">Item</param>
        protected virtual void OnItemRemove(T item)
        {

        }
        /// <summary>
        /// Añade todos los elementos
        /// </summary>
        /// <param name="items">Items</param>
        /// <returns>Devuelve el número de elementos añadidos</returns>
        public int AddAll(params T[] items)
        {
            if (items == null) return 0;

            foreach (T item in items) Add(item);
            return items.Length;
        }
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
        /// Añade un item a la colección
        /// </summary>
        /// <param name="item">Item</param>
        /// <returns>Devuelve la posición donde se añadió</returns>
        public int Add(T item)
        {
            if (item == null) return -1;

            if (_Items == null)
            {
                _Items = new T[] { item };
                OnItemAdd(item);
                OnItemsChange(1);
                return 0;
            }

            int l = _Items.Length;
            Array.Resize(ref _Items, l + 1);
            _Items[l] = item;

            OnItemAdd(item);
            OnItemsChange(l + 1);
            return l;
        }
        /// <summary>
        /// Añade varios items a la colección
        /// </summary>
        /// <param name="items">Items</param>
        /// <returns>Devuelve la posición desde donde se añadió</returns>
        public int Add(T[] items)
        {
            if (items == null) return -1;

            if (_Items == null)
            {
                _Items = items;
                return 0;
            }

            int l = _Items.Length;
            Array.Resize(ref _Items, l + items.Length);
            Array.Copy(items, 0, _Items, l, items.Length);

            return l;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (_Items == null) return GetEmpty();
            return _Items.GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_Items == null) return GetEmpty();
            return ((IEnumerable<T>)_Items).GetEnumerator();
        }

        IEnumerator<T> GetEmpty() { yield break; }
    }
}