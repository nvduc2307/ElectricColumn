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
using CadDev.Utils.CadTexts;
using CadDev.Utils.Compares;
using CadDev.Utils.ElementTransform;
using CadDev.Utils.Geometries;
using CadDev.Utils.Lines;
using CadDev.Utils.Messages;
using CadDev.Utils.Points;
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

    public class ScadDevSplitRebar : ICadCommand
    {
        public static double Diameter { get; set; } = 12;
        public static Int32 LapLength { get; set; } = 60;
        public static double Length { get; set; } = 11700;
        public static double AjustDistance { get; set; } = 100;
        [CommandMethod("Scad_Dev_SplitRebar")]
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
                            RebarProperties.Length.ToString(),
                            RebarProperties.LapLength.ToString(),
                            RebarProperties.AjustDistance.ToString(),
                        };
                        var options = new PromptSelectionOptions();
                        var messR = $"Pick rebar (PolyLine) or choose an option [Diameter, Length, LapLength, AjustDistance]:";
                        options.MessageForAdding = messR;
                        foreach (var item in keyWorks)
                        {
                            options.Keywords.Add(item);
                        }
                        options.KeywordInput += Options_KeywordInput;
                        using (DocumentLock documentLock = AC.DocumentCollection.MdiActiveDocument.LockDocument())
                        {
                            var objR = ts.PickObject(AC.Editor, options);
                            if (objR != null)
                            {
                                if (objR is Polyline || objR is Line)
                                {
                                    var results = new List<List<Point3d>>();
                                    //Get rebar points
                                    string layer = null;
                                    var rPoints = new List<Point3d>();
                                    if (objR is Polyline pl)
                                    {
                                        rPoints = pl.GetPoints();
                                        layer = pl.Layer;
                                    }
                                    if (objR is Line l)
                                    {
                                        rPoints = l.GetPoints().ToList();
                                        layer = l.Layer;
                                    }
                                    var rLengthCurrent = rPoints.GetDistance();
                                    if (rLengthCurrent > Length)
                                    {
                                        var a = 1;
                                        do
                                        {
                                            try
                                            {
                                                var rPointsCount = rPoints.Count;
                                                var rPointsCut = new List<Point3d>();
                                                var rPointsTobeCut = new List<Point3d>();
                                                var rl = 0.0;
                                                //tìm vị trí cắt
                                                var index = 1;
                                                for (int i = 1; i < rPointsCount; i++)
                                                {
                                                    var j = i - 1;
                                                    rl += rPoints[j].DistanceTo(rPoints[i]);

                                                    rPointsCut.Add(rPoints[j]);
                                                    if (rl > Length)
                                                    {
                                                        index = i;
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        if (i == rPointsCount - 1)
                                                        {
                                                            index = i;
                                                            rPointsCut.Add(rPoints[i]);
                                                        }
                                                    }
                                                }
                                                //tìm vector trên đoạn cắt
                                                var vtCut = (rPoints[index] - rPoints[index - 1]).GetNormal();
                                                var vtCutPrev = index >= 2
                                                    ? (rPoints[index - 1] - rPoints[index - 2]).GetNormal()
                                                    : vtCut;
                                                //tìm chiều dài của đoạn cắt
                                                var lengthPointsCut = rPointsCut.GetDistance();
                                                //tìm chiều dài đoạn còn lại của đoạn cắt
                                                var rlengthExt = Length - lengthPointsCut;
                                                var pCut = rlengthExt >= LapLength * Diameter
                                                    ? rPoints[index - 1] + vtCut * rlengthExt
                                                    : rPoints[index - 1];
                                                //thêm pCut vào rPointsCut và rPointsTobeCut
                                                if (rlengthExt > 0 && !rPointsCut.Any(x => x.IsSeem(pCut))) rPointsCut.Add(pCut);
                                                //tìm rPointsTobeCut
                                                rPointsTobeCut = rPoints
                                                    .Where(x => !rPointsCut.Any(y => y.IsSeem(x)))
                                                    .ToList();
                                                if (vtCutPrev.IsParallelTo(vtCut))
                                                {
                                                    rPointsTobeCut.Insert(0, pCut - vtCut * LapLength * Diameter);
                                                }
                                                else
                                                {
                                                    if (pCut.IsSeem(rPoints[index - 1]))
                                                    {
                                                        rPointsTobeCut.Insert(0, rPoints[index - 1]);
                                                        rPointsTobeCut.Insert(0, rPoints[index - 1] - vtCutPrev * LapLength * Diameter);
                                                    }
                                                    else
                                                    {
                                                        rPointsTobeCut.Insert(0, pCut - vtCut * LapLength * Diameter);
                                                    }
                                                }
                                                //
                                                results.Add(rPointsCut.Distinct(new ComparePoints()).ToList());
                                                if (rPointsTobeCut.Count < 2) throw new System.Exception();
                                                if (rPointsTobeCut.GetDistance() <= Length)
                                                {
                                                    results.Add(rPointsTobeCut.Distinct(new ComparePoints()).ToList());
                                                    a = 0;
                                                }
                                                if (rPointsTobeCut.GetDistance() > Length) rPoints = rPointsTobeCut;
                                            }
                                            catch (System.Exception)
                                            {
                                                a = 0;
                                            }
                                        } while (a == 1);

                                        var c = 0;
                                        foreach (var ps in results)
                                        {
                                            var psCount = ps.Count;
                                            var pll = ts.CreatePolyline(AC.Database, ps);
                                            pll.Layer = layer;
                                            if (c % 2 == 0)
                                            {
                                                var vt = (ps[1] - ps[0]).GetNormal();
                                                var normal = vt.CrossProduct(Vector3d.ZAxis);
                                                pll.Move(normal * AjustDistance);
                                            }
                                            c++;
                                        }

                                        //xoa doi tuong goc
                                        objR.Erase();
                                    }
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
            var parent = sender as PromptSelectionOptions;
            var optionDouble = new PromptDoubleOptions(e.Input);
            var optionInt = new PromptIntegerOptions(e.Input);
            var input = GetRebarProperties(e.Input);
            switch (input)
            {
                case RebarProperties.Diameter:
                    var diameterResult = AC.Editor.GetDouble(optionDouble);
                    Diameter = diameterResult.Value;
                    break;
                case RebarProperties.Length:
                    var lengthResult = AC.Editor.GetDouble(optionDouble);
                    Length = lengthResult.Value;
                    break;
                case RebarProperties.LapLength:
                    var lapLengthResult = AC.Editor.GetInteger(optionInt);
                    LapLength = lapLengthResult.Value;
                    break;
                case RebarProperties.AjustDistance:
                    var ajustDistanceResult = AC.Editor.GetDouble(optionDouble);
                    AjustDistance = ajustDistanceResult.Value;
                    break;
            }
        }
        private RebarProperties GetRebarProperties(string input)
        {
            var result = RebarProperties.Diameter;
            switch (input)
            {
                case "Diameter":
                    result = RebarProperties.Diameter;
                    break;
                case "Length":
                    result = RebarProperties.Length;
                    break;
                case "LapLength":
                    result = RebarProperties.LapLength;
                    break;
                case "AjustDistance":
                    result = RebarProperties.AjustDistance;
                    break;
            }
            return result;
        }

        public enum RebarProperties
        {
            Diameter = 0,
            Length = 1,
            LapLength = 2,
            AjustDistance = 3,
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

    public class SCadDevRebarLengthCmd
    {
        [CommandMethod("SCad_Dev_Rebar_Length")]
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
                                    //Get rebar length
                                    var rLength = 0.0;
                                    var rCenter = new Point3d();
                                    if (objL1 is Polyline pl)
                                    {
                                        rLength = Math.Round(pl.Length, 0);
                                        rCenter = pl.GetCenter();
                                    }
                                    if (objL1 is Line l)
                                    {
                                        rLength = Math.Round(l.Length, 0);
                                        rCenter = l.GetMid();
                                    }
                                    var text = ts.CreateText(AC.Database, rCenter, rLength.ToString());
                                    ts.EditHeightText(AC.Database, text);
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
