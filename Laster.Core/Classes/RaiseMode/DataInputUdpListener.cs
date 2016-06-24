using Laster.Core.Interfaces;
using System.Net.Sockets;
using System;
using System.Net;

namespace Laster.Core.Classes.RaiseMode
{
    public class DataInputUdpListener : IDataInputTriggerRaiseMode
    {
        /// <summary>
        /// Intervalo de actualización de la fuente de información
        /// </summary>
        public ushort Port { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public DataInputUdpListener() { }

        UdpClient _Udp;

        public override void Start(IDataInput input)
        {
            base.Start(input);

            _Udp = new UdpClient(Port);
            _Udp.BeginReceive(OnReceive, null);
        }

        public override void Stop(IDataInput input)
        {
            base.Stop(input);

            if (_Udp != null)
            {
                _Udp.Close();
                _Udp = null;
            }
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
            return true;
        }

        //public override Image GetIcon() { return Res.timer; }
        public override string ToString() { return "Udp"; }
    }
}