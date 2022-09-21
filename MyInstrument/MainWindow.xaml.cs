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

        public MainWindow()
        {
            InitializeComponent();

            MyInstrumentSetup myInstrumentSetup = new MyInstrumentSetup(this);
            myInstrumentSetup.Setup();

            surface = new MyInstrumentSurface(canvasMyInstrument);
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
