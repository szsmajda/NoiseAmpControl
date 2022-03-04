using System;
using System.IO.Ports;

namespace NoiseAmpControl
{
    public class SerialPortService : SerialPort
    {
        private byte[] _receivedBytes { get; set; }
        public int Ch1Value;
        public int Ch2Value;
        public int Ch3Value;
        public int Ch4Value;
        private UdpService _udpService;

        public SerialPortService(UdpService udpService) : base()
        {
            _udpService = udpService;
            BaudRate = Constants.BaudRate;
            PortName = Constants.PortName;
            Parity = Parity.None;
            StopBits = StopBits.One;

            _receivedBytes = new byte[Constants.MaxBytes];
        }

        public void Start()
        {
            this.Open();
            this.DataReceived += PortDataReceived;
        }

        public void Stop()
        {
            this.DataReceived -= PortDataReceived;
            this.Close();
            this.Dispose();
        }

        private void PortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            for (byte i = 0; i < Constants.MaxBytes; i++)
            {
                _receivedBytes[i] = Convert.ToByte(ReadByte());
            }
            //NoiseStringConversion(chr_array);

            ///ChannelDataWrite();

            NoiseFilter.StartCalculation(_receivedBytes);

        }

        private void ChannelDataWrite()
        {
            Console.WriteLine(
                $"Ch1_value:{Ch1Value,0:D}\tCh2_value:{Ch2Value,0:D}\tCh3_value:{Ch3Value,0:D}\tCh4_value:{Ch4Value,0:D}\n\r\n");
        }
    }
}
