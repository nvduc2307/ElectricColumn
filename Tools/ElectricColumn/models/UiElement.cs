using CadDev.Utils.CanvasUtils.Utils;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CadDev.Tools.ElectricColumn.models
{
    public class UiElement : ObservableObject
    {
        public CanvasBase ElevationSectionCanvas { get; set; }
        public CanvasBase ShortSectionCanvas { get; set; }
    }
}
