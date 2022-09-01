using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NeeqDMIs.MIDI;

namespace MyInstrument.MIDI
{
    public class Tentativo_1
    {
        private MidiModuleNAudio midiModule = new MidiModuleNAudio(1,1);
        
        public void playNote(int pitch, int velocity)
        {
            midiModule.PlayNote(pitch, velocity);
        }

        public void stopNote(int pitch)
        {
            midiModule.StopNote(pitch);
        }


    }

}
