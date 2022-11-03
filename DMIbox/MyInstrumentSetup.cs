using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using MyInstrument.DMIbox.Behaviors;
using MyInstrument.Surface;
using NeeqDMIs.ATmega;
using NeeqDMIs.Eyetracking.MouseEmulator;
using NeeqDMIs.Eyetracking.PointFilters;
using NeeqDMIs.Eyetracking.Tobii;
using NeeqDMIs.Keyboard;
using NeeqDMIs.MIDI;
using NeeqDMIs.NithSensors;
using Netytar;
using Netytar.DMIbox.SensorBehaviors;
using RawInputProcessor;
using Tobii.Interaction.Framework;

namespace MyInstrument.DMIbox
{
    internal class MyInstrumentSetup
    {
        // Used to track the first time the instrument is started: lots of objects need to be instantiated
        // just one time (this is due to the possibilitiy to start and stop the instrument).

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

                //Breath Sensor 
                Rack.DMIBox.SensorReader = new NithModule();

                Rack.DMIBox.KeyboardModule = new KeyboardModule(new WindowInteropHelper(Rack.DMIBox.MyInstrumentMainWindow).Handle, RawInputCaptureMode.ForegroundAndBackground);
                Rack.DMIBox.TobiiModule = new TobiiModule(GazePointDataMode.Unfiltered);

                Rack.DMIBox.KeyboardModule.KeyboardBehaviors.Add(new KBEyeTrackerToMouse());
                Rack.DMIBox.KeyboardModule.KeyboardBehaviors.Add(new KBsimulateBreathOn());

                Rack.DMIBox.SensorReader.SensorBehaviors.Add(new NBbreath(40, 40, 1.5f)); // 15 20 1.5f

                // SURFACE INIT
                Rack.DMIBox.AutoScroller = new AutoScroller(Rack.DMIBox.MyInstrumentMainWindow.scrlMyInstrument, 0, 300, new PointFilterMAExpDecaying(0.09f)); // OLD was 100, 0.1f
                Rack.DMIBox.MyInstrumentSurface = new MyInstrumentSurface(Rack.DMIBox.MyInstrumentMainWindow.canvasMyInstrument);

                firstTime = false;
            }                     
            Rack.DMIBox.MyInstrumentSurface.DrawOnCanvas();
        }
    }
}
