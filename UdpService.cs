using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NoiseAmpControl
{
    public class UdpService : UdpClient
    {
        private IPEndPoint _iPEndPoint;
        public UdpService(int udpClientPort, IPEndPoint iPEndPoint) : base(udpClientPort)
        {
            _iPEndPoint = iPEndPoint;
            this.BeginReceive(UdpReceive, null);
        }

        private void UdpReceive (IAsyncResult res)
        {
            if(this != null)
            {
                Byte[] receivedUdpbytes = this.EndReceive(res, ref _iPEndPoint);
                string receiveString = Encoding.ASCII.GetString(receivedUdpbytes);
            }

            this.BeginReceive(new AsyncCallback(UdpReceive), null);
        }

        public new void Send()
        {
            var sendstring = string.Format("00TC*31v{0:D2};", NoiseFilter.Vol1);
            byte[] sendBytes = Encoding.UTF8.GetBytes(sendstring);

            base.Send(sendBytes, sendBytes.Length, Constants.UdpEndPointAddress, Constants.UdpEndPointPort);


        }
    }
}
