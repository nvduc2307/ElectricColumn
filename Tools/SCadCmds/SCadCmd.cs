using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using CadDev.Extension.ICommand;
using CadDev.Tools.ElectricColumnGeneral.exceptions;
using CadDev.Utils;
using CadDev.Utils.CadTexts;
using CadDev.Utils.Geometries;
using CadDev.Utils.Lines;
using CadDev.Utils.Messages;
using CadDev.Utils.Polylines;
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
        private double _textHeight = 2.5;

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
                            Polyline rPl = null;
                            Line rl = null;
                            var objR = ts.PickObject(AC.Editor, "Pick rebar (PolyLine)");
                            if (objR != null)
                            {
                                if (objR is Polyline) rPl = objR as Polyline;
                                if (objR is Line) rl = objR as Line;
                            }
                            if (rPl != null)
                            {
                                var length = Math.Round(rPl.Length, 0);
                                var position = rPl.GetCenter();
                                var contentChange = $"{_markOrigin}{length}";
                                var text = ts.CreateText(AC.Database, position, contentChange);
                                ts.EditHeightText(AC.Database, text);
                            }

                            if (rl != null)
                            {
                                var length = Math.Round(rl.Length, 0);
                                var position = rl.GetMid();
                                var contentChange = $"{_markOrigin}{length}";
                                var text = ts.CreateText(AC.Database, position, contentChange);
                                ts.EditHeightText(AC.Database, text);
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
                                var qtyOdd = 1 + (dL - du) / 200;
                                var qty = du * 100 / _spacing >= 50 ? qtyOdd + 1 : qtyOdd;
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

    public class SCadDDCmd : ICadCommand
    {
        [CommandMethod("dcl")]
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
                            Line l1 = null;
                            var objL1 = ts.PickObject(AC.Editor, "Pick Line1");
                            if (objL1 != null && objL1 is Line) l1 = objL1 as Line;

                            Line l2 = null;
                            var objL2 = ts.PickObject(AC.Editor, "Pick Line1");
                            if (objL2 != null && objL2 is Line) l2 = objL2 as Line;

                            if (l1 != null && l2 != null)
                            {
                                var lCad1 = new LineCad(ts, AC.Database, l1.StartPoint, l1.EndPoint);
                                var lCad2 = new LineCad(ts, AC.Database, l2.StartPoint, l2.EndPoint);
                                var dot = lCad1.Dir.DotProduct(lCad2.Dir);

                                var p1 = dot > 0
                                    ? lCad1.StartP.MidPoint(lCad2.StartP)
                                    : lCad1.StartP.MidPoint(lCad2.EndP);
                                var p2 = dot > 0
                                    ? lCad1.EndP.MidPoint(lCad2.EndP)
                                    : lCad1.EndP.MidPoint(lCad2.StartP);

                                var lCad3 = new LineCad(ts, AC.Database, p1, p2);
                                var l = lCad3.Create();
                                l.Layer = l1.Layer;
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
