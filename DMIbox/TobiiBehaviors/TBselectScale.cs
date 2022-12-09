using NeeqDMIs.Eyetracking.Tobii;
using System.Windows;

namespace Kiroll.DMIbox.TobiiBehaviors
{
    public class TBselectScale : ATobiiBlinkBehavior
    {
        public TBselectScale()
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
                if (Rack.UserSettings.BlinkModes == _BlinkModes.Scale)
                {
                    if (Rack.DMIBox.KirollMainWindow.ScaleIndex > 0)
                    {
                        Rack.DMIBox.KirollMainWindow.ScaleIndex--;
                        Rack.UserSettings.ScaleName = Rack.DMIBox.KirollMainWindow.ComboScale[Rack.DMIBox.KirollMainWindow.ScaleIndex];
                    }
                }
            }            
        }

        public override void Event_leftOpen() { }

        public override void Event_rightClose()
        {
            if (Rack.UserSettings.EyeCtrl == _EyeCtrl.On)
             {
                if (Rack.UserSettings.BlinkModes == _BlinkModes.Scale)
                {
                    if (Rack.DMIBox.KirollMainWindow.ScaleIndex < 11)
                    {
                        Rack.DMIBox.KirollMainWindow.ScaleIndex++;
                        Rack.UserSettings.ScaleName = Rack.DMIBox.KirollMainWindow.ComboScale[Rack.DMIBox.KirollMainWindow.ScaleIndex];
                    }
                }
            }           
        }
        public override void Event_rightOpen() { }
    }
}

