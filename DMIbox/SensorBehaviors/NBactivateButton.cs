using NeeqDMIs.NithSensors;
using System.Globalization;

namespace MyInstrument.DMIbox.SensorBehaviors
{
    public class NBactivateButton : INithSensorBehavior
    {
        private bool letClickAgain = true;
        private int onThresh;
        public NBactivateButton(int onThresh)
        {
            this.onThresh = onThresh;
        }

        public void HandleData(NithSensorData val)
        {
            if (Rack.UserSettings.MyInstrumentControlMode == _MyInstrumentControlModes.Breath)
            {
                float b = 0;

                try
                {
                    b = float.Parse(val.GetValue(NithArguments.press), CultureInfo.InvariantCulture.NumberFormat);
                }
                catch
                {

                }

                if (b > onThresh)
                {
                    if (!Rack.DMIBox.MyInstrumentMainWindow.Click && letClickAgain)
                    {
                        letClickAgain= false;
                        Rack.DMIBox.MyInstrumentMainWindow.Click = true;
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
