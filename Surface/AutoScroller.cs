using Kiroll.DMIbox;
using NeeqDMIs.Eyetracking.PointFilters;
using System;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Kiroll.Surface
{
    public class AutoScroller
    {
        private ScrollViewer scrollViewer;
        private int radiusThreshold;
        private int proportionalVertical;
        public int ProportionalVertical { get => proportionalVertical; set => proportionalVertical = value; }
        private int proportionalHorizontal;
        public int ProportionalHorizontal { get => proportionalHorizontal; set => proportionalHorizontal = value; }

        private IPointFilter filter;

        private System.Windows.Point scrollCenter;
        public System.Windows.Point ScrollCenter { get => scrollCenter; set => scrollCenter = value; }
        private System.Windows.Point basePosition;

        private DispatcherTimer samplerTimer = new DispatcherTimer(DispatcherPriority.Render);

        private Point lastSampledPoint;
        private Point lastMean;
        private double Xdifference;
        private double Ydifference;
        public AutoScroller(ScrollViewer scrollViewer, int radiusThreshold, int proportional, IPointFilter filter)
        {
            this.radiusThreshold = radiusThreshold;
            this.filter = filter;
            this.scrollViewer = scrollViewer;                

            // Setting scrollviewer dimensions
            lastSampledPoint = new Point();
            proportionalHorizontal = proportional;
            proportionalVertical = proportional - 225;
            basePosition = scrollViewer.PointToScreen(new System.Windows.Point(0, 0));                        
            scrollCenter = new System.Windows.Point(200, scrollViewer.ActualHeight / 2 + 100); // scrollViewer.ActualWidth / 2
            
            // Setting sampling timer
            samplerTimer.Interval = TimeSpan.FromMilliseconds(15);//1000; //1;
            samplerTimer.Tick += ListenMouse;
            samplerTimer.Start();

        }
        protected void ListenMouse(object sender, EventArgs e)
        {
            int X = GetMousePos().X;
            int Y = GetMousePos().Y;

            if (Rack.UserSettings.Orientation == Orientation.Vertical)
            {
                if (X > scrollCenter.X)
                {
                    lastSampledPoint.X = X - (int)basePosition.X;
                }

                lastSampledPoint.Y = Y - (int)basePosition.Y;
            }
            else
            {
                if (Y > scrollCenter.Y + 80)
                {
                    lastSampledPoint.Y = Y - (int)basePosition.Y;
                }

                lastSampledPoint.X = X - (int)basePosition.X;
            }
            

            filter.Push(lastSampledPoint);
            lastMean = filter.GetOutput();               

            Scroll();
        }
        protected void Scroll()
        {
            Xdifference = (scrollCenter.X - lastMean.X);
            Ydifference = (scrollCenter.Y - lastMean.Y);
            if (Math.Abs(scrollCenter.Y - lastMean.Y) > radiusThreshold && Math.Abs(scrollCenter.X - lastMean.X) > radiusThreshold)
            {              
                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - Math.Pow((Xdifference / proportionalHorizontal), 2) * Math.Sign(Xdifference));
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - Math.Pow((Ydifference / proportionalVertical), 2) * Math.Sign(Ydifference));
            }
        }
        protected Point GetMousePos()
        {
            temp = scrollViewer.PointToScreen(Mouse.GetPosition(scrollViewer));
            return new Point((int)temp.X, (int)temp.Y);
        }
        protected System.Windows.Point temp = new System.Windows.Point();
    }
}