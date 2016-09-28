using Laster.Core.Data;
using Laster.Core.Enums;
using System.Collections.Generic;

namespace Laster.Core.Interfaces
{
    public class IScriptProcess
    {
        ITopologyItem _Source;

        /// <summary>
        /// Origen
        /// </summary>
        public ITopologyItem Source
        {
            get { return _Source; }
            set
            {
                if (_Source == null) _Source = value;
            }
        }

        /// <summary>
        /// Variables estáticas compartidas entre scripts
        /// </summary>
        public static Dictionary<string, object> SharedVariables { get; private set; }

        static IScriptProcess()
        {
            SharedVariables = new Dictionary<string, object>();
        }

        public virtual IData ProcessData(IDataProcess sender, IData data, EEnumerableDataState state)
        {
            return new DataBreak(sender);
        }

        #region Helpers
        public DataObject DataObject(object data) { return new DataObject(_Source, data); }
        public DataEmpty DataEmpty() { return new DataEmpty(_Source); }
        public DataBreak DataBreak() { return new DataBreak(_Source); }
        public DataArray DataArray(object[] items) { return new DataArray(_Source, items); }
        public DataArray DataArray(List<object> items) { return new DataArray(_Source, items); }
        public DataEnumerable DataEnumerable(IEnumerable<object> items) { return new DataEnumerable(_Source, items); }
        public IData Reduce(bool useBreak, List<object> v)
        {
            if (v == null)
                return useBreak ? new DataBreak(_Source) : null;

            switch (v.Count)
            {
                case 0: return useBreak ? new DataBreak(_Source) : null;
                case 1: return new DataObject(_Source, v[0]);
                default: return new DataArray(_Source, v);
            }
        }
        public IData Reduce(bool useBreak, object[] v)
        {
            if (v == null)
                return useBreak ? new DataBreak(_Source) : null;

            switch (v.Length)
            {
                case 0: return useBreak ? new DataBreak(_Source) : null;
                case 1: return new DataObject(_Source, v[0]);
                default: return new DataArray(_Source, v);
            }
        }
        #endregion
    }
}