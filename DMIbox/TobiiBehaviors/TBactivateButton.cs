using NeeqDMIs.Eyetracking.Tobii;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

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
            if (Rack.UserSettings.EyeCtrl == _EyeCtrl.On)
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
