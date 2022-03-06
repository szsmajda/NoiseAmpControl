using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace NoiseAmpControl
{
    public class StreamerService
    {
        public  static UInt16 Ch1Startpackets;
        public  static UInt16 DefaultStartpackets;
        public  static int CommandCounter;
        public  static int Ch1LastSampleSentOut = 0;
        public  static int Ch1Timerpackets;
        public  static int DefaultTimerpackets;
        public  static long Ch1Timertime;
        public  static long DefaultTimertime;
        public  static ulong Ch1TestCounter;
        
        public static bool Ch1Needtoplay;
        public static bool Ch1Filended;
        public static bool Ch1NodataNeeded;
        public static bool Ch1StopIt;

        public static List<short> Ch1SampleData = new List<short>();
        public static List<byte> Ch1SendDatagram = new List<byte>();

        private static System.Threading.Timer _ch1Timer;

        public StreamerService()
        {

        }
        public static void Ch1Play(string anntext)
        {
            Ch1Timerpackets = DefaultTimerpackets;
            Ch1Timertime = DefaultTimertime;
            Ch1Startpackets = DefaultStartpackets;
            Ch1TestCounter = 0;
            Ch1Needtoplay = true;
            Ch1Filended = false;
            Ch1StopIt = false;
            Ch1LastSampleSentOut = 0;
            Ch1SampleData.Clear();
            byte[] samples = new byte[] { 0 };

            //if (CH1_waveFormFile_rb.Checked)
            //{
                NAudio.Wave.WaveStream stream = new WaveFileReader(anntext);
                NAudio.Wave.WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(stream);
                samples = new byte[pcm.Length];

                pcm.Read(samples, 0, samples.Length);

                for (int i = 0; i < samples.Length; i = i + 2)
                {
                    long yshort = samples[i + 1];
                    yshort = yshort << 8;
                    yshort += samples[i];

                    Ch1SampleData.Add((short)yshort);
                }
            //}
            /*else if (CH1_waveFormSine_rb.Checked)
            {
                samples = new byte[int.Parse(sampling_txt.Text) / int.Parse(CH1_freq_txt.Text)];

                for (int i = 0; i < samples.Length; i++)
                {
                    double ydouble = int.Parse(CH1_ampl_txt.Text) * Math.Sin((2 * Math.PI * i * int.Parse(CH1_freq_txt.Text)) / int.Parse(sampling_txt.Text));

                    ydouble = Math.Round(ydouble, 0);

                    short yshort = (short)ydouble;
                    ch1_sampleData.Add((short)yshort);
                }
            }
            else if (CH1_waveFormSaw_rb.Checked)
            {
                samples = new byte[int.Parse(sampling_txt.Text) / int.Parse(CH1_freq_txt.Text)];

                for (int i = 0; i < samples.Length; i++)
                {
                    ushort yushort = (ushort)(int.Parse(CH1_ampl_txt.Text) / samples.Length * i);

                    ch1_sampleData.Add((short)yushort);
                }
            }*/

            UdpService.Ch1SendOn();

            for (int i = 0; i < Ch1Startpackets; i++)
            {
                UdpService.Ch1SendPackage();

            }
            Ch1NodataNeeded = false;
            _ch1Timer = new System.Threading.Timer(Ch1TimerCallback, null, 16, Timeout.Infinite);
            //OnPlayStarted(new PlayStateChangedEventArgs("CH1", null, true));



        }

        private static void Ch1TimerCallback(Object state)
        {
            Ch1NodataNeeded = false;
            if (!Ch1NodataNeeded)
            {
                Stopwatch watch = new Stopwatch();

                for (int i = 0; i < Ch1Timerpackets; i++)
                {
                    if (Ch1Needtoplay) UdpService.Ch1SendPackage();
                }

                if (Ch1Needtoplay)
                {
                    if (!Ch1StopIt) _ch1Timer.Change(Math.Max(0, Ch1Timertime - watch.ElapsedMilliseconds), Timeout.Infinite);
                }
            }
            else if (Ch1Needtoplay)
            {
                if (!Ch1StopIt) _ch1Timer.Change(Math.Max(0, Ch1Timertime), Timeout.Infinite);
            }

            if (Ch1StopIt)
            {
                UdpService.Ch1SendOff();
                Ch1Needtoplay = false;
                //OnPlayStopped(new PlayStateChangedEventArgs("CH1", null, true));
            }
        }
    }
}
