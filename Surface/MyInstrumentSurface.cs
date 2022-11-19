using MyInstrument.DMIbox;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MyInstrument.Surface
{
    public class MyInstrumentSurface
    {
        // Used to keep track of lastKeyboardPlayed. It is updated when keyboard is played.
        private string lastKeyboardPlayed = "";
        public string LastKeyboardPlayed { get => lastKeyboardPlayed; set => lastKeyboardPlayed = value; }

        private int lastKP = 0; // Integer version of last lastKeyboardPlayed

        // Used to keep track of lastKeyboardSelected. It is updated for every note selection (SelectNote on MyInstrumentButton).
        private string lastKeyboardSelected = "";
        public string LastKeyboardSelected { get => lastKeyboardSelected; set => lastKeyboardSelected = value; }

        // Used to keep track of lastKeyboardMoved.
        private static string lastKeyboardMoved = "";

        private List<StackPanel> musicKeyboards = new List<StackPanel>();

        public List<StackPanel> MusicKeyboards
        {
            get { return musicKeyboards; }
            set { musicKeyboards = value; }
        }

        private Canvas canvas;

        private double verticalDistance = 0;
        private double horizontalDistance;
        public MyInstrumentSurface(Canvas canvas)
        {
            this.canvas = canvas;
        }

        // Creating a keyboard list
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

        // Drawing keyboards on the screen with the scale associated
        public void DrawOnCanvas()
        {
            // Each time this method is called the canvas is cleaned at first, then the new keyboards will be added
            if (musicKeyboards.Count != 0)
            {
                ClearSurface();
                musicKeyboards = CreateMusicKeyboards();
                SetVerticalDistance(Rack.UserSettings.KeyVerticaDistance);
            }
            else
            {
                musicKeyboards = CreateMusicKeyboards();
                SetVerticalDistance(Rack.UserSettings.KeyVerticaDistance);
            }

            horizontalDistance = 0;
            for (int i = 0; i < musicKeyboards.Count; i++)
            {
                canvas.Children.Add(musicKeyboards[i]);
                
                Canvas.SetLeft(musicKeyboards[i], 75 + horizontalDistance); 
                Canvas.SetTop(musicKeyboards[i], (canvas.Height - musicKeyboards[i].Height) / 2);
                horizontalDistance += Rack.UserSettings.KeyHorizontalDistance;
            }

            MyInstrumentKeyboard.UpdateOpacity();
        }

        // Cleaning the canvas and resetting variables
        public void ClearSurface()
        {
            foreach (StackPanel instrumentKeyboard in musicKeyboards)
            {
                canvas.Children.Remove(instrumentKeyboard);
            }

            verticalDistance = Rack.UserSettings.KeyVerticaDistance;
            Rack.DMIBox.MyInstrumentMainWindow.canvasMyInstrument.Width = Rack.UserSettings.CanvasWidth;
            lastKeyboardMoved = "";
            lastKeyboardPlayed = "";
            lastKeyboardSelected = "";
            afterEighthKeyboard = false;
            firstTime = false;
            Rack.DMIBox.CheckedNote = null;

            // This needs to make checkPlayability works on DMIbox (keyboardID == (lastKeyboardPlayed + 1) % 16)
            MyInstrumentKeyboard.ID = 0;

            musicKeyboards.Clear();
        }
        
        // Setting vertical distance between keys
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

            foreach (StackPanel instrumentKeyboard in musicKeyboards)
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

        // These two variables helps to control which keyboard needs to move and when (afterEighthKeyboard), then where it needs to stop before moving (firstTime).

        // Start moving keyboards after the selection of the eighth one
        private bool afterEighthKeyboard = false;
        // Start the movement of scrollbar the second time the MoveKeybaords method is called
        private bool firstTime = false;
        public void MoveKeyboards(double distance)
        {
            if (!firstTime)
            {
                firstTime = true;
            }
            else {
                Rack.DMIBox.MyInstrumentMainWindow.canvasMyInstrument.Width += Rack.UserSettings.KeyHorizontalDistance;
            }

            if (lastKeyboardSelected != "")
            {
                if (!afterEighthKeyboard)
                {
                    if (lastKeyboardSelected == "8"){
                        Canvas.SetLeft(MyInstrumentKeyboard.GetKeyboard("_0"), MyInstrumentKeyboard.GetPosition(musicKeyboards[15].Name).X + distance);
                        lastKeyboardMoved = "0";
                        MyInstrumentKeyboard.ResetColors("_0");
                        afterEighthKeyboard = true;
                    }                                     
                }
                else
                {
                    lastKP = Int32.Parse(lastKeyboardSelected) - 8;
                    if (lastKP < 0)
                    {
                        lastKP = 16 + lastKP; // Es: if keyboard n° 5 is played -> 5 - 8 = -3, 16 - 3 = 13 is the keyboard to move.
                    }

                    Canvas.SetLeft(MyInstrumentKeyboard.GetKeyboard("_" + lastKP.ToString()), MyInstrumentKeyboard.GetPosition("_" + lastKeyboardMoved).X + distance);
                    lastKeyboardMoved = lastKP.ToString();
                    MyInstrumentKeyboard.ResetColors("_" + lastKeyboardMoved);
                }               
            }
        }
    }
}