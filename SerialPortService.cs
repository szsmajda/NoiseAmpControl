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

        public SerialPortService() : base()
        {
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
            Console.WriteLine("SerialPort is listening...");
        }

        public void Stop()
        {
            this.DataReceived -= PortDataReceived;
            this.Close();
            this.Dispose();
            Console.WriteLine("\nSerialPort is closed.");
        }

        private void PortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            for (byte i = 0; i < Constants.MaxBytes; i++)
            {
                _receivedBytes[i] = Convert.ToByte(ReadByte());
            }

            NoiseFilter.StartCalculation(_receivedBytes);
        }

    }
}
