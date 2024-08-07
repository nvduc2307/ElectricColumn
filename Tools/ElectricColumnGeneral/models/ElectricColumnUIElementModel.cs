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
        private string _swingPlaneCanvasTopName = "SwingPlaneCanvasTop";
        private string _swingPlaneCanvasBotName = "SwingPlaneCanvasBot";
        private ElectricColumnGeneralViewModel _viewModel;
        public ElectricColumnGeneralView MainView { get; set; }
        public CanvasBase SectionElevationCanvas { get; set; }
        public CanvasBase SectionPlaneCanvas { get; set; }
        public CanvasBase SwingPlaneTopCanvas { get; set; }
        public CanvasBase SwingPlaneBotCanvas { get; set; }
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
                var swingPlaneCanvasTopName = MainView.FindName(_swingPlaneCanvasTopName) as Canvas;
                var swingPlaneCanvasBotName = MainView.FindName(_swingPlaneCanvasBotName) as Canvas;
                SectionElevationCanvas = new CanvasBase(sectionElevationCanvas, 0.008);
                SectionPlaneCanvas = new CanvasBase(sectionPlaneCanvas, 0.008);
                SwingPlaneTopCanvas = new CanvasBase(swingPlaneCanvasTopName, 0.01);
                SwingPlaneBotCanvas = new CanvasBase(swingPlaneCanvasBotName, 0.01);
                DrawingSectionElevation(SectionElevationCanvas, _viewModel.ElectricColumnGeneralModel);
                DrawSectionPlan(SectionPlaneCanvas, _viewModel.ElectricColumnGeneralModel);
                DrawSwingTop(SwingPlaneTopCanvas, _viewModel.ElectricColumnGeneralModel);
                DrawSwingBot(SwingPlaneBotCanvas, _viewModel.ElectricColumnGeneralModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void UpdateStatusSectionSelectedAtElevation(ElectricColumnGeneralModel electricColumnGeneralModel)
        {
            var sections = electricColumnGeneralModel.SectionPlanes;
            var sectionIndexOf = electricColumnGeneralModel.SectionPlaneSelected;
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

        public static void UpdateStatusSwingSelectedAtElevation(ElectricColumnGeneralModel electricColumnGeneralModel)
        {
            var swings = electricColumnGeneralModel
                .ElectricColumnSwingModel
                .ElectricColumnTotalSwings
                .Select(x => x.LinesSection)
                .Aggregate((a, b) => a.Concat(b))
                .ToList();
            var sectionIndexOf = electricColumnGeneralModel.ElectricColumnSwingModel.ElectricColumnSwingSelected;
            foreach (var lc in swings)
            {
                lc.CanvasLine.ResetStatus();
            }

            foreach (var lc in sectionIndexOf.LinesSection)
            {
                lc.CanvasLine.SelectedStatus();
            }
        }

        public static void DrawingSectionElevation(CanvasBase canvasBase, ElectricColumnGeneralModel electricColumnGeneralModel)
        {
            var options = new OptionStyleInstanceInCanvas(
                StyleThicknessInCanvas.Thickness_1,
                StyleDashInCanvas.Style_Solid,
                StyleColorInCanvas.Color4,
                StyleColorInCanvas.Color4);
            canvasBase.Parent.Children.Clear();
            var sectionElevationMain = electricColumnGeneralModel.LinesBodyMain;
            var center = sectionElevationMain.GetPoints()
                .Select(x => x.ConvertPointOXZToOXY()).GetCenterCanvas();

            var swings = electricColumnGeneralModel
                .ElectricColumnSwingModel
                .ElectricColumnTotalSwings
                .Select(x => x.LinesSection)
                .Aggregate((a, b) => a.Concat(b))
                .ToList();

            foreach (var item in sectionElevationMain.Concat(swings))
            {
                item.CanvasLine = new InstanceInCanvasLine(
                    canvasBase,
                    options,
                    center,
                    item.StartP.ConvertPointOXZToOXY(),
                    item.EndP.ConvertPointOXZToOXY());
                item.CanvasLine.Obj = electricColumnGeneralModel;
                item.CanvasLine.DrawInCanvas();

                if (item.StartP.Z.IsEqual(item.EndP.Z))
                {
                    var elevation = item.StartP.Z;
                    var section = electricColumnGeneralModel.SectionPlanes.FirstOrDefault(x => x.Elevation.IsEqual(elevation));
                    if (section != null && !swings.Any(x => x.IsSeem(item))) section.LinesAtElevation.Add(item);
                }
            }
            UpdateStatusSwingSelectedAtElevation(electricColumnGeneralModel);
            UpdateStatusSectionSelectedAtElevation(electricColumnGeneralModel);
        }

        public static void DrawSectionPlan(CanvasBase canvasBase, ElectricColumnGeneralModel electricColumnGeneralModel)
        {
            var options = new OptionStyleInstanceInCanvas(
                StyleThicknessInCanvas.Thickness_1,
                StyleDashInCanvas.Style_Solid,
                StyleColorInCanvas.Color4,
                StyleColorInCanvas.Color4);
            canvasBase.Parent.Children.Clear();
            var sectionPlane = electricColumnGeneralModel.SectionPlaneSelected;

            foreach (var item in sectionPlane.Lines)
            {
                item.CanvasLine = new InstanceInCanvasLine(
                    canvasBase,
                    options,
                    sectionPlane.Center.ConvertPoint(),
                    item.StartP.ConvertPoint(),
                    item.EndP.ConvertPoint());
                item.CanvasLine.Obj = electricColumnGeneralModel;
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
                item.CanvasLine.Obj = electricColumnGeneralModel;
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
                item.Obj = electricColumnGeneralModel;
                item.Action += PointAction;
                item.InitAction();
                item.InstanceInCanvasCircel.DrawInCanvas();
            }
        }

        public static void DrawSwingTop(CanvasBase canvasBase, ElectricColumnGeneralModel electricColumnGeneralModel)
        {
            var options = new OptionStyleInstanceInCanvas(
                StyleThicknessInCanvas.Thickness_1,
                StyleDashInCanvas.Style_Solid,
                StyleColorInCanvas.Color4,
                StyleColorInCanvas.Color4);
            canvasBase.Parent.Children.Clear();
            var swingSelected = electricColumnGeneralModel.ElectricColumnSwingModel.ElectricColumnSwingSelected;

            foreach (var item in swingSelected.LinesTop)
            {
                item.CanvasLine = new InstanceInCanvasLine(
                    canvasBase,
                    options,
                    swingSelected.Center.ConvertPoint(),
                    item.StartP.ConvertPoint(),
                    item.EndP.ConvertPoint());
                item.CanvasLine.Obj = electricColumnGeneralModel;
                item.CanvasLine.DrawInCanvas();
            }

            foreach (var item in swingSelected.LinesTopAdd)
            {
                item.CanvasLine = new InstanceInCanvasLine(
                    canvasBase,
                    options,
                    swingSelected.Center.ConvertPoint(),
                    item.StartP.ConvertPoint(),
                    item.EndP.ConvertPoint());
                item.CanvasLine.Obj = electricColumnGeneralModel;
                item.CanvasLine.Delete += LinesAddDelete;
                item.CanvasLine.DrawInCanvas();
            }

            foreach (var item in swingSelected.PointsTop)
            {
                item.InstanceInCanvasCircel = new InstanceInCanvasCircel(
                    canvasBase,
                    options,
                    swingSelected.Center.ConvertPoint(),
                    5,
                    item.P.ConvertPoint(),
                    new System.Windows.Point(-1, -1), "");
                item.Obj = electricColumnGeneralModel;
                item.Action += PointsSwingTopAction;
                item.InitAction();
                item.InstanceInCanvasCircel.DrawInCanvas();
            }
        }
        public static void DrawSwingBot(CanvasBase canvasBase, ElectricColumnGeneralModel electricColumnGeneralModel)
        {
            var options = new OptionStyleInstanceInCanvas(
                StyleThicknessInCanvas.Thickness_1,
                StyleDashInCanvas.Style_Solid,
                StyleColorInCanvas.Color4,
                StyleColorInCanvas.Color4);
            canvasBase.Parent.Children.Clear();
            var swingSelected = electricColumnGeneralModel.ElectricColumnSwingModel.ElectricColumnSwingSelected;

            foreach (var item in swingSelected.LinesBot)
            {
                item.CanvasLine = new InstanceInCanvasLine(
                    canvasBase,
                    options,
                    swingSelected.Center.ConvertPoint(),
                    item.StartP.ConvertPoint(),
                    item.EndP.ConvertPoint());
                item.CanvasLine.Obj = electricColumnGeneralModel;
                item.CanvasLine.DrawInCanvas();
            }

            foreach (var item in swingSelected.LinesBotAdd)
            {
                item.CanvasLine = new InstanceInCanvasLine(
                    canvasBase,
                    options,
                    swingSelected.Center.ConvertPoint(),
                    item.StartP.ConvertPoint(),
                    item.EndP.ConvertPoint());
                item.CanvasLine.Obj = electricColumnGeneralModel;
                item.CanvasLine.Delete += LinesAddDelete;
                item.CanvasLine.DrawInCanvas();
            }

            foreach (var item in swingSelected.PointsBot)
            {
                item.InstanceInCanvasCircel = new InstanceInCanvasCircel(
                    canvasBase,
                    options,
                    swingSelected.Center.ConvertPoint(),
                    5,
                    item.P.ConvertPoint(),
                    new System.Windows.Point(-1, -1), "");
                item.Obj = electricColumnGeneralModel;
                item.Action += PointsSwingBotAction;
                item.InitAction();
                item.InstanceInCanvasCircel.DrawInCanvas();
            }
        }

        private static void PointsSwingBotAction(object vm)
        {
            if (vm is ElectricColumnGeneralModel electricColumnGeneralModel)
            {
                var swingSelected = electricColumnGeneralModel.ElectricColumnSwingModel.ElectricColumnSwingSelected;
                var itemsSelected = swingSelected.PointsBot.Where(x => x.IsSelected);
                var center = swingSelected.Center.ConvertPoint();
                var ts = electricColumnGeneralModel._ts;
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
                    swingSelected.LinesBotAdd.Add(l);

                    l.CanvasLine = new InstanceInCanvasLine(
                        canvasBase, options, center,
                        p1,
                        p2);
                    l.CanvasLine.Obj = electricColumnGeneralModel;
                    l.CanvasLine.Delete += LinesSwingBotAddDelete;
                    l.CanvasLine.DrawInCanvas();
                    foreach (var item in swingSelected.PointsBot)
                    {
                        if (item.IsSelected) item.ResetStatus();
                    }
                }
            }
        }

        private static void PointsSwingTopAction(object vm)
        {
            if (vm is ElectricColumnGeneralModel electricColumnGeneralModel)
            {
                var swingSelected = electricColumnGeneralModel.ElectricColumnSwingModel.ElectricColumnSwingSelected;
                var itemsSelected = swingSelected.PointsTop.Where(x => x.IsSelected);
                var center = swingSelected.Center.ConvertPoint();
                var ts = electricColumnGeneralModel._ts;
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
                    swingSelected.LinesTopAdd.Add(l);

                    l.CanvasLine = new InstanceInCanvasLine(
                        canvasBase, options, center,
                        p1,
                        p2);
                    l.CanvasLine.Obj = electricColumnGeneralModel;
                    l.CanvasLine.Delete += LinesSwingTopAddDelete;
                    l.CanvasLine.DrawInCanvas();
                    foreach (var item in swingSelected.PointsTop)
                    {
                        if (item.IsSelected) item.ResetStatus();
                    }
                }
            }
        }

        private static void PointAction(object vm)
        {
            if (vm is ElectricColumnGeneralModel electricColumnGeneralModel)
            {
                var sectionPlane = electricColumnGeneralModel.SectionPlaneSelected;
                var itemsSelected = sectionPlane.Points.Where(x => x.IsSelected);
                var center = sectionPlane.Center.ConvertPoint();
                var ts = electricColumnGeneralModel._ts;
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
                    electricColumnGeneralModel.SectionPlaneSelected.LinesAdd.Add(l);

                    l.CanvasLine = new InstanceInCanvasLine(
                        canvasBase, options, center,
                        p1,
                        p2);
                    l.CanvasLine.Obj = electricColumnGeneralModel;
                    l.CanvasLine.Delete += LinesAddDelete;
                    l.CanvasLine.DrawInCanvas();
                    foreach (var item in sectionPlane.Points)
                    {
                        if (item.IsSelected) item.ResetStatus();
                    }
                }
            }
        }

        private static void LinesSwingBotAddDelete(object obj, object vm)
        {
            if (vm is ElectricColumnGeneralModel electricColumnGeneralModel)
            {
                var swingSelected = electricColumnGeneralModel.ElectricColumnSwingModel.ElectricColumnSwingSelected;
                if (obj is System.Windows.Shapes.Line l)
                {
                    var lineAdd = swingSelected.LinesBotAdd.Find(x => x.CanvasLine.UIElement.Uid == l.Uid);
                    if (lineAdd != null) swingSelected.LinesBotAdd.Remove(lineAdd);
                    var parent = l.Parent as Canvas;
                    parent.Children.Remove(l);
                }
            }
        }
        private static void LinesSwingTopAddDelete(object obj, object vm)
        {
            if (vm is ElectricColumnGeneralModel electricColumnGeneralModel)
            {
                var swingSelected = electricColumnGeneralModel.ElectricColumnSwingModel.ElectricColumnSwingSelected;
                if (obj is System.Windows.Shapes.Line l)
                {
                    var lineAdd = swingSelected.LinesTopAdd.Find(x => x.CanvasLine.UIElement.Uid == l.Uid);
                    if (lineAdd != null) swingSelected.LinesTopAdd.Remove(lineAdd);
                    var parent = l.Parent as Canvas;
                    parent.Children.Remove(l);
                }
            }
        }

        private static void LinesAddDelete(object obj, object vm)
        {
            if (vm is ElectricColumnGeneralModel electricColumnGeneralModel)
            {
                var sectionPlane = electricColumnGeneralModel.SectionPlaneSelected;
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
