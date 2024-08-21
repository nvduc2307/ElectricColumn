using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using CadDev.Extension.ICommand;
using CadDev.Tools.ElectricColumnGeneral.exceptions;
using CadDev.Utils;
using CadDev.Utils.CadBlocks;
using CadDev.Utils.CadDimentions;
using CadDev.Utils.Cads;
using CadDev.Utils.Geometries;
using CadDev.Utils.Lines;
using CadDev.Utils.Messages;
using CadDev.Utils.Polylines;
using CadDev.Utils.Selections;

namespace CadDev.Tools.SCadCmds
{
    public class SCadCmd : ICadCommand
    {
        [CommandMethod("Scad_Dev_CreateFileCad")]
        public void Execute()
        {
            try
            {
                var saveDialog = new SaveFileDialog();
                saveDialog.Filter = "|.dwg";
                saveDialog.InitialDirectory = "C:\\";
                saveDialog.FileName = "index";
                var dialogResult = saveDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    var path = saveDialog.FileName;
                    CadExt.CreateNewFileCad(path);
                    var docCadnew = CadExt.OpenDocumentCad(path);
                    var edittor = docCadnew.Editor;
                    var database = docCadnew.Database;
                    using (var ts = database.TransactionManager.StartTransaction())
                    {
                        using (DocumentLock documentLock = docCadnew.LockDocument())
                        {
                            var lCad = new LineCad(ts, database, new Point3d(), new Point3d(1000, 0, 0));
                            lCad._db = database;
                            lCad.Create();
                        }
                        ts.Commit();
                    }
                }
            }
            catch (System.Exception ex)
            {
                IO.ShowException(ex);
            }
        }
    }

    public class ScadDevRemoveTextDimCmd : ICadCommand
    {
        [CommandMethod("Scad_Dev_RemoveTextDim")]
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

                            if (d != null)
                            {
                                ts.OverWriteTextDim(AC.Database, d);
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

    public class SCadDevMarkChangeCmd : ICadCommand
    {
        private String _blockTag = "CHU_THICH";
        public static double Diameter { get; set; } = 12;
        public static double Spacing { get; set; } = 200;

        [CommandMethod("SCad_Dev_MarkChange")]
        public void Execute()
        {
            try
            {
                AC.GetInfomation();
                using (Transaction ts = AC.Database.TransactionManager.StartTransaction())
                {
                    try
                    {
                        var keyWorks = new List<string>()
                        {
                            RebarProperties.Diameter.ToString(),
                            RebarProperties.Spacing.ToString(),
                        };
                        var options = new PromptSelectionOptions();
                        foreach (var item in keyWorks)
                        {
                            options.Keywords.Add(item);
                        }
                        options.KeywordInput += Options_KeywordInput;
                        using (DocumentLock documentLock = AC.DocumentCollection.MdiActiveDocument.LockDocument())
                        {
                            var messR = $"Pick rebar (PolyLine) or choose an option [Diameter, Spacing]:";
                            options.MessageForAdding = messR;
                            var objR = ts.PickObject(AC.Editor, options);

                            Dimension d = null;
                            var messD = $"Pick Dim or choose an option [Diameter, Spacing]:";
                            options.MessageForAdding = messR;
                            var objD = ts.PickObject(AC.Editor, options);
                            if (objD != null && objD is Dimension) d = objD as Dimension;

                            BlockReference blockRef = null;
                            var messBRef = $"Pick BlockReference an option [Diameter, Spacing]:";
                            options.MessageForAdding = messR;
                            var objL1 = ts.PickObject(AC.Editor, options);
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
                                    var du = dL % Spacing;
                                    var qtyOdd = 1 + (dL - du) / Spacing;
                                    var qty = du * 100 / Spacing >= 50 ? qtyOdd + 1 : qtyOdd;
                                    //Change content in block
                                    var contentChange = $"{qty}%%c{Diameter}a{Spacing} L={rLength}";
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

        private void Options_KeywordInput(object sender, SelectionTextInputEventArgs e)
        {
            var optionDouble = new PromptDoubleOptions(e.Input);
            var rebarPro = GetRebarProperties(e.Input);
            // Handle the keyword input
            switch (rebarPro)
            {
                case RebarProperties.Diameter:
                    var diameterResult = AC.Editor.GetDouble(optionDouble);
                    Diameter = diameterResult.Value;
                    break;
                case RebarProperties.Spacing:
                    var spacingResult = AC.Editor.GetDouble(optionDouble);
                    Spacing = spacingResult.Value;
                    break;
            }
        }

        private RebarProperties GetRebarProperties(string content)
        {
            var result = RebarProperties.Diameter;
            try
            {
                switch (content)
                {
                    case "Diameter":
                        result = RebarProperties.Diameter;
                        break;
                    case "Spacing":
                        result = RebarProperties.Spacing;
                        break;
                }
            }
            catch (System.Exception)
            {

            }
            return result;
        }

        private enum RebarProperties
        {
            Diameter = 0,
            Spacing = 1,
        }
    }

    public class SCadDevCreateMidLineFromTwoLineCmd : ICadCommand
    {
        [CommandMethod("SCad_Dev_CreateMidLine_FromTwoLine")]
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
        [CommandMethod("SCad_Dev_DimensionLine_ForPolygon")]
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
