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
        // Combobox settings
        string ScaleName { get; set; }
        string ScaleCode { get; set; }
        string Octave { get; set; }

        // Note visualizer settings
        string NoteName { get; set; }
        string NotePitch { get; set; }
        string NoteVelocity { get; set; }
        string NotePressure { get; set; }

        // Keybard-Canvas distances and sizes settings
        double KeyVerticaDistance { get; set; }
        double KeyHorizontalDistance { get; set; }
        double KeyboardHeight { get; set; }       
        int CanvasWidth { get; set; }

        // MIDI and Sensor settings
        int SensorPort { get; set; }
        int MIDIPort { get; set; }

        // Metronome settings
        double BPMmetronome { get; set; }

        // SwitchableSelectors settings
        _MyInstrumentControlModes MyInstrumentControlMode { get; set; }
        _SlidePlayModes SlidePlayMode { get; set; }
        _SharpNotesModes SharpNotesMode { get; set; }
        _BlinkModes BlinkModes { get; set; }

    }
}
