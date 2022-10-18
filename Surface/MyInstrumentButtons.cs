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
        private string key;
        private int keyboardID;

        private static MidiNotes oldMidiNote = MidiNotes.NaN; // Used to memorize the old note played, it helps to manage the Slide Play mode
        public MyInstrumentButtons(string key, int octave,  SolidColorBrush brush, int keyboardID) : base()
        {
            content = new TextBlock();
            content.Text = MusicConversions.ToAbsNote(key).ToStandardString();
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
            this.keyboardID = keyboardID;
        }
        private void Stop(object sender, MouseEventArgs e)
        {         
            if (Rack.UserSettings.MyInstrumentControlMode == _MyInstrumentControlModes.Keyboard && Rack.DMIBox.MidiModule.IsMidiOk())
            {
                MidiNotes md = MusicConversions.ToAbsNote(key).ToMidiNote(octave);
                if (Rack.UserSettings.SlidePlayMode != _SlidePlayModes.On)
                {                                       
                    Rack.DMIBox.MidiModule.StopNote(MidiNotesMethods.ToPitchValue(md));
                    Rack.UserSettings.NoteName = "_";
                    Rack.UserSettings.NotePitch = "_";
                    Rack.UserSettings.NoteVelocity = "_";                    
                }     
                oldMidiNote = md;

                Rack.DMIBox.MyInstrumentSurface.LastKeyboardPlayed = "_" + keyboardID;
                Rack.DMIBox.MyInstrumentSurface.MoveKeyboards(Rack.UserSettings.KeyHorizontalDistance);
            }            
        }

        private void Play(object sender, MouseEventArgs e)
        {
            if (Rack.UserSettings.MyInstrumentControlMode == _MyInstrumentControlModes.Keyboard && Rack.DMIBox.MidiModule.IsMidiOk())
            {
                if ((Rack.DMIBox.MyInstrumentSurface.LastKeyboardPlayed != "_" + keyboardID.ToString() &&
                    Rack.DMIBox.MyInstrumentSurface.LastKeyboardPlayed != "") || 
                    (Rack.DMIBox.MyInstrumentSurface.LastKeyboardPlayed == "" &&
                    "_" + keyboardID.ToString() != Rack.DMIBox.MyInstrumentSurface.TwoMusicKeyboards[1].Name))
                {
                    MidiNotes md = MusicConversions.ToAbsNote(key).ToMidiNote(octave);

                    //Check for slideplay - if it is on Stop the old note to start the new one
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
        }

        //Resettting the oldNote for SlidePlay
        public static void ResetSlidePlay()
        {
            oldMidiNote = MidiNotes.NaN;
        }
    }
}
