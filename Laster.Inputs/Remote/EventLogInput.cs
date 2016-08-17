using Laster.Core.Classes.RaiseMode;
using Laster.Core.Converters;
using Laster.Core.Designer;
using Laster.Core.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Text.RegularExpressions;

namespace Laster.Inputs.Remote
{
    /// <summary>
    /// Captura eventos del sistema
    /// </summary>
    public class EventLogInput : IDataInput
    {
        class Log
        {
            public Log(EventLogEntry entry)
            {
                UserName = entry.UserName;
                Message = entry.Message;

                //foreach (string s in entry.ReplacementStrings)
                //    if (Message.Contains(s))
                //    {
                //        //Message = Message.Replace(s,entry.);
                //    }

                Date = entry.TimeGenerated;
                MachineName = entry.MachineName;
                Type = entry.EntryType.ToString();
            }

            public string Type { get; set; }
            public string Category { get; set; }
            public string MachineName { get; set; }
            public DateTime Date { get; set; }
            public string UserName { get; set; }
            public string Message { get; set; }

            public override string ToString()
            {
                return Message;
            }
        }

        List<Log> l = new List<Log>();
        EventLog _Logger = null;

        public override string Title { get { return "Remote - EventLog"; } }

        #region Subscription
        [DefaultValue("")]
        public string LogName { get; set; }
        [DefaultValue(".")]
        public string MachineName { get; set; }
        [DefaultValue("")]
        public string Source { get; set; }
        #endregion

        #region Filters
        [Category("Filtros")]
        [JsonConverter(typeof(ImprovedRegexConverter))]
        [DefaultValue(null)]
        [TypeConverter(typeof(RegexConverter))]
        [Editor(typeof(RegexEditor), typeof(UITypeEditor))]
        public Regex RegexMessage { get; set; }
        [Category("Filtros")]
        [JsonConverter(typeof(ImprovedRegexConverter))]
        [DefaultValue(null)]
        [TypeConverter(typeof(RegexConverter))]
        [Editor(typeof(RegexEditor), typeof(UITypeEditor))]
        public Regex RegexUser { get; set; }

        [DefaultValue(-1)]
        [Category("Filtros")]
        public int CategoryNumber { get; set; }
        [DefaultValue(true)]
        [Category("Filtros")]
        public bool GetErrors { get; set; }
        [DefaultValue(true)]
        [Category("Filtros")]
        public bool GetWarnings { get; set; }
        [DefaultValue(true)]
        [Category("Filtros")]
        public bool GetInformations { get; set; }
        [DefaultValue(true)]
        [Category("Filtros")]
        public bool GetSuccessAudits { get; set; }
        [DefaultValue(true)]
        [Category("Filtros")]
        public bool GetFailureAudits { get; set; }
        #endregion

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public EventLogInput() : base()
        {
            MachineName = ".";

            CategoryNumber = -1;
            GetErrors = true;
            GetWarnings = true;
            GetInformations = true;
            GetSuccessAudits = true;
            GetFailureAudits = true;

            DesignBackColor = Color.DeepPink;
            RaiseMode = new DataInputEventListener()
            {
                EventName = "EventLogInput",
            };
        }


        protected override IData OnGetData()
        {
            if (l.Count == 0) return DataEmpty();

            Log[] ar;
            lock (l)
            {
                ar = l.ToArray();
                l.Clear();
            }
            return DataArray(ar);
        }

        public override void OnStart()
        {
            _Logger = new EventLog(
                LogName == null ? "" : LogName,
                string.IsNullOrEmpty(MachineName) ? "." : MachineName,
                Source == null ? "" : Source)
            {
                EnableRaisingEvents = true,
            };
            _Logger.EntryWritten += _Logger_EntryWritten;

            base.OnStart();
        }
        void _Logger_EntryWritten(object sender, EntryWrittenEventArgs e)
        {
            if (CategoryNumber != -1 && e.Entry.CategoryNumber == CategoryNumber) return;

            if (RegexMessage != null)
            {
                if (!RegexMessage.IsMatch(e.Entry.Message)) return;
            }
            if (RegexUser != null)
            {
                if (!RegexUser.IsMatch(e.Entry.UserName)) return;
            }

            if ((GetErrors && e.Entry.EntryType.HasFlag(EventLogEntryType.Error)) ||
                (GetWarnings && e.Entry.EntryType.HasFlag(EventLogEntryType.Warning)) ||
                (GetInformations && e.Entry.EntryType.HasFlag(EventLogEntryType.Information)) ||
                (GetSuccessAudits && e.Entry.EntryType.HasFlag(EventLogEntryType.SuccessAudit)) ||
                (GetFailureAudits && e.Entry.EntryType.HasFlag(EventLogEntryType.FailureAudit))
                )
            {
                l.Add(new Log(e.Entry));
            }

            // Lanzar el evento
            if (RaiseMode is DataInputEventListener)
            {
                DataInputEventListener ev = (DataInputEventListener)RaiseMode;
                ev.RaiseTrigger(e);
            }
        }
        public override void OnStop()
        {
            if (_Logger != null)
            {
                _Logger.Dispose();
                _Logger = null;
            }
            base.OnStop();
        }
    }
}