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
        int MIDIPort { get; set; }
        string ScaleName { get; set; }
        string ScaleCode { get; set; }
        string Octave { get; set; }
        _MyInstrumentControlModes MyInstrumentControlMode { get; set; }

        _SlidePlayModes SlidePlayMode { get; set; }

    }
}
