using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CadDev.Tools.ElectricColumnGeneral.Iservices;
using CadDev.Tools.ElectricColumnGeneral.viewModels;
using CadDev.Utils.Compares;
using CadDev.Utils.Faces;
using CadDev.Utils.Geometries;
using CadDev.Utils.Lines;
using System.Linq;

namespace CadDev.Tools.ElectricColumnGeneral.models
{
    public class ElectricColumnGeneralPlan : IElectricColumnGeneralPlanService
    {
        public Transaction TS { get; set; }
        public Database DB { get; set; }
        public List<LineCad> LinesSouth { get; set; }
        public List<LineCad> LinesEarth { get; set; }
        public List<LineCad> LinesNorth { get; set; }
        public List<LineCad> LinesWest { get; set; }
        public List<ElectricColumnSectionPlane> SectionPlanes { get; set; }
        public ElectricColumnSwingModel ElectricColumnSwingModel { get; set; }
        public double WidthBoxMainBody { get; set; }
        public double HeightBoxMainBody { get; set; }
        public FaceCad FaceX { get; set; }
        public FaceCad FaceY { get; set; }

        public List<LineInPlan> LinesMainFacesOnPlan { get; set; }
        public List<LineInPlan> LinesSubFacesOnPlan { get; set; }

        public ElectricColumnGeneralPlan(
            Transaction ts,
            Database db,
            List<LineCad> linesSouth,
            List<LineCad> linesEarth,
            List<LineCad> linesNorth,
            List<LineCad> linesWest,
            List<ElectricColumnSectionPlane> sectionPlanes,
            ElectricColumnSwingModel electricColumnSwingModel)
        {
            TS = ts;
            DB = db;
            FaceX = new FaceCad(Vector3d.YAxis, new Point3d());
            FaceY = new FaceCad(Vector3d.XAxis, new Point3d());
            LinesSouth = linesSouth;
            LinesEarth = linesEarth;
            LinesNorth = linesNorth;
            LinesWest = linesWest;
            SectionPlanes = sectionPlanes;
            ElectricColumnSwingModel = electricColumnSwingModel;
            NumberingFrame(); // danh so cac thanh o trong cot dien
            WidthBoxMainBody = GetWidthBoxMainBody(out double heigthBox);
            HeightBoxMainBody = heigthBox;

            LinesMainFacesOnPlan = GetLineMainOnPlan();
            LinesSubFacesOnPlan = GetLineSubOnPlan();
            GetLineOnSection();
        }

        public void CreateMainSection()
        {
            throw new NotImplementedException();
        }

        public void CreateSubSection()
        {
            throw new NotImplementedException();
        }

        public void CreateSwingOnMainSection()
        {
            throw new NotImplementedException();
        }

        public void CreateSection()
        {
            throw new NotImplementedException();
        }

        private double GetWidthBoxMainBody(out double heightBox)
        {
            heightBox = 0.0;
            var ps = LinesSouth.GetPoints().ToList();
            foreach (var electricColumnSwingsLeft in ElectricColumnSwingModel.ElectricColumnSwingsLeft)
            {
                ps.AddRange(electricColumnSwingsLeft.LinesRight.GetPoints());
            }
            foreach (var electricColumnSwingsLeft in ElectricColumnSwingModel.ElectricColumnSwingsRight)
            {
                ps.AddRange(electricColumnSwingsLeft.LinesRight.GetPoints());
            }

            var minx = ps.Min(x => x.X);
            var maxx = ps.Max(x => x.X);
            var minz = ps.Max(x => x.Z);
            var maxz = ps.Max(x => x.Z);
            heightBox = maxz - minz;
            return maxx - minx;
        }
        private List<LineCad> GetLinesBodyMainOnPlan()
        {
            var result = new List<LineCad>();
            
            return result;
        }

        private List<LineCad> GetLinesSwingOnPlan()
        {
            var result = new List<LineCad>();
            
            return result;
        }

        private List<LineCad> GetLinesBodySubOnPlan()
        {
            var extent = 1.2 * WidthBoxMainBody;
            var result = new List<LineCad>();
            
            return result;
        }

        private void NumberingFrame()
        {
            var c = 1;
            foreach (var l in LinesSouth)
            {
                l.Id = c;
                c++;
            }
            foreach (var l in LinesEarth)
            {
                l.Id = c;
                c++;
            }
            foreach (var l in LinesNorth)
            {
                l.Id = c;
                c++; 
            }
            foreach (var l in LinesWest)
            {
                l.Id = c;
                c++;
            }
            foreach (var electricColumnSwingsLeft in ElectricColumnSwingModel.ElectricColumnSwingsLeft)
            {
                foreach (var l in electricColumnSwingsLeft.LinesRight)
                {
                    l.Id = c;
                    c++;
                }
                foreach (var l in electricColumnSwingsLeft.LinesLeft)
                {
                    l.Id = c;
                    c++;
                }
                foreach (var l in electricColumnSwingsLeft.LinesTopAdd)
                {
                    l.Id = c;
                    c++;
                }
                foreach (var l in electricColumnSwingsLeft.LinesBotAdd)
                {
                    l.Id = c;
                    c++;
                }
            }
            foreach (var electricColumnSwingsRight in ElectricColumnSwingModel.ElectricColumnSwingsRight)
            {
                foreach (var l in electricColumnSwingsRight.LinesRight)
                {
                    l.Id = c;
                    c++;
                }
                foreach (var l in electricColumnSwingsRight.LinesLeft)
                {
                    l.Id = c;
                    c++;
                }
                foreach (var l in electricColumnSwingsRight.LinesTopAdd)
                {
                    l.Id = c;
                    c++;
                }
                foreach (var l in electricColumnSwingsRight.LinesBotAdd)
                {
                    l.Id = c;
                    c++;
                }
            }
        }

        private List<LineInPlan> GetLineMainOnPlan()
        {
            var results = new List<LineInPlan>();
            try
            {
                foreach (var item in LinesSouth)
                {
                    var p1 = item.StartP.RayPointToFace(Vector3d.YAxis, FaceX);
                    var p2 = item.EndP.RayPointToFace(Vector3d.YAxis, FaceX);
                    var l = new LineCad(TS, DB, new Point3d(p1.X, p1.Z, 0), new Point3d(p2.X, p2.Z, 0));
                    l.Id = item.Id;
                    results.Add(new LineInPlan(l));
                }
                foreach (var electricColumnSwingsLeft in ElectricColumnSwingModel.ElectricColumnSwingsLeft)
                {
                    foreach (var item in electricColumnSwingsLeft.LinesRight)
                    {
                        var p1 = item.StartP.RayPointToFace(Vector3d.YAxis, FaceX);
                        var p2 = item.EndP.RayPointToFace(Vector3d.YAxis, FaceX);
                        var l = new LineCad(TS, DB, new Point3d(p1.X, p1.Z, 0), new Point3d(p2.X, p2.Z, 0));
                        l.Id = item.Id;
                        results.Add(new LineInPlan(l));
                    }
                }
                foreach (var electricColumnSwingsLeft in ElectricColumnSwingModel.ElectricColumnSwingsRight)
                {
                    foreach (var item in electricColumnSwingsLeft.LinesRight)
                    {
                        var p1 = item.StartP.RayPointToFace(Vector3d.YAxis, FaceX);
                        var p2 = item.EndP.RayPointToFace(Vector3d.YAxis, FaceX);
                        var l = new LineCad(TS, DB, new Point3d(p1.X, p1.Z, 0), new Point3d(p2.X, p2.Z, 0));
                        l.Id = item.Id;
                        results.Add(new LineInPlan(l));
                    }
                }
            }
            catch (Exception)
            {
            }
            return results;
        }

        private List<LineInPlan> GetLineSubOnPlan()
        {
            var results = new List<LineInPlan>();
            var extent = WidthBoxMainBody * 1.2;
            try
            {
                foreach (var item in LinesEarth)
                {
                    var p1 = item.StartP.RayPointToFace(Vector3d.XAxis, FaceY);
                    var p2 = item.EndP.RayPointToFace(Vector3d.XAxis, FaceY);
                    var l = new LineCad(TS, DB, new Point3d(p1.Y + extent, p1.Z, 0), new Point3d(p2.Y + extent, p2.Z, 0));
                    l.Id = item.Id;
                    results.Add(new LineInPlan(l));
                }
            }
            catch (Exception)
            {
            }
            return results;
        }

        private void GetLineOnSection()
        {
            var results = new List<LineInPlan>();
            foreach(var section in SectionPlanes)
            {
                section.LinesOnSection = section.Lines
                    .Concat(section.LinesAdd)
                    .Select(x=>
                    {
                        var p1 = new Point3d(x.StartP.X, x.StartP.Y, 0);
                        var p2 = new Point3d(x.EndP.X, x.EndP.Y, 0);
                        return new LineInPlan(new LineCad(TS, DB, p1, p2));
                    })
                    .ToList();
                foreach (var electricColumnSwingsLeft in ElectricColumnSwingModel.ElectricColumnSwingsLeft)
                {
                    foreach (var l in electricColumnSwingsLeft.LinesRight)
                    {
                        var p1z = l.StartP.Z;
                        var p2z = l.EndP.Z;
                        if (p1z.IsEqual(section.Elevation) && p2z.IsEqual(section.Elevation))
                        {
                            var p1 = new Point3d(l.StartP.X, l.StartP.Y, 0);
                            var p2 = new Point3d(l.EndP.X, l.EndP.Y, 0);
                            section.LinesOnSection.Add(new LineInPlan(new LineCad(TS, DB, p1, p2)));
                        }
                    }
                    foreach (var l in electricColumnSwingsLeft.LinesLeft)
                    {
                        var p1z = l.StartP.Z;
                        var p2z = l.EndP.Z;
                        if (p1z.IsEqual(section.Elevation) && p2z.IsEqual(section.Elevation))
                        {
                            var p1 = new Point3d(l.StartP.X, l.StartP.Y, 0);
                            var p2 = new Point3d(l.EndP.X, l.EndP.Y, 0);
                            section.LinesOnSection.Add(new LineInPlan(new LineCad(TS, DB, p1, p2)));
                        }
                    }
                    foreach (var l in electricColumnSwingsLeft.LinesTopAdd)
                    {
                        var p1z = l.StartP.Z;
                        var p2z = l.EndP.Z;
                        if (p1z.IsEqual(section.Elevation) && p2z.IsEqual(section.Elevation))
                        {
                            var p1 = new Point3d(l.StartP.X, l.StartP.Y, 0);
                            var p2 = new Point3d(l.EndP.X, l.EndP.Y, 0);
                            section.LinesOnSection.Add(new LineInPlan(new LineCad(TS, DB, p1, p2)));
                        }
                    }
                    foreach (var l in electricColumnSwingsLeft.LinesBotAdd)
                    {
                        var p1z = l.StartP.Z;
                        var p2z = l.EndP.Z;
                        if (p1z.IsEqual(section.Elevation) && p2z.IsEqual(section.Elevation))
                        {
                            var p1 = new Point3d(l.StartP.X, l.StartP.Y, 0);
                            var p2 = new Point3d(l.EndP.X, l.EndP.Y, 0);
                            section.LinesOnSection.Add(new LineInPlan(new LineCad(TS, DB, p1, p2)));
                        }
                    }
                }
                foreach (var electricColumnSwingsRight in ElectricColumnSwingModel.ElectricColumnSwingsRight)
                {
                    foreach (var l in electricColumnSwingsRight.LinesRight)
                    {
                        var p1z = l.StartP.Z;
                        var p2z = l.EndP.Z;
                        if (p1z.IsEqual(section.Elevation) && p2z.IsEqual(section.Elevation))
                        {
                            var p1 = new Point3d(l.StartP.X, l.StartP.Y, 0);
                            var p2 = new Point3d(l.EndP.X, l.EndP.Y, 0);
                            section.LinesOnSection.Add(new LineInPlan(new LineCad(TS, DB, p1, p2)));
                        }
                    }
                    foreach (var l in electricColumnSwingsRight.LinesLeft)
                    {
                        var p1z = l.StartP.Z;
                        var p2z = l.EndP.Z;
                        if (p1z.IsEqual(section.Elevation) && p2z.IsEqual(section.Elevation))
                        {
                            var p1 = new Point3d(l.StartP.X, l.StartP.Y, 0);
                            var p2 = new Point3d(l.EndP.X, l.EndP.Y, 0);
                            section.LinesOnSection.Add(new LineInPlan(new LineCad(TS, DB, p1, p2)));
                        }
                    }
                    foreach (var l in electricColumnSwingsRight.LinesTopAdd)
                    {
                        var p1z = l.StartP.Z;
                        var p2z = l.EndP.Z;
                        if (p1z.IsEqual(section.Elevation) && p2z.IsEqual(section.Elevation))
                        {
                            var p1 = new Point3d(l.StartP.X, l.StartP.Y, 0);
                            var p2 = new Point3d(l.EndP.X, l.EndP.Y, 0);
                            section.LinesOnSection.Add(new LineInPlan(new LineCad(TS, DB, p1, p2)));
                        }
                    }
                    foreach (var l in electricColumnSwingsRight.LinesBotAdd)
                    {
                        var p1z = l.StartP.Z;
                        var p2z = l.EndP.Z;
                        if (p1z.IsEqual(section.Elevation) && p2z.IsEqual(section.Elevation))
                        {
                            var p1 = new Point3d(l.StartP.X, l.StartP.Y, 0);
                            var p2 = new Point3d(l.EndP.X, l.EndP.Y, 0);
                            section.LinesOnSection.Add(new LineInPlan(new LineCad(TS, DB, p1, p2)));
                        }
                    }
                }
            }
        }
    }
}
