using Laster.Core.Helpers;

namespace Laster.Core.Interfaces
{
    public interface IScriptConfig
    {
        string Code { get; set; }
        ScriptHelper.ScriptOptions Options { get; set; }
    }
}