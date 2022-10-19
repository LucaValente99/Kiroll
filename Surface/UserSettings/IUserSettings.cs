using MyInstrument.DMIbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MyInstrument.Surface.UserSettings
{
    public interface IUserSettings
    {    
        string ScaleName { get; set; }
        string ScaleCode { get; set; }
        string Octave { get; set; }
        string NoteName { get; set; }
        string NotePitch { get; set; }
        string NoteVelocity { get; set; }
        double KeyVerticaDistance { get; set; }
        double KeyHorizontalDistance { get; set; }
        double KeyboardHeight { get; set; }
        double BPMmetronome { get; set; }
        int CanvasWidth { get; set; }
        int SensorPort { get; set; }
        int MIDIPort { get; set; }
        _MyInstrumentControlModes MyInstrumentControlMode { get; set; }
        _SlidePlayModes SlidePlayMode { get; set; }
        _SharpNotesModes SharpNotesMode { get; set; }

    }
}
