using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using System.Diagnostics;

namespace CadDev.Utils.Cads
{
    public class CadExt
    {
        public static string CreateNewFileCad(string path)
        {
            Document result = null;
            try
            {
                DocumentCollection acDocMgr = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager;
                result = acDocMgr.Add(Process.GetCurrentProcess().MainModule.FileName);
                result.Database.SaveAs(path, true, DwgVersion.Current,
                              result.Database.SecurityParameters);
                result.CloseAndDiscard();
            }
            catch (Exception)
            {
            }
            return path;
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
