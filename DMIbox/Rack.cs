using Kiroll.Surface.UserSettings;

namespace Kiroll.DMIbox
{
    internal static class Rack
    {
        private static KirollDMIBox dmibox = new KirollDMIBox();
        public static KirollDMIBox DMIBox { get => dmibox; set => dmibox = value; }
        public static IUserSettings UserSettings { get; set; } = new DefaultUserSettings();
    }
}
