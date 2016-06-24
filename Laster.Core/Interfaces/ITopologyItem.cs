using Laster.Core.Classes;
using System.ComponentModel;
using System.Threading;

namespace Laster.Core.Interfaces
{
    public class ITopologyItem : NameClass
    {
        static int _CurrentId = 0;

        int _Id = 0;
        /// <summary>
        /// Identificador para la generación de relaciones
        /// </summary>
        [Browsable(false)]
        public int Id
        {
            get { return _Id; }
            set
            {
                _Id = value;
                if (_Id > _CurrentId)
                    _CurrentId = _Id;
            }
        }

        protected ITopologyItem()
        {
            Id = Interlocked.Increment(ref _CurrentId);
        }
    }
}