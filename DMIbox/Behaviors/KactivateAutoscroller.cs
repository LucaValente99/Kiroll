using NeeqDMIs.Eyetracking.PointFilters;
using NeeqDMIs.Keyboard;
using RawInputProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInstrument.DMIbox.Behaviors
{
    internal class KactivateAutoscroller : IKeyboardBehavior
    {
        const VKeyCodes activateAutoscroleller = VKeyCodes.A;
        const VKeyCodes deactivateAutoscroleller = VKeyCodes.D;

        public int ReceiveEvent(RawInputEventArgs e)
        {
            if (e.VirtualKey == (ushort) activateAutoscroleller)
            {
                Rack.DMIBox.TobiiModule.MouseEmulator.EyetrackerToMouse = true;
                Rack.DMIBox.TobiiModule.MouseEmulator.CursorVisible = false;

            }else if (e.VirtualKey == (ushort)deactivateAutoscroleller)
            {
                Rack.DMIBox.TobiiModule.MouseEmulator.EyetrackerToMouse = false;
                Rack.DMIBox.TobiiModule.MouseEmulator.CursorVisible = true;
            }
                return 1;
        }
    }
}
