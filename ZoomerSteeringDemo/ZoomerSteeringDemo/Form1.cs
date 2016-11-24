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
            
            zoomXYMover = new ZoomSteer(eyex, panel1.Size, 50);

            startLocation = new Point(panel1.Location.X, panel1.Location.Y);
            testpen = new Pen(Color.Red);
        }

        private void takeScreenShot()
        {
             
            screenShot.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            takeScreenShot();
            drawLocation = new Point(0,0);
            mainCanvas.DrawImage(wholeScreenShot, drawLocation);

            

            zoomXYMover.Start(startLocation);
            timer1.Start();
        }





        private void timer1_Tick(object sender, EventArgs e)
        {

            drawLocation = new Point(drawLocation.X + (int)zoomXYMover.GazeDirection.X, drawLocation.Y + (int)zoomXYMover.GazeDirection.Y);
            mainCanvas.DrawImage(wholeScreenShot, drawLocation);
            mainCanvas.DrawRectangle(testpen, startLocation.X, startLocation.Y, 1, 1);
        }
    }
}
