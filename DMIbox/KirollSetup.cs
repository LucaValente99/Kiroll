using Kiroll.DMIbox.KeyboardBehaviors;
using Kiroll.DMIbox.SensorBehaviors;
using Kiroll.DMIbox.TobiiBehaviors;
using Kiroll.Surface;
using NeeqDMIs.Eyetracking.PointFilters;
using NeeqDMIs.Eyetracking.Tobii;
using NeeqDMIs.Keyboard;
using NeeqDMIs.MIDI;
using NeeqDMIs.NithSensors;
using RawInputProcessor;
using System.Windows.Interop;
using Tobii.Interaction.Framework;

namespace Kiroll.DMIbox
{
    internal class KirollSetup
    {
        // Used to track the first time the instrument is started: lots of objects need to be instantiated
        // just one time (this is due to the possibilitiy to start and stop the instrument).

        private static bool firstTime = true;
        public KirollSetup(MainWindow window)
        {
            Rack.DMIBox.KirollMainWindow = window;
        }

        public void Setup()
        {           
            if (firstTime)
            {
                // MIDI
                Rack.DMIBox.MidiModule = new MidiModuleNAudio(1, 1);
                Rack.DMIBox.MidiModule.OutDevice = 1;

                // BREATH SENSOR 
                Rack.DMIBox.SensorReader = new NithModule();

                Rack.DMIBox.KeyboardModule = new KeyboardModule(new WindowInteropHelper(Rack.DMIBox.KirollMainWindow).Handle, RawInputCaptureMode.ForegroundAndBackground);
                Rack.DMIBox.TobiiModule = new TobiiModule(GazePointDataMode.Unfiltered);

                //BEHAVIORS - Keyboard, Tobii, Breath
                Rack.DMIBox.KeyboardModule.KeyboardBehaviors.Add(new KBEyeTrackerToMouse());
                Rack.DMIBox.KeyboardModule.KeyboardBehaviors.Add(new KBsimulateBreathOn());

                Rack.DMIBox.TobiiModule.BlinkBehaviors.Add(new TBselectScale());
                Rack.DMIBox.TobiiModule.BlinkBehaviors.Add(new TBselectCode());
                Rack.DMIBox.TobiiModule.BlinkBehaviors.Add(new TBselectOctave());
                Rack.DMIBox.TobiiModule.BlinkBehaviors.Add(new TBactivateButton());

                Rack.DMIBox.SensorReader.SensorBehaviors.Add(new NBbreath(20, 28, 1.5f)); // 15 20 1.5f
                Rack.DMIBox.SensorReader.SensorBehaviors.Add(new NBactivateButton(28)); 

                // SURFACE INIT
                Rack.DMIBox.AutoScroller = new AutoScroller(Rack.DMIBox.KirollMainWindow.scrlKiroll, 0, 300, new PointFilterMAExpDecaying(0.1f)); // OLD was 100, 0.1f
                Rack.DMIBox.KirollSurface = new KirollSurface(Rack.DMIBox.KirollMainWindow.canvasKiroll);

                firstTime = false;
            }
            
            Rack.DMIBox.KirollSurface.DrawOnCanvas();
        }
    }
}
