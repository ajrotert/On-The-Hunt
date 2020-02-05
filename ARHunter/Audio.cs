using System;
using AudioToolbox;
using System.Collections.Generic;
using System.IO;

namespace ARHunter
{
    public class Audio
    {
        public Audio()
        {
            High_Level_Detected = 0;
        }

        private const double sensitivity = .935;
        private const double shotSensitivity = .965;
        private const double maxSensitivity = .995;
        private InputAudioQueue mAudioQueue;
        public bool started = false;

        public int High_Level_Detected;
        bool debug = false;

        public struct xycoordinate
        {
            public double time;
            public double AvgPw;
            public double PekPw;
        };

        //public List<xycoordinate> data2d = new List<xycoordinate>();

        public void autoStart()
        {
             AudioStreamBasicDescription basic = new AudioStreamBasicDescription();
             basic.SampleRate = 44100;

             basic.Format = AudioFormatType.LinearPCM;
             basic.FormatFlags = AudioFormatFlags.LinearPCMIsBigEndian |
             AudioFormatFlags.LinearPCMIsSignedInteger |
             AudioFormatFlags.LinearPCMIsPacked;
             basic.BytesPerPacket = 2;
             basic.BytesPerFrame = 2;
             basic.FramesPerPacket = 1;
             basic.ChannelsPerFrame = 1;
             basic.BitsPerChannel = 16;

             mAudioQueue = new InputAudioQueue(basic);
             mAudioQueue.Start();
             mAudioQueue.EnableLevelMetering = true;

            started = true;
        }


        public void stop()
        {
            if(started)
                mAudioQueue.Stop(true);
            started = false;
        }
        public float workVolume()
        {

            int channel = 0;            

            AudioTimeStamp ats = mAudioQueue.CurrentTime;
            
            AudioQueueLevelMeterState[] levels = mAudioQueue.CurrentLevelMeterDB;
            AudioQueueLevelMeterState[] levels2 = mAudioQueue.CurrentLevelMeter;
            //Console.WriteLine("Test 1: {1}, {0}, {2}", (120 - levels[channel].AveragePower * -1), ats.SampleTime, (120 - levels[channel].PeakPower * -1));

            //Console.WriteLine("Test 2: {1}, {0}, {2}", (levels2[channel].AveragePower), ats.SampleTime, (levels2[channel].PeakPower));

            xycoordinate x;//= new xycoordinate();
            x.time = ats.SampleTime;
            x.AvgPw = (levels2[channel].AveragePower); //(120 - levels[channel].AveragePower * -1);
            x.PekPw = (levels2[channel].PeakPower);// (120 - levels[channel].PeakPower * -1);
            //            data2d.Add(x);            
            string output = x.time + " " + x.AvgPw + " " + x.PekPw + " ";

            if (debug)
            {
                output = "(" + x.time + "," + x.AvgPw + "," + x.PekPw + ") , ";
                try
                {
                    var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    var filename = Path.Combine(documents, "Write2.txt");
                    File.AppendAllText(filename, output);
                }
                catch { }
            }

            Console.WriteLine(output);

            double aLevel = 1 - (levels[channel].AveragePower / -120);
            double pLevel = 1 - (levels[channel].PeakPower / -120);
            //Console.WriteLine("{0} {1} {2}", ats.SampleTime, aLevel, pLevel);
            //            if (prior_shot >sensitivity && level > sensitivity)
            //if (aLevel > sensitivity && pLevel > shotSensitivity && pLevel < maxSensitivity)
            if(levels2[channel].AveragePower > .9)
            {
                High_Level_Detected++;
                //return 1;
            }

            //return (float)aLevel;
            return levels2[channel].AveragePower;
            //return 1- levels[channel].PeakPower / -120;
            //bar.Progress = levels[channel].PeakPower / -120;

        }

    }
}



