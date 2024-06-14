using CadDev.Utils.CanvasUtils.Utils;
using System.Windows.Shapes;
using wd = System.Windows;

namespace CadDev.Utils.CanvasUtils
{
    public class InstanceInCanvasPolyline : InstanceInCanvas
    {
        public List<wd.Point> Points { get; set; }
        public InstanceInCanvasPolyline(CanvasBase canvasBase, OptionStyleInstanceInCanvas options, wd.Point centerBase, List<wd.Point> points) : base(canvasBase, options, centerBase)
        {
            Points = points;
            var pll = new Polyline();
            foreach (wd.Point p in points)
            {
                pll.Points.Add(p);
            }
            pll.StrokeThickness = Options.Thickness;
            pll.StrokeDashArray = Options.LineStyle;
            pll.Stroke = Options.ColorBrush;

            UIElement = pll;
        }
    }
}
