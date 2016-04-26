using Laster.Core.Interfaces;
using System.Collections.Generic;
using System.Collections;

namespace Laster.Core.Data
{
    public class IEnumerableData : IData, IEnumerable<object>
    {
        IEnumerable<object> _Objects;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">Padre</param>
        public IEnumerableData(IDataSource parent, IEnumerable<object> objs) : base(parent)
        {
            _Objects = objs;
        }

        public IEnumerator<object> GetEnumerator()
        {
            return _Objects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Objects.GetEnumerator();
        }
    }
}