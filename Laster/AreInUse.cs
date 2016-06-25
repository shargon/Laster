using System;

namespace Laster
{
    public class AreInUse
    {
        public const int InUseMillisecons = 500;
        DateTime _InUse = DateTime.MinValue;
        bool _LastUse = false;

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
                if (_InUse == DateTime.MinValue)
                    return false;

                if ((DateTime.Now - _InUse).TotalMilliseconds > InUseMillisecons)
                {
                    _InUse = DateTime.MinValue;
                    return false;
                }

                return true;
            }
            set
            {
                _InUse = value ? DateTime.Now : DateTime.MinValue;
            }
        }
    }
}