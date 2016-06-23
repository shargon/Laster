using Laster.Core.Interfaces;
using System.Collections;
using System.Collections.Generic;

namespace Laster.Core.Data
{
    /// <summary>
    /// El enumerable se trata de forma distinta, solo se recorre una vez, y se manda uno a uno a los procesadores
    /// </summary>
    public class DataEnumerable : IData, IEnumerable<object>
    {
        IEnumerable<object> _Objects;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">Origen de datos</param>
        /// <param name="objs">Objetos</param>
        public DataEnumerable(IDataSource source, IEnumerable<object> objs) : base(source) { _Objects = objs; }

        public IEnumerator<object> GetEnumerator() { return _Objects.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return _Objects.GetEnumerator(); }
    }
}