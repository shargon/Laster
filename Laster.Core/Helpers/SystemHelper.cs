using System;

namespace Laster.Core.Helpers
{
    public class SystemHelper
    {
        static bool _IsWindows;
        /// <summary>
        /// Devuelve si es linux
        /// </summary>
        public static bool IsWindows { get { return _IsWindows; } }

        /// <summary>
        /// Constructor estático
        /// </summary>
        static SystemHelper()
        {
            PlatformID p = Environment.OSVersion.Platform;
            _IsWindows = p == PlatformID.Win32NT;
        }
    }
}