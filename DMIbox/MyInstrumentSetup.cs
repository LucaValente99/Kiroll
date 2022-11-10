using MyInstrument.DMIbox.KeyboardBehaviors;
using MyInstrument.DMIbox.SensorBehaviors;
using MyInstrument.DMIbox.TobiiBehaviors;
using MyInstrument.Surface;
using NeeqDMIs.Eyetracking.PointFilters;
using NeeqDMIs.Eyetracking.Tobii;
using NeeqDMIs.Keyboard;
using NeeqDMIs.MIDI;
using NeeqDMIs.NithSensors;
using RawInputProcessor;
using System.Windows.Interop;
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

                // Breath Sensor 
                Rack.DMIBox.SensorReader = new NithModule();

                Rack.DMIBox.KeyboardModule = new KeyboardModule(new WindowInteropHelper(Rack.DMIBox.MyInstrumentMainWindow).Handle, RawInputCaptureMode.ForegroundAndBackground);
                Rack.DMIBox.TobiiModule = new TobiiModule(GazePointDataMode.Unfiltered);

                Rack.DMIBox.KeyboardModule.KeyboardBehaviors.Add(new KBEyeTrackerToMouse());
                Rack.DMIBox.KeyboardModule.KeyboardBehaviors.Add(new KBsimulateBreathOn());

                Rack.DMIBox.TobiiModule.BlinkBehaviors.Add(new TBselectScale());
                Rack.DMIBox.TobiiModule.BlinkBehaviors.Add(new TBselectCode());
                Rack.DMIBox.TobiiModule.BlinkBehaviors.Add(new TBselectOctave());
                Rack.DMIBox.TobiiModule.BlinkBehaviors.Add(new TBactivateButton());

                Rack.DMIBox.SensorReader.SensorBehaviors.Add(new NBbreath(20, 28, 1.5f)); // 15 20 1.5f
                Rack.DMIBox.SensorReader.SensorBehaviors.Add(new NBactivateButton(28)); 

                // SURFACE INIT
                Rack.DMIBox.AutoScroller = new AutoScroller(Rack.DMIBox.MyInstrumentMainWindow.scrlMyInstrument, 0, 300, new PointFilterMAExpDecaying(0.1f)); // OLD was 100, 0.1f
                Rack.DMIBox.MyInstrumentSurface = new MyInstrumentSurface(Rack.DMIBox.MyInstrumentMainWindow.canvasMyInstrument);

                firstTime = false;
            }
            
            Rack.DMIBox.MyInstrumentSurface.DrawOnCanvas();
        }
    }
}
