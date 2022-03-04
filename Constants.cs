using System;

namespace NoiseAmpControl
{
    public static class Constants
    {
        public const int BaudRate = 38400;
        public const string PortName = "COM10";
        public const int UdpClientPort = 9;
        public const int UdpEndPointPort = 9;
        public const string UdpEndPointAddress = "192.168.1.122";

        public const UInt16 Noise1Cycle = 10;
        public const UInt16 Noise2Cycle = 10;
        public const UInt16 Noise3Cycle = 10;
        public const UInt16 Noise4Cycle = 10;

        public const bool N1IsEnable = true;
        public const bool N2IsEnable = true;
        public const bool N3IsEnable = false;
        public const bool N4IsEnable = false;

        public const int MaxChr = 4;
        public const int MaxBytes = 23;
    }
}
