using CadDev.Tools.ElectricColumnGeneral.viewModels;
using CadDev.Tools.ElectricColumnGeneral.views;
using CadDev.Utils.CanvasUtils;
using CadDev.Utils.CanvasUtils.Utils;
using System.Windows.Controls;

namespace CadDev.Tools.ElectricColumnGeneral.models
{
    public class ElectricColumnUIElementModel
    {
        private string _sectionElevationCanvasName = "SectionElevationCanvas";
        private string _sectionPlaneCanvasName = "SectionPlaneCanvas";
        private ElectricColumnGeneralViewModel _viewModel;
        public ElectricColumnGeneralView MainView { get; set; }
        public CanvasBase SectionElevationCanvas { get; set; }
        public CanvasBase SectionPlaneCanvas { get; set; }
        public ElectricColumnUIElementModel(ElectricColumnGeneralViewModel viewModel)
        {
            _viewModel = viewModel;
            MainView = new ElectricColumnGeneralView() { DataContext = viewModel };
            MainView.Loaded += MainView_Loaded;
        }

        private void MainView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                var sectionElevationCanvas = MainView.FindName(_sectionElevationCanvasName) as Canvas;
                var sectionPlaneCanvas = MainView.FindName(_sectionPlaneCanvasName) as Canvas;
                SectionElevationCanvas = new CanvasBase(sectionElevationCanvas, 0.01);
                SectionPlaneCanvas = new CanvasBase(sectionPlaneCanvas, 0.01);
                ElectricColumnUIElementModel.DrawSectionPlan(SectionPlaneCanvas, _viewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void DrawSectionPlan(CanvasBase canvasBase, ElectricColumnGeneralViewModel viewModel)
        {
            var options = new OptionStyleInstanceInCanvas(
                StyleThicknessInCanvas.Thickness_1,
                StyleDashInCanvas.Style_Solid,
                StyleColorInCanvas.Color4, null);
            canvasBase.Parent.Children.Clear();
            var sectionPlane = viewModel.ElectricColumnModel.SectionPlaneSelected;
            foreach (var item in sectionPlane.Lines)
            {
                item.CanvasLine = new InstanceInCanvasLine(
                    canvasBase, 
                    options, 
                    sectionPlane.Center.ConvertPoint(), 
                    item.StartP.ConvertPoint(), 
                    item.EndP.ConvertPoint());
                item.CanvasLine.DrawInCanvas();
            }
        }
    }
}
