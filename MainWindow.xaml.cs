using MyInstrument.DMIbox;
using MyInstrument.Surface;
using System;
using System.Collections.Generic;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Reactive.Linq;
using Timer = System.Windows.Forms.Timer;
using System.Security.Policy;

namespace MyInstrument
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Bool variables for checking activation and deactivation of buttons
        private bool myInstrumentStarted = false;
        private bool myInstrumentSettingsOpened = false;
        private bool btnKeyboardOn = false;
        private bool btnBreathOn = false;
        private bool btnSlidePlayOn = false;
        private bool btnSharpNotesOn = false;
        private int breathSensorValue = 0;
        private bool playMetronome = false;
        private SoundPlayer metronome = new SoundPlayer(@"D:\Universita\Tirocinio_tesi\1_Tirocinio\MyInstrument\Audio\Metronome.wav");
        public int BreathSensorValue { get => breathSensorValue; set => breathSensorValue = value; }
        public int SensorPort
        {
            get { return Rack.UserSettings.SensorPort; }
            set
            {
                if (value > 0)
                {
                    Rack.UserSettings.SensorPort = value;
                }
            }
        }

        //Dictionaries used to select index into combobox for Start and Stop phases
        private Dictionary<string, int> comboScale = new Dictionary<string, int>()
        {
            {"C", 0 }, { "C#", 1 }, { "D", 2 }, { "D#", 3 }, { "E", 4 }, { "F", 5 }, { "F#", 6 }, {"G", 7 }, { "G#", 8 }, { "A", 9 }, { "A#", 10 }, { "B", 11}, { "_", 12}
        };
        private Dictionary<string, int> comboCode = new Dictionary<string, int>()
        {
            {"maj", 0}, {"min", 1}, {"chrom", 2}, { "_", 3}
        };
        private Dictionary<string, int> comboOctave = new Dictionary<string, int>()
        {
            {"2", 0 }, {"3", 1 }, {"4", 2 }, {"5", 3}, {"6", 4}, { "_", 5}
        };

        private readonly SolidColorBrush ActiveBrush = new SolidColorBrush(Colors.LightYellow);
        private readonly SolidColorBrush WarningBrush = new SolidColorBrush(Colors.DarkRed);
        private readonly SolidColorBrush DisableBrush = new SolidColorBrush(Colors.Transparent);

        //Icons and Backgrounds
        BitmapImage startIcon = new BitmapImage(
                    new Uri(Environment.CurrentDirectory + @"\..\..\Images\Icons\Start.png"));

        BitmapImage pauseIcon = new BitmapImage(
                    new Uri(Environment.CurrentDirectory + @"\..\..\Images\Icons\Pause.png"));

        BitmapImage settingsIcon = new BitmapImage(
                    new Uri(Environment.CurrentDirectory + @"\..\..\Images\Icons\Settings.png"));

        BitmapImage closeSettingsIcon = new BitmapImage(
                    new Uri(Environment.CurrentDirectory + @"\..\..\Images\Icons\CloseSettings_1.png"));

        ImageBrush buttonBackground = new ImageBrush(new BitmapImage(
                    new Uri(Environment.CurrentDirectory + @"\..\..\Images\Backgrounds\Buttons.jpeg")));

        private Timer metronomeTimer;
        public MainWindow()
        {
            InitializeComponent();

            metronomeTimer = new Timer();
            metronomeTimer.Interval = Convert.ToInt32((1.0 / (Rack.UserSettings.BPMmetronome / 60.0)) * 1000);
            metronomeTimer.Tick += Metronome;
            metronomeTimer.Start();            
        }

        private void Metronome(object sender, EventArgs e)
        {
            if (playMetronome)
            {
                metronome.Play();
            }
            else { 
                metronome.Stop(); 
            }

        }

        #region TopBar (Row0)

        #region Start, Exit and Setting buttons

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Rack.DMIBox.Dispose();
            Close();
        }
       
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!myInstrumentStarted)
            {

                MyInstrumentSetup myInstrumentSetup = new MyInstrumentSetup(this);
                myInstrumentSetup.Setup();              

                // Graphic changes
                btnStartImage.Source = pauseIcon;
                btnStart.Background = ActiveBrush;
                btnStartLabel.Content = "Running...";

                // Enabling ComboBox & slider
                lstScaleChanger.IsEnabled = true;
                lstScaleChanger.SelectedIndex = comboScale[Rack.UserSettings.ScaleName];
                lstCodeChanger.IsEnabled = true;
                lstCodeChanger.SelectedIndex = comboCode[Rack.UserSettings.ScaleCode];
                lstOctaveChanger.IsEnabled = true;
                lstOctaveChanger.SelectedIndex = comboOctave[Rack.UserSettings.Octave];
                sldVerticalDistance.IsEnabled = true;
                sldHorizontalDistance.IsEnabled = true;


                // MIDI
                txtMidiPort.Text = "MP" + Rack.DMIBox.MidiModule.OutDevice.ToString();
                CheckMidiPort();

                // Breath Sensor
                UpdateSensorConnection();

                //Metronome                
                CheckMetronome();

                myInstrumentStarted = true;               
            }
            else
            {
                // Graphic changes
                myInstrumentStarted = false;
                btnStartImage.Source = startIcon;
                btnStart.Background = DisableBrush;
                btnStartLabel.Content = "Start";
                txtMidiPort.Text = "";
                txtBreathPort.Text = "";
                txtMetronome.Text = "";

                // Disabling ComboBox, slider & Metronome
                lstScaleChanger.IsEnabled = false;
                lstScaleChanger.SelectedIndex = comboScale["_"];
                lstCodeChanger.IsEnabled = false;
                lstCodeChanger.SelectedIndex = comboCode["_"];
                lstOctaveChanger.IsEnabled = false;
                lstOctaveChanger.SelectedIndex = comboOctave["_"];
                sldVerticalDistance.IsEnabled = false;
                sldHorizontalDistance.IsEnabled = false;
                playMetronome = false;

                // Resetting surface
                Rack.DMIBox.MyInstrumentSurface.ClearSurface();
            }
        }

        private void btnMetronome_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                if (!playMetronome)
                {
                    playMetronome = true;
                    btnMetronome.Background = ActiveBrush;
                }
                else
                {
                    playMetronome = false;
                    btnMetronome.Background = buttonBackground;
                }

                CheckMetronome();
            }                     
        }
        private void CheckMetronome()
        {
            txtMetronome.Text = Rack.UserSettings.BPMmetronome.ToString();

            if (playMetronome)
            {
                txtMetronome.Foreground = ActiveBrush;
            }
            else
            {
                txtMetronome.Foreground = WarningBrush;
            }
        }

        private void btnInstrumentSettings_Click(object sender, RoutedEventArgs e)
        {
            if (!myInstrumentSettingsOpened)
            {
                myInstrumentSettingsOpened = true;

                // Graphic changes
                WindowInstrumentSettings.Visibility = Visibility.Visible;
                btnInstrumentSettingImage.Source = closeSettingsIcon;
                btnInstrumentSettings.Background = ActiveBrush;
                btnInstrumentSettingLabel.Content = "Close Settings";
            }
            else
            {
                myInstrumentSettingsOpened = false;

                // Graphic changes
                WindowInstrumentSettings.Visibility = Visibility.Hidden;
                btnInstrumentSettingImage.Source = settingsIcon;
                btnInstrumentSettings.Background = DisableBrush;
                btnInstrumentSettingLabel.Content = "Instrument Settings";
            }
        }

        #endregion Start, Exit and Setting buttons

        #endregion TopBar (Row0)

        #region Instrument (Row1)

        #region Instrument Settings

        private void btnCtrlKeyboard_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                if (!btnKeyboardOn)
                {
                    btnKeyboardOn = true;
                    btnBreathOn = false;
                    btnCtrlKeyboard.IsEnabled = false;
                    btnCtrlBreath.IsEnabled = true;

                    Rack.UserSettings.MyInstrumentControlMode = _MyInstrumentControlModes.Keyboard;
                    Rack.DMIBox.ResetModulationAndPressure();
                }
            }           
        }
        private void btnCtrlBreath_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                if (!btnBreathOn)
                {
                    btnBreathOn = true;
                    btnKeyboardOn = false;
                    btnCtrlBreath.IsEnabled = false;
                    btnCtrlKeyboard.IsEnabled = true;

                    Rack.UserSettings.MyInstrumentControlMode = _MyInstrumentControlModes.Breath;
                    Rack.DMIBox.ResetModulationAndPressure();
                }
            }

        }

        private void btnBreathPortMinus_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                SensorPort--;
                UpdateSensorConnection();
            }
        }

        private void btnBreathPortPlus_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                SensorPort++;
                UpdateSensorConnection();
            }
        }

        private void UpdateSensorConnection()
        {
            txtBreathPort.Text = "COM" + SensorPort.ToString();

            if (Rack.DMIBox.SensorReader.Connect(SensorPort))
            {
                txtBreathPort.Foreground = ActiveBrush;
            }
            else
            {
                txtBreathPort.Foreground = WarningBrush;
            }
        }

        private void btnMidiPortMinus_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                Rack.UserSettings.MIDIPort--;
                Rack.DMIBox.MidiModule.OutDevice = Rack.UserSettings.MIDIPort;

                // Graphic changes
                txtMidiPort.Text = "MP" + Rack.DMIBox.MidiModule.OutDevice.ToString();
                CheckMidiPort();
            }
        }

        private void btnMidiPortPlus_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                Rack.UserSettings.MIDIPort++;
                Rack.DMIBox.MidiModule.OutDevice = Rack.UserSettings.MIDIPort;

                // graphic changes
                txtMidiPort.Text = "MP" + Rack.DMIBox.MidiModule.OutDevice.ToString();
                CheckMidiPort();
            }
        }
        private void CheckMidiPort()
        {
            if (Rack.DMIBox.MidiModule.IsMidiOk())
            {
                txtMidiPort.Foreground = ActiveBrush;
            }
            else
            {
                txtMidiPort.Foreground = WarningBrush;
            }
        }

        private void lstScaleChanger_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (myInstrumentStarted)
            {
                Rack.UserSettings.ScaleName = (e.AddedItems[0] as ComboBoxItem).Content as string;
                Rack.DMIBox.MyInstrumentSurface.DrawOnCanvas();
            }
        }

        private void lstCodeChanger_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (myInstrumentStarted)
            {
                Rack.UserSettings.ScaleCode = (e.AddedItems[0] as ComboBoxItem).Content as string;
                Rack.DMIBox.MyInstrumentSurface.DrawOnCanvas();
            }
        }

        private void lstOctaveChanger_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (myInstrumentStarted)
            {
                Rack.UserSettings.Octave = (e.AddedItems[0] as ComboBoxItem).Content as string;
                Rack.DMIBox.MyInstrumentSurface.DrawOnCanvas();
            }
        }

        // Setting vertical distance between keys in keyboards
        private void sldVerticalDistance_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                Rack.UserSettings.KeyVerticaDistance = sldVerticalDistance.Value;
                Rack.DMIBox.MyInstrumentSurface.DrawOnCanvas();
            }
        }

        // Setting horizontal distance between keyboards
        private void sldHorizontalDistance_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                Rack.UserSettings.KeyHorizontalDistance = sldHorizontalDistance.Value;
                Rack.DMIBox.MyInstrumentSurface.SetHorizontalDistance(sldHorizontalDistance.Value);
            }
        }

        private void btnSlidePlay_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                if (!btnSlidePlayOn)
                {
                    btnSlidePlayOn = true;

                    btnSlidePlay.Background = ActiveBrush;
                    Rack.UserSettings.SlidePlayMode = _SlidePlayModes.On;
                    MyInstrumentButtons.ResetSlidePlay();
                }
                else
                {
                    btnSlidePlayOn = false;

                    btnSlidePlay.Background = buttonBackground;
                    Rack.UserSettings.SlidePlayMode = _SlidePlayModes.Off;

                }
            }
        }

        private void btnSharpNotes_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                if (!btnSharpNotesOn)
                {
                    btnSharpNotesOn = true;

                    btnSharpNotes.Background = ActiveBrush;
                    Rack.UserSettings.SharpNotesMode = _SharpNotesModes.On;
                    Rack.DMIBox.MyInstrumentSurface.DrawOnCanvas();
                }
                else
                {
                    btnSharpNotesOn = false;

                    btnSharpNotes.Background = buttonBackground;
                    Rack.UserSettings.SharpNotesMode = _SharpNotesModes.Off;
                    Rack.DMIBox.MyInstrumentSurface.DrawOnCanvas();

                }
            }
        }

        #endregion Instrument Settings       

        #endregion Instrument (Row1)       

        private void btnMetrnomeMinus_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                Rack.UserSettings.BPMmetronome--;
                metronomeTimer.Interval = Convert.ToInt32((1.0 / (Rack.UserSettings.BPMmetronome / 60.0)) * 1000);
                CheckMetronome();
            }           
        }

        private void btnMetrnomePlus_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                Rack.UserSettings.BPMmetronome++;
                metronomeTimer.Interval = Convert.ToInt32((1.0 / (Rack.UserSettings.BPMmetronome / 60.0)) * 1000);
                CheckMetronome();
            }            
        }
    }
        
}
