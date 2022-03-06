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

        public new void Send(SendTypes sendTypes)
        {
            Console.WriteLine($"\nUdpService is sending {sendTypes}...");

            switch (sendTypes)
            {
                case SendTypes.SpeakOut:
                    SpeakOut();
                    break;
                case SendTypes.NoiseMeasure:
                    NoiseMeasure();
                    break;
                case SendTypes.KeepAlive:
                    KeepAlive();
                    break;
                case SendTypes.ChannelOn:
                    KeepAlive();
                    break;
                case SendTypes.ChannelOff:
                    KeepAlive();
                    break;
                case SendTypes.Ch1SendPackage:
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

            while (ReceivedString.Contains(Constants.AllZoneONAck)) ;

            StreamerService.Ch1Play("test.wav");
        }
        //
        private void NoiseMeasure()
        {
            byte[] sendBytes = Encoding.UTF8.GetBytes(Constants.AllZoneOFF);
            base.Send(sendBytes, sendBytes.Length, Constants.UdpEndPointAddress, Constants.UdpEndPointPort);
            Console.WriteLine($"\nSent {Constants.AllZoneOFF}");
        }

        
        public static void Ch1SendOn()
        {
            byte[] counterBytes = Encoding.ASCII.GetBytes(StreamerService.CommandCounter.ToString());
            if (StreamerService.CommandCounter < 99) StreamerService.CommandCounter++;
            else StreamerService.CommandCounter = 10;
            string sendstring = string.Format("{0}{1}{2}", Constants.Stream1OnFix, Constants.MyIP, Constants.Stream1Port);
            byte[] commandBytes = Encoding.ASCII.GetBytes(sendstring);
            byte[] sendBytes = new byte[counterBytes.Length + commandBytes.Length];

            Array.Copy(counterBytes, 0, sendBytes, 0, counterBytes.Length);
            Array.Copy(commandBytes, 0, sendBytes, counterBytes.Length, commandBytes.Length);

            

        }

        public  void Ch1SendOff()
        {
            byte[] counterBytes = Encoding.ASCII.GetBytes(StreamerService.CommandCounter.ToString());
            if (StreamerService.CommandCounter < 99) StreamerService.CommandCounter++;
            else StreamerService.CommandCounter = 10;
            byte[] commandBytes = Encoding.ASCII.GetBytes(Constants.Stream1Off);
            byte[] sendBytes = new byte[counterBytes.Length + commandBytes.Length];

            Array.Copy(counterBytes, 0, sendBytes, 0, counterBytes.Length);
            Array.Copy(commandBytes, 0, sendBytes, counterBytes.Length, commandBytes.Length);

            base.Send(sendBytes, sendBytes.Length, Constants.UdpEndPointAddress, Constants.UdpEndPointPort);
        }
        public static void Ch1SendPackage()
        {
            byte[] sendBytes;
            StreamerService.Ch1SendDatagram.Clear();
            for (int z = 0; z < 4; z++) StreamerService.Ch1SendDatagram.AddRange(BitConverter.GetBytes((ushort)0));

            for (int i = 0; i < 128; i++)
            {
                //if (CH1_waveFormFile_rb.Checked)
                //{
                    if (StreamerService.Ch1LastSampleSentOut >= StreamerService.Ch1SampleData.Count) StreamerService.Ch1Filended = true;
                //}
                /*else if (CH1_waveFormSine_rb.Checked || CH1_waveFormSaw_rb.Checked)
                {
                    if (ch1_LastSampleSentOut >= ch1_sampleData.Count) ch1_LastSampleSentOut = 0;
                }*/

                if (!StreamerService.Ch1Filended)
                {
                    StreamerService.Ch1SendDatagram.AddRange(BitConverter.GetBytes(StreamerService.Ch1SampleData[StreamerService.Ch1LastSampleSentOut]));
                    StreamerService.Ch1LastSampleSentOut++;
                }
                else
                {
                    StreamerService.Ch1SendDatagram.AddRange(BitConverter.GetBytes(1024));
                }

            }
            sendBytes = StreamerService.Ch1SendDatagram.ToArray();
            base.Send(sendBytes, sendBytes.Length, Constants.UdpEndPointAddress, Constants.UdpCh1Port);

            if (StreamerService.Ch1Filended)
            {
                Ch1SendOff();
                StreamerService.Ch1Needtoplay = false;
                // OnPlayEnded(new PlayStateChangedEventArgs("CH1", null, true));


            }
        }
        
        public void OnlySend(byte[] sendBytes)
        {
            base.Send(sendBytes, sendBytes.Length, Constants.UdpEndPointAddress, Constants.UdpEndPointPort);
        }
    }
}
