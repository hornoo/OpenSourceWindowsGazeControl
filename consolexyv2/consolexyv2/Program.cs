﻿using EyeXFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tobii.EyeX.Framework;
using GazeToolBar;

namespace consolexyv2
{
    class Program
    {
        public static void CustomGazeHandler(object eventRaiser, GazePointEventArgs ge)
        {
            Console.Clear();
            Console.WriteLine("X-Axis {0:0.0} Y-Axis {1:0.0} " , ge.X  , ge.Y);
            Console.WriteLine(teststream.varDebug);
        }

        static CustomFixationDataStream teststream;

        static void Main(string[] args)
        {

            

            using (var eyeXHost = new EyeXHost())
            {

                 teststream = new CustomFixationDataStream(eyeXHost);
                

                // Create a data stream: lightly filtered gaze point data.
                // Other choices of data streams include EyePositionDataStream and FixationDataStream.
                using (var lightlyFilteredGazeDataStream = eyeXHost.CreateGazePointDataStream(GazePointDataMode.LightlyFiltered))
                {
                    // Start the EyeX host.
                    eyeXHost.Start();

                    // Write the data to the console.
                     
                    //Create the Deligate

                    EventHandler<GazePointEventArgs> displaydel = new EventHandler<GazePointEventArgs>(CustomGazeHandler);

                    lightlyFilteredGazeDataStream.Next += displaydel;

                   // lightlyFilteredGazeDataStream.Next += (s, e) => Console.WriteLine("Gaze point at ({0:0.0}, {1:0.0})", e.X, e.Y);

                    //eyeXHost.UserPresenceChanged += (s, e) => Console.WriteLine("changed");
                    //var fixationStream = eyeXHost.CreateFixationDataStream(FixationDataMode.Slow);

                    //fixationStream.Next += (s, e) => Console.WriteLine("you fixated at x " + e.X + "y " + e.Y);

                    // Let it run until a key is pressed.
                    Console.WriteLine("Listening for gaze data, press any key to exit...");
                    Console.In.Read();
                }
            }

        }

    }
}
