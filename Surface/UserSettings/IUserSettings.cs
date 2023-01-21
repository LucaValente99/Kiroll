using Kiroll.DMIbox;
using System.Windows.Controls;

namespace Kiroll.Surface.UserSettings
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

        // Orientation default settings
        Orientation Orientation { get; set; }

        // SwitchableSelectors settings
        _KirollControlModes KirollControlMode { get; set; }
        _SlidePlayModes SlidePlayMode { get; set; }
        _SharpNotesModes SharpNotesMode { get; set; }
        _BlinkModes BlinkModes { get; set; }
        _EyeCtrl EyeCtrl { get; set; }
        _BreathControlModes BreathControlModes { get; set; } 
        _KeyName KeyName { get; set; }

    }
}
