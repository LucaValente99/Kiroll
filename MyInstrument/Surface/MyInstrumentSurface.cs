using NeeqDMIs.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MyInstrument.Surface
{
    public class MyInstrumentSurface
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

        private DispatcherTimer buttonsMovement = new DispatcherTimer();

        private List<StackPanel> threeStackPanel;

        private Canvas canvas;
        private ComboBox comboScale;
        private ComboBox comboCode;
        private ComboBox comboOctave;

        public MyInstrumentSurface(Canvas canvas, ComboBox comboScale, ComboBox comboCode, ComboBox comboOctave)
        {
            this.canvas = canvas;
            this.comboScale = comboScale;
            this.comboCode = comboCode;
            this.comboOctave = comboOctave;
            threeStackPanel = createStackPanelList();
        }


        public void DrawOnCanvas()
        {

            int distance = 0;

            for (int i = 0; i < threeStackPanel.Count; i++)
            {
                canvas.Children.Add(threeStackPanel[i]);
                Canvas.SetLeft(threeStackPanel[i], (canvas.Width - threeStackPanel[i].Width) / 2 + distance);
                Canvas.SetTop(threeStackPanel[i], (canvas.Height - threeStackPanel[i].Height - 10) / 2);
                distance += 400;
            }

            buttonsMovement.Tick += buttonsMovementEvent;
            buttonsMovement.Interval = TimeSpan.FromMilliseconds(10);
            buttonsMovement.Start();          
        }

        public List<Button> createButtons()
        {
            List<Button> rectangleButtons = new List<Button>();
            Scale scale = new Scale(AbsNotesMethods.ToAbsNote(comboScale.Text), ScaleCodesMethods.toScaleCode(comboCode.Text));
            List<AbsNotes> noteList = scale.NotesInScale;

            for (int i = 0; i < 7; i++)
            {
                SolidColorBrush brush = new SolidColorBrush(KeysColorCode[i]);
                Button button = new Button() { Width = 150, Height = 84.2, Background = brush, BorderThickness = new System.Windows.Thickness(3), BorderBrush = Brushes.Black};
                TextBlock textBlock = new TextBlock() { Text = noteList[i].ToStandardString(), Foreground = Brushes.Black, FontSize = 30};

                button.Content = textBlock;
                rectangleButtons.Add(button);
            }                                

            return rectangleButtons;

        }

        public List<StackPanel> createStackPanelList()
        {
            List<StackPanel> threeStackPanel = new List<StackPanel> ();

            for (int i = 0; i < 3; i++)
            {
                StackPanel buttonsStackPanel = new StackPanel() { Orientation = Orientation.Vertical, Background = Brushes.Black, Width = 150, Height = 590 };
                fillStackPanel(buttonsStackPanel);
                threeStackPanel.Add(buttonsStackPanel);
            }

            return threeStackPanel;

        }

        private void fillStackPanel(StackPanel sp)
        {
            
            foreach (Button button in createButtons())
            {                
                sp.Children.Add(button);

            }
        }

        private void buttonsMovementEvent(object? sender, EventArgs e)
        {

            foreach(StackPanel sp in threeStackPanel)
            {
                Canvas.SetLeft(sp, Canvas.GetLeft(sp) - 5);

                if (Canvas.GetLeft(sp) < (canvas.Width / 2 - canvas.Width / 4) - sp.Width)
                {
                    Canvas.SetLeft(sp, (canvas.Width - sp.Width) / 2 + 800);
                }
            }
        }
    }
}
