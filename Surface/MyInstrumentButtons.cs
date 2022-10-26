using MyInstrument.DMIbox;
using NeeqDMIs.Music;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Drawing;
using System.Windows.Documents;
using Brushes = System.Windows.Media.Brushes;
using System.Drawing.Drawing2D;



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

        private TextBlock content;
        private int octave;
        public int Octave { get { return octave; } set { octave = value; } }
        private string key;
        public string Key { get { return key; } set { key = value; } }
        private string keyboardID;
        public string KeyboardID { get { return keyboardID; } set { keyboardID = value; } }

        public MyInstrumentButtons(string key, int octave,  SolidColorBrush brush, int keyboardID) : base()
        {
            content = new TextBlock();
            content.Text = MusicConversions.ToAbsNote(key).ToStandardString();
            content.Foreground = Brushes.Black;
            content.FontSize = 30;
            content.FontWeight = FontWeights.Bold;

            toolKey = new Button();
            toolKey.Name = key;
            toolKey.Width = 170; //170
            toolKey.Height = 100; //84.2 
            toolKey.Background = brush;
            toolKey.BorderThickness = new Thickness(3);
            toolKey.BorderBrush = Brushes.Black;
            toolKey.Content = content;

            toolKey.MouseEnter += Play;
            toolKey.MouseLeave += Stop;                  

            this.octave = octave;
            this.key = key;
            this.keyboardID = "_" + keyboardID;
        }
        private void Stop(object sender, MouseEventArgs e)
        {
            Rack.DMIBox.SelectedNote = MusicConversions.ToAbsNote(key).ToMidiNote(octave);
            if (Rack.UserSettings.MyInstrumentControlMode == _MyInstrumentControlModes.Keyboard)
            {
                Rack.DMIBox.KbCtrl = false;
            }
            //else
            //{
            //    Rack.DMIBox.BreathOn = false;
            //}
        }

        private void Play(object sender, MouseEventArgs e)
        {
            Rack.DMIBox.CheckedNote = this;
            Rack.DMIBox.SelectedNote = MusicConversions.ToAbsNote(key).ToMidiNote(octave);
            if (Rack.UserSettings.MyInstrumentControlMode == _MyInstrumentControlModes.Keyboard)
            {
                Rack.DMIBox.KbCtrl = true;
            }
            //else
            //{
            //    Rack.DMIBox.BreathOn = true;
            //}

        }

    }
}
