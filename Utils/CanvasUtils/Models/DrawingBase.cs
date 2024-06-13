using CadDev.Utils.CanvasUtils.Interface;
using CadDev.Utils.CanvasUtils.Models;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace DaiwaLeaseUtils.CanvasUtils.Models
{
    public abstract class DrawingBase : IDrawing
    {
        public List<Shape> ShapesOnCanvas { get; set; } = new List<Shape>();

        public CanvasManager CanvasManager { get; set; }
        public IList<Point3D> BasePointsDecart { get; set; }
        public IList<Point3D> BasePointsCanvas { get; set; }

        public DrawingBase(IList<Point3D> psDecart, CanvasManager canvasManager)
        {
            BasePointsDecart = psDecart;
            BasePointsCanvas = psDecart.Select(x => canvasManager.TransformDecartToCanvas(x)).ToList();
            CanvasManager = canvasManager;
            OnBeforeInit();
            ShapesOnCanvas = CreateShapesOnCanvas();
        }
        protected abstract List<Shape> CreateShapesOnCanvas();
        protected virtual void OnBeforeInit()
        {
        }
    }
}
