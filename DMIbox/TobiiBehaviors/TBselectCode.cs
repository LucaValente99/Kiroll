using MyInstrument.DMIbox;
using MyInstrument;
using NeeqDMIs.Eyetracking.Tobii;
using NeeqDMIs.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace MyInstrument.DMIbox.TobiiBehaviors
{
    public class TBselectCode : ATobiiBlinkBehavior
    {
        public TBselectCode()
        {
            LCThresh = 2;
            RCThresh = 2;
        }

        public override void Event_doubleClose() { }

        public override void Event_doubleOpen() { }

        public override void Event_leftClose()
        {
            if (Rack.UserSettings.BlinkModes == _BlinkModes.Code)
            {
                if (Rack.DMIBox.MyInstrumentMainWindow.CodeIndex > 0)
                {
                    Rack.DMIBox.MyInstrumentMainWindow.CodeIndex--;
                    Rack.DMIBox.MyInstrumentMainWindow.txtCode.Text = Rack.DMIBox.MyInstrumentMainWindow.ComboCode[Rack.DMIBox.MyInstrumentMainWindow.CodeIndex];
                    Rack.UserSettings.ScaleCode = Rack.DMIBox.MyInstrumentMainWindow.txtCode.Text;
                    Rack.DMIBox.MyInstrumentSurface.DrawOnCanvas();
                }
            }
        }

        public override void Event_leftOpen() { }

        public override void Event_rightClose()
        {
            if (Rack.UserSettings.BlinkModes == _BlinkModes.Code)
            {
                if (Rack.DMIBox.MyInstrumentMainWindow.CodeIndex < 3)
                {
                    Rack.DMIBox.MyInstrumentMainWindow.CodeIndex++;
                    Rack.DMIBox.MyInstrumentMainWindow.txtCode.Text = Rack.DMIBox.MyInstrumentMainWindow.ComboCode[Rack.DMIBox.MyInstrumentMainWindow.CodeIndex];
                    Rack.UserSettings.ScaleCode = Rack.DMIBox.MyInstrumentMainWindow.ComboCode[Rack.DMIBox.MyInstrumentMainWindow.CodeIndex];
                    Rack.DMIBox.MyInstrumentSurface.DrawOnCanvas();
                }
            }
        }
        public override void Event_rightOpen() { }
    }
}

