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
            MessageBox.Show("abc");
        }
    }
}
