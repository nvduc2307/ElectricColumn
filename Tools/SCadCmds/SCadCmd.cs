using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using CadDev.Extension.ICommand;
using CadDev.Tools.ElectricColumnGeneral.exceptions;
using CadDev.Utils;
using CadDev.Utils.CadBlocks;
using CadDev.Utils.CadDimentions;
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

    public class SCadDevMarkChangeCmd : ICadCommand
    {
        private string _phi = "%%c";
        private string _markOrigin = "%%c12a200 L=";
        private string _mark = "xx_%%c12a200 L=_value";
        private double _spacing = 200;
        private String _blockTag = "CHU_THICH";

        [CommandMethod("SCadDevMarkChange")]
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

                            Dimension d = null;
                            var objD = ts.PickObject(AC.Editor, "Pick Dim");
                            if (objD != null && objD is Dimension) d = objD as Dimension;

                            BlockReference blockRef = null;
                            var objL1 = ts.PickObject(AC.Editor, "Pick BlockReference");
                            if (objL1 != null && objL1 is BlockReference) blockRef = objL1 as BlockReference;


                            if (objR != null && d != null && blockRef != null)
                            {
                                if (objR is Polyline || objR is Line)
                                {
                                    //Get rebar length
                                    var rLength = 0.0;
                                    if (objR is Polyline pl) rLength = Math.Round(pl.Length, 0);
                                    if (objR is Line l) rLength = Math.Round(l.Length, 0);
                                    //Get rebar quantity
                                    var dL = d.Measurement;
                                    var du = dL % _spacing;
                                    var qtyOdd = 1 + (dL - du) / _spacing;
                                    var qty = du * 100 / _spacing >= 50 ? qtyOdd + 1 : qtyOdd;
                                    //Change content in block
                                    var contentChange = $"{qty}{_markOrigin}{rLength}";
                                    ts.EditBlock(AC.Database, blockRef, _blockTag, contentChange);
                                }
                            }
                            ts.Commit();
                        }
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

    public class SCadDevCreateMidLineFromTwoLineCmd : ICadCommand
    {
        [CommandMethod("SCadDevCreateMidLineFromTwoLine")]
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

    public class SCadDevDimensionLineForPolygonCmd
    {
        [CommandMethod("SCadDevDimensionLineForPolygon")]
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
                            Polyline pll = null;
                            var objL1 = ts.PickObject(AC.Editor, "Pick Line1");
                            if (objL1 != null && objL1 is Polyline) pll = objL1 as Polyline;

                            if (objL1 != null)
                            {
                                if (objL1 is Polyline || objL1 is Line)
                                {
                                    var points = new List<Point3d>();
                                    var rLength = 0.0;
                                    if (objL1 is Polyline pl) points = pl.GetPoints();
                                    if (objL1 is Line l) points = l.GetPoints().ToList();
                                    var pointsCount = points.Count;
                                    for (int i = 1; i < pointsCount; i++)
                                    {
                                        var j = i - 1;
                                        var p1 = points[j];
                                        var p2 = points[i];
                                        var pMid = p1.MidPoint(p2);
                                        ts.CreateDim(AC.Database, p1, p2, pMid);
                                    }
                                }
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
