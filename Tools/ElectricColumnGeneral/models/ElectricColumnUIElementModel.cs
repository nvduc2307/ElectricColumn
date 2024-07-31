using CadDev.Tools.ElectricColumnGeneral.viewModels;
using CadDev.Tools.ElectricColumnGeneral.views;
using CadDev.Utils;
using CadDev.Utils.CanvasUtils;
using CadDev.Utils.CanvasUtils.Utils;
using CadDev.Utils.Compares;
using CadDev.Utils.Geometries;
using CadDev.Utils.Lines;
using CadDev.Utils.Points;
using System.Windows.Controls;

namespace CadDev.Tools.ElectricColumnGeneral.models
{
    public class ElectricColumnUIElementModel
    {
        private static List<PointCad> _pointsSelected = new List<PointCad>();
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
                SectionElevationCanvas = new CanvasBase(sectionElevationCanvas, 0.008);
                SectionPlaneCanvas = new CanvasBase(sectionPlaneCanvas, 0.008);
                DrawingSectionElevation(SectionElevationCanvas, _viewModel);
                DrawSectionPlan(SectionPlaneCanvas, _viewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void UpdateStatusSectionSelectedAtElevation(CanvasBase canvasBase, ElectricColumnGeneralViewModel viewModel)
        {
            var sections = viewModel.ElectricColumnModel.SectionPlanes;
            var sectionIndexOf = viewModel.ElectricColumnModel.SectionPlaneSelected;
            foreach (var section in sections)
            {
                foreach (var lc in section.LinesAtElevation)
                {
                    lc.CanvasLine.ResetStatus();
                }
            }

            foreach (var lc in sectionIndexOf.LinesAtElevation)
            {
                lc.CanvasLine.SelectedStatus();
            }
        }

        public static void UpdateStatusSwingSelectedAtElevation(CanvasBase canvasBase, ElectricColumnModel electricColumnModel)
        {
            var swings = electricColumnModel
                .ElectricColumnSwingModel
                .ElectricColumnTotalSwingsLeft
                .Select(x => x.LinesSection)
                .Aggregate((a, b) => a.Concat(b))
                .ToList();
            var sectionElevationMain = electricColumnModel.SectionElevationMain
                .Where(x => swings.Any(y => y.IsSeem(x)))
                .ToList();
            var sectionIndexOf = electricColumnModel.ElectricColumnSwingModel.ElectricColumnSwingSelected;
            foreach (var lc in sectionElevationMain)
            {
                lc.CanvasLine.ResetStatus();
            }

            foreach (var lc in sectionIndexOf.LinesSection)
            {
                lc.CanvasLine.SelectedStatus();
            }
        }

        public static void DrawingSectionElevation(CanvasBase canvasBase, ElectricColumnGeneralViewModel viewModel)
        {
            var options = new OptionStyleInstanceInCanvas(
                StyleThicknessInCanvas.Thickness_1,
                StyleDashInCanvas.Style_Solid,
                StyleColorInCanvas.Color4,
                StyleColorInCanvas.Color4);
            canvasBase.Parent.Children.Clear();
            var sectionElevationMain = viewModel.ElectricColumnModel.SectionElevationMain;
            var center = sectionElevationMain.GetPoints()
                .Select(x => x.ConvertPointOXZToOXY()).GetCenterCanvas();

            foreach (var item in sectionElevationMain)
            {
                item.CanvasLine = new InstanceInCanvasLine(
                    canvasBase,
                    options,
                    center,
                    item.StartP.ConvertPointOXZToOXY(),
                    item.EndP.ConvertPointOXZToOXY());
                item.CanvasLine.Obj = viewModel;
                item.CanvasLine.DrawInCanvas();

                if (item.StartP.Z.IsEqual(item.EndP.Z))
                {
                    var elevation = item.StartP.Z;
                    var section = viewModel.ElectricColumnModel.SectionPlanes.FirstOrDefault(x => x.Elevation.IsEqual(elevation));
                    if (section != null) section.LinesAtElevation.Add(item);
                }
            }
            UpdateStatusSwingSelectedAtElevation(canvasBase, viewModel.ElectricColumnModel);
            UpdateStatusSectionSelectedAtElevation(canvasBase, viewModel);
        }

        public static void DrawSectionPlan(CanvasBase canvasBase, ElectricColumnGeneralViewModel viewModel)
        {
            var options = new OptionStyleInstanceInCanvas(
                StyleThicknessInCanvas.Thickness_1,
                StyleDashInCanvas.Style_Solid,
                StyleColorInCanvas.Color4,
                StyleColorInCanvas.Color4);
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
                item.CanvasLine.Obj = viewModel;
                item.CanvasLine.DrawInCanvas();
            }

            foreach (var item in sectionPlane.LinesAdd)
            {
                item.CanvasLine = new InstanceInCanvasLine(
                    canvasBase,
                    options,
                    sectionPlane.Center.ConvertPoint(),
                    item.StartP.ConvertPoint(),
                    item.EndP.ConvertPoint());
                item.CanvasLine.Obj = viewModel;
                item.CanvasLine.Delete += LinesAddDelete;
                item.CanvasLine.DrawInCanvas();
            }

            foreach (var item in sectionPlane.Points)
            {
                item.InstanceInCanvasCircel = new InstanceInCanvasCircel(
                    canvasBase,
                    options,
                    sectionPlane.Center.ConvertPoint(),
                    5,
                    item.P.ConvertPoint(),
                    new System.Windows.Point(-1, -1), "");
                item.Obj = viewModel;
                item.Action += PointAction;
                item.InitAction();
                item.InstanceInCanvasCircel.DrawInCanvas();
            }
        }

        private static void PointAction(object vm)
        {
            if (vm is ElectricColumnGeneralViewModel viewModel)
            {
                var sectionPlane = viewModel.ElectricColumnModel.SectionPlaneSelected;
                var itemsSelected = sectionPlane.Points.Where(x => x.IsSelected);
                var center = sectionPlane.Center.ConvertPoint();
                var ts = viewModel.Ts;
                if (itemsSelected.Count() == 2)
                {
                    var canvasBase = itemsSelected.First().InstanceInCanvasCircel.CanvasBase;
                    var scale = canvasBase.Scale;
                    var options = itemsSelected.First().InstanceInCanvasCircel.Options;
                    var vt = itemsSelected.First().InstanceInCanvasCircel.VectorInit;
                    var d = itemsSelected.First().InstanceInCanvasCircel.Diameter;

                    var p1 = itemsSelected.First().P.ConvertPoint();
                    var p2 = itemsSelected.Last().P.ConvertPoint();

                    var l = new LineCad(ts, AC.Database, itemsSelected.First().P, itemsSelected.Last().P);
                    viewModel.ElectricColumnModel.SectionPlaneSelected.LinesAdd.Add(l);

                    l.CanvasLine = new InstanceInCanvasLine(
                        canvasBase, options, center,
                        p1,
                        p2);
                    l.CanvasLine.Obj = viewModel;
                    l.CanvasLine.Delete += LinesAddDelete;
                    l.CanvasLine.DrawInCanvas();
                    foreach (var item in sectionPlane.Points)
                    {
                        if (item.IsSelected) item.ResetStatus();
                    }
                }
            }
        }

        private static void LinesAddDelete(object obj, object vm)
        {
            if (vm is ElectricColumnGeneralViewModel viewModel)
            {
                var sectionPlane = viewModel.ElectricColumnModel.SectionPlaneSelected;
                if (obj is System.Windows.Shapes.Line l)
                {
                    var lineAdd = sectionPlane.LinesAdd.Find(x => x.CanvasLine.UIElement.Uid == l.Uid);
                    if (lineAdd != null) sectionPlane.LinesAdd.Remove(lineAdd);
                    var parent = l.Parent as Canvas;
                    parent.Children.Remove(l);
                }
            }
        }
    }
}
