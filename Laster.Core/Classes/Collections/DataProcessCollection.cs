using Laster.Core.Data;
using Laster.Core.Enums;
using Laster.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Laster.Core.Classes.Collections
{
    public class DataProcessCollection : IDataCollection<IDataProcess>
    {
        bool _UseParallel;
        ITopologyItem _Parent;

        /// <summary>
        /// Origen de datos
        /// </summary>
        public ITopologyItem Parent { get { return _Parent; } }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">Padre</param>
        public DataProcessCollection(ITopologyItem parent)
        {
            _Parent = parent;
        }
        // Cuando se añade o elimina un Origen a esta colección se le añade como origen esperado
        protected override void OnItemAdd(IDataProcess item)
        {
            item.Data.Add(_Parent);
        }
        protected override void OnItemRemove(IDataProcess item)
        {
            item.Data.Remove(_Parent);
        }
        /// <summary>
        /// Lanza el evento de creación
        /// </summary>
        public void RaiseOnCreate()
        {
            foreach (IDataProcess process in this) process.OnCreate();
        }
        /// <summary>
        /// Procesa los datos de entrada
        /// </summary>
        /// <param name="outPut">Salidas</param>
        /// <param name="data">Datos</param>
        /// <param name="useParallel">Usar paralelismo</param>
        public void ProcessData(DataOutputCollection outPut, IData data, bool useParallel)
        {
            if (data == null) return;

            // Si es un enumerador, hay que pasar fila a fila al procesado 
            if (data is DataEnumerable)
            {
                // Lo ejecuta secuencial
                using (IEnumerator<object> enumerator = ((DataEnumerable)data).GetEnumerator())
                {
                    EEnumerableDataState state = EEnumerableDataState.Start;
                    bool last = !enumerator.MoveNext();
                    IData current;

                    while (!last)
                    {
                        current = new DataObject(data.Source, enumerator.Current);

                        last = !enumerator.MoveNext();
                        if (last)
                        {
                            // Si es el último y era el primero, solo hay uno, sino, ha llegado al final
                            if (state == EEnumerableDataState.Start) state = EEnumerableDataState.OnlyOne;
                            else state = EEnumerableDataState.End;
                        }

                        // Ejecuta el procesado
                        if (useParallel)
                        {
                            Parallel.ForEach<IDataProcess>(this, p => { p.ProcessData(current, state); });
                        }
                        else
                        {
                            foreach (IDataProcess p in this) p.ProcessData(current, state);
                        }

                        // Ejecuta la salida
                        if (outPut != null)
                        {
                            if (useParallel)
                            {
                                Parallel.ForEach<IDataOutput>(outPut, p => { p.ProcessData(current, state); });
                            }
                            else
                            {
                                foreach (IDataOutput p in outPut) p.ProcessData(current, state);
                            }
                        }

                        state = EEnumerableDataState.Middle;
                    }
                }

                // Liberar los recursos del enumerado, posiblemente no se pueda reutilizar
                data.Dispose();
            }
            else
            {
                // Ejecuta el procesado
                if (useParallel)
                {
                    Parallel.ForEach<IDataProcess>(this, p => { p.ProcessData(data, EEnumerableDataState.NonEnumerable); });
                }
                else
                {
                    foreach (IDataProcess p in this) p.ProcessData(data, EEnumerableDataState.NonEnumerable);
                }

                // Ejecuta la salida
                if (outPut != null)
                {
                    if (useParallel)
                    {
                        Parallel.ForEach<IDataOutput>(outPut, p => { p.ProcessData(data, EEnumerableDataState.NonEnumerable); });
                    }
                    else
                    {
                        foreach (IDataOutput p in outPut) p.ProcessData(data, EEnumerableDataState.NonEnumerable);
                    }
                }
            }
        }
    }
}