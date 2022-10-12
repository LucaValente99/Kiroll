using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using MyInstrument.DMIbox.Behaviors;
using MyInstrument.Surface;
using NeeqDMIs.Eyetracking.MouseEmulator;
using NeeqDMIs.Eyetracking.PointFilters;
using NeeqDMIs.Eyetracking.Tobii;
using NeeqDMIs.Keyboard;
using NeeqDMIs.MIDI;
using RawInputProcessor;
using Tobii.Interaction.Framework;

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

            IntPtr windowHandle = new WindowInteropHelper(Rack.DMIBox.MyInstrumentMainWindow).Handle;
            Rack.DMIBox.KeyboardModule = new KeyboardModule(windowHandle, RawInputCaptureMode.Foreground);
            Rack.DMIBox.tobiiModule = new TobiiModule(GazePointDataMode.Unfiltered);
            Rack.DMIBox.KeyboardModule.KeyboardBehaviors.Add(new KactivateAutoscroller());

            // SURFACE INIT
            Rack.DMIBox.AutoScroller = new AutoScroller(Rack.DMIBox.MyInstrumentMainWindow.scrlMyInstrument, 0, 100, new PointFilterMAExpDecaying(0.1f)); // OLD was 100, 0.1f
            Rack.DMIBox.MyInstrumentSurface = new MyInstrumentSurface(Rack.DMIBox.MyInstrumentMainWindow.canvasMyInstrument);
            Rack.DMIBox.MyInstrumentSurface.DrawOnCanvas();
            
        }
        
    }
}
