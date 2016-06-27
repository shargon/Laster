using Laster.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Laster.Core.Classes.RaiseMode
{
    public class DataInputEventListener : ITriggerRaiseMode
    {
        /// <summary>
        /// Nombre del evento
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public DataInputEventListener() { }

        static Dictionary<string, List<EventHandler>> _Events = new Dictionary<string, List<EventHandler>>();

        /// <summary>
        /// Lanza el evento con nombre ...
        /// </summary>
        /// <param name="sender">El que lanza el evento</param>
        /// <param name="eventName">Nombre del evento</param>
        public static bool RaiseEvent(object sender, string eventName)
        {
            if (string.IsNullOrEmpty(eventName)) return false;

            List<EventHandler> ev;
            if (_Events.TryGetValue(eventName, out ev))
            {
                foreach (EventHandler evs in ev)
                    evs.Invoke(sender, EventArgs.Empty);

                return true;
            }
            return false;
        }

        public override void Start(IDataInput input)
        {
            // Añade el evento
            if (!string.IsNullOrEmpty(EventName))
                lock (_Events)
                {
                    List<EventHandler> ev;
                    if (_Events.TryGetValue(EventName, out ev))
                    {
                        if (!ev.Contains(RaiseTrigger))
                            ev.Add(RaiseTrigger);
                    }
                    else
                    {
                        _Events.Add(EventName, new List<EventHandler>(new EventHandler[] { RaiseTrigger }));
                    }
                }
            base.Start(input);
        }

        public override void Stop(IDataInput input)
        {
            // Elimina el evento
            if (!string.IsNullOrEmpty(EventName))
                lock (_Events)
                {
                    List<EventHandler> ev;
                    if (_Events.TryGetValue(EventName, out ev))
                    {
                        if (ev.Contains(RaiseTrigger))
                            ev.Remove(RaiseTrigger);
                    }
                }
            base.Stop(input);
        }

        public override Image GetIcon() { return Res.events; }
        public override string ToString() { return "Event"; }
    }
}