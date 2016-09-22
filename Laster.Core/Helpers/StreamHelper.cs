using System.IO;

namespace Laster.Core.Helpers
{
    public class StreamHelper
    {
        public static void ReadFull(FileStream stream, byte[] d, int index, int length)
        {
            while (length > 0)
            {
                int lee = stream.Read(d, index, length);
                index += lee;
                length -= lee;
            }
        }
    }
}