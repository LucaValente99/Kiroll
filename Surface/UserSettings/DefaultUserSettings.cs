using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyInstrument.DMIbox;

namespace MyInstrument.Surface.UserSettings
{
    class DefaultUserSettings : IUserSettings
    {       
        // Combobox default settings
        public string ScaleName { get; set; } = "C";
        public string ScaleCode { get; set; } = "maj";
        public string Octave { get; set; } = "4";

        // Note visualizer default settings
        public string NoteName { get; set; } = "_";
        public string NotePitch { get; set; } = "_";
        public string NoteVelocity { get; set; } = "_";
        public string NotePressure { get; set; } = "_";

        // Keybard-Canvas distances and sizes default settings
        public double KeyVerticaDistance { get; set; } = 14;
        public double KeyHorizontalDistance { get; set; } = 400;
        public double KeyboardHeight { get; set; } = 590;
        public int CanvasWidth { get; set; } = 1518;

        // MIDI and Sensor default settings
        public int SensorPort { get; set; } = 4;
        public int MIDIPort { get; set; } = 1;

        // Metronome default settings
        public double BPMmetronome { get; set; } = 90.0;

        // SwitchableSelectors default settings
        public _MyInstrumentControlModes MyInstrumentControlMode { get; set; } = _MyInstrumentControlModes.NaN;
        public _SlidePlayModes SlidePlayMode { get; set; } = _SlidePlayModes.Off;
        public _SharpNotesModes SharpNotesMode { get; set; } = _SharpNotesModes.Off;
            
    }
}
