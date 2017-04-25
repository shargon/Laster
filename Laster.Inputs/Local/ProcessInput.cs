using Laster.Core.Converters;
using Laster.Core.Designer;
using Laster.Core.Helpers;
using Laster.Core.Interfaces;
using Laster.Inputs.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Pr = System.Diagnostics;

namespace Laster.Inputs.Local
{
    /// <summary>
    /// Captura eventos del sistema
    /// </summary>
    public class ProcessInput : IDataInput
    {
        public enum EReturnType
        {
            PID,
            ProcessName,
            Object,
            MemoryDumpFileName,
            MemoryDumpData
        }

        [Category("Filter")]
        [JsonConverter(typeof(ImprovedRegexConverter))]
        [DefaultValue(null)]
        [TypeConverter(typeof(RegexConverter))]
        [Editor(typeof(RegexEditor), typeof(UITypeEditor))]
        public Regex Pattern { get; set; }

        [Category("General")]
        [DefaultValue(".")]
        public string Machine { get; set; }
        [Category("General")]
        [DefaultValue(EReturnType.PID)]
        public EReturnType Return { get; set; }

        public override string Title { get { return "Local - Process"; } }

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public ProcessInput() : base()
        {
            DesignBackColor = Color.Green;
            Machine = ".";
        }

        protected override IData OnGetData()
        {
            List<object> lp = new List<object>();

            foreach (Pr.Process p in Pr.Process.GetProcesses(Machine))
                try
                {
                    if (Pattern.IsMatch(p.ProcessName))
                    {
                        switch (Return)
                        {
                            case EReturnType.Object: lp.Add(p); break;
                            case EReturnType.PID: lp.Add(p.Id); break;
                            case EReturnType.ProcessName: lp.Add(p.ProcessName); break;
                            case EReturnType.MemoryDumpData:
                            case EReturnType.MemoryDumpFileName:
                                {
                                    string file = Path.GetTempFileName();
                                    if (ProcessHelper.ProcessMemoryDump(p.Id, file) > 0)
                                    {
                                        switch (Return)
                                        {
                                            case EReturnType.MemoryDumpData:
                                                {
                                                    lp.Add(File.ReadAllBytes(file));
                                                    File.Delete(file);
                                                    break;
                                                }
                                            case EReturnType.MemoryDumpFileName:
                                                {
                                                    lp.Add(file);
                                                    break;
                                                }
                                        }
                                    }
                                    else
                                    {
                                        File.Delete(file);
                                    }
                                    break;
                                }
                        }
                    }

                    p.Dispose();
                }
                catch { }

            return Reduce(Core.Enums.EReduceZeroEntries.Break, lp.ToArray());
        }
    }
}