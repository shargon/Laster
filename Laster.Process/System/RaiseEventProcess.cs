﻿using Laster.Core.Classes.RaiseMode;
using Laster.Core.Enums;
using Laster.Core.Interfaces;
using System.ComponentModel;
using System.Drawing;

namespace Laster.Process.System
{
    public class RaiseEventProcess : IDataProcess
    {
        /// <summary>
        /// Nombre del evento
        /// </summary>
        [DefaultValue("")]
        public string EventName { get; set; }
        /// <summary>
        /// Adjuntar los datos al evento
        /// </summary>
        [DefaultValue(false)]
        public bool AttachData { get; set; }

        public override string Title { get { return "System - Raise event"; } }

        public RaiseEventProcess()
        {
            DesignBackColor = Color.DeepPink;
        }

        /// <summary>
        /// Saca el contenido de los datos a un archivo
        /// </summary>
        /// <param name="data">Datos</param>
        /// <param name="state">Estado de la enumeración</param>
        protected override IData OnProcessData(IData data, EEnumerableDataState state)
        {
            DataInputEventListener.RaiseEvent(this, EventName, AttachData ? data : null);
            return data;
        }
    }
}