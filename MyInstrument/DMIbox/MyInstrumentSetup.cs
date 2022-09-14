using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeeqDMIs.MIDI;

namespace MyInstrument.DMIbox
{
    internal class MyInstrumentSetup
    {

        public MyInstrumentSetup(MainWindow window)
        {
            Rack.DMIBox.MyInstrumentMainWindow = window;
        }

        public void Setup()
        {
            // MIDI
            Rack.DMIBox.MidiModule = new MidiModuleNAudio(1, 1);
            //MidiDeviceFinder midiDeviceFinder = new MidiDeviceFinder(Rack.DMIBox.MidiModule);
            //midiDeviceFinder.SetToLastDevice();
            Rack.DMIBox.MidiModule.OutDevice = 1;
        }
        
    }
}
