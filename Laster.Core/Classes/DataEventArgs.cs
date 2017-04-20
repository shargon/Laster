using System;

namespace Laster.Core.Classes
{
    public class DataEventArgs : EventArgs
    {
        public object Data { get; private set; }

        public DataEventArgs(object data) { Data = data; }
    }
}