using MyInstrument.DMIbox;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace MyInstrument.Surface
{
    public class MyInstrumentSurface
    {
        private string lastKeyboardPlayed = "";
        public string LastKeyboardPlayed { get => lastKeyboardPlayed; set => lastKeyboardPlayed = value; }

        //private string lastKeyboardSelected = "";
        //public string LastKeyboardSelected { get => lastKeyboardSelected; set => lastKeyboardSelected = value; }

        private static string lastKeyboardMoved = "";

        private List<StackPanel> fourMusicKeyboards = new List<StackPanel>();

        public List<StackPanel> FourMusicKeyboards
        {
            get { return fourMusicKeyboards; }
            set { fourMusicKeyboards = value; }
        }

        private Canvas canvas;

        private double verticalDistance = 0;
        private double horizontalDistance = 0;
        public MyInstrumentSurface(Canvas canvas)
        {
            this.canvas = canvas;
        }

        // Creating a stack panel list to show in the MyInstrument section
        private List<StackPanel> CreateMusicKeyboards()
        {
            int count = 0;
            List<StackPanel> twoMusicKeyboards = new List<StackPanel>();
            for (int i = 0; i < 16; i++)
            {
                MyInstrumentKeyboard instrumentKeyboard = new MyInstrumentKeyboard();
                twoMusicKeyboards.Add(instrumentKeyboard.MusicKeyboard);
            }

            foreach (Button btn in twoMusicKeyboards[0].Children)
            {
                count++;
            }
            return twoMusicKeyboards;

        }

        // Drawing stack panels on the screen with the scale associated;
        // Generating movement from sx to dx
        public void DrawOnCanvas()
        {
            // Each time this method is called the canvas is cleaned at first, then the new keyboards will be added
            if (fourMusicKeyboards.Count != 0)
            {
                ClearSurface();
                fourMusicKeyboards = CreateMusicKeyboards();
                SetVerticalDistance(Rack.UserSettings.KeyVerticaDistance);
            }
            else
            {
                fourMusicKeyboards = CreateMusicKeyboards();
                SetVerticalDistance(Rack.UserSettings.KeyVerticaDistance);
            }

            double horizontalDistance = 0;
            for (int i = 0; i < fourMusicKeyboards.Count; i++)
            {
                canvas.Children.Add(fourMusicKeyboards[i]);
                
                Canvas.SetLeft(fourMusicKeyboards[i], 75 + horizontalDistance); //(canvas.Width - fourMusicKeyboards[i].Width) + horizontalDistance
                Canvas.SetTop(fourMusicKeyboards[i], (canvas.Height - fourMusicKeyboards[i].Height) / 2);
                horizontalDistance += Rack.UserSettings.KeyHorizontalDistance;
            }
        }

        //Cleaning the canvas
        public void ClearSurface()
        {
            foreach (StackPanel instrumentKeyboard in fourMusicKeyboards)
            {
                canvas.Children.Remove(instrumentKeyboard);
            }

            verticalDistance = Rack.UserSettings.KeyVerticaDistance;
            Rack.DMIBox.MyInstrumentMainWindow.canvasMyInstrument.Width = Rack.UserSettings.CanvasWidth;
            lastKeyboardMoved = "";
            lastKeyboardPlayed = "";
            firstFiveTimes = true;

            // this needs to make checkPlayability works on DMIbox (keyboardID == (lastKeyboardPlayed + 1) % 16)
            MyInstrumentKeyboard.ID = 0;

            fourMusicKeyboards.Clear();
        }

        public void SetVerticalDistance(double distance)
        {
            double addVerticalDistance;
            if (Rack.UserSettings.SharpNotesMode == _SharpNotesModes.On)
            {
                addVerticalDistance = distance * 11;
            }
            else
            {
                addVerticalDistance = distance * 6;
            }

            foreach (StackPanel instrumentKeyboard in fourMusicKeyboards)
            {
                if (verticalDistance > distance)
                {
                    instrumentKeyboard.Height += addVerticalDistance - verticalDistance;
                    Rack.UserSettings.KeyboardHeight = instrumentKeyboard.Height;
                    Canvas.SetTop(instrumentKeyboard, (canvas.Height - instrumentKeyboard.Height) / 2);

                }
                else if (verticalDistance < distance)
                {
                    instrumentKeyboard.Height -= verticalDistance - addVerticalDistance;
                    Rack.UserSettings.KeyboardHeight = instrumentKeyboard.Height;
                    Canvas.SetTop(instrumentKeyboard, (canvas.Height - instrumentKeyboard.Height) / 2);
                }
                else
                {
                    instrumentKeyboard.Height = Rack.UserSettings.KeyboardHeight + addVerticalDistance;
                    Canvas.SetTop(instrumentKeyboard, (canvas.Height - instrumentKeyboard.Height) / 2);
                }

                int i = 0;
                foreach (Button key in instrumentKeyboard.Children)
                {
                    if (i != 6 && addVerticalDistance == distance * 6)
                    {
                        key.Margin = new Thickness(0, 0, 0, Rack.UserSettings.KeyVerticaDistance);
                    }
                    else if (i != 11 && addVerticalDistance == distance * 11)
                    {
                        key.Margin = new Thickness(0, 0, 0, Rack.UserSettings.KeyVerticaDistance);
                    }
                    i++;
                }
            }
            verticalDistance = addVerticalDistance;
        }

        public void SetHorizontalDistance(double distance)
        {
            DrawOnCanvas();
            Canvas.SetLeft(fourMusicKeyboards[1], MyInstrumentKeyboard.GetPosition(fourMusicKeyboards[0].Name).X + distance);
            horizontalDistance = distance;        
        }


        private bool firstFiveTimes = true;
        public void MoveKeyboards(double distance)
        {
            Rack.DMIBox.MyInstrumentMainWindow.canvasMyInstrument.Width += distance;

            if (lastKeyboardPlayed != "")
            {
                if (firstFiveTimes)
                {
                    if (lastKeyboardPlayed == "8"){
                        Canvas.SetLeft(MyInstrumentKeyboard.getKeyboard("_0"), MyInstrumentKeyboard.GetPosition(fourMusicKeyboards[15].Name).X + distance);
                        lastKeyboardMoved = "0";
                        //Rack.DMIBox.MyInstrumentMainWindow.btnInstrumentSettingLabel.Content = lastKeyboardMoved;
                        MyInstrumentKeyboard.resetColors("_0");
                        firstFiveTimes = false;
                    }                                     
                }
                else
                {
                    int lastKP = Int32.Parse(lastKeyboardPlayed) - 8;
                    if (lastKP < 0)
                    {
                        lastKP = 16 + lastKP;
                    }

                    Canvas.SetLeft(MyInstrumentKeyboard.getKeyboard("_"+lastKP.ToString()), MyInstrumentKeyboard.GetPosition("_"+lastKeyboardMoved).X + distance);
                    lastKeyboardMoved = lastKP.ToString();
                    //Rack.DMIBox.MyInstrumentMainWindow.btnInstrumentSettingLabel.Content = lastKeyboardMoved;
                    MyInstrumentKeyboard.resetColors("_" + lastKeyboardMoved);
                }               
            }
        }
    }
}