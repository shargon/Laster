using System;
using System.IO;

namespace Laster.Core.Helpers
{
    public class LasterHelper
    {
        public static void SetEnvironmentPath(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                Environment.SetEnvironmentVariable("LasterConfigFile", "", EnvironmentVariableTarget.Process);
                Environment.SetEnvironmentVariable("LasterConfigPath", "", EnvironmentVariableTarget.Process);
            }
            else
            {
                Environment.SetEnvironmentVariable("LasterConfigFile", path, EnvironmentVariableTarget.Process);
                Environment.SetEnvironmentVariable("LasterConfigPath", Path.GetDirectoryName(path), EnvironmentVariableTarget.Process);
            }
        }
    }
}