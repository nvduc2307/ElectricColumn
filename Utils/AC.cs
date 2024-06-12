using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadDev.Utils
{
    public class AC
    {
        public static DocumentCollection DocumentCollection;
        public static Database Database;
        public static Editor Editor;
        public static void GetInfomation()
        {
            //do some things
            DocumentCollection = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager;
            Database = DocumentCollection.MdiActiveDocument.Database;
            Editor = DocumentCollection.MdiActiveDocument.Editor;
        }
    }
}
