using Laster.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Laster.Core.Data
{
    /// <summary>
    /// Objeto
    /// </summary>
    public class DataObject : IData
    {
        /// <summary>
        /// Datos
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">Origen de datos</param>
        /// <param name="data">Datos</param>
        internal DataObject(ITopologyItem source, object data) : base(source) { Data = data; }

        public override object GetInternalObject() { return Data; }

        /// <summary>
        /// Liberación de recursos
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            if (Data!=null && Data is IDisposable)
            {
                ((IDisposable)Data).Dispose();
            }
        }
        IEnumerator<object> GetEmpty()
        {
            if (Data != null) yield return Data;
        }
        public override IEnumerator<object> GetEnumerator()
        {
            if (Data != null)
            {
                if (Data is IEnumerator<object>)
                    return (IEnumerator<object>)Data;
            }
            return GetEmpty();
        }
    }
}