using MyInstrument.DMIbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MyInstrument.Surface;
using System.Windows.Threading;
using NeeqDMIs.Eyetracking.PointFilters;

namespace MyInstrument
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //interfaccia strumento
        private MyInstrumentSurface surface;

        //controlli booleani per attivazione/disattivazione bottoni
        private bool myInstrumentStarted = false;
        private bool myInstrumentSettingsOpened = false;
        private bool musicSheetSettingsOpened = false;
        private bool btnKeyboardOn = false;
        private bool btnFaceOn = false;
        private bool btnDisableWritingMode = false;
        private bool btnSlidePlayOn = false;

        // utilizzati per la selezione dell'indice dei combobox nelle fasi di Start and Stop
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
                    new Uri(Environment.CurrentDirectory + @"\..\..\..\Images\Icons\Start.png"));

        BitmapImage pauseIcon = new BitmapImage(
                    new Uri(Environment.CurrentDirectory + @"\..\..\..\Images\Icons\Pause.png"));

        BitmapImage settingsIcon = new BitmapImage(
                    new Uri(Environment.CurrentDirectory + @"\..\..\..\Images\Icons\Settings.png"));

        BitmapImage closeSettingsIcon = new BitmapImage(
                    new Uri(Environment.CurrentDirectory + @"\..\..\..\Images\Icons\CloseSettings_1.png"));

        ImageBrush buttonBackground = new ImageBrush(new BitmapImage(
                    new Uri(Environment.CurrentDirectory + @"\..\..\..\Images\Backgrounds\Buttons.jpeg")));

        private DispatcherTimer updater;
        public MainWindow()
        {
            InitializeComponent();

            MyInstrumentSetup myInstrumentSetup = new MyInstrumentSetup(this);
            myInstrumentSetup.Setup();

            updater = new DispatcherTimer();
            updater.Interval = TimeSpan.FromMilliseconds(10);
            updater.Tick += UpdateWindow;
            updater.Start();

            surface = new MyInstrumentSurface(canvasMyInstrument);
        }

        private void UpdateWindow(object? sender, EventArgs e)
        {
            txtPitch.Text = Rack.UserSettings.NotePitch;
            txtNoteName.Text = Rack.UserSettings.NoteName;
            txtVelocityMouth.Text = Rack.UserSettings.NoteVelocity;

        }

        #region TopBar (Row0)

        #region Start, Exit and Setting buttons

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!myInstrumentStarted)
            {

                btnStartImage.Source = pauseIcon;
                btnStart.Background = ActiveBrush;                
                btnStartLabel.Content = "Running...";

                surface.DrawOnCanvas();
            
                lstScaleChanger.IsEnabled = true;
                lstScaleChanger.SelectedIndex = comboScale[Rack.UserSettings.ScaleName];
                lstCodeChanger.IsEnabled = true;
                lstCodeChanger.SelectedIndex = comboCode[Rack.UserSettings.ScaleCode];
                lstOctaveChanger.IsEnabled = true;
                lstOctaveChanger.SelectedIndex = comboOctave[Rack.UserSettings.Octave];


                /* MIDI */
                txtMidiPort.Text = "MP" + Rack.DMIBox.MidiModule.OutDevice.ToString();
                CheckMidiPort();

                myInstrumentStarted = true;
            }
            else
            {
                myInstrumentStarted = false;
                btnStartImage.Source = startIcon;
                btnStart.Background = DisableBrush;
                btnStartLabel.Content = "Start";
                txtMidiPort.Text = "";

                lstScaleChanger.IsEnabled = false;
                lstScaleChanger.SelectedIndex = comboScale["_"];
                lstCodeChanger.IsEnabled = false;
                lstCodeChanger.SelectedIndex = comboCode["_"];
                lstOctaveChanger.IsEnabled = false;
                lstOctaveChanger.SelectedIndex = comboOctave["_"];

                surface.ClearSurface();
            }
        }

        private void btnInstrumentSettings_Click(object sender, RoutedEventArgs e)
        {
            if (!myInstrumentSettingsOpened)
            {
                myInstrumentSettingsOpened = true;
                WindowInstrumentSettings.Visibility = Visibility.Visible;
                btnInstrumentSettingImage.Source = closeSettingsIcon;
                btnInstrumentSettings.Background = ActiveBrush;
                btnInstrumentSettingLabel.Content = "Close Settings";
            }
            else
            {
                myInstrumentSettingsOpened = false;
                WindowInstrumentSettings.Visibility = Visibility.Hidden;
                btnInstrumentSettingImage.Source = settingsIcon;
                btnInstrumentSettings.Background = DisableBrush;
                btnInstrumentSettingLabel.Content = "Instrument Settings";
            }
        }

        private void btnMusicSheetSettings_Click(object sender, RoutedEventArgs e)
        {
            if (!musicSheetSettingsOpened)
            {
                musicSheetSettingsOpened = true;
                WindowMusicSheetSettings.Visibility = Visibility.Visible;
                btnMusicSheetSettingsImage.Source = closeSettingsIcon;
                btnMusicSheetSettings.Background = ActiveBrush;
                btnMusicSheetSettingsLabel.Content = "Close Settings";

            }
            else
            {
                musicSheetSettingsOpened = false;
                WindowMusicSheetSettings.Visibility = Visibility.Hidden;
                btnMusicSheetSettingsImage.Source = settingsIcon;
                btnMusicSheetSettings.Background = DisableBrush;
                btnMusicSheetSettingsLabel.Content = "Music Sheet Settings";
            }
        }

        #endregion Start, Exit and Setting buttons

        #endregion TopBar (Row0)

        #region Instrument (Row1)

        #region Instrument Settings

        private void btnCtrlKeyboard_Click(object sender, RoutedEventArgs e)
        {
            if (!btnKeyboardOn)
            {
                btnKeyboardOn = true;
                btnFaceOn = false;
                btnCtrlKeyboard.IsEnabled = false;
                btnCtrlFace.IsEnabled = true;

                Rack.UserSettings.MyInstrumentControlMode = _MyInstrumentControlModes.Keyboard;
                Rack.DMIBox.ResetModulationAndPressure();
            }

        }
        private void btnCtrlFace_Click(object sender, RoutedEventArgs e)
        {
            if (!btnFaceOn)
            {
                btnFaceOn = true;
                btnKeyboardOn = false;
                btnCtrlFace.IsEnabled = false;
                btnCtrlKeyboard.IsEnabled = true;

                Rack.UserSettings.MyInstrumentControlMode = _MyInstrumentControlModes.Face;
                Rack.DMIBox.ResetModulationAndPressure();
            }

        }

        private void btnMidiPortMinus_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                Rack.UserSettings.MIDIPort--;
                Rack.DMIBox.MidiModule.OutDevice = Rack.UserSettings.MIDIPort;
                //lblMIDIch.Text = "MP" + Rack.DMIBox.MidiModule.OutDevice.ToString();

                //CheckMidiPort();
                /* MIDI */
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
                //lblMIDIch.Text = "MP" + Rack.DMIBox.MidiModule.OutDevice.ToString();

                //CheckMidiPort();
                /* MIDI */
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
                surface.DrawOnCanvas();
            }
        }

        private void lstCodeChanger_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (myInstrumentStarted)
            {
                Rack.UserSettings.ScaleCode = (e.AddedItems[0] as ComboBoxItem).Content as string;
                surface.DrawOnCanvas();
            }
        }

        private void lstOctaveChanger_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (myInstrumentStarted)
            {
                Rack.UserSettings.Octave = (e.AddedItems[0] as ComboBoxItem).Content as string;
                surface.DrawOnCanvas();
            }
        }

        private void sldDistance_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                Rack.UserSettings.keyDistance = sldDistance.Value;
                surface.SetDistance(sldDistance.Value);
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
                    MyInstrumentButtons.resetSlidePlay();
                }
                else
                {
                    btnSlidePlayOn = false;
                    btnSlidePlay.Background = buttonBackground;
                    Rack.UserSettings.SlidePlayMode = _SlidePlayModes.Off;

                }
            }
        }

        #endregion Instrument Settings       

        #region Music Sheet Settings

        #endregion Music Sheet Settings

        #endregion Instrument (Row1)

        #region MusicSheet (Row2)

        private void btnDisable_Click(object sender, RoutedEventArgs e)
        {
            if (!btnDisableWritingMode)
            {
                btnDisableWritingMode = true;
                btnDisable.Background = ActiveBrush;
            }
            else
            {
                btnDisableWritingMode = false;
                btnDisable.Background = buttonBackground;

            }
        }

        #endregion MusicSheet (Row2)
        
    }
}
