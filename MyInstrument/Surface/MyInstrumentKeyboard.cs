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
            musicKeyboard.Background = Brushes.Black;
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

            for (int i = 0; i < 7; i++)
            {
                SolidColorBrush brush = new SolidColorBrush(KeysColorCode[i]);
                MyInstrumentButtons toolKey = new MyInstrumentButtons(noteList[i].ToString(), int.Parse(ComboOctave), brush);
                toolKeys.Add(toolKey.ToolKey);
            }
            
            return toolKeys;

        }
    }
}
