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

using RamGecTools;

namespace ZoomerSteeringDemo
{
    public partial class Form1 : Form
    {
        MouseHook globalMousehook;
        Graphics mainCanvas;
        Graphics offScreenCanvas;
        Graphics screenShot;
        Bitmap offScreenBitMap;
        Bitmap wholeScreenShot;
        Pen testpen;
        int moveaAmout;

        PointF drawLocation;

        ZoomSteer zoomXYMover;


        SizeF zoomAmount;
        SizeF zoomImageCenter;

        float zoomScalar = 0.01f;

        PointF ImageCenter; 
        
        //THis can be calculated from the size of the window
        PointF imageCenterOffset = new Point(150, 150);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            globalMousehook = new MouseHook();
            globalMousehook.LeftButtonDown += new MouseHook.MouseHookCallback(StartZoomAtLocation);
            globalMousehook.Install();
            
            mainCanvas = this.CreateGraphics();
            
            offScreenBitMap = new Bitmap(this.Width, this.Height);
            offScreenCanvas = Graphics.FromImage(offScreenBitMap);
            wholeScreenShot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            
            screenShot = Graphics.FromImage(wholeScreenShot);
            EyeXHost eyex = new EyeXHost();
            eyex.Start();
            
            zoomXYMover = new ZoomSteer(eyex, this.Size, 15);

            //This will gt passed in from screen fixation
            this.Location = new Point(800, 300);
            moveaAmout = 1;

            testpen = new Pen(Color.Red);

        }


        private void takeScreenShot()
        {
            screenShot.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            if((zoomXYMover.GazeDirection.X != 0) || (zoomXYMover.GazeDirection.Y != 0))
            {
                int xDirection = 0;
                int yDirection = 0;

                if (zoomXYMover.GazeDirection.X < 0)
                {
                    xDirection = -moveaAmout;
                }
                else if(zoomXYMover.GazeDirection.X > 0)
                {
                    xDirection = moveaAmout;
                }

                if (zoomXYMover.GazeDirection.Y < 0)
                {
                    yDirection = -moveaAmout;
                }
                else if (zoomXYMover.GazeDirection.Y > 0)
                {
                    yDirection = moveaAmout;
                }

                Console.WriteLine("X " + xDirection + " Y " + yDirection);

                drawLocation = new PointF(drawLocation.X + xDirection, drawLocation.Y + yDirection);

            }
            else
            {
            //calculate expansion and position amount.
            float xExpansionAmount = zoomAmount.Width * zoomScalar;
            float yExpansionAmount = zoomAmount.Height * zoomScalar;
            float xNewPositionAmount = ImageCenter.X * zoomScalar;
            float yNewPositionAmount = ImageCenter.Y * zoomScalar;
            ImageCenter.X += xNewPositionAmount;
            ImageCenter.Y += yNewPositionAmount;
            
            //May have to set up logic here, to move image according to direction of zoom..

            zoomAmount.Width = zoomAmount.Width + xExpansionAmount;
            zoomAmount.Height = zoomAmount.Height + yExpansionAmount;

            drawLocation = new PointF(drawLocation.X - xNewPositionAmount, drawLocation.Y - yNewPositionAmount);
            //Console.WriteLine(drawLocation);
            }

            offScreenCanvas.Clear(Color.White);
            offScreenCanvas.DrawImage(wholeScreenShot, drawLocation.X, drawLocation.Y, zoomAmount.Width, zoomAmount.Height);

            DrawBounds(zoomXYMover.deadZoneRect);

            mainCanvas.DrawImage(offScreenBitMap, 0, 0);

            
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            takeScreenShot();

            zoomAmount = new SizeF(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            drawLocation = new PointF(imageCenterOffset.X - ImageCenter.X, imageCenterOffset.Y - ImageCenter.Y);
            zoomImageCenter.Height =  drawLocation.Y + 150;
           zoomImageCenter.Width = drawLocation.Y + 150;

            mainCanvas.DrawImage(wholeScreenShot, drawLocation);

            zoomXYMover.Start(this.Location);
            timer1.Start();
        }

        private void StartZoomAtLocation(MouseHook.MSLLHOOKSTRUCT e)
        {
            timer1.Stop();
            //globalMousehook.Uninstall();

            ImageCenter = new PointF(e.pt.x,e.pt.y);

            takeScreenShot();

            zoomAmount = new SizeF(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            drawLocation = new PointF(imageCenterOffset.X - ImageCenter.X, imageCenterOffset.Y - ImageCenter.Y);
            zoomImageCenter.Height = drawLocation.Y + 150;
            zoomImageCenter.Width = drawLocation.Y + 150;

            mainCanvas.DrawImage(wholeScreenShot, drawLocation);



            zoomXYMover.Start(this.Location);
            timer1.Start();

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            globalMousehook.Uninstall();
        }

        private void DrawBounds(NoScollRect inputRect)
        {
            offScreenCanvas.DrawLine(testpen, inputRect.LeftBound, inputRect.TopBound, inputRect.RightBound, inputRect.TopBound);
        }

    }
}
