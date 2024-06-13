using CadDev.Utils.CanvasUtils.Utils;
using System.Windows;

namespace CadDev.Utils.CanvasUtils
{
    public abstract class InstanceInCanvas
    {
        public CanvasBase CanvasBase { get; set; }
        public UIElement UIElement { get; set; }
        public OptionStyleInstanceInCanvas Options { get; set; }
        public InstanceInCanvas(CanvasBase canvasBase, OptionStyleInstanceInCanvas options)
        {
            CanvasBase = canvasBase;
            Options = options;
        }

        public void DrawInCanvas()
        {
            if (CanvasBase != null) CanvasBase.Parent.Children.Add(UIElement);
        }
    }
}
