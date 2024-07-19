using CadDev.Tools.ElectricColumnGeneral.viewModels;
using CadDev.Utils.Compares;
using CadDev.Utils.Geometries;
using CadDev.Utils.Lines;

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
            SectionSwingRight = GroupSwing(swingRights);
            SectionSwingLeft = GroupSwing(swingLefts);
        }

        private List<ElectricColumnSwing> GroupSwing(List<LineCad> linesSwing)
        {
            linesSwing = linesSwing.OrderBy(x => x.MidP.Z).ToList();
            var results = new List<ElectricColumnSwing>();
            var c = linesSwing.Count();
            try
            {
                var lss = new List<List<LineCad>>();
                var ls = new List<LineCad>();
                for (int i = 0; i < c; i++)
                {
                    var lsnew = new List<LineCad>();
                    for (int j = i; j < c; j++)
                    {
                        if (linesSwing[i].IsContinuteLine(linesSwing[j]))
                            if (!ls.Any(x => x.IsSeem(linesSwing[j]))) lsnew.Add(linesSwing[j]);
                    }

                    if (lsnew.Count > 0)
                    {
                        ls.AddRange(lsnew);
                    }
                    else
                    {
                        results.Add(new ElectricColumnSwing(ls));
                        ls = new List<LineCad>();
                    }
                }
            }
            catch (Exception)
            {
            }

            return results;
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
                    if (p != null && p.X != double.NaN)
                    {
                        var minz = Math.Round(Math.Min(face.BaseLine.StartP.Z, face.BaseLine.EndP.Z), 4);
                        var maxz = Math.Round(Math.Max(face.BaseLine.StartP.Z, face.BaseLine.EndP.Z), 4);
                        var dk1 = p.Z.IsGreateOrEqual(minz);
                        var dk2 = p.Z.IsLessOrEqual(maxz);
                        if (dk1 && dk2)
                        {
                            var vt = (mid - p).GetNormal();
                            if (vt.DotProduct(vectorBase).IsGreate(0)) swingRights.Add(line);
                        }
                    }
                }

                foreach (var face in facesLeft)
                {
                    var p = mid.RayPointToFace(vectorBase, face);
                    if (p != null && p.X != double.NaN)
                    {
                        var minz = Math.Round(Math.Min(face.BaseLine.StartP.Z, face.BaseLine.EndP.Z), 4);
                        var maxz = Math.Round(Math.Max(face.BaseLine.StartP.Z, face.BaseLine.EndP.Z), 4);
                        var dk1 = p.Z.IsGreateOrEqual(minz);
                        var dk2 = p.Z.IsLessOrEqual(maxz);
                        if (dk1 && dk2)
                        {
                            var vt = (mid - p).GetNormal();
                            if (vt.DotProduct(vectorBase).IsLess(0)) swingLefts.Add(line);
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
