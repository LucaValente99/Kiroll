using NeeqDMIs.NithSensors;
using System.Globalization;

namespace Kiroll.DMIbox.SensorBehaviors
{
    public class NBactivateButton : INithSensorBehavior
    {
        // This variable helps to understand when it's possible to click again. In fact if the user gaze different
        // buttons consecutively continuing to blow, he won't enable them if he doesn't stop blowing first.
        private bool letClickAgain = true;
        private int onThresh;
        public NBactivateButton(int onThresh)
        {
            this.onThresh = onThresh;
        }

        public void HandleData(NithSensorData val)
        {
            if (Rack.UserSettings.KirollControlMode == _KirollControlModes.Breath)
            {
                float b = 0;

                try
                {
                    b = float.Parse(val.GetValue(NithArguments.press), CultureInfo.InvariantCulture.NumberFormat);
                }
                catch
                {

                }

                // Since NoteName == "_" we know that the user is not playing any key, necessary condition to
                // click a button using breath control.
                if (Rack.DMIBox.LastGazedButton != null && Rack.UserSettings.NoteName == "_")
                {
                    if (b > onThresh)
                    {
                        if (!Rack.DMIBox.KirollMainWindow.Click && letClickAgain)
                        {
                            letClickAgain = false;
                            Rack.DMIBox.KirollMainWindow.Click = true;
                        }
                    }
                    else
                    {
                        letClickAgain = true;
                    }
                }
            }

        }
    }
}
