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
        public static readonly bool debugPrint = false;
        private const bool debug = false;

        public struct xycoordinate
        {
            public double time;
            public double AvgPw;
            public double PekPw;
        };

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

            xycoordinate x;
            x.time = ats.SampleTime;
            x.AvgPw = (levels2[channel].AveragePower); 
            x.PekPw = (levels2[channel].PeakPower); 

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

            if(levels2[channel].AveragePower > .9)
            {
                High_Level_Detected++;
            }

            return levels2[channel].AveragePower;

        }

    }
}



/*  Old Code
        //public List<xycoordinate> data2d = new List<xycoordinate>();
        //= new xycoordinate();
        //            data2d.Add(x);            

Work Volume
            //Console.WriteLine("{0} {1} {2}", ats.SampleTime, aLevel, pLevel);
            //            if (prior_shot >sensitivity && level > sensitivity)
            //if (aLevel > sensitivity && pLevel > shotSensitivity && pLevel < maxSensitivity)

                    //return 1;
                    //return (float)aLevel;

                //return 1- levels[channel].PeakPower / -120;
            //bar.Progress = levels[channel].PeakPower / -120;



*/
