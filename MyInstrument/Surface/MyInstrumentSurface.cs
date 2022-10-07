using MyInstrument.DMIbox;
using NeeqDMIs.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public string KeyboardIsPlaying { get; set; }

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
        public MyInstrumentSurface(Canvas canvas)
        {
            this.canvas = canvas;
            alertTimer.Interval = TimeSpan.FromMilliseconds(70);
            alertTimer.Tick += blinkScale;
        }

        //creo una lista di 3 stack panel da mostrare nella sezione MyInstrument (canvas)
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

        // disegno a schermo gli stack panel con associata la relativa scala e genero il movimento da sx verso dx
        public void DrawOnCanvas()
        {
            alertTimer.Stop();

            if (twoMusicKeyboards.Count != 0)
            {
                ClearSurface();
                twoMusicKeyboards = CreateMusicKeyboards();
                SetDistance(Rack.UserSettings.keyDistance);
            }
            else
            {
                twoMusicKeyboards = CreateMusicKeyboards();
                SetDistance(Rack.UserSettings.keyDistance);
            }

            int horizontalDistance = 0;
            for (int i = 0; i < twoMusicKeyboards.Count; i++)
            {
                canvas.Children.Add(twoMusicKeyboards[i]);
                Canvas.SetLeft(twoMusicKeyboards[i], (canvas.Width - twoMusicKeyboards[i].Width) / 2 + horizontalDistance);
                Canvas.SetTop(twoMusicKeyboards[i], (canvas.Height - twoMusicKeyboards[i].Height) / 2);
                horizontalDistance += 600;
            }
        }

        //pulisco il canvas
        public void ClearSurface()
        {
            foreach (StackPanel instrumentKeyboard in twoMusicKeyboards)
            {
                canvas.Children.Remove(instrumentKeyboard);
            }

            verticalDistance = Rack.UserSettings.keyDistance;
            Rack.DMIBox.MyInstrumentMainWindow.canvasMyInstrument.Width = Rack.UserSettings.canvasWidth;
            lastKeyboardMoved = "";
            LastKeyboardPlayed = "";

            twoMusicKeyboards.Clear();
        }

        public void SetDistance(double distance)
        {
            double addDistance;
            if (Rack.UserSettings.SharpNotesMode == _SharpNotesModes.On)
            {
                addDistance = distance*11;
            }
            else
            {
                addDistance = distance * 6;
            }
            
            foreach (StackPanel instrumentKeyboard in twoMusicKeyboards)
            {
                if (verticalDistance > distance)
                {
                    instrumentKeyboard.Height += addDistance - verticalDistance;
                    Rack.UserSettings.keyboardHeight = instrumentKeyboard.Height;
                    Canvas.SetTop(instrumentKeyboard, (canvas.Height - instrumentKeyboard.Height) / 2);

                }
                else if (verticalDistance < distance)
                {
                    instrumentKeyboard.Height -= verticalDistance - addDistance;
                    Rack.UserSettings.keyboardHeight = instrumentKeyboard.Height;
                    Canvas.SetTop(instrumentKeyboard, (canvas.Height - instrumentKeyboard.Height) / 2);
                }
                else
                {
                    instrumentKeyboard.Height = Rack.UserSettings.keyboardHeight + addDistance;
                    Canvas.SetTop(instrumentKeyboard, (canvas.Height - instrumentKeyboard.Height) / 2);
                }

                int i = 0;
                foreach (Button key in instrumentKeyboard.Children)
                {
                    if (i != 6 && addDistance == distance*6)
                    {
                        key.Margin = new Thickness(0, 0, 0, Rack.UserSettings.keyDistance);
                    }
                    else if (i != 11 && addDistance == distance * 11)
                    {
                        key.Margin = new Thickness(0, 0, 0, Rack.UserSettings.keyDistance);
                    }
                    i++;
                }                      
            }
            verticalDistance = addDistance;
        }

        public void MoveKeyboard()
        {
            if (LastKeyboardPlayed != "")
            {
                if (lastKeyboardMoved == "")
                {
                    if ("_" + LastKeyboardPlayed != "_" + Rack.DMIBox.MyInstrumentSurface.TwoMusicKeyboards[1].Name)
                    {
                        alertTimer.Stop();
                        MyInstrumentKeyboard.resetColors(LastKeyboardPlayed);
                        Canvas.SetLeft(MyInstrumentKeyboard.getKeyboard(LastKeyboardPlayed), MyInstrumentKeyboard.GetPosition(Rack.DMIBox.MyInstrumentSurface.TwoMusicKeyboards[1].Name).X + 600);
                        Rack.DMIBox.MyInstrumentMainWindow.canvasMyInstrument.Width += 600;
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
                        Canvas.SetLeft(MyInstrumentKeyboard.getKeyboard(LastKeyboardPlayed), MyInstrumentKeyboard.GetPosition(MyInstrumentKeyboard.getKeyboard(lastKeyboardMoved).Name).X + 600);
                        Rack.DMIBox.MyInstrumentMainWindow.canvasMyInstrument.Width += 600;
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