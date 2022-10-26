using MyInstrument.Surface;
using NAudio.CoreAudioApi.Interfaces;
using NeeqDMIs;
using NeeqDMIs.ATmega;
using NeeqDMIs.Eyetracking.Tobii;
using NeeqDMIs.Keyboard;
using NeeqDMIs.MIDI;
using NeeqDMIs.Music;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using MessageBox = System.Windows.Forms.MessageBox;

namespace MyInstrument.DMIbox
{
    public class MyInstrumentDMIBox 
    {
        private const _BreathControlModes DEFAULT_BREATHCONTROLMODE = _BreathControlModes.Dynamic;        
        private _BreathControlModes breathControlMode = DEFAULT_BREATHCONTROLMODE;
        public _BreathControlModes BreathControlMode { get => breathControlMode; set { breathControlMode = value; ResetModulationAndPressure(); } }

        private bool breathOn = false;
        private int velocity = 127;
        private int pressure = 0;
        private bool kbCtrl = false;

        //MIDI & Sensors
        public IMidiModule MidiModule { get; set; }
        public TobiiModule TobiiModule { get; set; }

        private SensorModule sensorReader;
        public SensorModule SensorReader { get => sensorReader; set => sensorReader = value; }

        // Used to memorize the old note played, it helps to manage the Slide Play mode
        private MidiNotes oldMidiNote = MidiNotes.C5;
        private MidiNotes selectedNote = MidiNotes.C5;
        public MidiNotes SelectedNote {get => selectedNote; set => selectedNote = value;}

        private MyInstrumentButtons checkedNote;
        public MyInstrumentButtons CheckedNote { get => checkedNote; set => checkedNote = value; }

        public MainWindow MyInstrumentMainWindow { get; set; }
        public KeyboardModule KeyboardModule { get; set; }

        // Graphic components
        private AutoScroller autoScroller;
        private MyInstrumentSurface myInstrumentSurface;
        public AutoScroller AutoScroller { get => autoScroller; set => autoScroller = value; }
        public MyInstrumentSurface MyInstrumentSurface { get => myInstrumentSurface; set => myInstrumentSurface = value; }

        public bool BreathOn
        {
            get { return breathOn; }
            set
            {
                if (value != breathOn)
                {
                    breathOn = value;

                    if (breathOn)
                    {
                        PlaySelectedNote();
                    }
                    else
                    {
                        StopSelectedNote();
                    }
                }              
            }
        }

        public int Pressure
        {
            get { return pressure; }
            set
            {
                if (BreathControlMode == _BreathControlModes.Dynamic)
                {
                    if (value < 30 && value > 1)
                    {
                        pressure = 0;
                    }
                    else if (value > 127)
                    {
                        pressure = 127;
                    }
                    else if (value == 0)
                    {
                        pressure = 0;
                    }
                    else
                    {
                        pressure = value;
                    }
                    Rack.UserSettings.NotePressure = pressure.ToString();
                    SetPressure();
                }
                if (BreathControlMode == _BreathControlModes.Switch)
                {
                    pressure = 127;
                    SetPressure();
                }
            }
        }     

        public int Velocity
        {
            get { return velocity; }
            set
            {
                if (value < 0)
                {
                    velocity = 0;
                }
                else if (value > 127)
                {
                    velocity = 127;
                }
                else
                {
                    velocity = value;
                }
            }
        }

        public bool KbCtrl
        {
            get { return kbCtrl; }
            set
            {
                kbCtrl = value;
                if (kbCtrl)
                {
                    PlaySelectedNote();
                }
                else
                {
                    StopSelectedNote();
                }
            }
        }

        public void ResetModulationAndPressure()
        {
            breathOn = false;
            Pressure = 127;
            Velocity = 127;
        }

        private void PlaySelectedNote()
        {
            if (Rack.DMIBox.MidiModule.IsMidiOk() && checkedNote!= null)
            {
                //MessageBox.Show(((Rack.DMIBox.MyInstrumentSurface.LastKeyboardPlayed != checkedNote.KeyboardID &&
                //    Rack.DMIBox.MyInstrumentSurface.LastKeyboardPlayed != "") ||
                //    (Rack.DMIBox.MyInstrumentSurface.LastKeyboardPlayed == "" &&
                //    checkedNote.KeyboardID != Rack.DMIBox.MyInstrumentSurface.TwoMusicKeyboards[1].Name)).ToString());
                //if ((Rack.DMIBox.MyInstrumentSurface.LastKeyboardPlayed != checkedNote.KeyboardID &&
                //    Rack.DMIBox.MyInstrumentSurface.LastKeyboardPlayed != "") ||
                //    (Rack.DMIBox.MyInstrumentSurface.LastKeyboardPlayed == "" &&
                //    checkedNote.KeyboardID != Rack.DMIBox.MyInstrumentSurface.TwoMusicKeyboards[1].Name))
                //{
                //Check for slideplay - if it is on Stop the old note to start the new one
                    if (oldMidiNote != MidiNotes.NaN && Rack.UserSettings.SlidePlayMode == _SlidePlayModes.On)
                    {
                        Rack.DMIBox.MidiModule.StopNote((int)oldMidiNote);
                    }
                   
                    MidiModule.PlayNote((int)selectedNote, velocity);

                    Rack.UserSettings.NoteName = checkedNote.Key + checkedNote.Octave;
                    Rack.UserSettings.NotePitch = ((int)selectedNote).ToString();
                    Rack.UserSettings.NoteVelocity = velocity.ToString();                   
                //}
            }
        }

        private void StopSelectedNote()
        {
            if (Rack.DMIBox.MidiModule.IsMidiOk())
            {
                if (Rack.UserSettings.SlidePlayMode != _SlidePlayModes.On)
                {
                    MidiModule.StopNote((int)selectedNote);
                    Rack.UserSettings.NoteName = "_";
                    Rack.UserSettings.NotePitch = "_";
                    Rack.UserSettings.NoteVelocity = "_";                   
                }
                oldMidiNote = selectedNote;
               
                Rack.DMIBox.MyInstrumentSurface.LastKeyboardPlayed = checkedNote.KeyboardID;            
                Rack.DMIBox.MyInstrumentSurface.MoveKeyboards(Rack.UserSettings.KeyHorizontalDistance);
            }
            checkedNote = null;
        }     

        private void SetPressure()
        {
            MidiModule.SetPressure(pressure);
        }

        //Resettting the oldNote for SlidePlay
        public void ResetSlidePlay()
        {
            oldMidiNote = MidiNotes.NaN;
        }

        public void Dispose()
        {
            try
            {
                TobiiModule.Dispose();
            }
            catch
            {
            }
            try
            {
                SensorReader.Disconnect();
            }
            catch
            {
            }
        }

    }
}
