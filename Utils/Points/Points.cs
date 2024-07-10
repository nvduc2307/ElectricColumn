using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CadDev.Tools.ElectricColumnGeneral.viewModels;
using CadDev.Utils.CanvasUtils;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CadDev.Utils.Points
{
    public static class Points
    {
    }
    public class PointCad
    {
        public object Obj {  get; set; }
        public Point3d P { get; set; }
        public InstanceInCanvasCircel InstanceInCanvasCircel {  get; set; }
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
                        el.Stroke = StyleColorInCanvas.Color0;
                        el.Fill = StyleColorInCanvas.Color0;
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
