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
        public string ScaleName { get; set; } = "C";
        public string ScaleCode { get; set; } = "maj";
        public string Octave { get; set; } = "4";
        public string NoteName { get; set; } = "_";
        public string NotePitch { get; set; } = "_";
        public string NoteVelocity { get; set; } = "_";
        public double KeyVerticaDistance { get; set; } = 14;
        public double KeyHorizontalDistance { get; set; } = 600;
        public double KeyboardHeight { get; set; } = 590;
        public double BPMmetronome { get; set; } = 110.0;
        public int CanvasWidth { get; set; } = 1518;
        public int SensorPort { get; set; } = 4;
        public int MIDIPort { get; set; } = 1;
        public _MyInstrumentControlModes MyInstrumentControlMode { get; set; } = _MyInstrumentControlModes.Keyboard;
        public _SlidePlayModes SlidePlayMode { get; set; } = _SlidePlayModes.Off;
        public _SharpNotesModes SharpNotesMode { get; set; } = _SharpNotesModes.Off;
            
    }
}
