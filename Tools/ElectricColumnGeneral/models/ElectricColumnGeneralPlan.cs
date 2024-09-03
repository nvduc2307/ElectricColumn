using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CadDev.Tools.ElectricColumnGeneral.Iservices;
using CadDev.Utils.Lines;

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
            LinesSouth = linesSouth;
            LinesEarth = linesEarth;
            LinesNorth = linesNorth;
            LinesWest = linesWest;
            SectionPlanes = sectionPlanes;
            ElectricColumnSwingModel = electricColumnSwingModel;

            WidthBoxMainBody = GetWidthBoxMainBody(out double heigthBox);
            HeightBoxMainBody = heigthBox;
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
            var result = 0.0;
            
            return result;
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
    }
}
