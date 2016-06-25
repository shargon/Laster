using Laster.Core.Helpers;
using Laster.Core.Interfaces;
using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;

namespace Laster.Core.Classes.RaiseMode
{
    public class DataInputUdpListener : IDataInputTriggerRaiseMode
    {
        /// <summary>
        /// Intervalo de actualización de la fuente de información
        /// </summary>
        public ushort Port { get; set; }
        /// <summary>
        /// Ips en lista blanca
        /// </summary>
        public string[] WhiteListAddresses { get; set; }
        /// <summary>
        /// Paquete hexadecimal requerido
        /// </summary>
        public string RequiredHexPacket { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public DataInputUdpListener() { }

        UdpClient _Udp;

        public override void Start(IDataInput input)
        {
            _Udp = new UdpClient(Port);
            _Udp.BeginReceive(OnReceive, null);
            base.Start(input);
        }

        public override void Stop(IDataInput input)
        {
            if (_Udp != null)
            {
                _Udp.Close();
                _Udp = null;
            }
            base.Stop(input);
        }

        void OnReceive(IAsyncResult ar)
        {
            IPEndPoint ip = null;
            byte[] data = _Udp.EndReceive(ar, ref ip);

            if (Match(data, ip))
                RaiseTrigger(EventArgs.Empty);

            _Udp.BeginReceive(OnReceive, null);
        }

        bool Match(byte[] data, IPEndPoint ip)
        {
            if (WhiteListAddresses != null && WhiteListAddresses.Length > 0)
            {
                bool esta = false;
                foreach (string sip in WhiteListAddresses)
                {
                    if (ip.Address.ToString() == sip)
                    {
                        esta = true;
                        break;
                    }
                }
                if (!esta) return false;
            }

            if (!string.IsNullOrEmpty(RequiredHexPacket))
            {
                string hex = HexHelper.Buffer2Hex(data);
                if (!hex.Equals(RequiredHexPacket, StringComparison.InvariantCultureIgnoreCase)) return false;
            }

            return true;
        }

        public override Image GetIcon() { return Res.network; }
        public override string ToString() { return "Udp"; }
    }
}