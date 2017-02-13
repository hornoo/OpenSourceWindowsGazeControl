using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tobii.EyeX.Framework;
using Tobii.EyeX.Client;
using EyeXFramework;
using System.Drawing;
using System.Windows;

namespace ZoomerSteeringDemo
{

    // Struct that is used to define the bounds of the dead zone(zone where when users is looking there is no scrolling input to the current window that has focus.
   
    public struct NoScollRect
    {
        public float LeftBound, RightBound, TopBound, BottomBound;

        public NoScollRect(float leftBound, float rightBound, float topBound, float bottomBound)
        {
            LeftBound = leftBound;
            RightBound = rightBound;
            TopBound = topBound;
            BottomBound = bottomBound;
        }
    }

    class ZoomSteer
    {

        GazePointDataStream gazeStream;
        System.Drawing.Size displayImageSize;
        public NoScollRect deadZoneRect;
        float deadZonePercent;
        public Vector GazeDirection { get; set; }
        System.Drawing.PointF windowlocation;


        public ZoomSteer(EyeXFramework.EyeXHost eyeX, System.Drawing.Size DisplayImageSize, int DeadZonePercent)
        {
            deadZonePercent = DeadZonePercent;

            displayImageSize = DisplayImageSize;

            gazeStream = eyeX.CreateGazePointDataStream(GazePointDataMode.LightlyFiltered);

            GazeDirection = new Vector(0, 0);
        }


        public void Start(System.Drawing.PointF Windowlocation)
        {
            windowlocation = Windowlocation;
            setDeadZoneBounds();
            gazeStream.Next += updateGazeCoodinates;
        }

        public void Stop()
        {
            gazeStream.Next -= updateGazeCoodinates;
        }

        private void updateGazeCoodinates(object o, GazePointEventArgs currentGaze)
        {
            GazeDirection = CalculateZoomDirection(currentGaze.X, currentGaze.Y);
           // Console.WriteLine(currentGaze.ToString());
        }


        private Vector CalculateZoomDirection(double xGaze, double yGaze)
        {
            int xAcceleration = 0;
            int yAcceleration = 0;

            if(xGaze > deadZoneRect.RightBound)
            {
                xAcceleration -= 1;
            }


            if(xGaze < deadZoneRect.LeftBound)
            {
                xAcceleration += 1;
            }


            if(yGaze > deadZoneRect.BottomBound)
            {
                yAcceleration -= 1;
            }

            if(yGaze < deadZoneRect.TopBound)
            {
                yAcceleration += 1;
            }

            return new Vector(xAcceleration, yAcceleration);

        }


        private void setDeadZoneBounds()
        {
            //Work out bounds of deadZoneRect rectangle ie place where no scrolling happens when the user is looking there.

            //Find Center of each axis
            float screenHolizontalCenter = windowlocation.X + (displayImageSize.Width / 2);
            float screenVerticalCenter = windowlocation.Y + (displayImageSize.Height / 2);

            //work out how many pixels the deadZone is on each axis
            float deadZoneWidth = ((deadZonePercent / 100) * displayImageSize.Width);
            float deadZoneHeight = ((deadZonePercent / 100) * displayImageSize.Height);

            //half this amount.
            float halfDeadZoneWidth = deadZoneWidth / 2;
            float halfDeadZoneHeight = deadZoneHeight / 2;

            //Set deaZone bounds from center of each axis
            deadZoneRect.LeftBound = screenHolizontalCenter - halfDeadZoneWidth;
            deadZoneRect.RightBound = screenHolizontalCenter + halfDeadZoneWidth;

            deadZoneRect.TopBound = screenVerticalCenter - halfDeadZoneHeight;
            deadZoneRect.BottomBound = screenVerticalCenter + halfDeadZoneHeight;
        }


    }
}
