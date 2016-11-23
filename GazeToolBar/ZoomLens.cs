﻿using EyeXFramework;
using EyeXFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GazeToolBar
{
    public partial class ZoomLens : Form
    {
        const int ZOOMLEVEL = 3;// this is controls how far the lens will zoom in
        const int ZOOMLENS_SIZE = 500;//setting the width & height of the ZoomLens
        int EDGEOFFSET = (int)ValueNeverChange.Y_AXIS_TOP_OF_SCREEN_CUT_OFF_BOUNDARY;

        int topScreenEdgeOffset;

        Graphics graphics;
        Graphics offScreenGraphics;
        Graphics mainCanvas;
        Bitmap zoomedScreenshot;
        Bitmap offScreenBitmap;
        Point lensPoint;
        FixationDetection fixdet;
        GazeHighlight gazeHighlight;

       

        public ZoomLens(FixationDetection FixDet, FormsEyeXHost EyeXHost)
        {
            InitializeComponent();
            lensPoint = new Point();
            this.Width = ZOOMLENS_SIZE;
            this.Height = ZOOMLENS_SIZE;
            offScreenBitmap = new Bitmap(this.Width, this.Height);
            zoomedScreenshot = new Bitmap(this.Width / ZOOMLEVEL, this.Height / ZOOMLEVEL);
            mainCanvas = this.CreateGraphics();
            offScreenGraphics = Graphics.FromImage(offScreenBitmap);
            graphics = Graphics.FromImage(zoomedScreenshot);
            this.FormBorderStyle = FormBorderStyle.None;//removes window borders from form
            fixdet = FixDet;
            topScreenEdgeOffset = 0;

            gazeHighlight = new GazeHighlight(FixDet, offScreenGraphics, EHighlightShaderType.RedToGreen, this);
        }
        public void CreateZoomLens(Point FixationPoint)
        {
            Size zoomSize = new Size(this.Size.Width / 2, this.Size.Height / 2);
            this.Show();//make lens visible
            offScreenGraphics.DrawImage(zoomedScreenshot, 0, 0, zoomedScreenshot.Width, zoomedScreenshot.Height);
            this.TopMost = true;
            Console.WriteLine("ZoomLens.Bounds.X = " + this.Bounds.X);
            Console.WriteLine("ZoomLens.Bounds.Y = " + this.Bounds.Y);
            DrawTimer.Start();
            Application.DoEvents();
        }
        //This method measures from each corner to the fixation point to determine if the fixation point is in a corner
        //if the user looked in a corner to returns the appropriate Corner enum or it returns a noCorner enum
        public Corner checkCorners(Point FixationPoint)
        {
            //int maxCornerDistance = zoomedScreenshot.Width + (int)(Screen.PrimaryScreen.Bounds.Height * 0.1);
            //int screenWidth = Screen.FromControl(this).Bounds.Width;
            //int screenHeight = Screen.FromControl(this).Bounds.Height;


            ////corners as points
            //Point topLeft = new Point(0, 0);
            //Point topRight = new Point(screenWidth, 0);
            //Point bottomLeft = new Point(0, screenHeight);
            //Point bottomRight = new Point(screenWidth, screenHeight);

            //Point[] Corners = { topLeft, topRight, bottomLeft, bottomRight };

            ////checking each corner against maxCornerDistance
            //for (int i = 0; i < Corners.Length; i++)
            //{
            //    if (calculateCornerDistance(FixationPoint, Corners[i]) < maxCornerDistance)
            //    {
            //        return (Corner)i;
            //    }
            //}
            //return Corner.NoCorner;

            if (FixationPoint.X < EDGEOFFSET && FixationPoint.Y < EDGEOFFSET)
            {
                return Corner.TopLeft;
            }

            return Corner.NoCorner;

        }

        //This method checks if the user looked near an edge of the screen and returns the appropriate enum
        public Edge checkEdge()
        {
            Edge edge = Edge.NoEdge;



            if (this.DesktopLocation.Y < -EDGEOFFSET && this.DesktopLocation.X < -EDGEOFFSET)
            {
                return Edge.TopLeft;
            }
            if (this.DesktopLocation.Y < -EDGEOFFSET)//top
            {
                return Edge.Top;
            }
            if (this.DesktopLocation.X < -EDGEOFFSET)//left
            {
                return Edge.Left;
            }
            if (this.DesktopLocation.Y + this.Height > Screen.PrimaryScreen.Bounds.Size.Height + EDGEOFFSET)//bottom
            {
                return Edge.Bottom;
            }
            if (this.DesktopLocation.X + this.Width > Screen.PrimaryScreen.Bounds.Size.Width + EDGEOFFSET)//right
            {
                return Edge.Right;
            }
            return edge;
        }
        //Once a corner is detected this method is used to position the form right up against the corner. This is to prevent the window from moving off-screen
        public void setZoomLensPositionCorner(Corner corner)
        {
            switch (corner)
            {
                case Corner.TopLeft:
                    this.DesktopLocation = new Point(0, 0);
                    lensPoint = new Point(0, 0);
                    break;
                case Corner.TopRight:
                    this.DesktopLocation = new Point(Screen.FromControl(this).Bounds.Width - this.Width, 0);
                    lensPoint = new Point(Screen.FromControl(this).Bounds.Width - zoomedScreenshot.Width, 0);
                    break;
                case Corner.BottomLeft:
                    this.DesktopLocation = new Point(0, Screen.FromControl(this).Bounds.Height - this.Height);
                    lensPoint = new Point(0, Screen.FromControl(this).Bounds.Height - zoomedScreenshot.Height);
                    break;
                case Corner.BottomRight:
                    this.DesktopLocation = new Point(Screen.FromControl(this).Bounds.Width - this.Width, Screen.FromControl(this).Bounds.Height - this.Height);
                    lensPoint = new Point(Screen.FromControl(this).Bounds.Width - zoomedScreenshot.Width, Screen.FromControl(this).Bounds.Height - zoomedScreenshot.Height);
                    break;
            }
        }
        //If an edge is detected this method is called to position the zoomLens right up against an edge of the screen. 
        public void setZoomLensPositionEdge(Edge edge, Point fixationPoint)
        {
            switch (edge)
            {
                case Edge.TopLeft:
                    topScreenEdgeOffset = EDGEOFFSET;
                    this.DesktopLocation = new Point(0,  topScreenEdgeOffset);
                    lensPoint = new Point(this.DesktopLocation.X, this.DesktopLocation.Y - topScreenEdgeOffset);
                    break;
                case Edge.NoEdge:
                    break;
                case Edge.Top:
                    //Integrate here as well
                    topScreenEdgeOffset = EDGEOFFSET;
                    this.DesktopLocation = new Point(this.DesktopLocation.X,  topScreenEdgeOffset);
                    lensPoint = new Point(calculateLensPointX(fixationPoint.X), this.DesktopLocation.Y - topScreenEdgeOffset);
                    break;
                case Edge.Right:
                    this.DesktopLocation = new Point(Screen.PrimaryScreen.Bounds.Size.Width - this.Width, this.DesktopLocation.Y);
                    lensPoint = new Point(Screen.PrimaryScreen.Bounds.Size.Width - zoomedScreenshot.Width, calculateLensPointY(fixationPoint.Y));
                    break;
                case Edge.Bottom:
                    this.DesktopLocation = new Point(this.DesktopLocation.X, Screen.PrimaryScreen.Bounds.Size.Height - this.Height);
                    lensPoint = new Point(calculateLensPointX(fixationPoint.X), Screen.PrimaryScreen.Bounds.Size.Height - zoomedScreenshot.Height);
                    break;
                case Edge.Left:
                    this.DesktopLocation = new Point(0, this.DesktopLocation.Y);
                    lensPoint = new Point(this.DesktopLocation.X,calculateLensPointY(fixationPoint.Y));
                    break;
            }
        }
        //distance calculation from corner point to corner
        private int calculateCornerDistance(Point fixationPoint, Point corner)
        {
            int returnInt = Math.Abs(fixationPoint.X - corner.X) + Math.Abs(fixationPoint.Y - corner.Y);
            return returnInt;
        }

        public void determineDesktopLocation(Point FixationPoint)
        {
            this.DesktopLocation = new Point(FixationPoint.X - (this.Width / 2), FixationPoint.Y - (this.Height / 2));
            SetLensPoint(FixationPoint, Edge.NoEdge);
        }
        private int calculateLensPointX(int fixationX)
        {
            int x;
            x = fixationX - (int)((this.Width / ZOOMLEVEL) * 1.25);
            x = x + this.Size.Width / 4;
            return x;
        }
        private int calculateLensPointY(int fixationY)
        {
            int y;
            y = fixationY - (int)((this.Height / ZOOMLEVEL) * 1.25);
            y = y + this.Size.Height / 4;
            return y;
        }
        public void SetLensPoint(Point FixationPoint, Edge edge)//determines the location of the zoomed in screenshot
        {
            switch (edge)
            {
                case Edge.NoEdge:
                    lensPoint.X = calculateLensPointX(FixationPoint.X);
                    lensPoint.Y = calculateLensPointY(FixationPoint.Y);
                    break;
                case Edge.Top:
                    lensPoint.X = calculateLensPointX(FixationPoint.X);
                    lensPoint.Y = this.DesktopLocation.Y;
                    break;
                case Edge.Right:
                    lensPoint.X = Screen.PrimaryScreen.Bounds.Size.Width - zoomedScreenshot.Width;
                    lensPoint.Y = calculateLensPointY(FixationPoint.Y);
                    break;
                case Edge.Bottom:
                    lensPoint.X = calculateLensPointX(FixationPoint.X);
                    lensPoint.Y = Screen.PrimaryScreen.Bounds.Size.Height - zoomedScreenshot.Height;
                    break;
                case Edge.Left:
                    lensPoint.X = this.DesktopLocation.X;
                    lensPoint.Y = calculateLensPointY(FixationPoint.Y);
                    break;
            }
        }
        public void TakeScreenShot()
        {
            Size zoomSize = new Size(this.Size.Width / 2, this.Size.Height / 2);
            graphics.CopyFromScreen(lensPoint.X, lensPoint.Y, 0, 0, zoomSize, CopyPixelOperation.SourceCopy);
        }
        public void ResetZoomLens()
        {
            DrawTimer.Stop();
            this.Hide();
        }
        //This method takes a point on the zoomed-in form and translates it to the equivalent desktop coordinates
        public Point TranslateGazePoint(Point fixationPoint)
        {
            Point relativePoint = this.PointToClient(fixationPoint);//this gets the on form coordinates from the fixation point(which is screen coordinates)
            //check to see if the user actually fixated on the ZoomLens
            if (relativePoint.X < 0 || relativePoint.Y < 0 || relativePoint.X > this.Width || relativePoint.Y > this.Height)
            {
                return new Point(-1, -1);//If it is out of bound at all, this will return -1, -1. The zoom will be canceled
            }
            return TranslateToDesktop(relativePoint.X, relativePoint.Y);
        }
        private Point TranslateToDesktop(int x, int y)//This method translates on form coordinates to desktop coordinates
        {
            Point returnPoint = new Point();

            int halfHeight = this.Width / 2;
            int halfWidth = this.Height / 2;
            int halfHeightDivZoom = halfHeight / ZOOMLEVEL;
            int halfWidthDivZoom = halfWidth / ZOOMLEVEL;

            int finalY = halfHeight - halfHeightDivZoom;
            
            // ? tidy this up, integrate to work when in top zone of window.
            finalY = this.Top + finalY - topScreenEdgeOffset;
            returnPoint.Y = finalY + (y / ZOOMLEVEL);

            int finalX = halfWidth - halfWidthDivZoom;
            finalX = this.Left + finalX;
            returnPoint.X = finalX + (x / ZOOMLEVEL);
            topScreenEdgeOffset = 0;
            return returnPoint;
        }
        //This method offsets the final fixation position calculations based on what edge has been detected.
        //Instead of zooming from the middle of the zoomlens, when an edge happens it zooms from the edge of the screen, so the final positon needs to be offset in order to
        //accurately click where the user was looking.
        public Point edgeOffset(Edge edge, Point fixationPoint)
        {
            int offset = (int)(ZOOMLENS_SIZE * 0.34);/*This used to calculate the offset based on zoomlevel etc, but was lost in a git accident. RIP. This version works but only
                                                    * for zoom level 3*/
            switch (edge)
            {
                case Edge.NoEdge:
                    return fixationPoint;
                case Edge.Top:
                    fixationPoint.Y = fixationPoint.Y - offset;
                    break;
                case Edge.Right:
                    fixationPoint.X = fixationPoint.X + offset;
                    break;
                case Edge.Bottom:
                    fixationPoint.Y = fixationPoint.Y + offset;
                    break;
                case Edge.Left:
                    fixationPoint.X = fixationPoint.X - offset;
                    break;
            }
            return fixationPoint;
        }
        //this method is similar to the edge offset
        //it has to offset the final fixation value in by the X and the Y value to move the fixation into the corner.
        public Point cornerOffset(Corner corner, Point fixationPoint)
        { 
            int offset = (int)(ZOOMLENS_SIZE * 0.34);
            switch (corner)
            {
                case Corner.NoCorner:
                    return fixationPoint;
                case Corner.TopLeft:
                    fixationPoint.X = fixationPoint.X - offset;
                    fixationPoint.Y = fixationPoint.Y - offset;
                    return fixationPoint;
                case Corner.TopRight:
                    fixationPoint.X = fixationPoint.X + offset;
                    fixationPoint.Y = fixationPoint.Y - offset;
                    return fixationPoint;
                case Corner.BottomLeft:
                    fixationPoint.X = fixationPoint.X - offset;
                    fixationPoint.Y = fixationPoint.Y + offset;
                    return fixationPoint;
                case Corner.BottomRight:
                    fixationPoint.X = fixationPoint.X + offset;
                    fixationPoint.Y = fixationPoint.Y + offset;
                    return fixationPoint;
            }
            return fixationPoint;
        }

        private void DrawTimer_Tick(object sender, EventArgs e)
        {
            offScreenGraphics.DrawImage(zoomedScreenshot, 0, 0, ZOOMLENS_SIZE, ZOOMLENS_SIZE);

            gazeHighlight.DrawHightlight();

            mainCanvas.DrawImage(offScreenBitmap, 0, 0);
        }
    }
}