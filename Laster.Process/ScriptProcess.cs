using Laster.Core.Enums;
using Laster.Core.Helpers;
using Laster.Core.Interfaces;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;

namespace Laster.Process
{
    /// <summary>
    /// Compila un macro
    /// </summary>
    public class ScriptProcess : IDataProcess
    {
        /// <summary>
        /// Código
        /// </summary>
        [EditorAttribute(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Code { get; set; }
        /// <summary>
        /// Opciones
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public ScriptHelper.ScriptOptions Options { get; set; }

        public interface Script
        {
            IData ProcessData(IDataProcess sender, IData data, EEnumerableDataState state);
        }

        Script _Script;

        public ScriptProcess()
        {
            Options = new ScriptHelper.ScriptOptions()
            {
                includeUsings = new string[] { "Laster.Process", "Laster.Core.Interfaces", "Laster.Core.Enums", "Laster.Core.Data" },
                IncludeFiles = new string[] { "Laster.Process.dll", "Laster.Core.dll", },
                Inherited = new Type[] { typeof(Script) }
            };

            Code = @"
public IData ProcessData(IDataProcess sender, IData data, EEnumerableDataState state)
{
    return data;
}
";
        }
        public override void OnCreate()
        {
            ScriptHelper helper = ScriptHelper.CreateFromString(Code, Options);
            _Script = helper.CreateNewInstance<Script>();
            base.OnCreate();
        }
        protected override IData OnProcessData(IData data, EEnumerableDataState state)
        {
            return _Script.ProcessData(this, data, state);
        }
    }
}