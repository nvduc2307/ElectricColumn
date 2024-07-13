using Autodesk.AutoCAD.Geometry;
using CadDev.Utils.Geometries;
using CadDev.Utils.Lines;
using CadDev.Utils.Points;
using System.Xml.Linq;

namespace CadDev.Tools.ElectricColumnGeneral.models
{
    public class ElectricColumnSectionPlane
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<LineCad> Lines { get; set; }
        public List<LineCad> LinesAdd { get; set; }
        public List<LineCad> LinesAtElevation { get; set; }
        public double Elevation { get; set; }
        public List<PointCad> Points { get; set; }
        public Point3d Center { get; set; }
        public ElectricColumnSectionPlane(int id, List<LineCad> lines)
        {
            Id = id;
            Name = $"Section{id + 1}";
            Lines = lines;
            LinesAdd = new List<LineCad>();
            LinesAtElevation = new List<LineCad>();
            Points = GetPoints();
            Elevation = GetElevation();
            Center = GetCenter();
        }

        private double GetElevation()
        {
            var elevation = 0.0;
            try
            {
                elevation = Points.First().P.Z;
            }
            catch (Exception)
            {
            }
            return elevation;
        }

        private List<PointCad> GetPoints()
        {
            var results = new  List<PointCad>();
            try
            {
                foreach (var line in Lines)
                {
                    results.Add(new PointCad(line.StartP));
                    results.Add(new PointCad(line.EndP));
                }
            }
            catch (Exception)
            {
            }
            return results;
        }

        private Point3d GetCenter()
        {
            var center = new Point3d();
            try
            {
                var maxX = Points.Max(x => x.P.X);
                var minX = Points.Min(x => x.P.X);
                var maxY = Points.Max(x => x.P.Y);
                var minY = Points.Min(x => x.P.Y);

                var max = new Point3d(maxX, maxY, Elevation);
                var min = new Point3d(minX, minY, Elevation);
                center = max.MidPoint(min);
            }
            catch (Exception)
            {
                center = new Point3d();
            }
            return center;
        }
    }
}
