using Laster.Core.Enums;

namespace Laster.Core.Interfaces
{
    public interface IScriptProcess
    {
        IData ProcessData(IDataProcess sender, IData data, EEnumerableDataState state);
    }
}