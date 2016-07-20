using System;

namespace Laster
{
    public class AreInUse
    {
        public const int InUseMillisecons = 400;
        DateTime _LastInUse = DateTime.MinValue;
        bool _LastUse = false;
        bool _InUse = false;

        /// <summary>
        /// Devuelve si ha habido algún cambio
        /// </summary>
        public bool AreChanged
        {
            get
            {
                if (_LastUse != InUse)
                {
                    _LastUse = InUse;
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Devuelve si está en uso
        /// </summary>
        public bool InUse
        {
            get
            {
                if (_InUse)
                    return true;

                if (_LastInUse == DateTime.MinValue)
                    return false;

                if ((DateTime.Now - _LastInUse).TotalMilliseconds > InUseMillisecons)
                {
                    _LastInUse = DateTime.MinValue;
                    return false;
                }

                return true;
            }
            set
            {
                _LastInUse = DateTime.Now;
                _InUse = value;
            }
        }
        /// <summary>
        /// Vacia el buffer de uso
        /// </summary>
        public void Clear()
        {
            _InUse = false;
            _LastInUse = DateTime.MinValue;
        }
    }
}