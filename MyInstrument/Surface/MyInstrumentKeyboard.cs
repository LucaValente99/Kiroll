using MyInstrument.DMIbox;
using NeeqDMIs.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace MyInstrument.Surface
{
    public class MyInstrumentKeyboard : StackPanel
    {
        public List<Color> KeysColorCode = new List<Color>()
        {
            Colors.Red,
            Colors.Yellow,
            Colors.Blue,
            Colors.Orange,
            Colors.Purple,
            Colors.Green,
            Colors.Coral
        };

        // utili per andare incontro allo scostamento in termini di ottava nelle varie scale:
        // es. scala maj C# sulla 4a ottava -> C#4, D#4, F4, F#4, G#4, A#4, C5. L'ultimo C sarà sull'ottava superiore, da qui { "C#", 1 }, o meglio "nella scala C# maj c'è una sola nota
        // che necessita di essere aumnetata di un ottava".
        private Dictionary<string, int> deviationMaj = new Dictionary<string, int>() {
            {"C", 0 }, { "C#", 1 }, { "D", 1 }, { "D#", 2 }, { "E", 2 }, { "F", 3 }, { "F#", 3 }, {"G", 4 }, { "G#", 4 }, { "A", 5 }, { "A#", 5 }, { "B", 6 }
            };
        private Dictionary<string, int> deviationMin = new Dictionary<string, int>() {
            {"C", 0 }, { "C#", 0 }, { "D", 1 }, { "D#", 1 }, { "E", 2 }, { "F", 3 }, { "F#", 3 }, {"G", 4 }, { "G#", 4 }, { "A", 5 }, { "A#", 6 }, { "B", 6 }
            };
        private Dictionary<string, int> deviationChrom = new Dictionary<string, int>() {
            {"C", 0 }, { "C#", 0 }, { "D", 0 }, { "D#", 0 }, { "E", 0 }, { "F", 0 }, { "F#", 1 }, {"G", 2 }, { "G#", 3 }, { "A", 4 }, { "A#", 5 }, { "B", 6 }
            };

        private string comboScale = Rack.UserSettings.ScaleName;
        private string comboCode = Rack.UserSettings.ScaleCode;
        private string comboOctave = Rack.UserSettings.Octave;
        public string ComboScale { get => comboScale; set { comboScale = value; } }
        public string ComboCode { get => comboCode; set { comboCode = value; } }
        public string ComboOctave { get => comboOctave; set { comboOctave = value; } }

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

        public MyInstrumentKeyboard() : base()
        {
            musicKeyboard = new StackPanel();
            musicKeyboard.Orientation = Orientation.Vertical;
            musicKeyboard.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            musicKeyboard.Background = Brushes.Transparent;
            musicKeyboard.Width = 150;
            musicKeyboard.Height = 590;

            FillStackPanel();
        }

        //riempo gli stack panel coi i tasti associati ognuno alla rispettiva nota della scala
        private void FillStackPanel()
        {

            foreach (Button toolKey in CreateKeys())
            {
                musicKeyboard.Children.Add(toolKey);

            }
        }

        //creo il tasto e lo associo alla rispettiva nota della scala per aggiungerlo alla tastiera
        public List<Button> CreateKeys()
        {
            List<Button> toolKeys = new List<Button>();
            Scale scale = new Scale(AbsNotesMethods.ToAbsNote(ComboScale), ScaleCodesMethods.toScaleCode(ComboCode));
            List<AbsNotes> noteList = scale.NotesInScale;
            int deviation_maj = deviationMaj[noteList[0].ToStandardString()];
            int deviation_min = deviationMin[noteList[0].ToStandardString()];
            int deviation_chrom = deviationChrom[noteList[0].ToStandardString()];

            for (int i = 0; i < 7; i++)
            {
                SolidColorBrush brush = new SolidColorBrush(KeysColorCode[i]);
                if (ComboCode == "maj")
                {
                    if (i >= 7 - deviation_maj)
                    {
                        MyInstrumentButtons toolKey = new MyInstrumentButtons(noteList[i].ToString(), int.Parse(ComboOctave) + 1, brush);
                        toolKeys.Add(toolKey.ToolKey);
                    }
                    else
                    {
                        MyInstrumentButtons toolKey = new MyInstrumentButtons(noteList[i].ToString(), int.Parse(ComboOctave), brush);
                        toolKeys.Add(toolKey.ToolKey);
                    }
                    
                }

                if (ComboCode == "min")
                {
                    if (i >= 7 - deviation_min)
                    {
                        MyInstrumentButtons toolKey = new MyInstrumentButtons(noteList[i].ToString(), int.Parse(ComboOctave) + 1, brush);
                        toolKeys.Add(toolKey.ToolKey);
                    }
                    else
                    {
                        MyInstrumentButtons toolKey = new MyInstrumentButtons(noteList[i].ToString(), int.Parse(ComboOctave), brush);
                        toolKeys.Add(toolKey.ToolKey);
                    }

                }

                if (ComboCode == "chrom")
                {
                    if (i >= 7 - deviation_chrom)
                    {
                        MyInstrumentButtons toolKey = new MyInstrumentButtons(noteList[i].ToString(), int.Parse(ComboOctave) + 1, brush);
                        toolKeys.Add(toolKey.ToolKey);
                    }
                    else
                    {
                        MyInstrumentButtons toolKey = new MyInstrumentButtons(noteList[i].ToString(), int.Parse(ComboOctave), brush);
                        toolKeys.Add(toolKey.ToolKey);
                    }

                }

            }
            
            return toolKeys;

        }
        
    }
}
