using CadDev.Utils.CanvasUtils.Utils;
using System.Windows.Shapes;
using wd = System.Windows;

namespace CadDev.Utils.CanvasUtils
{
    public class InstanceInCanvasLine : InstanceInCanvas
    {
        public wd.Point P1 { get; set; }
        public wd.Point P2 { get; set; }
        public Action Action { get; set; }

        public InstanceInCanvasLine(CanvasBase canvasBase, OptionStyleInstanceInCanvas Options, wd.Point p1, wd.Point p2) : base(canvasBase, Options)
        {
            P1 = p1;
            P2 = p2;
            UIElement = new Line()
            {
                StrokeThickness = Options.Thickness,
                StrokeDashArray = Options.LineStyle,
                Stroke = Options.ColorBrush
            };
            GenerateUi();
        }
        public void GenerateUi()
        {
            if (UIElement is Line l)
            {
                l.X1 = P1.X * CanvasBase.Scale;
                l.Y1 = P1.Y * CanvasBase.Scale;
                l.X2 = P2.X * CanvasBase.Scale;
                l.Y2 = P2.Y * CanvasBase.Scale;
            }
        }
    }
}
