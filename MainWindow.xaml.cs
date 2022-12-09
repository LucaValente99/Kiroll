using Kiroll.DMIbox;
using Kiroll.Surface;
using NeeqDMIs.ErrorLogging;
using System;
using System.Collections.Generic;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Timer = System.Windows.Forms.Timer;

namespace Kiroll
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Bool variables for checking activation and deactivation of buttons
        #region BoolVariables

        private bool KirollStarted = false;
        
        //If settings are opened the instrument will not play and keyboard opacity will be set to 0.6;
        private bool kirollSettingsOpened = false;
        public bool KirollSettingsOpened { get => kirollSettingsOpened; }
        private bool btnKeyboardOn = false;
        private bool btnBreathOn = false;
        private bool btnSlidePlayOn = false;
        private bool btnSharpNotesOn = false;
        private bool btnBlinkOn = false;
        private bool btnKeyNameOn = true;
        private bool btnEyeOn = false;
        private bool playMetronome = false;

        #endregion        

        // Variables to control blinkKeyboard behave
        #region BlinkKeyboard_variables
        StackPanel blinkKeyboard;
        bool flag = false;
        bool letBlink = false;
        public bool LetBlink { get => letBlink; set => letBlink = value; }
        #endregion

        // Sensors params and behaviors
        #region SensorParams&behaviors
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

        //Used to track doubleClose eyes behave
        private bool click = false;
        public bool Click
        {

            get => click;
            set
            {
                if (value != click)
                {
                    click = value;
                }
            }
        }

        #endregion

        // Dictionaries used to select Scale, Octave and Code
        #region Scale-Octave-Code

        private int scaleIndex = 0;
        private int oldScaleIndex = 0;
        public int ScaleIndex { get => scaleIndex; set => scaleIndex = value; }

        private Dictionary<int, string> comboScale = new Dictionary<int, string>()
        {
            { 0, "C" }, { 1, "C#" }, { 2, "D" }, { 3, "D#" }, { 4, "E" }, { 5, "F" }, { 6, "F#" }, { 7, "G" }, { 8, "G#" }, { 9, "A" }, { 10, "A#" }, { 11, "B"}
        };
        public Dictionary<int, string> ComboScale { get => comboScale; }

        private int codeIndex = 0;
        private int oldCodeIndex = 0;
        public int CodeIndex { get => codeIndex; set => codeIndex = value; }

        private Dictionary<int, string> comboCode = new Dictionary<int, string>()
        {
            { 0, "maj" }, { 1, "min" }, { 2, "min_arm" }, { 3, "min_mel" }
        };
        public Dictionary<int, string> ComboCode { get => comboCode; }

        private int octaveIndex = 2;
        private int oldOctaveIndex = 2;
        public int OctaveIndex { get => octaveIndex; set => octaveIndex = value; }

        private Dictionary<int, string> comboOctave = new Dictionary<int, string>()
        {
            { 0, "2" }, { 1, "3" }, { 2, "4" }, { 3, "5" }, { 4, "6" }
        };
        public Dictionary<int, string> ComboOctave { get => comboOctave; }

        #endregion

        // Dictionaries to manage Blink and Blow Behaviors
        #region BlinkAndBlowBehaviors

        //Blink Behaviors
        private int blinkIndex = 0;
        private Dictionary<int, string> blinkBehaviors = new Dictionary<int, string>()
        {
            { 0, "Key" }, { 1, "Octave" }, { 2, "Scale" }
        };

        //Blow Behaviors
        private int blowIndex = 0;
        private Dictionary<int, string> blowBehaviors = new Dictionary<int, string>()
        {
            { 0, "Dynamic" }, { 1, "Static" }
        };
        #endregion

        //Colors and backgrounds are used to highlight activation or deactivation of features clicking on the relevant buttons
        #region ColorsAndBackgrounds

        private readonly SolidColorBrush ActiveBrush = new SolidColorBrush(Colors.LightYellow);
        private readonly SolidColorBrush WarningBrush = new SolidColorBrush(Colors.DarkRed);
        private readonly SolidColorBrush DisableBrush = new SolidColorBrush(Colors.Transparent);
        // SelectionBrush is used just when eyetracker control is ON, because the mouse could not be seen anymore.
        private readonly SolidColorBrush SelectionBrush = new SolidColorBrush(Colors.Yellow); 

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

        #endregion

        // Timers
        private Timer updater;
        private Timer metronomeTimer;

        // Metronome sound
        private MediaPlayer metronome = new MediaPlayer();     

        public MainWindow()
        {
            InitializeComponent();

            TraceAdder.AddTrace(); // Used to log errors

            metronomeTimer = new Timer();
            updater = new Timer();
            // " (1000 * 60) / (Rack.UserSettings.BPMmetronome" is used to convert BPM into ms (milliseconds).
            metronomeTimer.Interval = Convert.ToInt32(1000 * 60 / (Rack.UserSettings.BPMmetronome));
            updater.Interval = 10;

            metronomeTimer.Tick += Metronome;
            updater.Tick += Update;

            metronomeTimer.Start();
           
        }

        #region Timers Behaviors
        private void Metronome(object sender, EventArgs e)
        {
            if (playMetronome)
            {
                metronome.Open(new Uri(Environment.CurrentDirectory + @"\..\..\Audio\Metronome.wav"));
                metronome.Play();
            }
            else {                
                metronome.Stop();
                metronome.Close();
            }
        }
        private void Update(object sender, EventArgs e)
        {
            //Graphic changes on Note Visualizer

            #region NoteVisualizer_graphicChanges

            txtPitch.Text = Rack.UserSettings.NotePitch;
            txtNoteName.Text = Rack.UserSettings.NoteName;

            if (Rack.UserSettings.KirollControlMode == _KirollControlModes.Keyboard)
            {
                txtVelocityMouth.Text = Rack.UserSettings.NoteVelocity;
            }
            else
            {
                if (txtPitch.Text != "_")
                {
                    txtVelocityMouth.Text = Rack.UserSettings.NotePressure; // Intensity of user blow
                }
                else
                {
                    // If the user is not blowing "_" will be showned
                    txtVelocityMouth.Text = "_";
                }
            }

            #endregion

            // When Scale, Code or Octave will change 'DrawOnCanvas' will be called.
            // This is necessary cause is not possible to access canvas when using blink behaviors.
            // Canvas is in fact managed by a different thread that avoid the access to.

            #region ComboScale-Octave-Code
            // oldScaleIndex, oldOctaveIndex and oldCodeIndex are used to track when scaleIndex change
            if (oldScaleIndex != scaleIndex)
            {
                oldScaleIndex = scaleIndex;
                txtScale.Text = comboScale[scaleIndex];
                Rack.UserSettings.ScaleName = txtScale.Text;
                Rack.DMIBox.KirollSurface.DrawOnCanvas();
            }

            if (oldOctaveIndex != octaveIndex)
            {
                oldOctaveIndex = octaveIndex;
                txtOctave.Text = comboOctave[octaveIndex];
                Rack.UserSettings.Octave = txtOctave.Text;
                Rack.DMIBox.KirollSurface.DrawOnCanvas();
            }

            if (oldCodeIndex != codeIndex)
            {
                oldCodeIndex = codeIndex;
                txtCode.Text = comboCode[codeIndex];
                Rack.UserSettings.ScaleCode = txtCode.Text;
                Rack.DMIBox.KirollSurface.DrawOnCanvas();
            }
            #endregion

            // Behaviors when EyeCtrl is On

            #region EyeCtrlON
            if (Rack.UserSettings.EyeCtrl == _EyeCtrl.On)
            {
                if (Rack.DMIBox.LastGazedButton != null)
                {
                    // If doubleClose eyes behave (TBactivateButton) or blow behave (TBactivateButton) 
                    // happens click became True so a button will be clicked, the last gazed

                    if (Click)
                    {
                        Rack.DMIBox.LastGazedButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

                        //Background changes of gazed buttons
                        if (Rack.DMIBox.LastGazedButton.Background == SelectionBrush)
                        {
                            oldBackGround = buttonBackground;
                        }
                        else
                        {
                            oldBackGround = Rack.DMIBox.LastGazedButton.Background;
                        }

                        Click = false;
                    }
                }

                // BlinkBehaviors will work when EyeCtrl is ON
                if (btnBlinkOn)
                {
                    txtBlink.Foreground = new SolidColorBrush(Colors.White); ;
                }
                else
                {
                    txtBlink.Foreground = WarningBrush;
                }

            }
            else
            {
                btnCtrlEye.Background = buttonBackground;
                txtBlink.Foreground = WarningBrush;
            }

            #endregion

            // When user plays a wrong keyboard, the right one will blink

            #region BlinkKeyboard_behave

            string lastKeyboardPlayed = Rack.DMIBox.KirollSurface.LastKeyboardPlayed;

            // Selecting the keyboard that has to blink
            if (lastKeyboardPlayed == "")
            {
                blinkKeyboard = KirollKeyboard.GetKeyboard("_0");
            }
            else
            {
                blinkKeyboard = KirollKeyboard.GetKeyboard("_" + ((Convert.ToInt32(lastKeyboardPlayed) + 1) % 16).ToString());
            }

            // If the user is on the right keyboard, that one should stop blinking, so Opacity will be restored
            if (letBlink)
                {
                    if (blinkKeyboard.Opacity > 0.5 && !flag)
                    {
                        blinkKeyboard.Opacity -= 0.02;
                    }
                    else if (blinkKeyboard.Opacity <= 0.5 && !flag)
                    {
                        flag = true;
                    }
                    else if (flag && blinkKeyboard.Opacity < 1)
                    {
                        blinkKeyboard.Opacity += 0.02;
                    }
                    else if (flag && blinkKeyboard.Opacity >= 1)
                    {
                        flag = false;
                    }
                }
                else
                {
                    blinkKeyboard.Opacity = 1;
                }
            #endregion
        }

        #endregion

        // Each setting button (Start & Stop excluded) has this behave, if gazed, the button will be selected waiting to be clicked

        private dynamic oldBackGround; // used to select correct button background when it is gazed, selected or unselected
        public dynamic OldBackGround { get => oldBackGround; set => oldBackGround = value; }
        private void eyeGazeHandler(object sender, MouseEventArgs e)
        {
            if (Rack.UserSettings.EyeCtrl == _EyeCtrl.On)
            {
                // Giving to the last gazed button the old background that he had before being gazed
                if (Rack.DMIBox.LastGazedButton != null)
                {
                    Rack.DMIBox.LastGazedButton.Background = oldBackGround;
                }
                
                Rack.DMIBox.LastGazedButton = (Button)sender;

                // Memorizing the background of gazed button before changing it
                oldBackGround = Rack.DMIBox.LastGazedButton.Background;

                Rack.DMIBox.LastGazedButton.Background = SelectionBrush;

                // This avoid playing a key while clicking on button              
                if (Rack.DMIBox.SelectedNote != NeeqDMIs.Music.MidiNotes.NaN)
                {                   
                    Rack.DMIBox.CheckedNote = null; //checkedNote != null is a condition to play a key
                }              

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
            if (!KirollStarted)
            {
                KirollSetup KirollSetup = new KirollSetup(this);
                KirollSetup.Setup();              

                // Graphic changes
                btnStartImage.Source = pauseIcon;
                btnStart.Background = ActiveBrush;
                btnStartLabel.Content = "Running...";

                if (Rack.UserSettings.KeyName == _KeyName.On)
                {
                    btnCtrlKeyName.Background = ActiveBrush;
                }         

                // Enabling various settings
                txtScale.Text = Rack.UserSettings.ScaleName;
                txtCode.Text = Rack.UserSettings.ScaleCode;
                txtOctave.Text = Rack.UserSettings.Octave;
                txtBlink.Text = blinkBehaviors[blinkIndex];
                txtBSwitc.Text = blowBehaviors[blowIndex];
                txtVerticalDistance.Text = Rack.UserSettings.KeyVerticaDistance.ToString();
                txtHorizontalDistance.Text = Rack.UserSettings.KeyHorizontalDistance.ToString();

                // MIDI
                CheckMidiPort();

                // Breath Sensor
                CheckSensorPort();

                //Metronome                
                CheckMetronome();
               
                updater.Start();               

                KirollStarted = true;   
            }
            else
            {
                KirollStarted = false; // disabling all settings

                // Graphic changes             
                btnStartImage.Source = startIcon;
                btnStart.Background = DisableBrush;
                btnStartLabel.Content = "Start";
                txtMidiPort.Text = "";
                txtBreathPort.Text = "";
                txtMetronome.Text = "";
                btnMetronome.Background = buttonBackground;
                btnCtrlKeyName.Background = buttonBackground;

                // Disabling various settings, metronome and blinkKeyboards behave
                txtScale.Text = "";
                txtCode.Text = "";
                txtOctave.Text = "";
                txtVerticalDistance.Text = "";
                txtHorizontalDistance.Text = "";
                txtBlink.Text = "";
                txtBSwitc.Text = "";
                playMetronome = false;
                letBlink = false;

                // Resetting surface
                Rack.DMIBox.KirollSurface.ClearSurface();

                updater.Stop();
            }
        }

        private void btnInstrumentSettings_Click(object sender, RoutedEventArgs e)
        {
            if (!KirollSettingsOpened)
            {
                kirollSettingsOpened = true;

                // Graphic changes
                WindowInstrumentSettings_1.Visibility = Visibility.Visible;
                WindowInstrumentSettings_2.Visibility = Visibility.Visible;
                btnInstrumentSettingImage.Source = closeSettingsIcon;
                btnInstrumentSettings.Background = ActiveBrush;
                btnInstrumentSettingLabel.Content = "Close Settings";

                // This changes the opacity of keybaords when settings are opened
                if (KirollStarted)
                {
                    KirollKeyboard.UpdateOpacity();
                }               
            }
            else
            {
                kirollSettingsOpened = false;

                // Graphic changes
                WindowInstrumentSettings_1.Visibility = Visibility.Hidden;
                WindowInstrumentSettings_2.Visibility = Visibility.Hidden;
                btnInstrumentSettingImage.Source = settingsIcon;
                btnInstrumentSettings.Background = DisableBrush;
                btnInstrumentSettingLabel.Content = "Instrument Settings";
                
                if (KirollStarted)
                {
                    KirollKeyboard.UpdateOpacity();
                }
            }
        }

        private void btnCtrlEye_Click(object sender, RoutedEventArgs e)
        {
            if (KirollStarted)
            {
                if (!btnEyeOn)
                {
                    btnEyeOn = true;
                    Rack.UserSettings.EyeCtrl = _EyeCtrl.On;
                    Rack.DMIBox.TobiiModule.MouseEmulator.EyetrackerToMouse = true;
                    Rack.DMIBox.TobiiModule.MouseEmulator.CursorVisible = false;

                    btnCtrlEye.Background = ActiveBrush;
                }
                else
                {
                    btnEyeOn = false;
                    Rack.UserSettings.EyeCtrl = _EyeCtrl.Off;
                    Rack.DMIBox.TobiiModule.MouseEmulator.EyetrackerToMouse = false;
                    Rack.DMIBox.TobiiModule.MouseEmulator.CursorVisible = true;

                    btnCtrlEye.Background = DisableBrush;
                }
            }
        }

        private void btnCtrlKeyName_Click(object sender, RoutedEventArgs e)
        {
            if (KirollStarted)
            {
                if (!btnKeyNameOn)
                {
                    btnKeyNameOn = true;
                    Rack.UserSettings.KeyName = _KeyName.On;
                    btnCtrlKeyName.Background = ActiveBrush;
                    Rack.DMIBox.KirollSurface.DrawOnCanvas();
                }
                else
                {
                    btnKeyNameOn = false;
                    Rack.UserSettings.KeyName = _KeyName.Off;
                    btnCtrlKeyName.Background = buttonBackground;
                    Rack.DMIBox.KirollSurface.DrawOnCanvas();
                }
            }
        }

        #endregion Start, Exit and Setting buttons

        #endregion TopBar (Row0)

        #region Instrument (Row1)

        #region Instrument Settings 1

        private void btnCtrlKeyboard_Click(object sender, RoutedEventArgs e)
        {
            if (KirollStarted)
            {
                if (!btnKeyboardOn)
                {
                    btnKeyboardOn = true;
                    btnBreathOn = false;
                    btnCtrlKeyboard.IsEnabled = false;
                    btnCtrlBreath.IsEnabled = true;
                    btnCtrlKeyboard.Background = ActiveBrush;
                    btnCtrlBreath.Background = buttonBackground;
                    txtBSwitc.Foreground = WarningBrush;

                    Rack.UserSettings.KirollControlMode = _KirollControlModes.Keyboard;
                    Rack.DMIBox.ResetModulationAndPressure();
                }
            }           
        }

        private void btnCtrlBreath_Click(object sender, RoutedEventArgs e)
        {
            if (KirollStarted)
            {
                if (!btnBreathOn)
                {
                    btnBreathOn = true;
                    btnKeyboardOn = false;
                    btnCtrlBreath.IsEnabled = false;
                    btnCtrlKeyboard.IsEnabled = true;
                    btnCtrlBreath.Background = ActiveBrush;
                    btnCtrlKeyboard.Background = buttonBackground;
                    txtBSwitc.Foreground = new SolidColorBrush(Colors.White);

                    Rack.UserSettings.KirollControlMode = _KirollControlModes.Breath;
                    Rack.DMIBox.ResetModulationAndPressure();
                }
            }

        }

        private void btnScaleMinus_Click(object sender, RoutedEventArgs e)
        {
            if (KirollStarted)
            {
                if (scaleIndex > 0)               
                    scaleIndex--;                    
                
            }
        }

        private void btnScalePlus_Click(object sender, RoutedEventArgs e)
        {
            if (KirollStarted)
            {
                if (scaleIndex < 11)                
                    scaleIndex++;                                
            }
        }

        public void btnCodeMinus_Click(object sender, RoutedEventArgs e)
        {
            if (KirollStarted)
            {
                if (!btnSharpNotesOn)
                {
                    if (codeIndex > 0)                    
                        codeIndex--;                  
                }
            }
        }

        private void btnCodePlus_Click(object sender, RoutedEventArgs e)
        {
            if (KirollStarted)
            {
                if (!btnSharpNotesOn)
                {
                    if (codeIndex < 3)
                        codeIndex++;
                }
            }
        }

        private void btnOctaveMinus_Click(object sender, RoutedEventArgs e)
        {
            if (KirollStarted)
            {
                if (octaveIndex > 0)                
                    octaveIndex--;                                
            }
        }

        private void btnOctavePlus_Click(object sender, RoutedEventArgs e)
        {
            if (KirollStarted)
            {
                if (octaveIndex < 4)              
                    octaveIndex++;               
            }
        }

        private void btnMidiPortMinus_Click(object sender, RoutedEventArgs e)
        {
            if (KirollStarted)
            {
                Rack.UserSettings.MIDIPort--;
                Rack.DMIBox.MidiModule.OutDevice = Rack.UserSettings.MIDIPort;

                // Graphic changes               
                CheckMidiPort();
            }
        }

        private void btnMidiPortPlus_Click(object sender, RoutedEventArgs e)
        {
            if (KirollStarted)
            {
                Rack.UserSettings.MIDIPort++;
                Rack.DMIBox.MidiModule.OutDevice = Rack.UserSettings.MIDIPort;

                // Graphic changes
                CheckMidiPort();
            }
        }

        // this just changes MIDIport selector graphic
        private void CheckMidiPort()
        {
            txtMidiPort.Text = "MP" + Rack.DMIBox.MidiModule.OutDevice.ToString();
            if (Rack.DMIBox.MidiModule.IsMidiOk())
            {
                txtMidiPort.Foreground = ActiveBrush;
            }
            else
            {
                txtMidiPort.Foreground = WarningBrush;
            }
        }

        private void btnBreathPortMinus_Click(object sender, RoutedEventArgs e)
        {
            if (KirollStarted)
            {
                SensorPort--;
                //Graphic changes
                CheckSensorPort();
            }
        }

        private void btnBreathPortPlus_Click(object sender, RoutedEventArgs e)
        {
            if (KirollStarted)
            {
                SensorPort++;
                //Graphic changes
                CheckSensorPort();
            }
        }

        // this just changes SensorPort selector graphic
        private void CheckSensorPort()
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

        #endregion Instrument Settings 1

        #region Instrument Settings 2      

        private void btnSharpNotes_Click(object sender, RoutedEventArgs e)
        {
            if (KirollStarted)
            {
                if (!btnSharpNotesOn)
                {
                    btnSharpNotesOn = true;

                    btnSharpNotes.Background = ActiveBrush;
                    Rack.UserSettings.SharpNotesMode = _SharpNotesModes.On;
                    Rack.DMIBox.KirollSurface.DrawOnCanvas();
                }
                else
                {
                    btnSharpNotesOn = false;

                    btnSharpNotes.Background = buttonBackground;
                    Rack.UserSettings.SharpNotesMode = _SharpNotesModes.Off;
                    Rack.DMIBox.KirollSurface.DrawOnCanvas();

                }
            }
        }

        private void btnSlidePlay_Click(object sender, RoutedEventArgs e)
        {
            if (KirollStarted)
            {
                if (!btnSlidePlayOn)
                {
                    btnSlidePlayOn = true;

                    btnSlidePlay.Background = ActiveBrush;
                    Rack.UserSettings.SlidePlayMode = _SlidePlayModes.On;
                    //Resetting the oldNote because at start there is no an old note.
                    //This is true until the first note is played.
                    Rack.DMIBox.OldMidiNote = NeeqDMIs.Music.MidiNotes.NaN;
                }
                else
                {
                    btnSlidePlayOn = false;

                    btnSlidePlay.Background = buttonBackground;
                    Rack.UserSettings.SlidePlayMode = _SlidePlayModes.Off;

                }
            }
        }

        private void btnBlink_Click(object sender, RoutedEventArgs e)
        {
            if (KirollStarted)
            {
                if (!btnBlinkOn)
                {
                    btnBlinkOn = true;
                    btnBlink.Background = ActiveBrush;

                    switch (txtBlink.Text)
                    {
                        case "Key":
                            Rack.UserSettings.BlinkModes = _BlinkModes.Scale;
                            break;
                        case "Octave":
                            Rack.UserSettings.BlinkModes = _BlinkModes.Octave;
                            break;
                        case "Scale":
                            Rack.UserSettings.BlinkModes = _BlinkModes.Code;
                            break;
                    }
                }
                else
                {
                    btnBlinkOn = false;
                    btnBlink.Background = buttonBackground;
                    Rack.UserSettings.BlinkModes = _BlinkModes.Off;
                }
            }
        }

        private void btnBlinkMinus_Click(object sender, RoutedEventArgs e)
        {
            if (KirollStarted)
            {
                if (blinkIndex > 0)
                {
                    blinkIndex--;
                    txtBlink.Text = blinkBehaviors[blinkIndex];
                    switch (blinkBehaviors[blinkIndex])
                    {
                        case "Key":
                            Rack.UserSettings.BlinkModes = _BlinkModes.Scale;
                            break;
                        case "Octave":
                            Rack.UserSettings.BlinkModes = _BlinkModes.Octave;
                            break;
                        case "Scale":
                            Rack.UserSettings.BlinkModes = _BlinkModes.Code;
                            break;
                    }
                }
            }
        }

        private void btnBlinkPlus_Click(object sender, RoutedEventArgs e)
        {
            if (KirollStarted)
            {
                if (blinkIndex < 2)
                {
                    blinkIndex++;
                    txtBlink.Text = blinkBehaviors[blinkIndex];
                    switch (blinkBehaviors[blinkIndex])
                    {
                        case "Key":
                            Rack.UserSettings.BlinkModes = _BlinkModes.Scale;
                            break;
                        case "Octave":
                            Rack.UserSettings.BlinkModes = _BlinkModes.Octave;
                            break;
                        case "Scale":
                            Rack.UserSettings.BlinkModes = _BlinkModes.Code;
                            break;
                    }
                }
            }
        }

        private void btnMetronome_Click(object sender, RoutedEventArgs e)
        {
            if (KirollStarted)
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

        private void btnMetrnomeMinus_Click(object sender, RoutedEventArgs e)
        {
            if (KirollStarted)
            {
                Rack.UserSettings.BPMmetronome--;
                metronomeTimer.Interval = Convert.ToInt32(1000 * 60 / (Rack.UserSettings.BPMmetronome)); ;
                CheckMetronome();
            }
        }

        private void btnMetrnomePlus_Click(object sender, RoutedEventArgs e)
        {
            if (KirollStarted)
            {
                Rack.UserSettings.BPMmetronome++;
                metronomeTimer.Interval = Convert.ToInt32(1000 * 60 / (Rack.UserSettings.BPMmetronome)); ;
                CheckMetronome();
            }
        }

        // this just changes metronome button's graphic
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

        private void btnBSwitchMinus_Click(object sender, RoutedEventArgs e)
        {
            if (KirollStarted)
            {
                if (blowIndex > 0)
                {
                    blowIndex--;
                    txtBSwitc.Text = blowBehaviors[blowIndex];
                    switch (blowBehaviors[blowIndex])
                    {
                        case "Dynamic":
                            Rack.UserSettings.BreathControlModes = _BreathControlModes.Dynamic;
                            break;
                        case "Static":
                            Rack.UserSettings.BreathControlModes = _BreathControlModes.Static;
                            break;
                    }
                }
            }
        }

        private void btnBSwitcPlus_Click(object sender, RoutedEventArgs e)
        {
            if (KirollStarted)
            {
                if (blowIndex < 1)
                {
                    blowIndex++;
                    txtBSwitc.Text = blowBehaviors[blowIndex];
                    switch (blowBehaviors[blowIndex])
                    {
                        case "Dynamic":
                            Rack.UserSettings.BreathControlModes = _BreathControlModes.Dynamic;
                            break;
                        case "Static":
                            Rack.UserSettings.BreathControlModes = _BreathControlModes.Static;
                            break;
                    }
                }
            }
        }

        // Setting vertical distance between keys in keyboards
        private void btnVerticalDistanceMinus_Click(object sender, RoutedEventArgs e)
        {
            if (KirollStarted)
            {
                if (Rack.UserSettings.KeyVerticaDistance >= 5)
                {
                    Rack.UserSettings.KeyVerticaDistance -= 5;
                    txtVerticalDistance.Text = Rack.UserSettings.KeyVerticaDistance.ToString();
                    Rack.DMIBox.KirollSurface.DrawOnCanvas();
                }
            }
        }

        private void btnVerticalDistancePlus_Click(object sender, RoutedEventArgs e)
        {
            if (KirollStarted)
            {
                if (Rack.UserSettings.KeyVerticaDistance <= 25)
                {
                    Rack.UserSettings.KeyVerticaDistance += 5;
                    txtVerticalDistance.Text = Rack.UserSettings.KeyVerticaDistance.ToString();
                    Rack.DMIBox.KirollSurface.DrawOnCanvas();
                }
            }
        }

        // Setting horizontal distance between keyboards
        private void btnHorizontalDistanceMinus_Click(object sender, RoutedEventArgs e)
        {
            if (KirollStarted)
            {
                if (Rack.UserSettings.KeyHorizontalDistance > 200)
                {
                    Rack.UserSettings.KeyHorizontalDistance -= 100;
                    txtHorizontalDistance.Text = Rack.UserSettings.KeyHorizontalDistance.ToString();
                    Rack.DMIBox.KirollSurface.DrawOnCanvas();
                }
            }
        }

        private void btnHorizontalDistancePlus_Click(object sender, RoutedEventArgs e)
        {
            if (KirollStarted)
            {
                if (Rack.UserSettings.KeyHorizontalDistance < 600)
                {
                    Rack.UserSettings.KeyHorizontalDistance += 100;
                    txtHorizontalDistance.Text = Rack.UserSettings.KeyHorizontalDistance.ToString();
                    Rack.DMIBox.KirollSurface.DrawOnCanvas();
                }
            }
        }

        #endregion Instrument Settings 2

        #endregion Instrument (Row1)                        

    }

}
