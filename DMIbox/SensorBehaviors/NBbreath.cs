using NeeqDMIs.NithSensors;
using System.Globalization;

namespace Kiroll.DMIbox.SensorBehaviors
{
    public class NBbreath : INithSensorBehavior
    {
        private int offThresh;
        private int onThresh;
        private float sensitivity;
        public NBbreath(int offThresh, int onThresh, float sensitivity) 
        {
            this.offThresh = offThresh;
            this.onThresh = onThresh;
            this.sensitivity = sensitivity;
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

                if (Rack.DMIBox.MidiModule.IsMidiOk())
                {
                    Rack.DMIBox.Pressure = (int)(b * sensitivity);
                }

                if (b > onThresh)
                {
                    Rack.DMIBox.BreathOn = true;
                }

                if (b < offThresh)
                {
                    Rack.DMIBox.BreathOn = false;
                }
            }

        }
    }
}
