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

        public void CreateElectricColumn()
        {
            //draw main line
            foreach (var l in ElectricColumnGeneralViewModel.ElectricColumnGeneralModel.LinesSouth)
            {
                l.Create();
            }
            foreach (var l in ElectricColumnGeneralViewModel.ElectricColumnGeneralModel.LinesEarth)
            {
                l.Create();
            }
            foreach (var l in ElectricColumnGeneralViewModel.ElectricColumnGeneralModel.LinesNorth)
            {
                l.Create();
            }
            foreach (var l in ElectricColumnGeneralViewModel.ElectricColumnGeneralModel.LinesWest)
            {
                l.Create();
            }

            foreach (var electricColumnSwingsLeft in ElectricColumnGeneralViewModel.ElectricColumnGeneralModel.ElectricColumnSwingModel.ElectricColumnSwingsLeft)
            {
                foreach (var l in electricColumnSwingsLeft.LinesRight)
                {
                    l.Create();
                }
                foreach (var l in electricColumnSwingsLeft.LinesLeft)
                {
                    l.Create();
                }
            }

            foreach (var electricColumnSwingsRight in ElectricColumnGeneralViewModel.ElectricColumnGeneralModel.ElectricColumnSwingModel.ElectricColumnSwingsRight)
            {
                foreach (var l in electricColumnSwingsRight.LinesLeft)
                {
                    l.Create();
                }
                foreach (var l in electricColumnSwingsRight.LinesRight)
                {
                    l.Create();
                }
            }


            //foreach (var sectionSwingRight in ElectricColumnGeneralViewModel.ElectricColumnGeneralModel.ElectricColumnSwingModel.SectionSwingRight)
            //{
            //    foreach (var l in sectionSwingRight.LinesLeft)
            //    {
            //        l.Create();
            //    }
            //    foreach (var l in sectionSwingRight.LinesRight)
            //    {
            //        l.Create();
            //    }
            //}
            //foreach (var sectionSwingLeft in ElectricColumnGeneralViewModel.ElectricColumnGeneralModel.ElectricColumnSwingModel.SectionSwingLeft)
            //{
            //    foreach (var l in sectionSwingLeft.LinesLeft)
            //    {
            //        l.Create();
            //    }
            //    foreach (var l in sectionSwingLeft.LinesRight)
            //    {
            //        l.Create();
            //    }
            //}
            //draw line sub (line design on ui)
            //foreach (var item in ElectricColumnGeneralViewModel.ElectricColumnModel.SectionPlanes)
            //{
            //    foreach (var l in item.LinesAdd)
            //    {
            //        l.Create();
            //    }
            //}
        }
    }
}
