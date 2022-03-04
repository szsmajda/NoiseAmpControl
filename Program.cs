using System;
using System.Net;
using System.Threading.Tasks;

namespace NoiseAmpControl
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Description What to do when key is provided!");

            var voipEndPoint = new IPEndPoint(IPAddress.Parse(Constants.UdpEndPointAddress), Constants.UdpEndPointPort);
            var udpService = new UdpService(Constants.UdpClientPort, voipEndPoint);
            var serialPort = new SerialPortService(udpService);
            serialPort.Start();

            ReadConsoleAndSendAction(serialPort, udpService);
        }

        private static void ReadConsoleAndSendAction(SerialPortService serialPort, UdpService udpService)
        {
            while (true)
            {
                var taskState = Task.Run(() =>
                {
                    if (Console.ReadKey().KeyChar == 'i')
                    {
                        Console.WriteLine("\nSerialPort is closed\n");
                        serialPort.Stop();
                        Console.WriteLine("UdpService is sending...");
                        udpService.Send();
                    }
                });
            }
        }
    }
}