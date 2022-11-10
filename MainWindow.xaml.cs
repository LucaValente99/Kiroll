using MyInstrument.DMIbox;
using MyInstrument.Surface;
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

namespace MyInstrument
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Bool variables for checking activation and deactivation of buttons
        #region BoolVariables

        private bool myInstrumentStarted = false;
        
        //If settings are opened the instrument will not play and keyboard opacity will be set to 0.6;
        private bool myInstrumentSettingsOpened = false;
        public bool MyInstrumentSettingsOpened { get => myInstrumentSettingsOpened; }
        private bool btnKeyboardOn = false;
        private bool btnBreathOn = false;
        private bool btnSlidePlayOn = false;
        private bool btnSharpNotesOn = false;
        private bool btnBlinkOn = false;
        private bool btnBlowkOn = false;
        private bool btnEyeOn = false;
        private bool playMetronome = false;

        //Used to track doubleClose eyes behave
        private bool click = false;
        public bool Click { 
            
            get => click; 
            set { 
                    if (value != click)
                    {
                        click = value;
                    }
            }
        }

        #endregion

        // Sensors params
        #region SensorParams
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
        public int BlinkIndex { get => blinkIndex; set => blinkIndex = value; }

        private Dictionary<int, string> blinkBehaviors = new Dictionary<int, string>()
        {
            { 0, "Scale" }, { 1, "Octave" }, { 2, "Code" }
        };

        //Blow Behaviors
        private int blowIndex = 0;
        public int BlowIndex { get => blowIndex; set => blowIndex = value; }

        private Dictionary<int, string> blowBehaviors = new Dictionary<int, string>()
        {
            { 0, "Dynamic" }, { 1, "Static" }
        };
        #endregion

        // Colors and backgrounds of buttons
        #region ColorsAndBackgrounds
        //Colors used to highlight activation or deactivation of features clicking on the relevant buttons
        private readonly SolidColorBrush ActiveBrush = new SolidColorBrush(Colors.LightYellow);
        private readonly SolidColorBrush WarningBrush = new SolidColorBrush(Colors.DarkRed);
        private readonly SolidColorBrush DisableBrush = new SolidColorBrush(Colors.Transparent);
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
        private SoundPlayer metronome = new SoundPlayer(@"D:\Universita\Tirocinio_tesi\1_Tirocinio\MyInstrument\Audio\Metronome.wav");

        public MainWindow()
        {
            InitializeComponent();

            metronomeTimer = new Timer();
            updater = new Timer();

            metronomeTimer.Interval = Convert.ToInt32((1.0 / (Rack.UserSettings.BPMmetronome / 60.0)) * 1000);
            updater.Interval = 10;

            metronomeTimer.Tick += Metronome;
            updater.Tick += Update;

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
        private void Update(object sender, EventArgs e)
        {
            //Graphic changes on Note Visualizer
            txtPitch.Text = Rack.UserSettings.NotePitch;
            txtNoteName.Text = Rack.UserSettings.NoteName;

            if (Rack.UserSettings.MyInstrumentControlMode == _MyInstrumentControlModes.Keyboard)
            {
                txtVelocityMouth.Text = Rack.UserSettings.NoteVelocity;
            }
            else
            {
                if (txtPitch.Text != "_")
                {
                    txtVelocityMouth.Text = Rack.UserSettings.NotePressure;
                }
                else
                {
                    txtVelocityMouth.Text = "_";
                }
            }

            // When Scale, Code or Octave will change 'DrawOnCanvas' will be called.
            // This is necessary cause is not possible to access canvas when using blink behaviors.
            // Canvas is in fact managed by a different thread that avoid the access to.

            if (oldScaleIndex != scaleIndex)
            {
                oldScaleIndex = scaleIndex;
                txtScale.Text = comboScale[scaleIndex];
                Rack.DMIBox.MyInstrumentSurface.DrawOnCanvas();
            }

            if (oldOctaveIndex != octaveIndex)
            {
                oldOctaveIndex = octaveIndex;
                txtOctave.Text = comboOctave[octaveIndex];
                Rack.DMIBox.MyInstrumentSurface.DrawOnCanvas();
            }

            if (oldCodeIndex != codeIndex)
            {
                oldCodeIndex = codeIndex;
                txtCode.Text = comboCode[codeIndex];
                Rack.DMIBox.MyInstrumentSurface.DrawOnCanvas();
            }

            // Behaviors when EyeCtrl is On
            if (Rack.UserSettings.EyeCtrl == _EyeCtrl.On)
            {
                // If doubleClose eyes behave happen click became True so a button will be clicked, the last gazed          
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

                //Background changes of gazed buttons spcific for btnCtrlEye
                if (Rack.DMIBox.LastGazedButton == btnCtrlEye)
                {
                    btnCtrlEye.Background = SelectionBrush;
                }
                else
                {
                    btnCtrlEye.Background = ActiveBrush;
                }
                

                if (btnBlinkOn) 
                {                  
                    txtBlink.Foreground = new SolidColorBrush(Colors.White);
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
        }

        // Each button (Start & Stop excluded) has this behave, if gazed, the button will be selected waiting to be clicked
        
        private dynamic oldBackGround; // used to select correct button background when it is gazed, selected or unselected
        private void eyeGazeHandler(object sender, MouseEventArgs e)
        {
            if (Rack.UserSettings.EyeCtrl == _EyeCtrl.On)
            {
                Rack.DMIBox.LastGazedButton.Background = oldBackGround;
                
                Rack.DMIBox.LastGazedButton = (Button)sender;

                oldBackGround = Rack.DMIBox.LastGazedButton.Background;

                Rack.DMIBox.LastGazedButton.Background = SelectionBrush;
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

                // Enabling Scale-Octave-Code & slider
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
                UpdateSensorConnection();

                //Metronome                
                CheckMetronome();
               
                updater.Start();               

                myInstrumentStarted = true;   
            }
            else
            {
                myInstrumentStarted = false;
                // Graphic changes             
                btnStartImage.Source = startIcon;
                btnStart.Background = DisableBrush;
                btnStartLabel.Content = "Start";
                txtMidiPort.Text = "";
                txtBreathPort.Text = "";
                txtMetronome.Text = "";
                btnMetronome.Background = buttonBackground;

                // Disabling Scale-Octave-Code, slider & Metronome, Blink & Blow Behaviors
                // lstScaleChanger.IsEnabled = false;
                // lstScaleChanger.SelectedIndex = comboScale["_"];
                txtScale.Text = "";
                txtCode.Text = "";
                txtOctave.Text = "";
                txtVerticalDistance.Text = "";
                txtHorizontalDistance.Text = "";
                txtBlink.Text = "";
                txtBSwitc.Text = "";
                playMetronome = false;  
                         
                // Resetting surface
                Rack.DMIBox.MyInstrumentSurface.ClearSurface();

                updater.Stop();
            }
        }

        private void btnInstrumentSettings_Click(object sender, RoutedEventArgs e)
        {
            if (!myInstrumentSettingsOpened)
            {
                myInstrumentSettingsOpened = true;

                // Graphic changes
                WindowInstrumentSettings_1.Visibility = Visibility.Visible;
                WindowInstrumentSettings_2.Visibility = Visibility.Visible;
                btnInstrumentSettingImage.Source = closeSettingsIcon;
                btnInstrumentSettings.Background = ActiveBrush;
                btnInstrumentSettingLabel.Content = "Close Settings";
                if (myInstrumentStarted)
                {
                    MyInstrumentKeyboard.UpdateOpacity();
                }               
            }
            else
            {
                myInstrumentSettingsOpened = false;
                // Graphic changes
                WindowInstrumentSettings_1.Visibility = Visibility.Hidden;
                WindowInstrumentSettings_2.Visibility = Visibility.Hidden;
                btnInstrumentSettingImage.Source = settingsIcon;
                btnInstrumentSettings.Background = DisableBrush;
                btnInstrumentSettingLabel.Content = "Instrument Settings";
                if (myInstrumentStarted)
                {
                    MyInstrumentKeyboard.UpdateOpacity();
                }
            }
        }

        private void btnCtrlEye_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                if (!btnEyeOn)
                {
                    btnEyeOn = true;
                    Rack.UserSettings.EyeCtrl = _EyeCtrl.On;
                    Rack.DMIBox.TobiiModule.MouseEmulator.EyetrackerToMouse = true;
                    Rack.DMIBox.TobiiModule.MouseEmulator.CursorVisible = false;

                    if (Rack.UserSettings.MyInstrumentControlMode == _MyInstrumentControlModes.Keyboard)
                    {
                        btnCtrlBreath.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    }
                }
                else
                {
                    btnEyeOn = false;
                    Rack.UserSettings.EyeCtrl = _EyeCtrl.Off;
                    Rack.DMIBox.TobiiModule.MouseEmulator.EyetrackerToMouse = false;
                    Rack.DMIBox.TobiiModule.MouseEmulator.CursorVisible = true;
                }
            }
        }


        #endregion Start, Exit and Setting buttons

        #endregion TopBar (Row0)

        #region Instrument (Row1)

        #region Instrument Settings 1

        private void btnCtrlKeyboard_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                if (!btnKeyboardOn && !btnEyeOn)
                {
                    btnKeyboardOn = true;
                    btnBreathOn = false;
                    btnCtrlKeyboard.IsEnabled = false;
                    btnCtrlBreath.IsEnabled = true;
                    txtBSwitc.Foreground = WarningBrush;

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
                    txtBSwitc.Foreground = new SolidColorBrush(Colors.White);

                    Rack.UserSettings.MyInstrumentControlMode = _MyInstrumentControlModes.Breath;
                    Rack.DMIBox.ResetModulationAndPressure();
                }
            }

        }

        private void btnScaleMinus_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                if (scaleIndex > 0)
                {
                    scaleIndex--;
                    txtScale.Text = comboScale[scaleIndex];
                    Rack.UserSettings.ScaleName = txtScale.Text;
                    //Rack.DMIBox.MyInstrumentSurface.DrawOnCanvas();
                }
            }
        }

        private void btnScalePlus_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                if (scaleIndex < 11)
                {
                    scaleIndex++;
                    txtScale.Text = comboScale[scaleIndex];
                    Rack.UserSettings.ScaleName = txtScale.Text;
                    //Rack.DMIBox.MyInstrumentSurface.DrawOnCanvas();
                }
            }
        }

        public void btnCodeMinus_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                if (!btnSharpNotesOn)
                {
                    if (codeIndex > 0)
                    {
                        codeIndex--;
                        txtCode.Text = comboCode[codeIndex];
                        Rack.UserSettings.ScaleCode = txtCode.Text;
                        //Rack.DMIBox.MyInstrumentSurface.DrawOnCanvas();
                    }
                }
            }
        }

        private void btnCodePlus_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                if (!btnSharpNotesOn)
                {
                    if (codeIndex < 3)
                    {
                        codeIndex++;
                        txtCode.Text = comboCode[codeIndex];
                        Rack.UserSettings.ScaleCode = txtCode.Text;
                        //Rack.DMIBox.MyInstrumentSurface.DrawOnCanvas();
                    }
                }
            }
        }

        private void btnOctaveMinus_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                if (octaveIndex > 0)
                {
                    octaveIndex--;
                    txtOctave.Text = comboOctave[octaveIndex];
                    Rack.UserSettings.Octave = txtOctave.Text;
                    //Rack.DMIBox.MyInstrumentSurface.DrawOnCanvas();
                }
            }
        }

        private void btnOctavePlus_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                if (octaveIndex < 4)
                {
                    octaveIndex++;
                    txtOctave.Text = comboOctave[octaveIndex];
                    Rack.UserSettings.Octave = txtOctave.Text;
                    //Rack.DMIBox.MyInstrumentSurface.DrawOnCanvas();
                }
            }
        }

        private void btnMidiPortMinus_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                Rack.UserSettings.MIDIPort--;
                Rack.DMIBox.MidiModule.OutDevice = Rack.UserSettings.MIDIPort;

                // Graphic changes               
                CheckMidiPort();
            }
        }

        private void btnMidiPortPlus_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                Rack.UserSettings.MIDIPort++;
                Rack.DMIBox.MidiModule.OutDevice = Rack.UserSettings.MIDIPort;

                // Graphic changes
                CheckMidiPort();
            }
        }

        // this just changes MIDI port selector graphic
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
            if (myInstrumentStarted)
            {
                SensorPort--;
                //Graphic changes
                UpdateSensorConnection();
            }
        }

        private void btnBreathPortPlus_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                SensorPort++;
                //Graphic changes
                UpdateSensorConnection();
            }
        }

        // this just changes sensor port selector graphic
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

        #endregion Instrument Settings 1

        #region Instrument Settings 2      

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

        private void btnSlidePlay_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
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

            if (myInstrumentStarted)
            {
                if (!btnBlinkOn)
                {
                    btnBlinkOn = true;
                    btnBlink.Background = ActiveBrush;

                    switch (txtBlink.Text)
                    {
                        case "Scale":
                            Rack.UserSettings.BlinkModes = _BlinkModes.Scale;
                            break;
                        case "Octave":
                            Rack.UserSettings.BlinkModes = _BlinkModes.Octave;
                            break;
                        case "Code":
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
            if (myInstrumentStarted)
            {
                if (blinkIndex > 0)
                {
                    blinkIndex--;
                    txtBlink.Text = blinkBehaviors[blinkIndex];
                    switch (blinkBehaviors[blinkIndex])
                    {
                        case "Scale":
                            Rack.UserSettings.BlinkModes = _BlinkModes.Scale;
                            break;
                        case "Octave":
                            Rack.UserSettings.BlinkModes = _BlinkModes.Octave;
                            break;
                        case "Code":
                            Rack.UserSettings.BlinkModes = _BlinkModes.Code;
                            break;
                    }
                }
            }
        }

        private void btnBlinkPlus_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                if (blinkIndex < 2)
                {
                    blinkIndex++;
                    txtBlink.Text = blinkBehaviors[blinkIndex];
                    switch (blinkBehaviors[blinkIndex])
                    {
                        case "Scale":
                            Rack.UserSettings.BlinkModes = _BlinkModes.Scale;
                            break;
                        case "Octave":
                            Rack.UserSettings.BlinkModes = _BlinkModes.Octave;
                            break;
                        case "Code":
                            Rack.UserSettings.BlinkModes = _BlinkModes.Code;
                            break;
                    }
                }
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
            if (myInstrumentStarted)
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
            if (myInstrumentStarted)
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
            if (myInstrumentStarted)
            {
                if (Rack.UserSettings.KeyVerticaDistance > 4)
                {
                    Rack.UserSettings.KeyVerticaDistance -= 5;
                    txtVerticalDistance.Text = Rack.UserSettings.KeyVerticaDistance.ToString();
                    Rack.DMIBox.MyInstrumentSurface.DrawOnCanvas();
                }
            }
        }

        private void btnVerticalDistancePlus_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                if (Rack.UserSettings.KeyVerticaDistance < 26)
                {
                    Rack.UserSettings.KeyVerticaDistance += 5;
                    txtVerticalDistance.Text = Rack.UserSettings.KeyVerticaDistance.ToString();
                    Rack.DMIBox.MyInstrumentSurface.DrawOnCanvas();
                }
            }
        }

        // Setting horizontal distance between keyboards
        private void btnHorizontalDistanceMinus_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                if (Rack.UserSettings.KeyHorizontalDistance > 200)
                {
                    Rack.UserSettings.KeyHorizontalDistance -= 100;
                    txtHorizontalDistance.Text = Rack.UserSettings.KeyHorizontalDistance.ToString();
                    Rack.DMIBox.MyInstrumentSurface.DrawOnCanvas();
                }
            }
        }

        private void btnHorizontalDistancePlus_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                if (Rack.UserSettings.KeyHorizontalDistance < 600)
                {
                    Rack.UserSettings.KeyHorizontalDistance += 100;
                    txtHorizontalDistance.Text = Rack.UserSettings.KeyHorizontalDistance.ToString();
                    Rack.DMIBox.MyInstrumentSurface.DrawOnCanvas();
                }
            }
        }      

        #endregion Instrument Settings 2

        #endregion Instrument (Row1)                        

    }

}
