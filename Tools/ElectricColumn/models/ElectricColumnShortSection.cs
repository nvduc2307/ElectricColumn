using Autodesk.AutoCAD.Geometry;
using CadDev.Utils.Geometries;
using CadDev.Utils.Lines;
using CadDev.Utils.Points;

namespace CadDev.Tools.ElectricColumn.models
{
    public class ElectricColumnShortSection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Elevation { get; set; }
        public List<LineCad> Lines { get; set; }
        public List<PointCad> Points { get; set; }
        public Point3d Center { get; set; }
        public List<LineCad> LinesAdd { get; set; } = new List<LineCad>();

        public ElectricColumnShortSection(int id, List<LineCad> lines, List<PointCad> points)
        {
            Id = id;
            Name = $"Section{id}";
            Lines = lines;
            Points = points;
            GetElevation(out double elevation);
            Elevation = elevation;
            GetCenter(out Point3d center);
            Center = center;
        }
        private void GetElevation(out double elevation)
        {
            elevation = 0;
            try
            {
                elevation = Points.First().P.Z;
            }
            catch (Exception)
            {
            }
        }
        private void GetCenter(out Point3d center)
        {
            center = new Point3d();
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

        }
    }
}
