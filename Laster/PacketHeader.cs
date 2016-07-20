using Laster.Core.Helpers;

namespace Laster
{
    class PacketHeader
    {
        public byte[] H { get; set; }
        public byte[] D { get; set; }

        public void Encrypt(bool encrypt)
        {
            AESHelper aes = new AESHelper(HexHelper.Buffer2Hex(H), "*LasterLaster*", 100, "LasterLasterLast", AESHelper.EKeyLength.Length256);
            D = encrypt ? aes.Encrypt(D) : aes.Decrypt(D);
        }
    }
}