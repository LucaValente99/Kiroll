using MyInstrument.DMIbox;
using NeeqDMIs.Music;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Brushes = System.Windows.Media.Brushes;

namespace MyInstrument.Surface
{
    public class MyInstrumentButtons : Button
    {
        private Button toolKey;
        public Button ToolKey
        {
            get
            {
                return toolKey;
            }

            set
            {
                toolKey = value;
            }
        }

        private int octave;
        public int Octave { get { return octave; } set { octave = value; } }

        private string key;
        public string Key { get { return key; } set { key = value; } }

        private string keyboardID;
        public string KeyboardID { get { return keyboardID; } set { keyboardID = value; } }
        public MyInstrumentButtons(string key, int octave,  SolidColorBrush brush, int keyboardID) : base()
        {

            // Playable key
            toolKey = new Button();
            toolKey.Name = key;
            toolKey.Width = 150; //170
            toolKey.Height = 84.2; //100
            toolKey.Background = brush;
            toolKey.BorderBrush = Brushes.Black;
            toolKey.BorderThickness = new Thickness(3);

            if (Rack.UserSettings.KeyName == _KeyName.On)
            {
                toolKey.Content = MusicConversions.ToAbsNote(key).ToStandardString();

                // Button content
                toolKey.Foreground = Brushes.Black;
                toolKey.FontSize = 30;
                toolKey.FontWeight = FontWeights.Bold;
            }
            else
            {
                toolKey.Content = ".";

                // Button content
                toolKey.Foreground = Brushes.Black;
                toolKey.VerticalContentAlignment = VerticalAlignment.Top;
                toolKey.FontSize = 40;
                toolKey.FontWeight = FontWeights.Bold;
            }          

            if (key[0] == 's')
            {
                toolKey.Opacity = 0.7;
            }


            toolKey.MouseEnter += SelectNote;                

            this.octave = octave;
            this.key = key;
            this.keyboardID = keyboardID.ToString();
        }
        private void SelectNote(object sender, MouseEventArgs e)
        {
            //Rack.DMIBox.OldCheckedNote = Rack.DMIBox.CheckedNote;
            Rack.DMIBox.CheckedNote = this;

            //if (Rack.DMIBox.OldCheckedNote != null)
            //{
            //    Point btn1Point = Rack.DMIBox.OldCheckedNote.ToolKey.TransformToAncestor(Rack.DMIBox.MyInstrumentMainWindow).Transform(new Point(0, 0));
            //    Point btn2Point = toolKey.TransformToAncestor(Rack.DMIBox.MyInstrumentMainWindow).Transform(new Point(0, 0));
            //    Line l = new Line();
            //    l.Stroke = new SolidColorBrush(Colors.Black);
            //    l.StrokeThickness = 2.0;
            //    l.X1 = btn1Point.X + 65;
            //    l.X2 = btn2Point.X + 65;
            //    l.Y1 = btn1Point.Y - 30;
            //    l.Y2 = btn2Point.Y - 30;
            //    Rack.DMIBox.MyInstrumentMainWindow.canvasMyInstrument.Children.Add(l);
            //}

            Rack.DMIBox.SelectedNote = MusicConversions.ToAbsNote(key).ToMidiNote(octave);

            // This is used to avoid 'play' and 'stop' behaviors of notes to happen at wrong moments
            Rack.DMIBox.IsPlaying = false;

            // If blow is used to click buttons, it should not work when user is playing keys
            Rack.DMIBox.LastGazedButton.Background = Rack.DMIBox.MyInstrumentMainWindow.OldBackGround;
            Rack.DMIBox.LastGazedButton = new Button();

            // If the keyboard that contains the note is valid, colors will be update and the movement will be started.
            if (Rack.DMIBox.CheckPlayability())
            {
                //Note selection behaviors 
                MyInstrumentKeyboard.ResetColors("_" + keyboardID);
                MyInstrumentKeyboard.UpdateColors("_" + keyboardID, toolKey);             

                //Movement of keyboards
                if (Rack.DMIBox.MyInstrumentSurface.LastKeyboardSelected != keyboardID)
                {
                    Rack.DMIBox.MyInstrumentSurface.LastKeyboardSelected = keyboardID;
                    Rack.DMIBox.MyInstrumentSurface.MoveKeyboards(Rack.UserSettings.KeyHorizontalDistance);
                }
            }

            if (Rack.UserSettings.SlidePlayMode == _SlidePlayModes.On && Rack.DMIBox.BreathOn == true)
            {
                Rack.DMIBox.PlaySelectedNote();
            }            
        }
    }
}
