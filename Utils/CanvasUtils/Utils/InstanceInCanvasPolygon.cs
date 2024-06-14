using CadDev.Utils.CanvasUtils.Utils;
using System.Windows.Shapes;
using wd = System.Windows;
using wdi = System.Windows.Input;

namespace CadDev.Utils.CanvasUtils
{
    public class InstanceInCanvasPolygon : InstanceInCanvas
    {
        public IEnumerable<wd.Point> Points { get; set; }
        public InstanceInCanvasPolygon(CanvasBase canvasBase, OptionStyleInstanceInCanvas options,wd.Point centerBase, IEnumerable<wd.Point> points) : base(canvasBase, options, centerBase)
        {
            Points = points;
            var plg = new Polygon();
            foreach (wd.Point p in points)
            {
                var pn = new wd.Point(p.X * CanvasBase.Scale, p.Y * CanvasBase.Scale);
                plg.Points.Add(pn);
            }
            plg.StrokeThickness = Options.Thickness;
            plg.StrokeDashArray = Options.LineStyle;
            plg.Stroke = Options.ColorBrush;

            if (options.Fill != null) plg.Fill = options.Fill;

            UIElement = plg;

            //action
            plg.MouseMove += Plg_MouseMove;
        }

        private void Plg_MouseMove(object sender, wdi.MouseEventArgs e)
        {
            if (sender is Polygon plg) plg.Cursor = wdi.Cursors.Hand;
        }
    }
}
