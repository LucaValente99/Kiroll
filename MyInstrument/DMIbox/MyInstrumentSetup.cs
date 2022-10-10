using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyInstrument.Surface;
using NeeqDMIs.Eyetracking.PointFilters;
using NeeqDMIs.MIDI;

namespace MyInstrument.DMIbox
{
    internal class MyInstrumentSetup
    {
        private static bool firstTime = true;
        public MyInstrumentSetup(MainWindow window)
        {
            Rack.DMIBox.MyInstrumentMainWindow = window;
        }

        public void Setup()
        {           

            if (firstTime)
            {
                // MIDI
                Rack.DMIBox.MidiModule = new MidiModuleNAudio(1, 1);
                Rack.DMIBox.MidiModule.OutDevice = 1;
                firstTime = false;
            }           

            // SURFACE INIT
            Rack.DMIBox.AutoScroller = new AutoScroller(Rack.DMIBox.MyInstrumentMainWindow.scrlMyInstrument, 0, 100, new PointFilterMAExpDecaying(0.1f)); // OLD was 100, 0.1f
            Rack.DMIBox.MyInstrumentSurface = new MyInstrumentSurface(Rack.DMIBox.MyInstrumentMainWindow.canvasMyInstrument);
            Rack.DMIBox.MyInstrumentSurface.DrawOnCanvas();
            
        }
        
    }
}
