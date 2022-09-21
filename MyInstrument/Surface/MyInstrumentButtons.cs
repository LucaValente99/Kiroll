using MyInstrument.DMIbox;
using NeeqDMIs.Music;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

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

        public MyInstrumentButtons( string key, int octave,  SolidColorBrush brush) : base()
        {
            content = new TextBlock();
            content.Text = AbsNotesMethods.ToStandardString(AbsNotesMethods.ToAbsNote(key));
            content.Foreground = Brushes.Black;
            content.FontSize = 30;
            content.FontWeight = FontWeights.Bold;

            toolKey = new Button();
            toolKey.Name = key;
            toolKey.Width = 150;
            toolKey.Height = 84.2;
            toolKey.Background = brush;
            toolKey.BorderThickness = new System.Windows.Thickness(3);
            toolKey.BorderBrush = Brushes.Black;
            toolKey.Content = content;

            toolKey.MouseEnter += Play;
            toolKey.MouseLeave += Stop;

            this.octave = octave;
        }

        private void Stop(object sender, MouseEventArgs e)
        {
            if (Rack.UserSettings.MyInstrumentControlMode == _MyInstrumentControlModes.Keyboard)
            {
                MidiNotes md = AbsNotesMethods.ToMidiNote(AbsNotesMethods.ToAbsNote(toolKey.Name), octave);
                Rack.DMIBox.MidiModule.StopNote(MidiNotesMethods.ToPitchValue(md));
            }            
        }

        private void Play(object sender, MouseEventArgs e)
        {
            if (Rack.UserSettings.MyInstrumentControlMode == _MyInstrumentControlModes.Keyboard)
            {
                MidiNotes md = AbsNotesMethods.ToMidiNote(AbsNotesMethods.ToAbsNote(toolKey.Name), octave);
                Rack.DMIBox.MidiModule.PlayNote(MidiNotesMethods.ToPitchValue(md), 127);
            }           
        }
    }
}
