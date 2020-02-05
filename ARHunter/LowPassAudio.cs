using System;
using AVFoundation;
using Foundation;
using AudioToolbox;
using System.Runtime.InteropServices;
using CoreFoundation;
using UIKit;
using System.Collections.Generic;

namespace ARHunter
{
    public class LowPassAudio
    {
        public static int High_Level_Detected;
        public static bool started = false;
        AVAudioEngine engine;
        AVAudioUnitEQ nodeEQ;
        public static Queue<float> FloatQueue;

        public LowPassAudio()
        {
            High_Level_Detected = 0;
        }
        public void AudioSetupStart()
        {
            FloatQueue = new Queue<float>();
            engine = new AVAudioEngine();
            nodeEQ = new AVAudioUnitEQ(1);
            nodeEQ.GlobalGain = 1;
            engine.AttachNode(nodeEQ);

            AVAudioUnitEQFilterParameters filter = nodeEQ.Bands[0];

            filter.FilterType = AVAudioUnitEQFilterType.LowPass;
            filter.Frequency = 1000; //In hertz
            filter.Bandwidth = 1;
            filter.Bypass = false;
            // in db -96 db through 24 d
            filter.Gain = 50;

            //not sure if this is necessary
            nodeEQ.Bands[0] = filter;

            //1
            AVAudioFormat format2 = engine.MainMixerNode.GetBusOutputFormat(0);

            //2
            //AVAudioPcmBuffer buffMix = new AVAudioPcmBuffer(engine.MainMixerNode.GetBusInputFormat(0),2);
            //AVAudioTime timeMix = engine.MainMixerNode.LastRenderTime;
            //AVAudioNodeTapBlock MixerBlock = new AVAudioNodeTapBlock((buffMix, timeMix) =>

                        //2
            engine.MainMixerNode.InstallTapOnBus(0, 1024, format2, (AVAudioPcmBuffer buffMix, AVAudioTime when)=>
            {
                Console.WriteLine("Called");
                
                //3     **Dont have an 'Updater' also not checking for null**
                IntPtr channelData = buffMix.FloatChannelData;

                int lengthOfBuffer = (int)buffMix.FrameLength;

                byte[] bytesArray = new byte[lengthOfBuffer];

                Marshal.Copy(channelData, bytesArray, 0, lengthOfBuffer);

                double total = 0;
                int nonZero = 0;
                for (int a = 0; a < buffMix.FrameLength - 4; a+=1)
                {
                    //float tempx = BitConverter.ToSingle(bytesArray, a);
                    float tempx = bytesArray[a];
                    Console.WriteLine(tempx);
                    double temp = Math.Pow(tempx, 2);
                    total += temp;
                    if (temp.Equals(0))
                        nonZero++;
                }
                total = Math.Sqrt(total / nonZero);
                double avgPower = 20 * Math.Log10(total);
                avgPower /= 160;

                if (avgPower > .9)
                    High_Level_Detected++;
                FloatQueue.Enqueue((float)avgPower);
                //Console.WriteLine(avgPower);

                Marshal.FreeHGlobal(channelData);
            });

            AVAudioFormat format = engine.InputNode.GetBusInputFormat(0);
            engine.Connect(engine.InputNode, engine.MainMixerNode, format);
            engine.Connect(nodeEQ, engine.MainMixerNode, format);

            StartEngine();
            started = true;
        }

        private void StartEngine()
        {
            NSError error;
            if (engine.StartAndReturnError(out error))
                Console.WriteLine("Did not start audio");
            if (error != null)
                Console.WriteLine(error.LocalizedDescription);
        }

    }
}
