using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInstrument.DMIbox
{
    
    public enum _MyInstrumentControlModes
    {
        Keyboard,
        Breath,
        NaN
    }

    public enum _EyeCtrl
    {
        On,
        Off
    }

    public enum _SlidePlayModes
    {
        On,
        Off
    }

    public enum _BreathControlModes
    {
        Dynamic,
        Static
    }

    public enum _SharpNotesModes {
        On,
        Off
    }

    public enum _BlinkModes
    {
        Scale,
        Octave,
        Code,
        Off
    }
}
