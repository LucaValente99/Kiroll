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
        public List<Color> KeysColorCode = new List<Color>()
        {
            Colors.Red,
            Colors.Yellow,
            Colors.Blue,
            Colors.Orange,
            Colors.Purple,
            Colors.Green,
            Colors.Coral
        };

        private List<StackPanel> threeMusicKeyboards = new List<StackPanel>();

        private Canvas canvas;

        private double distance = Rack.UserSettings.keyDistance;
        public MyInstrumentSurface(Canvas canvas)
        {
            this.canvas = canvas;
        }

        //creo una lista di 3 stack panel da mostrare nella sezione MyInstrument (canvas)
        public List<StackPanel> CreateMusicKeyboards()
        {
            List<StackPanel> threeMusicKeyboards = new List<StackPanel>();

            for (int i = 0; i < 3; i++)
            {
                MyInstrumentKeyboard instrumentKeyboard = new MyInstrumentKeyboard();
                threeMusicKeyboards.Add(instrumentKeyboard.MusicKeyboard);
            }

            return threeMusicKeyboards;

        }

        // disegno a schermo gli stack panel con associata la relativa scala e genero il movimento da sx verso dx
        public void DrawOnCanvas()
        {

            if (threeMusicKeyboards.Count != 0)
            {
                ClearSurface();
                threeMusicKeyboards = CreateMusicKeyboards();
                SetDistance(Rack.UserSettings.keyDistance);
            }
            else
            {
                threeMusicKeyboards = CreateMusicKeyboards();
                SetDistance(Rack.UserSettings.keyDistance);
            }
            int distance = 0;

            for (int i = 0; i < threeMusicKeyboards.Count; i++)
            {
                canvas.Children.Add(threeMusicKeyboards[i]);
                Canvas.SetLeft(threeMusicKeyboards[i], (canvas.Width - threeMusicKeyboards[i].Width) / 3 + distance);
                Canvas.SetTop(threeMusicKeyboards[i], (canvas.Height - threeMusicKeyboards[i].Height - 10) / 2);
                distance += 400;
            }
        }

        //pulisco il canvas
        public void ClearSurface()
        {
            foreach (StackPanel instrumentKeyboard in threeMusicKeyboards)
            {
                canvas.Children.Remove(instrumentKeyboard);
            }

            this.distance = Rack.UserSettings.keyDistance;

            threeMusicKeyboards.Clear();
        }

        public void SetDistance(double distance)
        {
            foreach (StackPanel instrumentKeyboard in threeMusicKeyboards)
            {
                if (this.distance > distance)
                {
                    instrumentKeyboard.Height += (distance * 7) - this.distance;
                    Rack.UserSettings.keyboardHeight = instrumentKeyboard.Height;

                }
                else if (this.distance < distance)
                {
                    instrumentKeyboard.Height -= this.distance - (distance * 7);
                    Rack.UserSettings.keyboardHeight = instrumentKeyboard.Height;
                }
                else
                {
                    instrumentKeyboard.Height = Rack.UserSettings.keyboardHeight;
                }

                foreach (Button key in instrumentKeyboard.Children)
                {
                    key.Margin = new Thickness(0, 0, 0, Rack.UserSettings.keyDistance);
                }
            }

            canvas.Height = Rack.UserSettings.keyboardHeight + 22;
            this.distance = distance * 7;
        }

    }
}