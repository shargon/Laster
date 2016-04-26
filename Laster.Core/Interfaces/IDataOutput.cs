using Laster.Core.Classes.Collections;
using System;

namespace Laster.Core.Interfaces
{
    public class IDataOutput : IName, IDisposable
    {
        //DataProcessCollection _In;
        DataVariableCollection _Variables;

        ///// <summary>
        ///// Una salida de información siempre proviene de un procesado
        ///// </summary>
        //public DataProcessCollection In { get { return _In; } set { _In = value; } }
        /// <summary>
        /// Variables
        /// </summary>
        public DataVariableCollection Variables { get { return _Variables; } }

        /// <summary>
        /// Constructor privado
        /// </summary>
        protected IDataOutput()
        {
            _Variables = new DataVariableCollection();
        }

        /// <summary>
        /// Recibe una información
        /// </summary>
        /// <param name="data">Información</param>
        protected virtual void OnProcessData(IData data)
        {
            // Obtiene el dato y se pasa al procesador
        }

        /// <summary>
        /// Procesa los datos
        /// </summary>
        /// <param name="data"></param>
        public void ProcessData(IData data)
        {
            // Procesa los datos
            OnProcessData(data);
        }

        public virtual void Dispose()
        {
        }
    }
}