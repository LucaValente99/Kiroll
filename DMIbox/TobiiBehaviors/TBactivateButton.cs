using NeeqDMIs.Eyetracking.Tobii;

namespace Kiroll.DMIbox.TobiiBehaviors
{
    internal class TBactivateButton : ATobiiBlinkBehavior
    {
        public TBactivateButton()
        {
            DCThresh = 15;
        }
        public override void Event_doubleClose()
        {               
            if (Rack.UserSettings.EyeCtrl == _EyeCtrl.On && (Rack.UserSettings.KirollControlMode != _KirollControlModes.Breath 
                || !Rack.DMIBox.SensorReader.Connect(Rack.DMIBox.KirollMainWindow.SensorPort)))
            {
                if (!Rack.DMIBox.KirollMainWindow.Click)
                {
                    Rack.DMIBox.KirollMainWindow.Click = true;
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
