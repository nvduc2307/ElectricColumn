using CadDev.Utils.CanvasUtils.Utils;
using System.Windows;

namespace CadDev.Utils.CanvasUtils
{
    public abstract class InstanceInCanvas
    {
        public CanvasBase CanvasBase { get; set; }
        public UIElement UIElement { get; set; }
        public OptionStyleInstanceInCanvas Options { get; set; }
        public System.Windows.Point CenterBase { get; set; }

        public InstanceInCanvas(CanvasBase canvasBase, OptionStyleInstanceInCanvas options, System.Windows.Point centerBase)
        {
            CanvasBase = canvasBase;
            Options = options;
            CenterBase = centerBase;
        }

        public void DrawInCanvas()
        {
            if (CanvasBase != null) CanvasBase.Parent.Children.Add(UIElement);
        }
    }
}
