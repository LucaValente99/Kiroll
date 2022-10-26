using MyInstrument.DMIbox;
using NeeqDMIs.ATmega;
using System.Globalization;
using System.Runtime.ExceptionServices;

namespace Netytar.DMIbox.SensorBehaviors
{
    public class NBbreath : ISensorBehavior
    {
        private int v = 1;
        private int offThresh;
        private int onThresh;
        private float sensitivity;

        public NBbreath(int offThresh, int onThresh, float sensitivity)
        {
            this.offThresh = offThresh;
            this.onThresh = onThresh;
            this.sensitivity = sensitivity;
        }

        public void ReceiveSensorRead(string val)
        {
            if (Rack.UserSettings.MyInstrumentControlMode == _MyInstrumentControlModes.Breath)
            {
                float b = 0;

                try
                {
                    b = float.Parse(val, CultureInfo.InvariantCulture.NumberFormat);
                }
                catch
                {

                }
              
                v = (int)(b / 3);

                //Rack.DMIBox.MyInstrumentMainWindow.BreathSensorValue = v;
                Rack.DMIBox.Pressure = (int)(v * 2 * sensitivity);

                if (v > onThresh && Rack.DMIBox.BreathOn == false)
                {
                    Rack.DMIBox.BreathOn = true;
                }

                if (v < offThresh && Rack.DMIBox.BreathOn == true)
                {
                    Rack.DMIBox.BreathOn = false;
                }
            }

        }
    }
}
