using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CadDev.Tools.ElectricColumnGeneral.viewModels;
using CadDev.Utils.Compares;
using CadDev.Utils.Geometries;
using CadDev.Utils.Lines;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CadDev.Tools.ElectricColumnGeneral.models
{
    /// <summary>
    /// LinesSouth các line của cột điện ở phía bắc
    /// LinesEarth các line của cột điện ở phía đông
    /// LinesNorth các line của cột điện ở phía nam
    /// LinesWest các line của cột điện ở phía tây
    /// SectionPlanes là các line nằm trên cùng mặt phẳng (mặt bằng) - tạo thành các mặt cắt ngang
    /// </summary>
    public class ElectricColumnModel : ObservableObject
    {
        private Transaction _ts;
        private Database _db;
        private ElectricColumnGeneralViewModel _viewModel;
        private ElectricColumnSectionPlane _sectionPlaneSelected;

        public List<LineCad> LinesSouth { get; set; }
        public List<LineCad> LinesEarth { get; set; }
        public List<LineCad> LinesNorth { get; set; }
        public List<LineCad> LinesWest { get; set; }

        public List<LineCad> SectionElevationMain { get; set; }
        public List<LineCad> SectionElevationSub { get; set; }

        public List<ElectricColumnSectionPlane> SectionPlanes { get; set; }
        public ElectricColumnSectionPlane SectionPlaneSelected
        {
            get { return _sectionPlaneSelected; } 
            set
            {
                _sectionPlaneSelected = value;
                OnPropertyChanged();
                if (_viewModel.UIElement != null)
                {
                    ElectricColumnUIElementModel.DrawSectionPlan(_viewModel.UIElement.SectionPlaneCanvas, _viewModel);
                    ElectricColumnUIElementModel.UpdateStatusSectionSelectedAtElevation(_viewModel.UIElement.SectionPlaneCanvas, _viewModel);
                }
            }
        }

        public ElectricColumnModel(Transaction ts, Database db, ElectricColumnGeneralViewModel viewModel)
        {
            _ts = ts;
            _db = db;
            _viewModel = viewModel;
            LinesSouth = viewModel.ElectricColumnGeneralModel.LinesSouth;
            LinesEarth = viewModel.ElectricColumnGeneralModel.LinesEarth;
            LinesNorth = viewModel.ElectricColumnGeneralModel.LinesNorth;
            LinesWest = viewModel.ElectricColumnGeneralModel.LinesWest;
            SectionElevationMain = viewModel.ElectricColumnGeneralModel.LinesMain;
            SectionElevationSub = viewModel.ElectricColumnGeneralModel.LinesSub;
            SectionPlanes = GetSectionPlanes();
            SectionPlaneSelected = SectionPlanes.FirstOrDefault();
        }

        public List<ElectricColumnSectionPlane> GetSectionPlanes()
        {
            var result = new List<ElectricColumnSectionPlane>();
            var allLines = new List<LineCad>();
            allLines.AddRange(LinesSouth);
            allLines.AddRange(LinesEarth);
            allLines.AddRange(LinesNorth);
            allLines.AddRange(LinesWest);
            try
            {
                var dms = allLines
                    .Where(x => Math.Round(x.Dir.Distance(), 0) > 0)
                    .Where(x=>x.Dir.DotProduct(Vector3d.ZAxis).IsEqual(0))
                    .GroupBy(x => x, new CompareLinesOnPlane())
                    .OrderBy(x => Math.Round(x.First().MidP.Z, 2));

                result = allLines
                    .Where(x=>Math.Round(x.Dir.Distance(), 0) > 0)
                    .Where(x=>x.Dir.DotProduct(Vector3d.ZAxis).IsEqual(0))
                    .GroupBy(x => x, new CompareLinesOnPlane())
                    .Where(x=>x.Count() >= 2)
                    .OrderBy(x=>x.First().MidP.Z)
                    .Select((x, index) => new ElectricColumnSectionPlane(index, x.ToList()))
                    .ToList();
            }
            catch (Exception)
            {
            }
            return result;
        }
    }
}
