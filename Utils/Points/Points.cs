using Autodesk.AutoCAD.Geometry;
using CadDev.Utils.CanvasUtils;

namespace CadDev.Utils.Points
{
    public static class Points
    {
        public static double GetDistance(this List<Point3d> points)
        {
            var pCount = points.Count;
            var result = 0.0;
            try
            {
                for (global::System.Int32 i = 1; i < pCount; i++)
                {
                    var j = i - 1;
                    result += points[j].DistanceTo(points[i]);
                }
            }
            catch (Exception)
            {
            }
            return result;
        }
    }
    public class PointCad
    {
        public object Obj { get; set; }
        public Point3d P { get; set; }
        public InstanceInCanvasCircel InstanceInCanvasCircel { get; set; }
        public Action<object> Action { get; set; }
        public bool IsSelected { get; set; }
        public PointCad(Point3d p)
        {
            P = p;
        }

        public void InitAction()
        {
            if (InstanceInCanvasCircel != null)
            {
                if (InstanceInCanvasCircel.UIElement is System.Windows.Shapes.Ellipse el)
                {
                    el.MouseLeftButtonUp += El_MouseLeftButtonUp;
                }
            }
        }

        public void ResetStatus()
        {
            IsSelected = false;
            if (InstanceInCanvasCircel.UIElement is System.Windows.Shapes.Ellipse el)
            {
                el.Stroke = StyleColorInCanvas.Color4;
                el.Fill = StyleColorInCanvas.Color4;
            }
        }

        private void El_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (InstanceInCanvasCircel.UIElement is System.Windows.Shapes.Ellipse el)
            {
                if (Obj != null)
                {
                    IsSelected = !IsSelected;
                    if (IsSelected)
                    {
                        el.Stroke = StyleColorInCanvas.Selected;
                        el.Fill = StyleColorInCanvas.Selected;
                        Action?.Invoke(Obj);
                    }
                    else
                    {
                        el.Stroke = StyleColorInCanvas.Color4;
                        el.Fill = StyleColorInCanvas.Color4;
                    }
                }
            }
        }
    }
}
