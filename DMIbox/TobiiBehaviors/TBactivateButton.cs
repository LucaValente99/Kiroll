using NeeqDMIs.Eyetracking.Tobii;

namespace MyInstrument.DMIbox.TobiiBehaviors
{
    internal class TBactivateButton : ATobiiBlinkBehavior
    {
        public TBactivateButton()
        {
            DCThresh = 15;
        }
        public override void Event_doubleClose()
        {               
            if (Rack.UserSettings.EyeCtrl == _EyeCtrl.On && (Rack.UserSettings.MyInstrumentControlMode != _MyInstrumentControlModes.Breath 
                || !Rack.DMIBox.SensorReader.Connect(Rack.DMIBox.MyInstrumentMainWindow.SensorPort)))
            {
                if (!Rack.DMIBox.MyInstrumentMainWindow.Click)
                {
                    Rack.DMIBox.MyInstrumentMainWindow.Click = true;
                }
            }                            
        }

        public override void Event_doubleOpen() { }
        public override void Event_leftClose() { }
        public override void Event_leftOpen() { }
        public override void Event_rightClose() { }              
        public override void Event_rightOpen() { }          
        
    }
}
