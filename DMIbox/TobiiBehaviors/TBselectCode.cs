using NeeqDMIs.Eyetracking.Tobii;

namespace MyInstrument.DMIbox.TobiiBehaviors
{
    public class TBselectCode : ATobiiBlinkBehavior
    {
        public TBselectCode()
        {
            LCThresh = 6;
            RCThresh = 6;
        }

        public override void Event_doubleClose() { }

        public override void Event_doubleOpen() { }

        public override void Event_leftClose()
        {
            if (Rack.UserSettings.EyeCtrl == _EyeCtrl.On)
            {
                if (Rack.UserSettings.BlinkModes == _BlinkModes.Code)
                {
                    if (Rack.DMIBox.MyInstrumentMainWindow.CodeIndex > 0)
                    {
                        Rack.DMIBox.MyInstrumentMainWindow.CodeIndex--;
                        Rack.UserSettings.ScaleCode = Rack.DMIBox.MyInstrumentMainWindow.ComboCode[Rack.DMIBox.MyInstrumentMainWindow.CodeIndex];
                    }
                }
            }           
        }

        public override void Event_leftOpen() { }

        public override void Event_rightClose()
        {
            if (Rack.UserSettings.EyeCtrl == _EyeCtrl.On)
            {
                if (Rack.UserSettings.BlinkModes == _BlinkModes.Code)
                {
                    if (Rack.DMIBox.MyInstrumentMainWindow.CodeIndex < 3)
                    {
                        Rack.DMIBox.MyInstrumentMainWindow.CodeIndex++;
                        Rack.UserSettings.ScaleCode = Rack.DMIBox.MyInstrumentMainWindow.ComboCode[Rack.DMIBox.MyInstrumentMainWindow.CodeIndex];
                    }
                }
            }           
        }
        public override void Event_rightOpen() { }
    }
}

