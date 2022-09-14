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
        public _MyInstrumentControlModes MyInstrumentControlMode { get; set; } = _MyInstrumentControlModes.Face;
        public _SlidePlayModes SlidePlayMode { get; set; } = _SlidePlayModes.On;
    }
}
