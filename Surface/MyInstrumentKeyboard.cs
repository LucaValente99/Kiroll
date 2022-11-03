using MyInstrument.DMIbox;
using NeeqDMIs.Music;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;

namespace MyInstrument.Surface
{
    public class MyInstrumentKeyboard : StackPanel
    {
        public static List<Color> KeysColorCode7 = new List<Color>()
        {
            Colors.Red, // C
            Colors.Yellow, // D
            Colors.Blue, // E
            Colors.SaddleBrown, // F - it should be black (email Ludovico)
            Colors.Orange, // G
            Colors.Green, // A
            Colors.Purple, // B
        };

        public static List<Color> KeysColorCode12 = new List<Color>()
        {
            Colors.Red, // C
            Colors.DarkRed, // C# - DarkRed
            Colors.Yellow, // D
            Colors.DarkGoldenrod, // D# - DarkGoldenrod
            Colors.Blue, // E
            Colors.SaddleBrown, // F - it should be black (email Ludovico)
            Colors.Brown, // F# - it should be black (email Ludovico) 
            Colors.Orange, // G
            Colors.DarkOrange, // G# - DarkOrange
            Colors.Green, // A
            Colors.DarkGreen, // A# - DarkGreen
            Colors.Purple, // B
        };       

        private StackPanel musicKeyboard;
        public StackPanel MusicKeyboard
        {
            get
            {
                return musicKeyboard;
            }

            set
            {
                musicKeyboard = value;
            }
        }

        // Useful for meeting the deviation in terms of octave in scales:
        // es. maj C# scale on 4th octave -> C#4, D#4, F4, F#4, G#4, A#4, C5. The last C in the scale will be in the superior octave, so { "C#", 1 }, that means "into the C# maj scale there is just one note
        // that needs to be increased about one octave".
        private Dictionary<string, int> deviationMaj = new Dictionary<string, int>() {
            {"C", 0 }, { "C#", 1 }, { "D", 1 }, { "D#", 2 }, { "E", 2 }, { "F", 3 }, { "F#", 3 }, {"G", 4 }, { "G#", 4 }, { "A", 5 }, { "A#", 5 }, { "B", 6 }
            };
        private Dictionary<string, int> deviationMin_nat = new Dictionary<string, int>() {
            {"C", 0 }, { "C#", 0 }, { "D", 1 }, { "D#", 1 }, { "E", 2 }, { "F", 3 }, { "F#", 3 }, {"G", 4 }, { "G#", 4 }, { "A", 5 }, { "A#", 6 }, { "B", 6 }
            };
        private Dictionary<string, int> deviationMin_arm = new Dictionary<string, int>() {
            {"C", 0 }, { "C#", 1 }, { "D", 1 }, { "D#", 1 }, { "E", 2 }, { "F", 3 }, { "F#", 3 }, {"G", 4 }, { "G#", 4 }, { "A", 5 }, { "A#", 6 }, { "B", 6 }
            };
        private Dictionary<string, int> deviationMin_mel = new Dictionary<string, int>() {
            {"C", 0 }, { "C#", 1 }, { "D", 1 }, { "D#", 2 }, { "E", 2 }, { "F", 3 }, { "F#", 3 }, {"G", 4 }, { "G#", 4 }, { "A", 5 }, { "A#", 6 }, { "B", 6 }
            };
        private Dictionary<string, int> deviationChrom_12 = new Dictionary<string, int>() {
            {"C", 0 }, { "C#", 1 }, { "D", 2 }, { "D#", 3 }, { "E", 4 }, { "F", 5 }, { "F#", 6 }, {"G", 7 }, { "G#", 8 }, { "A", 9 }, { "A#", 10 }, { "B", 11 }
            };

        private string comboScale = Rack.UserSettings.ScaleName;
        private string comboCode = Rack.UserSettings.ScaleCode;
        private string comboOctave = Rack.UserSettings.Octave;
        public string ComboScale { get => comboScale; set { comboScale = value; } }
        public string ComboCode { get => comboCode; set { comboCode = value; } }
        public string ComboOctave { get => comboOctave; set { comboOctave = value; } }

        private static int id = 0;
        public static int ID { get => id; set => id = value; }

        public MyInstrumentKeyboard() : base()
        {
            musicKeyboard = new StackPanel();
            musicKeyboard.Orientation = Orientation.Vertical;
            musicKeyboard.Background = Brushes.Transparent;
            musicKeyboard.Width = 150; //170
            musicKeyboard.Name = "_" + id.ToString();

            if (Rack.UserSettings.SharpNotesMode == _SharpNotesModes.On)
            {
                Rack.UserSettings.KeyboardHeight = 1011; //1200
                musicKeyboard.Height = Rack.UserSettings.KeyboardHeight;
                Rack.DMIBox.MyInstrumentMainWindow.canvasMyInstrument.Height = 1338; //1588
            }
            else
            {
                Rack.UserSettings.KeyboardHeight = 590; //700
                musicKeyboard.Height = Rack.UserSettings.KeyboardHeight;
                Rack.DMIBox.MyInstrumentMainWindow.canvasMyInstrument.Height = 781; //927
            }          
            
            FillStackPanel();
        }

        // Filling stack panels with associated keys 
        private void FillStackPanel()
        {
            foreach (Button toolKey in CreateKeys())
            {
                musicKeyboard.Children.Add(toolKey);
            }
        }

        // Creating keys, giving it the note of the scale, then add it to the keyboard
        private List<Button> CreateKeys()
        {
            List<Button> toolKeys = new List<Button>();
            Scale scale;
            List<AbsNotes> noteList;

            int deviation_maj;
            int deviation_min_nat;
            int deviation_min_arm;
            int deviation_min_mel;
            int deviation_chrom_12; // just in SharpNote On case

            if (Rack.UserSettings.SharpNotesMode == _SharpNotesModes.Off)
            {
                scale = new Scale(MusicConversions.ToAbsNote(ComboScale), MusicConversions.ToScaleCode(ComboCode));
                noteList = scale.NotesInScale;

                deviation_maj = deviationMaj[noteList[0].ToStandardString()];
                deviation_min_nat = deviationMin_nat[noteList[0].ToStandardString()];
                deviation_min_arm = deviationMin_arm[noteList[0].ToStandardString()];
                deviation_min_mel = deviationMin_mel[noteList[0].ToStandardString()];

                for (int i = 0; i < 7; i++)
                {                    
                    // Taking right color from the list dependign on note
                    SolidColorBrush brush = new SolidColorBrush(KeysColorCode7[i]);

                    if (ComboCode == "maj")
                    {
                        if (i >= 7 - deviation_maj)
                        {
                            MyInstrumentButtons toolKey = new MyInstrumentButtons(noteList[i].ToString(), int.Parse(ComboOctave) + 1, brush, id);
                            toolKeys.Add(toolKey.ToolKey);
                        }
                        else
                        {
                            MyInstrumentButtons toolKey = new MyInstrumentButtons(noteList[i].ToString(), int.Parse(ComboOctave), brush, id);
                            toolKeys.Add(toolKey.ToolKey);
                        }

                    }

                    if (ComboCode == "min")
                    {
                        if (i >= 7 - deviation_min_nat)
                        {
                            MyInstrumentButtons toolKey = new MyInstrumentButtons(noteList[i].ToString(), int.Parse(ComboOctave) + 1, brush, id);
                            toolKeys.Add(toolKey.ToolKey);
                        }
                        else
                        {
                            MyInstrumentButtons toolKey = new MyInstrumentButtons(noteList[i].ToString(), int.Parse(ComboOctave), brush, id);
                            toolKeys.Add(toolKey.ToolKey);
                        }
                    }

                    if (ComboCode == "min_arm")
                    {
                        if (i >= 7 - deviation_min_arm)
                        {
                            MyInstrumentButtons toolKey = new MyInstrumentButtons(noteList[i].ToString(), int.Parse(ComboOctave) + 1, brush, id);
                            toolKeys.Add(toolKey.ToolKey);
                        }
                        else
                        {
                            MyInstrumentButtons toolKey = new MyInstrumentButtons(noteList[i].ToString(), int.Parse(ComboOctave), brush, id);
                            toolKeys.Add(toolKey.ToolKey);
                        }
                    }

                    if (ComboCode == "min_mel")
                    {
                        if (i >= 7 - deviation_min_mel)
                        {
                            MyInstrumentButtons toolKey = new MyInstrumentButtons(noteList[i].ToString(), int.Parse(ComboOctave) + 1, brush, id);
                            toolKeys.Add(toolKey.ToolKey);
                        }
                        else
                        {
                            MyInstrumentButtons toolKey = new MyInstrumentButtons(noteList[i].ToString(), int.Parse(ComboOctave), brush, id);
                            toolKeys.Add(toolKey.ToolKey);
                        }
                    }
                }
            }
            else
            {
                scale = new Scale(MusicConversions.ToAbsNote(ComboScale), MusicConversions.ToScaleCode("chrom"));
                noteList = scale.NotesInScale;
                deviation_chrom_12 = deviationChrom_12[noteList[0].ToStandardString()];

                for (int i = 0; i < 12; i++)
                {
                    if (i >= 12 - deviation_chrom_12)
                    {
                        SolidColorBrush brush = new SolidColorBrush(KeysColorCode12[i]);
                        MyInstrumentButtons toolKey = new MyInstrumentButtons(noteList[i].ToString(), int.Parse(ComboOctave) + 1, brush, id);
                        toolKeys.Add(toolKey.ToolKey);
                    }
                    else
                    {
                        SolidColorBrush brush = new SolidColorBrush(KeysColorCode12[i]);
                        MyInstrumentButtons toolKey = new MyInstrumentButtons(noteList[i].ToString(), int.Parse(ComboOctave), brush, id);
                        toolKeys.Add(toolKey.ToolKey);
                    }                  
                }
            }

            // For each scale created the id will be incremented by one
            id++;           
            return toolKeys;
        }

        // STATIC METHODS

        // Getting the position of keyboard as child of the Canvas
        public static Point GetPosition(string id)
        {
            StackPanel keyboard = Rack.DMIBox.MyInstrumentMainWindow.canvasMyInstrument.Children[Int32.Parse(id.Substring(1))] as StackPanel;
            int x = (int)Canvas.GetLeft(keyboard);
            int y = (int)Canvas.GetTop(keyboard);

            return new Point(x, y);
        }

        // Getting the keyboard as child of the Canvas
        public static StackPanel GetKeyboard(string id)
        {
            StackPanel keyboard = Rack.DMIBox.MyInstrumentMainWindow.canvasMyInstrument.Children[Int32.Parse(id.Substring(1))] as StackPanel;
            return keyboard;
        }

        // Resetting the keyboard colors
        public static void ResetColors(string id)
        {
            StackPanel keyboard = Rack.DMIBox.MyInstrumentMainWindow.canvasMyInstrument.Children[Int32.Parse(id.Substring(1))] as StackPanel;

            int i = 0;
            foreach (Button key in keyboard.Children)
            {
                if (keyboard.Children.Count == 7)
                {
                    key.Background = new SolidColorBrush(KeysColorCode7[i]);
                }
                else
                {
                    key.Background = new SolidColorBrush(KeysColorCode12[i]);
                }
                key.Opacity = 1;
                key.Foreground = new SolidColorBrush(Colors.Black);
                i++;
            }

        }

        // Updating the keyboard colors: all the notes in keyboard will turn balck (opacity: 0.5) except for the selected one.
        public static void UpdateColors(string id, Button btn)
        {
            StackPanel keyboard = Rack.DMIBox.MyInstrumentMainWindow.canvasMyInstrument.Children[Int32.Parse(id.Substring(1))] as StackPanel;

            foreach (Button key in keyboard.Children)
            {
                if (key.Name != btn.Name)
                {
                    key.Background = new SolidColorBrush(Colors.Black);
                    key.Opacity = 0.5;
                    key.Foreground = new SolidColorBrush(Colors.DarkGray);
                }
            }
        }

    }

}
