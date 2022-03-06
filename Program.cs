using System;
using System.Net;
using System.Threading.Tasks;
using System.Text;

namespace NoiseAmpControl
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press 'i' to run 'SpeakOut', press 'x' to run 'NoiseMeasure'!");

            var voipEndPoint = new IPEndPoint(IPAddress.Parse(Constants.UdpEndPointAddress), Constants.UdpEndPointPort);
            var udpService = new UdpService(Constants.UdpClientPort, voipEndPoint);
            udpService.Send(MeasureTypes.KeepAlive);
            while (!udpService.ReceivedString.Contains(Constants.KeepAliveAck)) ;

            var serialPort = new SerialPortService();
            serialPort.Start();
            ReadConsoleAndSendAction(serialPort, udpService);
        }

        private static void ReadConsoleAndSendAction(SerialPortService serialPort, UdpService udpService)
        {
            while (true)
            {
                var taskState = Task.Run(() =>
                {
                    switch (Console.ReadKey().KeyChar)
                    {
                        case 'i':
                            serialPort.Stop();
                            udpService.Send(MeasureTypes.SpeakOut);
                            break;
                        case 'x':
                            udpService.Send(MeasureTypes.NoiseMeasure);
                            if (!serialPort.IsOpen)
                            {
                                serialPort.Start();
                            }
                            break;
                        default:
                            break;
                    }

                    Console.WriteLine("----------------------------------");
                });
            }
        }

        
    }
}