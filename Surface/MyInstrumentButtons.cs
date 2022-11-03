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
            toolKey.BorderThickness = new Thickness(3);
            toolKey.BorderBrush = Brushes.Black;
            toolKey.Content = MusicConversions.ToAbsNote(key).ToStandardString();

            // Button content
            toolKey.Foreground = Brushes.Black;
            toolKey.FontSize = 30;
            toolKey.FontWeight = FontWeights.Bold;

            toolKey.MouseEnter += SelectNote;                

            this.octave = octave;
            this.key = key;
            this.keyboardID = keyboardID.ToString();
        }

        private void SelectNote(object sender, MouseEventArgs e)
        {
            Rack.DMIBox.CheckedNote = this;
            Rack.DMIBox.SelectedNote = MusicConversions.ToAbsNote(key).ToMidiNote(octave);
            Rack.DMIBox.IsPlaying = false;

            // If the keyboard that contains the note is valid, colors will be update and the movement will be started.
            if (Rack.DMIBox.CheckPlayability())
            {
                MyInstrumentKeyboard.ResetColors("_" + keyboardID);
                MyInstrumentKeyboard.UpdateColors("_" + keyboardID, toolKey);             

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
