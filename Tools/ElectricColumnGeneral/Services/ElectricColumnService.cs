using Autodesk.AutoCAD.DatabaseServices;
using CadDev.Tools.ElectricColumnGeneral.iservices;
using CadDev.Tools.ElectricColumnGeneral.viewModels;

namespace CadDev.Tools.ElectricColumnGeneral.services
{
    public class ElectricColumnService : IElectricColumnService
    {
        public ElectricColumnGeneralViewModel ElectricColumnGeneralViewModel { get; set; }

        public ElectricColumnService(ElectricColumnGeneralViewModel electricColumnGeneralViewModel)
        {
            ElectricColumnGeneralViewModel = electricColumnGeneralViewModel;
        }

        public void CreateElectricColumn3D(Transaction ts, Database db)
        {
            //draw main line
            foreach (var l in ElectricColumnGeneralViewModel.ElectricColumnGeneralModel.LinesSouth)
            {
                l._ts = ts;
                l._db = db;
                l.Create();
            }
            foreach (var l in ElectricColumnGeneralViewModel.ElectricColumnGeneralModel.LinesEarth)
            {
                l._ts = ts;
                l._db = db;
                l.Create();
            }
            foreach (var l in ElectricColumnGeneralViewModel.ElectricColumnGeneralModel.LinesNorth)
            {
                l._ts = ts;
                l._db = db;
                l.Create();
            }
            foreach (var l in ElectricColumnGeneralViewModel.ElectricColumnGeneralModel.LinesWest)
            {
                l._ts = ts;
                l._db = db;
                l.Create();
            }
            foreach (var electricColumnSwingsLeft in ElectricColumnGeneralViewModel.ElectricColumnGeneralModel.ElectricColumnSwingModel.ElectricColumnSwingsLeft)
            {
                foreach (var l in electricColumnSwingsLeft.LinesRight)
                {
                    l._ts = ts;
                    l._db = db;
                    l.Create();
                }
                foreach (var l in electricColumnSwingsLeft.LinesLeft)
                {
                    l._ts = ts;
                    l._db = db;
                    l.Create();
                }
                foreach (var l in electricColumnSwingsLeft.LinesTopAdd)
                {
                    l._ts = ts;
                    l._db = db;
                    l.Create();
                }
                foreach (var l in electricColumnSwingsLeft.LinesBotAdd)
                {
                    l._ts = ts;
                    l._db = db;
                    l.Create();
                }
            }
            foreach (var electricColumnSwingsRight in ElectricColumnGeneralViewModel.ElectricColumnGeneralModel.ElectricColumnSwingModel.ElectricColumnSwingsRight)
            {
                foreach (var l in electricColumnSwingsRight.LinesRight)
                {
                    l._ts = ts;
                    l._db = db;
                    l.Create();
                }
                foreach (var l in electricColumnSwingsRight.LinesLeft)
                {
                    l._ts = ts;
                    l._db = db;
                    l.Create();
                }
                foreach (var l in electricColumnSwingsRight.LinesTopAdd)
                {
                    l._ts = ts;
                    l._db = db;
                    l.Create();
                }
                foreach (var l in electricColumnSwingsRight.LinesBotAdd)
                {
                    l._ts = ts;
                    l._db = db;
                    l.Create();
                }
            }
        }

        public void CreateElectricColumnPlan(Transaction ts, Database db)
        {
            foreach (var l in ElectricColumnGeneralViewModel.ElectricColumnGeneralModel.LinesBodyMain)
            {
                l._ts = ts;
                l._db = db;
                l.Create();
            }
        }
    }
}
