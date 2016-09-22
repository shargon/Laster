using Laster.Core.Designer;
using Laster.Core.Enums;
using Laster.Core.Helpers;
using Laster.Core.Interfaces;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;

namespace Laster.Process.Developer
{
    /// <summary>
    /// Compila un macro
    /// </summary>
    public class ScriptProcess : IDataProcess, IScriptConfig
    {
        /// <summary>
        /// Código
        /// </summary>
        [DefaultValue(@"
public override IData ProcessData(IDataProcess sender, IData data, EEnumerableDataState state)
{
    return data;
}
")]
        [Browsable(true)]
        [Category("Script")]
        [Editor(typeof(ScriptEditor), typeof(UITypeEditor))]
        public string Code { get; set; }
        /// <summary>
        /// Opciones
        /// </summary>
        [Category("Script Options")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public ScriptHelper.ScriptOptions Options { get; set; }

        IScriptProcess _Script;

        public override string Title { get { return "Developer - Script"; } }

        public ScriptProcess()
        {
            DesignBackColor = Color.Red;
            Options = new ScriptHelper.ScriptOptions() { Inherited = new Type[] { typeof(IScriptProcess) } };

            Options.IncludeFiles = Options.IncludeFiles.Concat(
                new string[]
                {
                "@Laster.Process.dll", "@Laster.Core.dll"
                }).ToArray();
            Options.IncludeUsings = Options.IncludeUsings.Concat(new string[] { "Laster.Process", "Laster.Core.Interfaces", "Laster.Core.Enums", "Laster.Core.Data" }).ToArray();

            Code = @"
public override IData ProcessData(IDataProcess sender, IData data, EEnumerableDataState state)
{
    return data;
}
";
        }
        protected override void OnStart()
        {
            if (_Script == null)
            {
                ScriptHelper helper = ScriptHelper.CreateFromString(Code, Options);
                if (helper != null)
                {
                    _Script = helper.CreateNewInstance<IScriptProcess>();
                    _Script.Source = this;
                }
            }
        }
        protected override void OnStop() { _Script = null; }
        protected override IData OnProcessData(IData data, EEnumerableDataState state)
        {
            if (_Script == null) return DataEmpty();
            return _Script.ProcessData(this, data, state);
        }
    }
}