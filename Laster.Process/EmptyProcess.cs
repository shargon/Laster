using Laster.Core.Interfaces;

namespace Laster.Process
{
    public class EmptyProcess: IDataProcess
    {
        protected override IData OnProcessData(IData data)
        {
            return base.OnProcessData(data);
        }
    }
}