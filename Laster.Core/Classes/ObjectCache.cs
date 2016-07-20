using Laster.Core.Interfaces;
using System.Reflection;

namespace Laster.Core.Classes
{
    public class ObjectCache
    {
        ITopologyItem _Item;
        PropertyInfo _PropertyInfo;
        object _Value;

        public ObjectCache(PropertyInfo propertyInfo, ITopologyItem item)
        {
            _PropertyInfo = propertyInfo;
            _Item = item;
            _Value = propertyInfo.GetValue(item);
        }
        public void Restore()
        {
            _PropertyInfo.SetValue(_Item, _Value);
        }
    }
}