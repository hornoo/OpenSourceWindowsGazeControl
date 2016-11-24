using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZoomerSteeringDemo
{
    public partial class Form1 : Form
    {
        Graphics mainCanvas;
        Graphics offScreenCanvas;
        Graphics screenShot;
        Bitmap offScreenBitMap;
        Bitmap wholeScreenShot;

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

        }

        private void takeScreenShot()
        {
             
            screenShot.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            takeScreenShot();


            mainCanvas.DrawImage(wholeScreenShot, -500, -100);
        }





        private void timer1_Tick(object sender, EventArgs e)
        {

        }
    }
}
