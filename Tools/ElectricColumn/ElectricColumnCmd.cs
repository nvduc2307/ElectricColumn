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
using CadDev.Utils.Selections;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using Cursors = System.Windows.Input.Cursors;

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
                    MessageBox.Show(ex.Message);
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
            center1 = new System.Windows.Point(center1.X * 0.01, center1.Y * 0.01);
            var vt = center1.GetVector(_viewModel.UiElement.ShortSectionCanvas.Center);
            vt = new System.Windows.Point(vt.X / 0.01, vt.Y / 0.01);

            var options = new OptionStyleInstanceInCanvas(
                StyleThicknessInCanvas.Thickness_1, 
                StyleDashInCanvas.Style_Solid, 
                StyleColorInCanvas.Color4, null);
            foreach (var item in _viewModel.ElectricColumn.LineFaceYs)
            {
                var p1 = item.StartP.ConvertPointOXZToOXY();
                var p2 = item.EndP.ConvertPointOXZToOXY();
                item.CanvasLine = new InstanceInCanvasLine(
                    _viewModel.UiElement.ShortSectionCanvas, 
                    options,
                    new System.Windows.Point(p1.X + vt.X, p1.Y + vt.Y),
                    new System.Windows.Point(p2.X + vt.X, p2.Y + vt.Y));
                if (item.CanvasLine.UIElement is System.Windows.Shapes.Line l) l.Cursor = Cursors.Hand;
                item.CanvasLine.DrawInCanvas();
            }
            foreach (var item in _viewModel.ElectricColumn.LineFaceEarYs)
            {
                var p1 = item.StartP.ConvertPointOXZToOXY();
                var p2 = item.EndP.ConvertPointOXZToOXY();
                item.CanvasLine = new InstanceInCanvasLine(
                    _viewModel.UiElement.ShortSectionCanvas,
                    options,
                    new System.Windows.Point(p1.X + vt.X, p1.Y + vt.Y),
                    new System.Windows.Point(p2.X + vt.X, p2.Y + vt.Y));
                if (item.CanvasLine.UIElement is System.Windows.Shapes.Line l) l.Cursor = Cursors.Hand;
                item.CanvasLine.DrawInCanvas();
            }
        }
    }
}
