using MyInstrument.Surface.UserSettings;

namespace MyInstrument.DMIbox
{
    internal static class Rack
    {
        private static MyInstrumentDMIBox dmibox = new MyInstrumentDMIBox();
        public static MyInstrumentDMIBox DMIBox { get => dmibox; set => dmibox = value; }
        public static IUserSettings UserSettings { get; set; } = new DefaultUserSettings();
    }
}
