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
            foreach (var l in ElectricColumnGeneralViewModel.ElectricColumnModel.LinesSouth)
            {
                l.Create();
            }
            foreach (var l in ElectricColumnGeneralViewModel.ElectricColumnModel.LinesEarth)
            {
                l.Create();
            }
            foreach (var l in ElectricColumnGeneralViewModel.ElectricColumnModel.LinesNorth)
            {
                l.Create();
            }
            foreach (var l in ElectricColumnGeneralViewModel.ElectricColumnModel.LinesWest)
            {
                l.Create();
            }

            foreach (var l in ElectricColumnGeneralViewModel.ElectricColumnGeneralModel.ElectricColumnSwingModel.SwingLefts)
            {
                l.Create();
            }
            foreach (var l in ElectricColumnGeneralViewModel.ElectricColumnGeneralModel.ElectricColumnSwingModel.SwingRights)
            {
                l.Create();
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
