using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tobii.EyeX.Framework;
using Tobii.EyeX.Client;
using EyeXFramework;

namespace ZoomerSteeringDemo
{
    class ZoomSteer
    {

        GazePointDataStream gazeStream;

            double currentGazeLocationX;
            double currentGazeLocationY;

        public ZoomSteer(EyeXFramework.EyeXHost eyeX)
        {
            gazeStream = eyeX.CreateGazePointDataStream(GazePointDataMode.LightlyFiltered);

            gazeStream.Next += updateGazeCoodinates;
        }


        private void updateGazeCoodinates(object o, GazePointEventArgs currentGaze)
        {
            //Save the users gaze to a field that has global access in this class.
            currentGazeLocationX = currentGaze.X;
            currentGazeLocationY = currentGaze.Y;

            //if the users gaze goes off screen, Stop Scroll control running, this returns control back to the statemanager.
            //if (checkIFGazeOffScreen(currentGaze.X, currentGaze.Y))
            //{
            //    stopScroll();
            //}

        }

    }
}
