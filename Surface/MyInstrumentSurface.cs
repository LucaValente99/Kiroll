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
        public string LastKeyboardPlayed { get; set; } = "";

        private static string lastKeyboardMoved = "";

        private List<StackPanel> twoMusicKeyboards = new List<StackPanel>();

        public List<StackPanel> TwoMusicKeyboards
        {
            get { return twoMusicKeyboards; }
            set { twoMusicKeyboards = value; }
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
            for (int i = 0; i < 2; i++)
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
            if (twoMusicKeyboards.Count != 0)
            {
                ClearSurface();
                twoMusicKeyboards = CreateMusicKeyboards();
                SetVerticalDistance(Rack.UserSettings.KeyVerticaDistance);
            }
            else
            {
                twoMusicKeyboards = CreateMusicKeyboards();
                SetVerticalDistance(Rack.UserSettings.KeyVerticaDistance);
            }

            double horizontalDistance = 0;
            for (int i = 0; i < twoMusicKeyboards.Count; i++)
            {
                canvas.Children.Add(twoMusicKeyboards[i]);
                
                Canvas.SetLeft(twoMusicKeyboards[i], (canvas.Width - twoMusicKeyboards[i].Width) / 2 + horizontalDistance);
                Canvas.SetTop(twoMusicKeyboards[i], (canvas.Height - twoMusicKeyboards[i].Height) / 2);
                horizontalDistance += Rack.UserSettings.KeyHorizontalDistance;
            }
        }

        //Cleaning the canvas
        public void ClearSurface()
        {
            foreach (StackPanel instrumentKeyboard in twoMusicKeyboards)
            {
                canvas.Children.Remove(instrumentKeyboard);
            }

            verticalDistance = Rack.UserSettings.KeyVerticaDistance;
            Rack.DMIBox.MyInstrumentMainWindow.canvasMyInstrument.Width = Rack.UserSettings.CanvasWidth;
            lastKeyboardMoved = "";
            LastKeyboardPlayed = "";

            twoMusicKeyboards.Clear();
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

            foreach (StackPanel instrumentKeyboard in twoMusicKeyboards)
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
            Canvas.SetLeft(twoMusicKeyboards[1], MyInstrumentKeyboard.GetPosition(twoMusicKeyboards[0].Name).X + distance);
            horizontalDistance = distance;        
        }

        public void MoveKeyboards(double distance)
        {
            if (LastKeyboardPlayed != "")
            {
                if (lastKeyboardMoved == "")
                {
                    if ("_" + LastKeyboardPlayed != "_" + twoMusicKeyboards[1].Name)
                    {
                        Canvas.SetLeft(MyInstrumentKeyboard.getKeyboard(LastKeyboardPlayed), MyInstrumentKeyboard.GetPosition(twoMusicKeyboards[1].Name).X + distance);
                        Rack.DMIBox.MyInstrumentMainWindow.canvasMyInstrument.Width += distance;
                        lastKeyboardMoved = MyInstrumentKeyboard.getKeyboard(LastKeyboardPlayed).Name;
                    }
                }
                else
                {
                    if ("_" + LastKeyboardPlayed != "_" + lastKeyboardMoved)
                    {
                        Canvas.SetLeft(MyInstrumentKeyboard.getKeyboard(LastKeyboardPlayed), MyInstrumentKeyboard.GetPosition(MyInstrumentKeyboard.getKeyboard(lastKeyboardMoved).Name).X + distance);
                        Rack.DMIBox.MyInstrumentMainWindow.canvasMyInstrument.Width += distance;
                        lastKeyboardMoved = MyInstrumentKeyboard.getKeyboard(LastKeyboardPlayed).Name;
                    }
                }
            }
        }
    }
}