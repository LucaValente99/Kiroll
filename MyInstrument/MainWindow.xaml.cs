using MyInstrument.MIDI;
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

namespace MyInstrument
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Tentativo_1 t1 = new Tentativo_1();

        private bool myInstrumentStarted = false;
        private bool myInstrumentSettingsOpened = false;
        private bool musicSheetSettingsOpened = false;
        private bool btnKeyboardOn = false;
        private bool btnFaceOn = false;

        //Icons
        BitmapImage startIcon = new BitmapImage(
                    new Uri(Environment.CurrentDirectory + @"\..\..\..\Images\Icons\Start.png"));

        BitmapImage pauseIcon = new BitmapImage(
                    new Uri(Environment.CurrentDirectory + @"\..\..\..\Images\Icons\Pause.png"));

        BitmapImage settingsIcon = new BitmapImage(
                    new Uri(Environment.CurrentDirectory + @"\..\..\..\Images\Icons\Settings.png"));

        BitmapImage closeSettingsIcon = new BitmapImage(
                    new Uri(Environment.CurrentDirectory + @"\..\..\..\Images\Icons\CloseSettings_1.png"));

        public MainWindow()
        {
            InitializeComponent();
        }

        #region TopBar (Row0)

        #region Exit and Start Buttons
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!myInstrumentStarted)
            {
                myInstrumentStarted = true;

                temporaryNoteButtons.Visibility = Visibility.Visible;
                btnStartImage.Source = pauseIcon;
                btnStart.Background = new SolidColorBrush(Colors.White);
                btnStartLabel.Content = "Running...";

                if (myInstrumentSettingsOpened)
                {
                    closeInstrumentSettings();
                }

                if (musicSheetSettingsOpened)
                {
                    closeMusicSheetSettings();
                }
            }
            else
            {
                stopInstrument();
            }
        }

        #endregion Exit and Start Buttons

        #region Settings Buttons
        private void btnInstrumentSettings_Click(object sender, RoutedEventArgs e)
        {
            if (!myInstrumentSettingsOpened)
            {
                stopInstrument();

                myInstrumentSettingsOpened = true;
                WindowInstrumentSettings.Visibility = Visibility.Visible;
                btnInstrumentSettingImage.Source = closeSettingsIcon;
                btnInstrumentSettings.Background = new SolidColorBrush(Colors.White);
                btnInstrumentSettingLabel.Content = "Close Settings";         
            }
            else
            {
                closeInstrumentSettings();
            }
        }

        private void btnMusicSheetSettings_Click(object sender, RoutedEventArgs e)
        {
            if (!musicSheetSettingsOpened)
            {
                stopInstrument();

                musicSheetSettingsOpened = true;
                WindowMusicSheetSettings.Visibility = Visibility.Visible;
                btnMusicSheetSettingsImage.Source = closeSettingsIcon;
                btnMusicSheetSettings.Background = new SolidColorBrush(Colors.White);
                btnMusicSheetSettingsLabel.Content = "Close Settings";

            }
            else
            {
                closeMusicSheetSettings();
            }
        }

        private void closeInstrumentSettings()
        {
            myInstrumentSettingsOpened = false;
            WindowInstrumentSettings.Visibility = Visibility.Hidden;
            btnInstrumentSettingImage.Source = settingsIcon;
            btnInstrumentSettings.Background = new SolidColorBrush(Colors.Transparent);
            btnInstrumentSettingLabel.Content = "Instrument Settings";
        }

        private void closeMusicSheetSettings()
        {
            musicSheetSettingsOpened = false;
            WindowMusicSheetSettings.Visibility = Visibility.Hidden;
            btnMusicSheetSettingsImage.Source = settingsIcon;
            btnMusicSheetSettings.Background = new SolidColorBrush(Colors.Transparent);
            btnMusicSheetSettingsLabel.Content = "Music Sheet Settings";
        }

        private void stopInstrument()
        {
            myInstrumentStarted = false;
            temporaryNoteButtons.Visibility = Visibility.Hidden;
            btnStartImage.Source = startIcon;
            btnStart.Background = new SolidColorBrush(Colors.Transparent);
            btnStartLabel.Content = "Start";
        }

        #endregion Settings Buttons

        #endregion TopBar (Row0)

        #region Instrument (Row1)

        private void btnCtrlKeyboard_Click(object sender, RoutedEventArgs e)
        {
            if (!btnKeyboardOn)
            {
                btnKeyboardOn = true;
                btnFaceOn = false;
                btnCtrlKeyboard.IsEnabled = false;
                btnCtrlFace.IsEnabled = true;  
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
            }

        }

        #endregion Instrument (Row1)

        #region temporary
        private void Button_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {             
                t1.playNote(60, 80);
                txtPitch.Text = "60";
                txtNoteName.Text = "C4";
                txtVelocityMouth.Text = "80";
            }
        }

        private void Button_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                t1.stopNote(60);
                txtPitch.Text = "_";
                txtNoteName.Text = "_";
                txtVelocityMouth.Text = "_";
            }
        }

        #endregion temporary


    }

}
