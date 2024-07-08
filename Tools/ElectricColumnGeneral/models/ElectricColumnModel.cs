using Autodesk.AutoCAD.DatabaseServices;
using CadDev.Tools.ElectricColumnGeneral.viewModels;
using CadDev.Utils.Compares;
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

        public List<ElectricColumnSectionPlane> SectionPlanes { get; set; }
        public ElectricColumnSectionPlane SectionPlaneSelected
        {
            get { return _sectionPlaneSelected; } 
            set
            {
                _sectionPlaneSelected = value;
                OnPropertyChanged();
                if (_viewModel.UIElement != null) ElectricColumnUIElementModel.DrawSectionPlan(_viewModel.UIElement.SectionPlaneCanvas, _viewModel);
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
                result = allLines
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
