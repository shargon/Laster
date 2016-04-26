using Laster.Core.Interfaces;

namespace Laster.Outputs
{
    public class FileOutput : IDataOutput
    {
        string _FileName;

        /// <summary>
        /// Archivo de salida
        /// </summary>
        public string FileName { get { return _FileName; } set { _FileName = value; } }
    }
}