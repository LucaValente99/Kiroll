using NeeqDMIs.Keyboard;
using RawInputProcessor;

namespace MyInstrument.DMIbox.Behaviors
{
    public class KBEyeTrackerToMouse : IKeyboardBehavior
    {
        private VKeyCodes activate = VKeyCodes.Q;
        private VKeyCodes deactivate = VKeyCodes.A;

        public int ReceiveEvent(RawInputEventArgs e)
        {
            if (e.VirtualKey == (ushort)activate)
            {
                Rack.DMIBox.TobiiModule.MouseEmulator.EyetrackerToMouse = true;
                //Rack.DMIBox.TobiiModule.MouseEmulator.CursorVisible = false;
                return 0;
            }
            if (e.VirtualKey == (ushort)deactivate)
            {
                Rack.DMIBox.TobiiModule.MouseEmulator.EyetrackerToMouse = false;
                //Rack.DMIBox.TobiiModule.MouseEmulator.CursorVisible = true;
                return 0;
            }
            return 1;
        }
    }
}
