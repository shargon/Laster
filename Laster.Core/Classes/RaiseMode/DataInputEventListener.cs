using Laster.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Laster.Core.Classes.RaiseMode
{
    public class DataInputEventListener : IDataInputTriggerRaiseMode
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
            base.Start(input);

            // Añade el evento
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
        }

        public override void Stop(IDataInput input)
        {
            base.Stop(input);

            // Elimina el evento
            lock (_Events)
            {
                List<EventHandler> ev;
                if (_Events.TryGetValue(EventName, out ev))
                {
                    if (ev.Contains(RaiseTrigger))
                        ev.Remove(RaiseTrigger);
                }
            }
        }

        public override Image GetIcon() { return Res.events; }
        public override string ToString() { return "Event"; }
    }
}