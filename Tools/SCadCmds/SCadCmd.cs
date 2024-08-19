using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using CadDev.Extension.ICommand;
using CadDev.Tools.ElectricColumnGeneral.exceptions;
using CadDev.Tools.ElectricColumnGeneral.models;
using CadDev.Tools.ElectricColumnGeneral.viewModels;
using CadDev.Utils;
using CadDev.Utils.CadTexts;
using CadDev.Utils.Messages;
using CadDev.Utils.Selections;

namespace CadDev.Tools.SCadCmds
{
    public class SCadCmd
    {
        
    }

    public class SCadDMLCmd : ICadCommand
    {
        private string _phi = "%%c";
        private string _markOrigin = "%%c12a200 L=";
        private string _mark = "xx_%%c12a200 L=_value";

        [CommandMethod("dml")]
        public void Execute()
        {
            try
            {
                AC.GetInfomation();
                using (Transaction ts = AC.Database.TransactionManager.StartTransaction())
                {
                    try
                    {
                        using (DocumentLock documentLock = AC.DocumentCollection.MdiActiveDocument.LockDocument())
                        {
                            Polyline r = null;
                            var objR = ts.PickObject(AC.Editor, "Pick rebar (PolyLine)");
                            if (objR != null && objR is Polyline) r = objR as Polyline;
                            var rL = Math.Round(r.Length, 0);

                            DBText text = null;
                            var objtext = ts.PickObject(AC.Editor, "Pick Text");
                            if (objtext != null && objtext is DBText) text = objtext as DBText;

                            if (text != null && r != null)
                            {
                                var contentChange = $"{text.TextString}{rL}";
                                ts.EditText(AC.Database, text, contentChange);
                            }
                        }
                        ts.Commit();
                    }
                    catch (System.Exception ex)
                    {
                        if (ex.Message != ObjectNotFoundException.MessageError) IO.ShowWarning("Đối tượng đã chọn không phù hợp");
                    }
                }
            }
            catch (System.Exception)
            {
            }
        }
    }

    public class SCadDMQCmd : ICadCommand
    {
        private string _phi = "%%c";
        private string _markOrigin = "%%c12a200 L=";
        private string _mark = "xx_%%c12a200 L=_value";
        private double _spacing = 200;

        [CommandMethod("dmq")]
        public void Execute()
        {
            try
            {
                AC.GetInfomation();
                using (Transaction ts = AC.Database.TransactionManager.StartTransaction())
                {
                    try
                    {
                        using (DocumentLock documentLock = AC.DocumentCollection.MdiActiveDocument.LockDocument())
                        {
                            Dimension d = null;
                            var objD = ts.PickObject(AC.Editor, "Pick Dim");
                            if (objD != null && objD is Dimension) d = objD as Dimension;

                            DBText text = null;
                            var objtext = ts.PickObject(AC.Editor, "Pick Text");
                            if (objtext != null && objtext is DBText) text = objtext as DBText;

                            if (text != null && d != null)
                            {
                                var dL = d.Measurement;
                                var du = dL % _spacing;
                                var qtyOdd =1 + (dL - du) / 200;
                                var qty = du *100 / _spacing >= 50 ? qtyOdd + 1: qtyOdd;
                                var contentChange = $"{qty}{text.TextString}";
                                ts.EditText(AC.Database, text, contentChange);
                            }
                        }
                        ts.Commit();
                    }
                    catch (System.Exception ex)
                    {
                        if (ex.Message != ObjectNotFoundException.MessageError) IO.ShowWarning("Đối tượng đã chọn không phù hợp");
                    }
                }
            }
            catch (System.Exception)
            {
            }
        }
    }
}
