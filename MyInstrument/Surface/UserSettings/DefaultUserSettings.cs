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
        public int MIDIPort { get; set; } = 1;

        public string ScaleName { get; set; } = "C";
        public string ScaleCode { get; set; } = "maj";
        public string Octave { get; set; } = "4";
        public string NoteName { get; set; } = "_";
        public string NotePitch { get; set; } = "_";
        public string NoteVelocity { get; set; } = "_";
        public _MyInstrumentControlModes MyInstrumentControlMode { get; set; } = _MyInstrumentControlModes.Face;
        public _SlidePlayModes SlidePlayMode { get; set; } = _SlidePlayModes.On;
    }
}
