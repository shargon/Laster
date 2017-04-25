using System;

namespace Laster.Core.Helpers
{
    public class SystemHelper
    {
        static bool _IsMono, _IsWindows, _IsMac, _IsLinux;

        /// <summary>
        /// Is Mono
        /// </summary>
        public static bool IsMono { get { return _IsMono; } }
        /// <summary>
        /// Is Mac
        /// </summary>
        public static bool IsMac { get { return _IsMac; } }
        /// <summary>
        /// Is Linux
        /// </summary>
        public static bool IsLinux { get { return _IsLinux; } }
        /// <summary>
        /// Is Windows
        /// </summary>
        public static bool IsWindows { get { return _IsWindows; } }

        /// <summary>
        /// Constructor estático
        /// </summary>
        static SystemHelper()
        {
            PlatformID p = Environment.OSVersion.Platform;
            _IsMono = Type.GetType("Mono.Runtime") != null;
            _IsWindows = p == PlatformID.Win32NT || p == PlatformID.Win32S || p == PlatformID.Win32Windows;
            _IsLinux = (p == PlatformID.Unix) /*|| (p == 6) || (p == 128)*/;
            _IsMac = (p == PlatformID.MacOSX) /*|| (p == 6) || (p == 128)*/;
        }
    }
}