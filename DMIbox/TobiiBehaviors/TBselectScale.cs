using MyInstrument.DMIbox;
using MyInstrument;
using NeeqDMIs.Eyetracking.Tobii;
using NeeqDMIs.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInstrument.DMIbox.TobiiBehaviors
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
                    if (Rack.DMIBox.MyInstrumentMainWindow.ScaleIndex > 0)
                    {
                        Rack.DMIBox.MyInstrumentMainWindow.ScaleIndex--;
                        Rack.UserSettings.ScaleName = Rack.DMIBox.MyInstrumentMainWindow.ComboScale[Rack.DMIBox.MyInstrumentMainWindow.ScaleIndex];
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
                    if (Rack.DMIBox.MyInstrumentMainWindow.ScaleIndex < 11)
                    {
                        Rack.DMIBox.MyInstrumentMainWindow.ScaleIndex++;
                        Rack.UserSettings.ScaleName = Rack.DMIBox.MyInstrumentMainWindow.ComboScale[Rack.DMIBox.MyInstrumentMainWindow.ScaleIndex];
                    }
                }
            }           
        }
        public override void Event_rightOpen() { }
    }
}

