using wd = System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using wdi = System.Windows.Input;
using winform = System.Windows.Forms;

namespace CadDev.Utils.CanvasUtils.Controls
{
    public class BorderZoomPanDrag : Border
    {
        private wd.UIElement child = null;
        private wd.Point origin;
        private wd.Point start;
        private static DateTime lastClickTime = DateTime.MinValue;

        private TranslateTransform GetTranslateTransform(wd.UIElement element)
        {
            return (TranslateTransform)((TransformGroup)element.RenderTransform)
              .Children.First(tr => tr is TranslateTransform);
        }

        private ScaleTransform GetScaleTransform(wd.UIElement element)
        {
            return (ScaleTransform)((TransformGroup)element.RenderTransform)
              .Children.First(tr => tr is ScaleTransform);
        }

        public override wd.UIElement Child
        {
            get { return base.Child; }
            set
            {
                if (value != null && value != this.Child)
                    this.Initialize(value);
                base.Child = value;
            }
        }

        public void Initialize(UIElement element)
        {
            this.child = element;
            if (child != null)
            {
                var canvas = child as Canvas;
                if (canvas != null)
                {
                    canvas.Loaded += (s, e) =>
                    {
                        //this.Background = canvas.Background;
                    };
                }

                TransformGroup group = new TransformGroup();
                ScaleTransform st = new ScaleTransform();
                group.Children.Add(st);
                TranslateTransform tt = new TranslateTransform();
                group.Children.Add(tt);
                child.RenderTransform = group;
                child.RenderTransformOrigin = new wd.Point(0.0, 0.0);
                this.MouseWheel += child_MouseWheel;
                this.MouseMove += child_MouseMove;
                this.MouseDown += child_MouseDown;
                this.MouseUp += child_MouseUp;
            }
        }
        public void Reset()
        {
            if (child != null)
            {
                // reset zoom
                var st = GetScaleTransform(child);
                st.ScaleX = 1.0;
                st.ScaleY = 1.0;

                // reset pan
                var tt = GetTranslateTransform(child);
                tt.X = 0.0;
                tt.Y = 0.0;
            }
        }

        private void child_MouseWheel(object sender, wdi.MouseWheelEventArgs e)
        {
            if (child != null)
            {
                var st = GetScaleTransform(child);
                var tt = GetTranslateTransform(child);

                double zoom = e.Delta > 0 ? .2 : -.2;
                if (!(e.Delta > 0) && (st.ScaleX < .4 || st.ScaleY < .4))
                    return;

                wd.Point relative = e.GetPosition(child);
                double absoluteX;
                double absoluteY;

                absoluteX = relative.X * st.ScaleX + tt.X;
                absoluteY = relative.Y * st.ScaleY + tt.Y;

                st.ScaleX += zoom;
                st.ScaleY += zoom;

                tt.X = absoluteX - relative.X * st.ScaleX;
                tt.Y = absoluteY - relative.Y * st.ScaleY;
            }
        }

        private void child_MouseUp(object sender, wdi.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == wdi.MouseButton.Middle && child != null)
            {
                child.ReleaseMouseCapture();
                this.Cursor = wdi.Cursors.Arrow;
            }
        }

        private void child_MouseDown(object sender, wdi.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == wdi.MouseButton.Middle && child != null)
            {
                var tt = GetTranslateTransform(child);
                start = e.GetPosition(this);
                origin = new wd.Point(tt.X, tt.Y);
                this.Cursor = wdi.Cursors.Hand;
                child.CaptureMouse();

                //Fit to UIElement
                TimeSpan elapsed = DateTime.Now - lastClickTime;
                lastClickTime = DateTime.Now;
                if (elapsed.TotalMilliseconds < winform.SystemInformation.DoubleClickTime)
                {
                    this.Reset();
                }
            }
        }

        private void child_MouseMove(object sender, wdi.MouseEventArgs e)
        {
            if (e.MiddleButton == wdi.MouseButtonState.Pressed && child.IsMouseCaptured)
            {
                var tt = GetTranslateTransform(child);
                Vector v = start - e.GetPosition(this);
                tt.X = origin.X - v.X;
                tt.Y = origin.Y - v.Y;
            }
        }
    }
}
