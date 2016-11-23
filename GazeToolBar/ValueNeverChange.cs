﻿using System.Drawing;
using System.Windows.Forms;

namespace GazeToolBar
{
    /*
        Date: 17/05/2016
        Name: Derek Dai
        Description: Put all the constant value or method in here
    */
    static class ValueNeverChange
    {
        public static readonly int MAX_TIME_LENGTH = 2500;
        public static readonly int MIN_TIME_LENGTH = 500;
        public static readonly int GAP_TIME_LENGTH = 200;
        public static readonly int DEFAULT_TIME_LENGTH = 1500;
        public static readonly int MAX_TIME_OUT = 9500;
        public static readonly int MIN_TIME_OUT = 4500;
        public static readonly int GAP_TIME_OUT = 500;
        public static readonly int DEFAULT_TIME_OUT = 7000;
        public static readonly int DELAY_MILLISECONDS = 1500;
        public static readonly double FORM_WEIGTH_PERCENTAGE = 0.1;
        public static readonly string RES_NAME = "GazeToolBar";
        public static readonly string AUTO_START_ON = "Auto Start Is On";
        public static readonly string AUTO_START_OFF = "Auto Start Is OFF";
        public static readonly string KEY_FUNCTION_UNASSIGNED_MESSAGE = "N/A";
        public static readonly int FIXED_HEIGHT = 800;
        public static readonly int FIXED_WIDTH = 600;
        public static readonly Size SCREEN_SIZE = Screen.PrimaryScreen.WorkingArea.Size;
        public static readonly Rectangle PRIMARY_SCREEN = Screen.PrimaryScreen.Bounds;
        public static readonly Color SelectedColor = Color.White;
        public static readonly Color SettingButtonColor = Color.Black;
        public static readonly Point pnlLocation = new Point(12, 84);
        public const int ONE_HUNDERED = 100;
        
        public static readonly double SCREEN_BOUNDARY_CUT_OFF_PERCENT = 10;
        public static double Y_AXIS_TOP_OF_SCREEN_CUT_OFF_BOUNDARY = PRIMARY_SCREEN.Height * (SCREEN_BOUNDARY_CUT_OFF_PERCENT / 100);



    }
}
