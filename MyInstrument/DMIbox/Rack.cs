using MyInstrument.Surface.UserSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInstrument.DMIbox
{
    internal static class Rack
    {
        private static MyInstrumentDMIBox dmibox = new MyInstrumentDMIBox();
        public static MyInstrumentDMIBox DMIBox { get => dmibox; set => dmibox = value; }

        public static IUserSettings UserSettings { get; set; } = new DefaultUserSettings();
    }
}
