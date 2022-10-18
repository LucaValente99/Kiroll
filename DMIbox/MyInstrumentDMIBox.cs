using MyInstrument.Surface;
using NeeqDMIs;
using NeeqDMIs.ATmega;
using NeeqDMIs.Eyetracking.Tobii;
using NeeqDMIs.Keyboard;
using NeeqDMIs.Music;

namespace MyInstrument.DMIbox
{
    public class MyInstrumentDMIBox : DMIBox
    {
        private const _ModulationControlModes DEFAULT_MODULATIONCONTROLMODE = _ModulationControlModes.On;
        private const _MouthControlModes DEFAULT_MOUTHCONTROLMODE = _MouthControlModes.Dynamic;

        
        private _ModulationControlModes modulationControlMode = DEFAULT_MODULATIONCONTROLMODE;
        private _MouthControlModes mouthControlMode = DEFAULT_MOUTHCONTROLMODE;
        public _ModulationControlModes ModulationControlMode { get => modulationControlMode; set { modulationControlMode = value; ResetModulationAndPressure(); } }
        public _MouthControlModes MouthControlMode { get => mouthControlMode; set { mouthControlMode = value; ResetModulationAndPressure(); } }

        public TobiiModule tobiiModule { get; set; }

        private SensorModule sensorReader;
        public SensorModule SensorReader { get => sensorReader; set => sensorReader = value; }

        private bool breathOn = false;
        private int velocity = 127;
        private int pressure = 127;
        private int modulation = 0;
        private MidiNotes selectedNote = MidiNotes.C5;
        private MidiNotes nextNote = MidiNotes.C5;

        public MainWindow MyInstrumentMainWindow { get; set; }
        public KeyboardModule KeyboardModule { get; set; }

        #region Graphic components

        private AutoScroller autoScroller;
        private MyInstrumentSurface myInstrumentSurface;
        public AutoScroller AutoScroller { get => autoScroller; set => autoScroller = value; }
        public MyInstrumentSurface MyInstrumentSurface { get => myInstrumentSurface; set => myInstrumentSurface = value; }

        #endregion Graphic components      

        public bool BreathOn
        {
            get { return breathOn; }
            set
            {
                switch (Rack.UserSettings.SlidePlayMode)
                {
                    case _SlidePlayModes.On:
                        if (value != breathOn)
                        {
                            breathOn = value;
                            if (breathOn == true)
                            {
                                PlaySelectedNote();
                            }
                            else
                            {
                                StopSelectedNote();
                            }
                        }
                        break;
                    case _SlidePlayModes.Off:
                        if (value != breathOn)
                        {
                            breathOn = value;
                            if (breathOn == true)
                            {
                                selectedNote = nextNote;
                                PlaySelectedNote();
                            }
                            else
                            {
                                StopSelectedNote();
                            }
                        }
                        break;
                }

            }
        }

        public int Pressure
        {
            get { return pressure; }
            set
            {
                if (MouthControlMode == _MouthControlModes.Dynamic)
                {
                    if (value < 50 && value > 1)
                    {
                        pressure = 50;
                    }
                    else if (value > 127)
                    {
                        pressure = 127;
                    }
                    else if (value == 0)
                    {
                        pressure = 0;
                    }
                    else
                    {
                        pressure = value;
                    }
                    SetPressure();
                }
                if (MouthControlMode == _MouthControlModes.Switch)
                {
                    pressure = 127;
                    SetPressure();
                }
            }
        }

        public int Modulation
        {
            get { return modulation; }
            set
            {
                if (ModulationControlMode == _ModulationControlModes.On)
                {
                    if (value < 50 && value > 1)
                    {
                        modulation = 50;
                    }
                    else if (value > 127)
                    {
                        modulation = 127;
                    }
                    else if (value == 0)
                    {
                        modulation = 0;
                    }
                    else
                    {
                        modulation = value;
                    }
                    SetModulation();
                }
                else if (ModulationControlMode == _ModulationControlModes.Off)
                {
                    modulation = 0;
                    SetModulation();
                }
            }
        }

        public int Velocity
        {
            get { return velocity; }
            set
            {
                if (value < 0)
                {
                    velocity = 0;
                }
                else if (value > 127)
                {
                    velocity = 127;
                }
                else
                {
                    velocity = value;
                }
            }
        }

        public void ResetModulationAndPressure()
        {
            BreathOn = false;
            Modulation = 0;
            Pressure = 127;
            Velocity = 127;
        }

        private void PlaySelectedNote()
        {
            MidiModule.PlayNote((int)selectedNote, velocity);
        }

        private void StopSelectedNote()
        {
            MidiModule.StopNote((int)selectedNote);
        }

        private void SetModulation()
        {
            MidiModule.SetModulation(Modulation);
        }

        private void SetPressure()
        {
            MidiModule.SetPressure(pressure);
        }

        public void Dispose()
        {
            try
            {
                TobiiModule.Dispose();
            }
            catch
            {
            }
            try
            {
                SensorReader.Disconnect();
            }
            catch
            {
            }
        }

    }
}
