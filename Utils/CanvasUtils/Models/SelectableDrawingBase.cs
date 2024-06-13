using CadDev.Utils.CanvasUtils.Interface;
using CadDev.Utils.CanvasUtils.Models;
using System.Windows.Media.Media3D;
using Brushes = System.Windows.Media.Brushes;
using wdm = System.Windows.Media;

namespace DaiwaLeaseUtils.CanvasUtils.Models
{
    public abstract class SelectableDrawingBase : DrawingBase, ISelectableDrawing
    {
        protected SelectableDrawingBase(IList<Point3D> psDecart, CanvasManager canvasManager) : base(psDecart, canvasManager)
        {
            ShapesOnCanvas.ForEach(p =>
            {
                p.MouseLeftButtonDown += OnLMouseDownClick;
            });
        }

        protected virtual wdm.Brush COLOR_STROKE_DEFAULT { get; set; } = Brushes.LightGray;
        protected virtual wdm.Brush COLOR_STROKE_HIGHLIGHT { get; set; } = Brushes.Black;
        protected virtual wdm.Brush COLOR_STROKE_SELECTED { get; set; } = Brushes.Red;
        protected virtual wdm.Brush COLOR_STROKE_CREATED { get; set; } = Brushes.Blue;
        protected virtual wdm.Brush COLOR_FILL_DEFAULT { get; set; } = Brushes.Transparent;
        protected virtual wdm.Brush COLOR_FILL_HIGHLIGHT { get; set; } = Brushes.Transparent;
        protected virtual wdm.Brush COLOR_FILL_SELECTED { get; set; } = Brushes.Transparent;
        protected virtual wdm.Brush COLOR_FILL_CREATED { get; set; } = Brushes.Transparent;

        private bool _isSelected = false;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnSelected(_isSelected);
                SelectedChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private bool _isCreated = false;
        public bool IsCreated
        {
            get => _isCreated;
            set
            {
                _isCreated = value;
                OnCreated(_isCreated);
            }
        }

        public event EventHandler SelectedChanged;

        public virtual void OnMouseEnter()
        {
            ShapesOnCanvas.ForEach(ins =>
            {
                ins.Stroke = COLOR_STROKE_HIGHLIGHT;
                ins.Fill = COLOR_FILL_HIGHLIGHT;
            }
            );
            CanvasManager.IsMouseOverDrawing = true;
        }
        public virtual void OnMouseLeave()
        {
            ShapesOnCanvas.ForEach(ins =>
            {
                if (IsSelected)
                {
                    ins.Stroke = COLOR_STROKE_SELECTED;
                    ins.Fill = COLOR_FILL_SELECTED;
                }
                else
                {
                    if (_isCreated)
                    {
                        ins.Stroke = COLOR_STROKE_CREATED;
                        ins.Fill = COLOR_FILL_CREATED;
                    }
                    else
                    {
                        ins.Stroke = COLOR_STROKE_DEFAULT;
                        ins.Fill = COLOR_FILL_DEFAULT;
                    }
                }
            }
            );
            CanvasManager.IsMouseOverDrawing = false;
        }
        protected virtual void OnSelected(bool val)
        {
            ShapesOnCanvas.ForEach(ins =>
            {
                if (val == true)
                {
                    ins.Stroke = COLOR_STROKE_SELECTED;
                    ins.Fill = COLOR_FILL_SELECTED;
                }
                else
                {
                    if (_isCreated)
                    {
                        ins.Stroke = COLOR_STROKE_CREATED;
                        ins.Fill = COLOR_FILL_CREATED;
                    }
                    else
                    {
                        ins.Stroke = COLOR_STROKE_DEFAULT;
                        ins.Fill = COLOR_FILL_DEFAULT;
                    }
                }
            }
            );
        }
        protected virtual void OnCreated(bool val)
        {
            ShapesOnCanvas.ForEach(ins =>
            {
                if (val == true)
                {
                    ins.Stroke = COLOR_STROKE_CREATED;
                    ins.Fill = COLOR_FILL_CREATED;
                }
                else
                {
                    ins.Stroke = COLOR_STROKE_DEFAULT;
                    ins.Fill = COLOR_FILL_DEFAULT;
                }
            }
            );
        }
        protected virtual void OnLMouseDownClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            IsSelected = !IsSelected;
        }
    }
}
