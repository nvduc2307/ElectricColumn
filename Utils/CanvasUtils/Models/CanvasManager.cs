using CadDev.Utils.CanvasUtils.Interface;
using CadDev.Utils.CanvasUtils.Utils;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using wdi = System.Windows.Input;

namespace CadDev.Utils.CanvasUtils.Models
{
    public class CanvasManager
    {
        public bool IsMouseOverDrawing { get; set; } = false;
        public bool CanAdd { get; set; } = false;
        public int SelectedCount
            => AllDrawings.Count(x => x is ISelectableDrawing selectable && selectable.IsSelected);
        public List<ISelectableDrawing> SelectedDrawings
            => AllDrawings.Where(x => x is ISelectableDrawing selectable && selectable.IsSelected)
            .Cast<ISelectableDrawing>().ToList();

        public Canvas Canvas { get; protected set; }
        public IList<IDrawing> AllDrawings { get; set; } = new List<IDrawing>();
        public double Angle { get; set; } = 0;
        public double Ratio { get; set; } = 0.9;
        public double Scale { get; protected set; }
        public double WidthCanvas { get; protected set; }
        public double HeightCanvas { get; protected set; }
        public double WidthDecart { get; protected set; }
        public double HeightDecart { get; protected set; }
        public Point3D CenterPointCanvas { get; protected set; }
        public Point3D CenterPointDecart { get; protected set; }

        public event EventHandler SelectedChanged = null;
        public event EventHandler DraggedChanged = null;
        public event EventHandler<AddClickEventAgrs> AddClicked = null;

        public CanvasManager(Canvas canvas, Point3D minPointDecart, Point3D maxPointDecart)
        {
            canvas.Focusable = true;
            //canvas.MouseEnter += Canvas_MouseEnter;
            //canvas.KeyDown += Canvas_KeyDown;
            //canvas.MouseDown += Canvas_MouseDown;
            Canvas = canvas;
            WidthCanvas = canvas.Width;
            HeightCanvas = canvas.Height;
            WidthDecart = Math.Abs(maxPointDecart.X - minPointDecart.X);
            HeightDecart = Math.Abs(maxPointDecart.Y - minPointDecart.Y);
            Scale = Math.Min(WidthCanvas, HeightCanvas) * Ratio / Math.Max(WidthDecart, HeightDecart);
            CenterPointDecart = minPointDecart.MidPointWith(maxPointDecart);
            var pMinCanvas = new Point3D();
            var pMaxCanvas = new Point3D(WidthCanvas, HeightCanvas, 0);
            CenterPointCanvas = pMinCanvas.MidPointWith(pMaxCanvas);
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CanAdd)
            {
                if (e.LeftButton == MouseButtonState.Pressed && !IsMouseOverDrawing)
                {
                    var pcanvas = e.GetPosition(Canvas);
                    var pDescarte = TransformCanvasToDecart(pcanvas.ToPoint3D());
                    var agrs = new AddClickEventAgrs(pcanvas, pDescarte);
                    AddClicked?.Invoke(this, agrs);
                }
            }
        }
        private void Canvas_KeyDown(object sender, wdi.KeyEventArgs e)
        {
            if (CanAdd)
            {
                if (e.Key == Key.Escape)
                {
                    CanAdd = false;
                    Canvas.Cursor = wdi.Cursors.Arrow;
                }
            }
        }
        private void Canvas_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (CanAdd)
            {
                Canvas.Focus();
                Canvas.Cursor = wdi.Cursors.Cross;
            }
        }

        public Point3D TransformDecartToCanvas(Point3D pDecart)
        {
            var dir = pDecart - CenterPointDecart;
            var len = dir.Length;
            if (len == 0)
            {
                return CenterPointCanvas;
            }
            dir.Normalize();
            var dirCanvas = new Vector3D(dir.X, -dir.Y, 0);
            var pCanvas = CenterPointCanvas + dirCanvas * len * Scale;
            pCanvas = pCanvas.RotateOnXY(CenterPointCanvas, Angle);
            return pCanvas;
        }
        public Point3D TransformCanvasToDecart(Point3D pCanvas, double z = 0)
        {
            pCanvas = pCanvas.RotateOnXY(CenterPointCanvas, -Angle);
            var dir = pCanvas - CenterPointCanvas;
            var len = dir.Length;
            if (len == 0)
            {
                return CenterPointDecart;
            }
            dir.Normalize();
            var dirDecart = new Vector3D(dir.X, -dir.Y, 0);
            var pDecart = CenterPointDecart + dirDecart * len / Scale;
            pDecart = new Point3D(pDecart.X, pDecart.Y, z);
            return pDecart;
        }

        public void Add<T>(T drawing) where T : IDrawing
        {
            AllDrawings.Add(drawing);
            drawing.ShapesOnCanvas.ForEach(shape =>
            {
                Canvas.Children.Add(shape);
                if (drawing is ISelectableDrawing selectableDrawing)
                {
                    selectableDrawing.SelectedChanged += SelectableDrawing_SelectedChanged;
                    shape.MouseEnter += (s, e) => { selectableDrawing.OnMouseEnter(); };
                    shape.MouseLeave += (s, e) => { selectableDrawing.OnMouseLeave(); };
                    if (drawing is IDraggableDrawing draggable)
                    {
                        draggable.Dragged += Draggable_Dragged;
                    }
                }
            });
        }

        public void Delete<T>(T drawing) where T : IDrawing
        {
            AllDrawings.Remove(drawing);
            drawing.ShapesOnCanvas.ForEach(shape =>
            {
                Canvas.Children.Remove(shape);
                if (drawing is ISelectableDrawing selectableDrawing)
                {
                    selectableDrawing.SelectedChanged -= SelectableDrawing_SelectedChanged;
                    shape.MouseEnter -= (s, e) => { selectableDrawing.OnMouseEnter(); };
                    shape.MouseLeave -= (s, e) => { selectableDrawing.OnMouseLeave(); };
                    if (drawing is IDraggableDrawing draggable)
                    {
                        draggable.Dragged -= Draggable_Dragged;
                    }
                }
            });
        }

        private void Draggable_Dragged(object sender, EventArgs e)
        {
            DraggedChanged?.Invoke(sender, e);
        }
        private void SelectableDrawing_SelectedChanged(object sender, EventArgs e)
        {
            SelectedChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
