using System;

namespace NoiseAmpControl
{
    public static class Constants
    {
        public const int BaudRate = 38400;
        public const string PortName = "COM10";
        public const int UdpClientPort = 9;
        public const int UdpEndPointPort = 9;
        public const int UdpCh1Port = 23410;
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

        public const string VolumeFix = "00TC*31v";
        public const string VolumeAck = "ACK:00TC*31v";
        public const string AllZoneON = "00TX8";
        public const string AllZoneONAck = "ACK:00TX8";
        public const string AllZoneOFF = "00TX0";
        public const string KeepAliveAck = "ACK:00KEEPALIVE";
        public const string KeepAlive = "00KEEPALIVE";

        public const string Stream1OnFix = "X1";

        public const string Stream1Off = "C00";



        public const string MyIP = "192.168.1.87";
        public const string Stream1Port = "23410";
        public const string Stream2Port = "23420";

        


    }
}
