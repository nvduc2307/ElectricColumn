using CadDev.Tools.ElectricColumnGeneral.viewModels;
using CadDev.Utils.Compares;
using CadDev.Utils.Geometries;
using CadDev.Utils.Lines;
using System.Linq;

namespace CadDev.Tools.ElectricColumnGeneral.models
{
    public class ElectricColumnSwingModel
    {
        private ElectricColumnGeneralViewModel _viewModel;

        public IEnumerable<ElectricColumnSwing> SectionSwingRight { get; set; }
        public IEnumerable<ElectricColumnSwing> SectionSwingLeft { get; set; }

        public IEnumerable<LineCad> SwingRights { get; set; }
        public IEnumerable<LineCad> SwingLefts { get; set; }

        public ElectricColumnSwingModel(ElectricColumnGeneralViewModel viewModel)
        {
            _viewModel = viewModel;
            GetLinesSwing(out List<LineCad> swingRights, out List<LineCad> swingLefts);
            SwingRights = swingRights;
            SwingLefts = swingLefts;
        }

        private void GroupSwing(IEnumerable<LineCad> linesSwing)
        {
            var results = new List<ElectricColumnSwing>();
            var c = linesSwing.Count();
            var a = 0;
            var lCheck = linesSwing.FirstOrDefault();
            try
            {
                do
                {
                    var re = new List<LineCad>();
                    foreach (var item in linesSwing)
                    {
                        var dk1 = lCheck.StartP.IsSeem(item.StartP) || lCheck.StartP.IsSeem(item.StartP);
                        var dk2 = lCheck.EndP.IsSeem(item.StartP) || lCheck.EndP.IsSeem(item.StartP);
                        var dk3 = lCheck.StartP.IsSeem(item.EndP) || lCheck.StartP.IsSeem(item.EndP);
                        var dk4 = lCheck.EndP.IsSeem(item.EndP) || lCheck.EndP.IsSeem(item.EndP);
                        if (dk1 || dk2|| dk3|| dk4) re.Add(item);
                    }
                    a = re.Count;
                    if (a > 0)
                    {

                    }
                } while (a < c);
            }
            catch (Exception)
            {
            }
        }

        private void GetLinesSwing(out List<LineCad> swingRights, out List<LineCad> swingLefts)
        {
            swingRights = new List<LineCad>();
            swingLefts = new List<LineCad>();
            SectionSwingLeft = new List<ElectricColumnSwing>();
            var lines = _viewModel.ElectricColumnModel.SectionElevationMain;
            var facesRight = _viewModel.ElectricColumnGeneralModel.FacesMainFaceRight;
            var facesLeft = _viewModel.ElectricColumnGeneralModel.FacesMainFaceLeft;
            var vectorBase = _viewModel.ElectricColumnGeneralModel.VectorBaseMain;
            foreach (var line in lines)
            {
                var mid = line.MidP;
                foreach (var face in facesRight)
                {
                    var p = mid.RayPointToFace(vectorBase, face);
                    if (p != null)
                    {
                        var minz = Math.Min(face.BaseLine.StartP.Z, face.BaseLine.EndP.Z);
                        var maxz = Math.Max(face.BaseLine.StartP.Z, face.BaseLine.EndP.Z);
                        if (p.Z >= minz && p.Z <= maxz)
                        {
                            var vt = (mid - p).GetNormal();
                            if (vt.DotProduct(vectorBase) > 0.0001)
                            {
                                swingRights.Add(line);
                            }
                        }
                    }
                }

                foreach (var face in facesLeft)
                {
                    var p = mid.RayPointToFace(vectorBase, face);
                    if (p != null)
                    {
                        var minz = Math.Min(face.BaseLine.StartP.Z, face.BaseLine.EndP.Z);
                        var maxz = Math.Max(face.BaseLine.StartP.Z, face.BaseLine.EndP.Z);
                        if (p.Z >= minz && p.Z <= maxz)
                        {
                            var vt = (mid - p).GetNormal();
                            if (vt.DotProduct(vectorBase) < -0.0001)
                            {
                                swingLefts.Add(line);
                            }
                        }
                    }
                }
            }
            swingRights = swingRights.Distinct(new CompareLines()).ToList();
            swingLefts = swingLefts.Distinct(new CompareLines()).ToList();
        }

        public class ElectricColumnSwing
        {
            /// <summary>
            /// cac line canh tren mat cat.
            /// </summary>
            public IEnumerable<LineCad> LineCads { get; set; }
            public ElectricColumnSwing(IEnumerable<LineCad> lineCads)
            {
                LineCads = lineCads;
            }
        }
    }
}
