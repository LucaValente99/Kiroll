using MyInstrument.Surface;
using NeeqDMIs.Eyetracking.Tobii;
using NeeqDMIs.Keyboard;
using NeeqDMIs.MIDI;
using NeeqDMIs.Music;
using NeeqDMIs.NithSensors;
using System;
using System.Windows.Controls;

namespace MyInstrument.DMIbox
{
    public class MyInstrumentDMIBox 
    {
        private bool breathOn = false;
        private int velocity = 127;
        private int pressure = 0;

        // Used to track when a note and relative keyboard is playing
        private bool isPlaying = false;
        public bool IsPlaying { get => isPlaying; set => isPlaying = value; }

        // Used to click buttons using eye tracker
        private Button lastGazedButton = new Button();
        public Button LastGazedButton { get => lastGazedButton; set => lastGazedButton = value; }

        // MIDI & Sensors
        public IMidiModule MidiModule { get; set; }
        public TobiiModule TobiiModule { get; set; }

        private NithModule sensorReader;
        public NithModule SensorReader { get => sensorReader; set => sensorReader = value; }

        // Used to memorize the old note played, it helps to manage the Slide Play mode
        private MidiNotes oldMidiNote = MidiNotes.NaN;
        private MidiNotes selectedNote = MidiNotes.NaN;
        public MidiNotes SelectedNote {get => selectedNote; set => selectedNote = value;}
        public MidiNotes OldMidiNote { get => oldMidiNote; set => oldMidiNote = value; }

        // Used to update note visualizer when a note is played
        private MyInstrumentButtons checkedNote = null;
        public MyInstrumentButtons CheckedNote { get => checkedNote; set => checkedNote = value; }

        // Main classes instantiated when the application starts, into MyInstrumentSetup class
        public MainWindow MyInstrumentMainWindow { get; set; }
        public KeyboardModule KeyboardModule { get; set; }    
        public AutoScroller AutoScroller { get; set; }

        private MyInstrumentSurface myInstrumentSurface;
        public MyInstrumentSurface MyInstrumentSurface { get => myInstrumentSurface; set => myInstrumentSurface = value; }

        // Playing a note when user blows
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

        // Setting pressure when user blows
        public int Pressure
        {
            get { return pressure; }
            set
            {
                if (Rack.UserSettings.BreathControlModes == _BreathControlModes.Dynamic)
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
                if (Rack.UserSettings.BreathControlModes == _BreathControlModes.Static)
                {
                    pressure = 127;
                    Rack.UserSettings.NotePressure = pressure.ToString();
                    SetPressure();
                }
            }
        }     

        // Setting velocity (it will be 127 by default)
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

        // Called evrey time the control mode changes (keyboard - breath)
        public void ResetModulationAndPressure()
        {
            breathOn = false;
            pressure = 127;
            velocity = 127;
        }

        // Playing the selected note
        public void PlaySelectedNote()
        {
            if (MidiModule.IsMidiOk() && checkedNote!= null && 
                Rack.UserSettings.MyInstrumentControlMode != _MyInstrumentControlModes.NaN &&
                !MyInstrumentMainWindow.MyInstrumentSettingsOpened)
            {
                if (CheckPlayability())
                {
                    // Check for slideplay - if it is on Stop the old note to start the new one
                    if (oldMidiNote != MidiNotes.NaN && Rack.UserSettings.SlidePlayMode == _SlidePlayModes.On)
                    {
                        MidiModule.StopNote((int)oldMidiNote);
                    }
                   
                    MidiModule.PlayNote((int)selectedNote, velocity);
                    isPlaying = true;

                    oldMidiNote = selectedNote;

                    myInstrumentSurface.LastKeyboardPlayed = checkedNote.KeyboardID;

                    if (checkedNote.Key.Contains("s"))
                    {
                        Rack.UserSettings.NoteName = checkedNote.Key[1] + "#" + checkedNote.Octave;
                    }
                    else
                    {
                        Rack.UserSettings.NoteName = checkedNote.Key + checkedNote.Octave;
                    }
                    
                    Rack.UserSettings.NotePitch = ((int)selectedNote).ToString();
                    Rack.UserSettings.NoteVelocity = velocity.ToString();
                }
            }
        }

        // Stopping the last played note 
        public void StopSelectedNote()
        {
            if (MidiModule.IsMidiOk() && isPlaying == true &&
                Rack.UserSettings.MyInstrumentControlMode != _MyInstrumentControlModes.NaN &&
                !MyInstrumentMainWindow.MyInstrumentSettingsOpened)
            {                
                MidiModule.StopNote((int)selectedNote);
                isPlaying = false;                                                                                         
                checkedNote = null;
            }
            else
            {
                MidiModule.StopNote((int)oldMidiNote);
            }

            Rack.UserSettings.NoteName = "_";
            Rack.UserSettings.NotePitch = "_";
            Rack.UserSettings.NoteVelocity = "_";
        }

        // This method helps to understand if the note selected, the one that will be played, is valid or not.
        // The logic is easy: when the application starts just the keyboard with id == "_0", so the first keboard on screen, could be played.
        // After the first case only the keyboard with id greater than the previous one by one can be played.
        public bool CheckPlayability()
        {
            bool startingCase = myInstrumentSurface.LastKeyboardPlayed == "";           

            if (startingCase)
            {
                bool firstNoteToPlay = "_" + checkedNote.KeyboardID == "_0";
                return firstNoteToPlay;
            }

            int lastKeyboardPlayed = Convert.ToInt32(myInstrumentSurface.LastKeyboardPlayed);
            int keyboardID = Convert.ToInt32(checkedNote.KeyboardID);

            return keyboardID == (lastKeyboardPlayed + 1) % 16;
        }

        private void SetPressure()
        {
            MidiModule.SetPressure(pressure);
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
