using CadDev.Utils.CanvasUtils.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using wd = System.Windows;

namespace CadDev.Utils.CanvasUtils
{
    public class InstanceInCanvasCircel : InstanceInCanvas
    {
        public wd.Point VectorInit {  get; set; }
        public wd.Point Point { get; set; }
        public double Diameter { get; set; }
        public TextBlock Title { get; set; }
        public InstanceInCanvasCircel(CanvasBase canvasBase, OptionStyleInstanceInCanvas options, double diameter, wd.Point point, wd.Point vectorInit, string title) : base(canvasBase, options)
        {
            Diameter = diameter;
            Title = new TextBlock();
            Title.Text = title;
            CanvasBase.Parent.Children.Add(Title);
            Point = point;
            VectorInit = vectorInit;
            UIElement = new Ellipse()
            {
                Height = diameter,
                Width = diameter,
                StrokeThickness = options.Thickness,
                StrokeDashArray = options.LineStyle,
                Stroke = options.ColorBrush
            };
            GenerateUi();
        }
        public void GenerateUi()
        {
            var p = new wd.Point(Point.X * CanvasBase.Scale - VectorInit.X * Diameter / 2, Point.Y * CanvasBase.Scale - VectorInit.Y * Diameter / 2);
            Canvas.SetLeft(UIElement, p.X - Diameter / 2);
            Canvas.SetTop(UIElement, p.Y - Diameter / 2);

            Canvas.SetLeft(Title, p.X - Diameter / 2 + 3);
            Canvas.SetTop(Title, p.Y - Diameter / 2 + 2);
        }
    }
}
