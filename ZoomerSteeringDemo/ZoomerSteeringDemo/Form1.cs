using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Tobii.EyeX.Framework;
using Tobii.EyeX.Client;
using EyeXFramework;

namespace ZoomerSteeringDemo
{
    public partial class Form1 : Form
    {
        Graphics mainCanvas;
        Graphics offScreenCanvas;
        Graphics screenShot;
        Bitmap offScreenBitMap;
        Bitmap wholeScreenShot;

        PointF drawLocation;

        ZoomSteer zoomXYMover;

        Pen testpen;

        SizeF zoomAmount;
        SizeF zoomImageCenter;

        float zoomScalar = 0.01f;


        Point startLocation = new Point(450, 450);
        Point imageCenterOffset = new Point(150, 150);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            mainCanvas = this.CreateGraphics();
            offScreenBitMap = new Bitmap(this.Width, this.Height);
            offScreenCanvas = Graphics.FromImage(offScreenBitMap);
            wholeScreenShot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            //wholeScreenShot = new Bitmap( this.Width, this.Height);
            screenShot = Graphics.FromImage(wholeScreenShot);
            EyeXHost eyex = new EyeXHost();
            eyex.Start();
            
            zoomXYMover = new ZoomSteer(eyex, this.Size, 20);

            this.Location = new Point(400, 0);



            //startLocation = new Point(panel1.Location.X, panel1.Location.Y);

           // startLocation = this.PointToScreen(new Point(this.Location.X, this.Location.Y));
            //startLocation = new Point(0, 0);
            testpen = new Pen(Color.Red);
        }

        private void takeScreenShot()
        {
             
            screenShot.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
            //screenShot.CopyFromScreen(0, 0, 0, 0, this.Size, CopyPixelOperation.SourceCopy);
           
            //Set this value later on to be what ever the image zoom location is plus half the size of the zoom window.
           
        }







        private void timer1_Tick(object sender, EventArgs e)
        {
           
            //mainCanvas.DrawRectangle(testpen, startLocation.X, startLocation.Y, 1, 1);

            // Take screen shot of screen region, zoom into ,keep centered.
            float xExpansionAmount = zoomAmount.Width * zoomScalar;
            float yExpansionAmount = zoomAmount.Height * zoomScalar;

            float xNewPositionAmount = startLocation.X * zoomScalar;
            float yNewPositionAmount = startLocation.Y * zoomScalar;

            //May have to set up logic here, to move image according to direction of zoom..
            zoomImageCenter.Width = zoomImageCenter.Width + xNewPositionAmount;
            zoomImageCenter.Height = zoomImageCenter.Height + yNewPositionAmount;

            zoomAmount.Width = zoomAmount.Width + xExpansionAmount;
            zoomAmount.Height = zoomAmount.Height + yExpansionAmount;




            drawLocation = new PointF(drawLocation.X - yNewPositionAmount, drawLocation.Y - yNewPositionAmount);
            offScreenCanvas.Clear(Color.White);
            offScreenCanvas.DrawImage(wholeScreenShot, drawLocation.X, drawLocation.Y, zoomAmount.Width, zoomAmount.Height);
            mainCanvas.DrawImage(offScreenBitMap, 0, 0);

            //    int xDirection = 0;
            //    int yDirection = 0;

            //    if (zoomXYMover.GazeDirection.X < 0)
            //    {
            //        xDirection = (int)((float)zoomXYMover.GazeDirection.X - xExpansionAmount);
            //    }
            //    else
            //    {
            //        xDirection = (int)((float)zoomXYMover.GazeDirection.X + xExpansionAmount);
            //    }

            //    if (zoomXYMover.GazeDirection.Y < 0)
            //    {
            //        yDirection = (int)((float)zoomXYMover.GazeDirection.Y - yExpansionAmount);
            //    }
            //    else
            //    {
            //        yDirection = (int)((float)zoomXYMover.GazeDirection.Y + yExpansionAmount);
            //    }

            //    drawLocation = new Point(drawLocation.X + xDirection, drawLocation.Y + yDirection);
            
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            takeScreenShot();

            zoomAmount = new SizeF(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            drawLocation = new PointF(imageCenterOffset.X - startLocation.X, imageCenterOffset.Y - startLocation.Y);
            zoomImageCenter.Height =  drawLocation.Y + 150;
           zoomImageCenter.Width = drawLocation.Y + 150;

            mainCanvas.DrawImage(wholeScreenShot, drawLocation);

            zoomXYMover.Start(this.Location);
            timer1.Start();
        }
    }
}
