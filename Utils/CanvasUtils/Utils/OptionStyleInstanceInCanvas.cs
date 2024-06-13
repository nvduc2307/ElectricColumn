using System.Windows.Media;

namespace CadDev.Utils.CanvasUtils
{
    public class OptionStyleInstanceInCanvas
    {
        public double Thickness { get; set; }
        public DoubleCollection LineStyle { get; set; }
        public SolidColorBrush ColorBrush { get; set; }
        public SolidColorBrush Fill { get; set; }
        public OptionStyleInstanceInCanvas(
            double thickness, 
            DoubleCollection lineStyle, 
            SolidColorBrush colorBrush,
            SolidColorBrush fill)
        {
            Thickness = thickness;
            LineStyle = lineStyle;
            ColorBrush = colorBrush;
            Fill = fill;
        }
    }
}
