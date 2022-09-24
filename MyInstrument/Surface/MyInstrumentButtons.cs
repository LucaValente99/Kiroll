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
        private readonly string key;

        private static MidiNotes oldMidiNote = MidiNotes.NaN; // aiuta a gestire la SlidePlayMode
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
            toolKey.BorderThickness = new Thickness(3);
            toolKey.BorderBrush = Brushes.Black;
            toolKey.Content = content;

            toolKey.MouseEnter += Play;
            toolKey.MouseLeave += Stop;                  

            this.octave = octave;
            this.key = key;
        }

        private void Stop(object sender, MouseEventArgs e)
        {         
            if (Rack.UserSettings.MyInstrumentControlMode == _MyInstrumentControlModes.Keyboard)
            {
                MidiNotes md = AbsNotesMethods.ToMidiNote(AbsNotesMethods.ToAbsNote(toolKey.Name), octave);
                if (Rack.UserSettings.SlidePlayMode != _SlidePlayModes.On)
                {                                       
                    Rack.DMIBox.MidiModule.StopNote(MidiNotesMethods.ToPitchValue(md));
                    Rack.UserSettings.NoteName = "_";
                    Rack.UserSettings.NotePitch = "_";
                    Rack.UserSettings.NoteVelocity = "_";                    
                }     
                oldMidiNote = md;
            }            
        }

        private void Play(object sender, MouseEventArgs e)
        {
            if (Rack.UserSettings.MyInstrumentControlMode == _MyInstrumentControlModes.Keyboard)
            {
                MidiNotes md = AbsNotesMethods.ToMidiNote(AbsNotesMethods.ToAbsNote(toolKey.Name), octave);
                if (oldMidiNote != MidiNotes.NaN && Rack.UserSettings.SlidePlayMode == _SlidePlayModes.On)
                {
                    Rack.DMIBox.MidiModule.StopNote(MidiNotesMethods.ToPitchValue(oldMidiNote));
                }                   
                Rack.DMIBox.MidiModule.PlayNote(MidiNotesMethods.ToPitchValue(md), 127);
                Rack.UserSettings.NoteName = content.Text + octave.ToString();
                Rack.UserSettings.NotePitch = md.ToPitchValue().ToString();
                Rack.UserSettings.NoteVelocity = "127";                              
            }           
        }

        public static void resetSlidePlay()
        {
            oldMidiNote = MidiNotes.NaN;
        }
    }
}
