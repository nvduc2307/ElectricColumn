using System.Windows;
using System.Windows.Controls;
using wd = System.Windows;

namespace CadDev.Utils.CanvasUtils.Utils
{
    public class CanvasBase
    {
        public Canvas Parent { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Scale { get; set; }
        public wd.Point Center { get; set; }

        public CanvasBase(Canvas parent, double scale)
        {
            Parent = parent;
            Scale = scale;
            Width = parent.ActualWidth;
            Height = parent.ActualHeight;
            Center = new wd.Point(Width / 2, Height / 2);
        }
    }
}
