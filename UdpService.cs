using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NoiseAmpControl
{
    public class UdpService : UdpClient
    {
        private IPEndPoint _iPEndPoint;
        public string ReceivedString = string.Empty;
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
                ReceivedString = Encoding.ASCII.GetString(receivedUdpbytes);
            }

            this.BeginReceive(new AsyncCallback(UdpReceive), null);
        }

        public new void Send(MeasureTypes measureTypes)
        {
            Console.WriteLine($"\nUdpService is sending {measureTypes}...");

            switch (measureTypes)
            {
                case MeasureTypes.SpeakOut:
                    SpeakOut();
                    break;
                case MeasureTypes.NoiseMeasure:
                    NoiseMeasure();
                    break;
                case MeasureTypes.KeepAlive:
                    KeepAlive();
                    break;
                default:
                    break;
            }
        }
        private void KeepAlive()
        {
            byte[] sendBytes = Encoding.UTF8.GetBytes(Constants.KeepAlive);
            base.Send(sendBytes, sendBytes.Length, Constants.UdpEndPointAddress, Constants.UdpEndPointPort);
        }

        private void SpeakOut()
        {
            var sendstring = string.Format("{0}{1:D2};", Constants.VolumeFix, NoiseFilter.Vol1);
            byte[] sendBytes = Encoding.UTF8.GetBytes(sendstring);
            base.Send(sendBytes, sendBytes.Length, Constants.UdpEndPointAddress, Constants.UdpEndPointPort);

            while (ReceivedString.Contains(Constants.VolumeAck));

            Console.WriteLine($"\nReceived {Constants.VolumeAck}");
            sendBytes = Encoding.UTF8.GetBytes(Constants.AllZoneON);
            base.Send(sendBytes, sendBytes.Length, Constants.UdpEndPointAddress, Constants.UdpEndPointPort);
            Console.WriteLine($"\nSent {Constants.AllZoneON}");
        }

        private void NoiseMeasure()
        {
            byte[] sendBytes = Encoding.UTF8.GetBytes(Constants.AllZoneOFF);
            base.Send(sendBytes, sendBytes.Length, Constants.UdpEndPointAddress, Constants.UdpEndPointPort);
            Console.WriteLine($"\nSent {Constants.AllZoneOFF}");
        }
    }
}
