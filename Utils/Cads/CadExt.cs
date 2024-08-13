using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using System.Diagnostics;

namespace CadDev.Utils.Cads
{
    public class CadExt
    {
        public static string CreateNewFileCad(string path)
        {
            try
            {
                DocumentCollection acDocMgr = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager;
                Document acDoc = acDocMgr.Add(Process.GetCurrentProcess().MainModule.FileName);
                acDoc.Database.SaveAs(path, true, DwgVersion.Current,
                              acDoc.Database.SecurityParameters);
                acDoc.CloseAndDiscard();
                return path;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static Document OpenDocumentCad(string path)
        {
            Document result = null;
            try
            {
                DocumentCollection acDocMgr = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager;
                result = acDocMgr.Open(path, false);
                acDocMgr.MdiActiveDocument = result;
            }
            catch (Exception)
            {
            }
            return result;
        }
    }
}
