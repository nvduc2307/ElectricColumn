using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using CadDev.Extension.CustomAttribute;
using CadDev.Extension.ICommand;

namespace CadDev.Command
{
    public class Command : ICadCommand
    {
        [CommandMethod("Test")]
        [RibbonButton("HC_Tools", "My Tools", "MyButton", "Day la phan mo ta ve chuc nang cua button nay")]
        public void Execute()
        {
            // Specify the template to use, if the template is not found
            // the default settings are used.
            string name = "D:\\nvduc\\AutocadAPI\\ElectricColumn\\CadDev\\Resources\\cads\\dm2.dwg";
            DocumentCollection acDocMgr = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager;
        }
    }
}
