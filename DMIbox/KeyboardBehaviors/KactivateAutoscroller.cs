using NeeqDMIs.Eyetracking.MouseEmulator;
using NeeqDMIs.Eyetracking.PointFilters;
using NeeqDMIs.Keyboard;
﻿using NeeqDMIs.Keyboard;
using RawInputProcessor;

namespace MyInstrument.DMIbox.Behaviors
{
    public class KactivateAutoscroller : IKeyboardBehavior
    {
        private VKeyCodes activateAutoscroller = VKeyCodes.Q;
        private VKeyCodes deactivateAutoscroller = VKeyCodes.D;

        public int ReceiveEvent(RawInputEventArgs e)
        {
            if (e.VirtualKey == (ushort)activateAutoscroller && e.KeyPressState == KeyPressState.Down)
            {
                Rack.DMIBox.TobiiModule.MouseEmulator.EyetrackerToMouse = true;
                Rack.DMIBox.TobiiModule.MouseEmulator.CursorVisible = false;
                return 0;
            }
            if (e.VirtualKey == (ushort)deactivateAutoscroller && e.KeyPressState == KeyPressState.Down)
            {
                Rack.DMIBox.TobiiModule.MouseEmulator.EyetrackerToMouse = false;
                Rack.DMIBox.TobiiModule.MouseEmulator.CursorVisible = true;
                return 0;
            }
            return 1;
        }
    }
}
