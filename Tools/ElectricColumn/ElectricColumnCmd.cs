using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using CadDev.Extension.CustomAttribute;
using CadDev.Extension.ICommand;
using CadDev.Tools.ElectricColumn.viewModels;
using CadDev.Utils;
using CadDev.Utils.CanvasUtils;
using CadDev.Utils.CanvasUtils.Utils;
using CadDev.Utils.Geometries;
using CadDev.Utils.Lines;
using System.Windows.Controls;

namespace CadDev.Tools.ElectricColumn
{
    public class ElectricColumnCmd : ICadCommand
    {
        private ElectricColumnViewModel _viewModel;

        [CommandMethod("11")]
        [RibbonButton("NVD", "My Tools", "Electric Column", "Create 3d electric column")]
        public void Execute()
        {
            AC.GetInfomation();
            using (Transaction ts = AC.Database.TransactionManager.StartTransaction())
            {
                try
                {
                    using (DocumentLock documentLock = AC.DocumentCollection.MdiActiveDocument.LockDocument())
                    {
                        _viewModel = new ElectricColumnViewModel(ts, AC.Database);
                        _viewModel.MainView.Loaded += MainView_Loaded;
                        _viewModel.MainView.ShowDialog();
                    }
                    ts.Commit();
                }
                catch (System.Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }

        private void MainView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var elevationSectionCanvas = _viewModel.MainView.FindName("elevationSectionView") as Canvas;
            var shortSectionCanvas = _viewModel.MainView.FindName("shortSectionView") as Canvas;
            _viewModel.UiElement.ElevationSectionCanvas = new CanvasBase(elevationSectionCanvas, 0.01);
            _viewModel.UiElement.ShortSectionCanvas = new CanvasBase(shortSectionCanvas, 0.01);
            var midFaces = _viewModel.ElectricColumn.ElectricColumnFaceBase.MidFace.ToList();
            var center1 = midFaces[0].MidPoint(midFaces[2]).ConvertPointOXZToOXY();
            var center = _viewModel.ElectricColumn.ElectricColumnShortSectionSelected.Center.ConvertPoint();
            var options = new OptionStyleInstanceInCanvas(
                StyleThicknessInCanvas.Thickness_1,
                StyleDashInCanvas.Style_Solid,
                StyleColorInCanvas.Color4, null);
            var options1 = new OptionStyleInstanceInCanvas(
                StyleThicknessInCanvas.Thickness_2,
                StyleDashInCanvas.Style_Solid,
                StyleColorInCanvas.Color4,
                StyleColorInCanvas.Color4);
            foreach (var item in _viewModel.ElectricColumn.LineFaceYs)
            {
                item.CanvasLine = new InstanceInCanvasLine(
                    _viewModel.UiElement.ElevationSectionCanvas,
                    options,
                    center1,
                    item.StartP.ConvertPointOXZToOXY(),
                    item.EndP.ConvertPointOXZToOXY());
                item.CanvasLine.DrawInCanvas();
            }
            foreach (var item in _viewModel.ElectricColumn.LineFaceEarYs)
            {
                item.CanvasLine = new InstanceInCanvasLine(
                    _viewModel.UiElement.ElevationSectionCanvas,
                    options,
                    center1,
                    item.StartP.ConvertPointOXZToOXY(),
                    item.EndP.ConvertPointOXZToOXY());
                item.CanvasLine.DrawInCanvas();
            }

            foreach (var p in _viewModel.ElectricColumn.ElectricColumnShortSectionSelected.Points)
            {
                var p1 = p.P.ConvertPoint();
                p.InstanceInCanvasCircel = new InstanceInCanvasCircel(
                    _viewModel.UiElement.ShortSectionCanvas,
                    options1,
                    center,
                    10,
                    p1,
                    new System.Windows.Point(-1, -1),
                    "");
                p.Action += PointAction;
                p.InitAction();
                p.InstanceInCanvasCircel.DrawInCanvas();
            }
        }

        private void PointAction()
        {
            var itemsSelected = _viewModel.ElectricColumn.ElectricColumnShortSectionSelected.Points.Where(x => x.IsSelected);
            var center = _viewModel.ElectricColumn.ElectricColumnShortSectionSelected.Center.ConvertPoint();
            var ts = _viewModel._ts;
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
                _viewModel.ElectricColumn.ElectricColumnShortSectionSelected.LinesAdd.Add(l);

                l.CanvasLine = new InstanceInCanvasLine(
                    canvasBase, options, center,
                    new System.Windows.Point(p1.X + vt.X * d / (2 * scale), p1.Y + vt.Y * d / (2 * scale)),
                    new System.Windows.Point(p2.X + vt.X * d / (2 * scale), p2.Y + vt.Y * d / (2 * scale)));
                l.CanvasLine.Delete += LinesAddDelete;
                l.CanvasLine.DrawInCanvas();
                foreach (var item in _viewModel.ElectricColumn.ElectricColumnShortSectionSelected.Points)
                {
                    if (item.IsSelected) item.ResetStatus();
                }
            }
        }

        private void LinesAddDelete(object obj)
        {
            if (obj is System.Windows.Shapes.Line l)
            {
                var dm = _viewModel.ElectricColumn.ElectricColumnShortSectionSelected.LinesAdd.Find(x => x.CanvasLine.UIElement.Uid == l.Uid);
                if (dm != null) { _viewModel.ElectricColumn.ElectricColumnShortSectionSelected.LinesAdd.Remove(dm); }
                var parent = l.Parent as Canvas;
                parent.Children.Remove(l);
            }
        }
    }
}
