using NeeqDMIs.Keyboard;
using RawInputProcessor;

namespace MyInstrument.DMIbox.KeyboardBehaviors
{
    public class KBsimulateBreathOn : IKeyboardBehavior
    {
        private VKeyCodes keyBreathOn = VKeyCodes.Space;

        private bool breathOn = false;
        int returnVal = 0;

        public int ReceiveEvent(RawInputEventArgs e)
        {
            returnVal = 0;

            if (Rack.UserSettings.MyInstrumentControlMode == _MyInstrumentControlModes.Keyboard)
            {
                if (e.VirtualKey == (ushort)keyBreathOn && e.KeyPressState == KeyPressState.Down)
                {
                    breathOn = true;
                    returnVal = 1;
                }
                else if (e.VirtualKey == (ushort)keyBreathOn && e.KeyPressState == KeyPressState.Up)
                {
                    breathOn = false;
                    returnVal = 1;
                }
                Rack.DMIBox.BreathOn = breathOn;
            }

            return returnVal;
        }
    }
}
