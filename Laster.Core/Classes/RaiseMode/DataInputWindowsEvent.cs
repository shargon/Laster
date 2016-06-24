using System.Drawing;
using Laster.Core.Interfaces;
using Microsoft.Win32;

namespace Laster.Core.Classes.RaiseMode
{
    public class DataInputWindowsEvent : IDataInputTriggerRaiseMode
    {
        public enum EEvent
        {
            DisplaySettingsChanged,
            DisplaySettingsChanging,
            EventsThreadShutdown,
            InstalledFontsChanged,
            PaletteChanged,
            PowerModeChanged,
            SessionEnded,
            SessionEnding,
            SessionSwitch,
            TimeChanged,
            TimerElapsed,
            UserPreferenceChanged,
            UserPreferenceChanging,
        };

        public EEvent Event { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public DataInputWindowsEvent()
        {
            Event = EEvent.TimeChanged;
        }

        public override Image GetIcon()
        {
            return Res.windows;
        }

        public override void Start(IDataInput input)
        {
            base.Start(input);

            switch (Event)
            {
                case EEvent.DisplaySettingsChanged: SystemEvents.DisplaySettingsChanged += RaiseTrigger; break;
                case EEvent.DisplaySettingsChanging: SystemEvents.DisplaySettingsChanging += RaiseTrigger; break;
                case EEvent.EventsThreadShutdown: SystemEvents.EventsThreadShutdown += RaiseTrigger; break;
                case EEvent.InstalledFontsChanged: SystemEvents.InstalledFontsChanged += RaiseTrigger; break;
                case EEvent.PaletteChanged: SystemEvents.PaletteChanged += RaiseTrigger; break;
                case EEvent.PowerModeChanged: SystemEvents.PowerModeChanged += RaiseTrigger; break;
                case EEvent.SessionEnded: SystemEvents.SessionEnded += RaiseTrigger; break;
                case EEvent.SessionEnding: SystemEvents.SessionEnding += RaiseTrigger; break;
                case EEvent.SessionSwitch: SystemEvents.SessionSwitch += RaiseTrigger; break;
                case EEvent.TimeChanged: SystemEvents.TimeChanged += RaiseTrigger; break;
                case EEvent.TimerElapsed: SystemEvents.TimerElapsed += RaiseTrigger; break;
                case EEvent.UserPreferenceChanged: SystemEvents.UserPreferenceChanged += RaiseTrigger; break;
                case EEvent.UserPreferenceChanging: SystemEvents.UserPreferenceChanging += RaiseTrigger; break;
            }
        }
        public override void Stop(IDataInput input)
        {
            base.Stop(input);

            switch (Event)
            {
                case EEvent.DisplaySettingsChanged: SystemEvents.DisplaySettingsChanged -= RaiseTrigger; break;
                case EEvent.DisplaySettingsChanging: SystemEvents.DisplaySettingsChanging -= RaiseTrigger; break;
                case EEvent.EventsThreadShutdown: SystemEvents.EventsThreadShutdown -= RaiseTrigger; break;
                case EEvent.InstalledFontsChanged: SystemEvents.InstalledFontsChanged -= RaiseTrigger; break;
                case EEvent.PaletteChanged: SystemEvents.PaletteChanged -= RaiseTrigger; break;
                case EEvent.PowerModeChanged: SystemEvents.PowerModeChanged -= RaiseTrigger; break;
                case EEvent.SessionEnded: SystemEvents.SessionEnded -= RaiseTrigger; break;
                case EEvent.SessionEnding: SystemEvents.SessionEnding -= RaiseTrigger; break;
                case EEvent.SessionSwitch: SystemEvents.SessionSwitch -= RaiseTrigger; break;
                case EEvent.TimeChanged: SystemEvents.TimeChanged -= RaiseTrigger; break;
                case EEvent.TimerElapsed: SystemEvents.TimerElapsed -= RaiseTrigger; break;
                case EEvent.UserPreferenceChanged: SystemEvents.UserPreferenceChanged -= RaiseTrigger; break;
                case EEvent.UserPreferenceChanging: SystemEvents.UserPreferenceChanging -= RaiseTrigger; break;
            }
        }

        public override string ToString()
        {
            return "WindowsEvent";
        }
    }
}