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
        private int proportionalHorizontal;
        private IPointFilter filter;

        private System.Windows.Point scrollCenter;
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
            this.proportionalHorizontal = proportional;
            this.proportionalVertical = proportional - 225;

            // Setting scrollviewer dimensions
            lastSampledPoint = new Point();
            basePosition = scrollViewer.PointToScreen(new System.Windows.Point(0, 0));
            scrollCenter = new System.Windows.Point(200, scrollViewer.ActualHeight / 2 + 100); // scrollViewer.ActualWidth / 2

            // Setting sampling timer
            samplerTimer.Interval = TimeSpan.FromMilliseconds(15);//1000; //1;
            samplerTimer.Tick += ListenMouse;
            samplerTimer.Start();

        }
        private void ListenMouse(object sender, EventArgs e)
        {           
            if (GetMousePos().X > scrollCenter.X) // +15 to avoid a small mistake that causes the keyboard to go back
            {
                lastSampledPoint.X = GetMousePos().X - (int)basePosition.X;
            }
               
            lastSampledPoint.Y = GetMousePos().Y - (int)basePosition.Y;

            filter.Push(lastSampledPoint);
            lastMean = filter.GetOutput();               

            Scroll();
        }
        private void Scroll()
        {
            Xdifference = (scrollCenter.X - lastMean.X);
            Ydifference = (scrollCenter.Y - lastMean.Y);
            if (Math.Abs(scrollCenter.Y - lastMean.Y) > radiusThreshold && Math.Abs(scrollCenter.X - lastMean.X) > radiusThreshold)
            {              
                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - Math.Pow((Xdifference / proportionalHorizontal), 2) * Math.Sign(Xdifference));
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - Math.Pow((Ydifference / proportionalVertical), 2) * Math.Sign(Ydifference));
            }
        }
        private Point GetMousePos()
        {
            temp = scrollViewer.PointToScreen(Mouse.GetPosition(scrollViewer));
            return new Point((int)temp.X, (int)temp.Y);
        }
        private System.Windows.Point temp = new System.Windows.Point();
    }
}