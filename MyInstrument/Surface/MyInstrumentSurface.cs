using MyInstrument.DMIbox;
using NeeqDMIs.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MyInstrument.Surface
{
    public class MyInstrumentSurface
    {
        public string LastKeyboardPlayed { get; set; } = "";

        private static string lastKeyboardMoved = "";

        private DispatcherTimer alertTimer = new DispatcherTimer(DispatcherPriority.Render);

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
            alertTimer.Interval = TimeSpan.FromMilliseconds(70);
            alertTimer.Tick += blinkScale;
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
            alertTimer.Stop();

            if (twoMusicKeyboards.Count != 0)
            {
                ClearSurface();
                twoMusicKeyboards = CreateMusicKeyboards();
                SetVerticalDistance(Rack.UserSettings.keyVerticaDistance);
            }
            else
            {
                twoMusicKeyboards = CreateMusicKeyboards();
                SetVerticalDistance(Rack.UserSettings.keyVerticaDistance);
            }

            double horizontalDistance = 0;
            for (int i = 0; i < twoMusicKeyboards.Count; i++)
            {
                canvas.Children.Add(twoMusicKeyboards[i]);
                Canvas.SetLeft(twoMusicKeyboards[i], (canvas.Width - twoMusicKeyboards[i].Width) / 2 + horizontalDistance);
                Canvas.SetTop(twoMusicKeyboards[i], (canvas.Height - twoMusicKeyboards[i].Height) / 2);
                horizontalDistance += Rack.UserSettings.keyHorizontalDistance;
            }
        }

        //Cleaning the canvas
        public void ClearSurface()
        {
            foreach (StackPanel instrumentKeyboard in twoMusicKeyboards)
            {
                canvas.Children.Remove(instrumentKeyboard);
            }

            verticalDistance = Rack.UserSettings.keyVerticaDistance;
            Rack.DMIBox.MyInstrumentMainWindow.canvasMyInstrument.Width = Rack.UserSettings.canvasWidth;
            lastKeyboardMoved = "";
            LastKeyboardPlayed = "";

            twoMusicKeyboards.Clear();
        }

        public void SetVerticalDistance(double distance)
        {
            double addVerticalDistance;
            if (Rack.UserSettings.SharpNotesMode == _SharpNotesModes.On)
            {
                addVerticalDistance = distance*11;
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
                    Rack.UserSettings.keyboardHeight = instrumentKeyboard.Height;
                    Canvas.SetTop(instrumentKeyboard, (canvas.Height - instrumentKeyboard.Height) / 2);

                }
                else if (verticalDistance < distance)
                {
                    instrumentKeyboard.Height -= verticalDistance - addVerticalDistance;
                    Rack.UserSettings.keyboardHeight = instrumentKeyboard.Height;
                    Canvas.SetTop(instrumentKeyboard, (canvas.Height - instrumentKeyboard.Height) / 2);
                }
                else
                {
                    instrumentKeyboard.Height = Rack.UserSettings.keyboardHeight + addVerticalDistance;
                    Canvas.SetTop(instrumentKeyboard, (canvas.Height - instrumentKeyboard.Height) / 2);
                }

                int i = 0;
                foreach (Button key in instrumentKeyboard.Children)
                {
                    if (i != 6 && addVerticalDistance == distance*6)
                    {
                        key.Margin = new Thickness(0, 0, 0, Rack.UserSettings.keyVerticaDistance);
                    }
                    else if (i != 11 && addVerticalDistance == distance * 11)
                    {
                        key.Margin = new Thickness(0, 0, 0, Rack.UserSettings.keyVerticaDistance);
                    }
                    i++;
                }                      
            }
            verticalDistance = addVerticalDistance;
        }

        public void SetHorizontalDistance(double distance)
        {
            if (LastKeyboardPlayed == "")
            {
                Canvas.SetLeft(twoMusicKeyboards[1], MyInstrumentKeyboard.GetPosition(twoMusicKeyboards[0].Name).X + distance);
            }
            else
            {
                if (horizontalDistance > distance)
                {
                    Canvas.SetLeft(MyInstrumentKeyboard.getKeyboard(lastKeyboardMoved), MyInstrumentKeyboard.GetPosition(MyInstrumentKeyboard.getKeyboard(lastKeyboardMoved).Name).X + distance - horizontalDistance);

                }
                else if (horizontalDistance < distance)
                {
                    Canvas.SetLeft(MyInstrumentKeyboard.getKeyboard(lastKeyboardMoved), MyInstrumentKeyboard.GetPosition(MyInstrumentKeyboard.getKeyboard(lastKeyboardMoved).Name).X + distance - horizontalDistance);
                }
            }

            horizontalDistance = distance;
        }

            public async void MoveKeyboards(double distance)
        {
            if (LastKeyboardPlayed != "")
            {
                if (lastKeyboardMoved == "")
                {
                    if ("_" + LastKeyboardPlayed != "_" + twoMusicKeyboards[1].Name)
                    {
                        alertTimer.Stop();
                        MyInstrumentKeyboard.resetColors(LastKeyboardPlayed);
                        Canvas.SetLeft(MyInstrumentKeyboard.getKeyboard(LastKeyboardPlayed), MyInstrumentKeyboard.GetPosition(twoMusicKeyboards[1].Name).X + distance);
                        Rack.DMIBox.MyInstrumentMainWindow.canvasMyInstrument.Width += distance;
                        lastKeyboardMoved = MyInstrumentKeyboard.getKeyboard(LastKeyboardPlayed).Name;
                    }
                    else
                    {
                        alertTimer.Start();
                    }
                }
                else
                {
                    if ("_" + LastKeyboardPlayed != "_" + lastKeyboardMoved)
                    {
                        alertTimer.Stop();
                        MyInstrumentKeyboard.resetColors(LastKeyboardPlayed);
                        Canvas.SetLeft(MyInstrumentKeyboard.getKeyboard(LastKeyboardPlayed), MyInstrumentKeyboard.GetPosition(MyInstrumentKeyboard.getKeyboard(lastKeyboardMoved).Name).X + distance);
                        Rack.DMIBox.MyInstrumentMainWindow.canvasMyInstrument.Width += distance;
                        lastKeyboardMoved = MyInstrumentKeyboard.getKeyboard(LastKeyboardPlayed).Name;
                    }
                    else
                    {
                        alertTimer.Start();
                    }
                }             
            }
        }

        bool blink_on = false;
        // If the user play the wrong scale, the correct one to play will blink
        private void blinkScale(object? sender, EventArgs e)
        {         
            if (blink_on)
            {
                foreach (StackPanel keyboard in twoMusicKeyboards)
                {
                    if (keyboard.Name != LastKeyboardPlayed)
                    {
                        int i = 0;
                        foreach (Button key in keyboard.Children)
                        {
                            key.Background = new SolidColorBrush(Colors.DarkRed);
                            key.Background = new SolidColorBrush(MyInstrumentKeyboard.KeysColorCode[i]);
                            i++;
                        }
                    }
                }

            }
            else
            {
                foreach (StackPanel keyboard in twoMusicKeyboards)
                {
                    if (keyboard.Name != LastKeyboardPlayed)
                    {
                        int i = 0;
                        foreach (Button key in keyboard.Children)
                        {
                            key.Background = new SolidColorBrush(MyInstrumentKeyboard.KeysColorCode[i]);
                            key.Background = new SolidColorBrush(Colors.DarkRed);
                            i++;
                        }
                    }
                }
            }
            blink_on = !blink_on;
            }
        }           
}