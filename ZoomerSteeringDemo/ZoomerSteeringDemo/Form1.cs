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

        Point drawLocation;

        ZoomSteer zoomXYMover;

        Pen testpen;
        Point startLocation;
        SizeF zoomAmount;

        float zoomScalar = 0.001f;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            mainCanvas = panel1.CreateGraphics();
            offScreenBitMap = new Bitmap(panel1.Width, panel1.Height);
            offScreenCanvas = Graphics.FromImage(offScreenBitMap);
            wholeScreenShot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            screenShot = Graphics.FromImage(wholeScreenShot);
            EyeXHost eyex = new EyeXHost();
            eyex.Start();
            
            zoomXYMover = new ZoomSteer(eyex, panel1.Size, 15);



            //startLocation = new Point(panel1.Location.X, panel1.Location.Y);

            startLocation = this.PointToScreen(new Point(panel1.Location.X, panel1.Location.Y));
            testpen = new Pen(Color.Red);
        }

        private void takeScreenShot()
        {
             
            screenShot.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            takeScreenShot();

            zoomAmount = new SizeF(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            
            drawLocation = new Point(0,0);


            mainCanvas.DrawImage(wholeScreenShot, drawLocation);


            zoomXYMover.Start(startLocation);
            timer1.Start();
        }





        private void timer1_Tick(object sender, EventArgs e)
        {
           

            
           // drawLocation = new Point(drawLocation.X + (int)zoomXYMover.GazeDirection.X, drawLocation.Y + (int)zoomXYMover.GazeDirection.Y);
           
            
            offScreenCanvas.Clear(Color.White);
            offScreenCanvas.DrawImage(wholeScreenShot, drawLocation.X, drawLocation.Y, zoomAmount.Width, zoomAmount.Height);
            mainCanvas.DrawImage(offScreenBitMap, 0, 0);
           
            mainCanvas.DrawRectangle(testpen, startLocation.X, startLocation.Y, 1, 1);


            float xExpansionAmount = zoomAmount.Width * zoomScalar;
            float yExpansionAmount = zoomAmount.Height * zoomScalar;

            zoomAmount.Width = zoomAmount.Width + xExpansionAmount;
            zoomAmount.Height = zoomAmount.Height + yExpansionAmount;

            if (zoomXYMover.GazeDirection.X != 0 || zoomXYMover.GazeDirection.Y != 0)
            {
                
                
                int xDirection = 0;
                int yDirection = 0;

                if (zoomXYMover.GazeDirection.X < 0)
                {
                    xDirection = (int)((float)zoomXYMover.GazeDirection.X - xExpansionAmount);
                }else
                {
                    xDirection = (int)((float)zoomXYMover.GazeDirection.X + xExpansionAmount);
                }

                if (zoomXYMover.GazeDirection.Y < 0)
                {
                    yDirection = (int)((float)zoomXYMover.GazeDirection.Y - yExpansionAmount);
                }else
                {
                    yDirection = (int)((float)zoomXYMover.GazeDirection.Y + yExpansionAmount);
                }
                
                drawLocation = new Point(drawLocation.X + xDirection, drawLocation.Y + yDirection);
            }
        }
    }
}
