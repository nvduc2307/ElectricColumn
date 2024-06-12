using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using CadDev.Extension.CustomAttribute;
using CadDev.Extension.ICommand;
using CadDev.Tools.ElectricColumn.models;
using CadDev.Tools.ElectricColumn.services;
using CadDev.Utils;
using CadDev.Utils.Compares;
using CadDev.Utils.Faces;
using CadDev.Utils.Geometries;
using CadDev.Utils.Lines;
using CadDev.Utils.Polylines;
using CadDev.Utils.Selections;
using System.Runtime.InteropServices;

namespace CadDev.Tools.ElectricColumn
{
    public class ElectricColumnCmd : ICadCommand
    {
        private ElectricColumnService _electricColumnService;

        [CommandMethod("11")]
        [RibbonButton("NVD", "My Tools", "Electric Column", "Create 3d electric column")]
        public void Execute()
        {
            AC.GetInfomation();
            using (Transaction ts = AC.Database.TransactionManager.StartTransaction())
            {
                try
                {
                    _electricColumnService = new ElectricColumnService(ts, AC.Database);
                    using (DocumentLock documentLock = AC.DocumentCollection.MdiActiveDocument.LockDocument())
                    {
                        var ls = ts.SelectObjs<Line>(AC.Editor);
                        var ps = ls.GetPoints().ToList();
                        _electricColumnService.GetControlPoints(ps, out Point3d MaxXMinY, out Point3d MinXMinY, out Point3d MaxXMaxY, out Point3d MinXMaxY);
                        _electricColumnService.GetFaceBaseElectricColumn(
                        new Point3d(),
                        ps,
                        MaxXMinY,
                        MinXMinY,
                        MaxXMaxY,
                        MinXMaxY,
                        out double widthMax,
                        out double widthMin,
                        out List<Point3d> startFace,
                        out List<Point3d> midFace,
                        out List<Point3d> endFace);

                        ts.CreateLine(AC.Database, startFace);
                        ts.CreateLine(AC.Database, midFace);
                        ts.CreateLine(AC.Database, endFace);

                        //faces of electric
                        var facesElectricColumn = new FacesElectricColumn(startFace, midFace, endFace);

                        //oxy -> oxz
                        var cp = MaxXMinY.MidPoint(MinXMinY);
                        var centerBase = new Point3d(cp.X, 0, cp.Y - MinXMinY.Y);
                        _electricColumnService.GetLinesOfFourDirection(
                            ls, 
                            MinXMinY.Y, 
                            centerBase, 
                            new Point3d(), 
                            out IEnumerable<LineCad> lineFaceYs, 
                            out IEnumerable<LineCad> lineFaceXs);

                        var linesBody = new List<LineCad>();

                        foreach (var line in lineFaceYs)
                        {
                            if (line.MidP.Z <= midFace.First().Z)
                            {
                                var p1 = line.StartP.RayPointToFace(-Vector3d.YAxis, facesElectricColumn.South1Face);
                                var p2 = line.EndP.RayPointToFace(-Vector3d.YAxis, facesElectricColumn.South1Face);

                                var p11 = line.StartP.RayPointToFace(Vector3d.YAxis, facesElectricColumn.North1Face);
                                var p22 = line.EndP.RayPointToFace(Vector3d.YAxis, facesElectricColumn.North1Face);

                                var l1 = new LineCad(ts, AC.Database, p1, p2);
                                var l2 = new LineCad(ts, AC.Database, p11, p22);

                                linesBody.Add(l1);
                                linesBody.Add(l2);
                            }
                            else
                            {
                                var p1 = line.StartP.RayPointToFace(-Vector3d.YAxis, facesElectricColumn.South2Face);
                                var p2 = line.EndP.RayPointToFace(-Vector3d.YAxis, facesElectricColumn.South2Face);

                                var p11 = line.StartP.RayPointToFace(Vector3d.YAxis, facesElectricColumn.North2Face);
                                var p22 = line.EndP.RayPointToFace(Vector3d.YAxis, facesElectricColumn.North2Face);

                                var l1 = new LineCad(ts, AC.Database, p1, p2);
                                var l2 = new LineCad(ts, AC.Database, p11, p22);

                                linesBody.Add(l1);
                                linesBody.Add(l2);
                            }
                        }

                        foreach (var line in lineFaceXs)
                        {
                            if (line.MidP.Z <= midFace.First().Z)
                            {
                                var p1 = line.StartP.RayPointToFace(Vector3d.XAxis, facesElectricColumn.Earth1Face);
                                var p2 = line.EndP.RayPointToFace(Vector3d.XAxis, facesElectricColumn.Earth1Face);

                                var p11 = line.StartP.RayPointToFace(-Vector3d.XAxis, facesElectricColumn.West1Face);
                                var p22 = line.EndP.RayPointToFace(-Vector3d.XAxis, facesElectricColumn.West1Face);

                                var l1 = new LineCad(ts, AC.Database, p1, p2);
                                var l2 = new LineCad(ts, AC.Database, p11, p22);

                                linesBody.Add(l1);
                                linesBody.Add(l2);
                            }
                            else
                            {
                                var p1 = line.StartP.RayPointToFace(Vector3d.XAxis, facesElectricColumn.Earth2Face);
                                var p2 = line.EndP.RayPointToFace(Vector3d.XAxis, facesElectricColumn.Earth2Face);

                                var p11 = line.StartP.RayPointToFace(-Vector3d.XAxis, facesElectricColumn.West2Face);
                                var p22 = line.EndP.RayPointToFace(-Vector3d.XAxis, facesElectricColumn.West2Face);

                                var l1 = new LineCad(ts, AC.Database, p1, p2);
                                var l2 = new LineCad(ts, AC.Database, p11, p22);

                                linesBody.Add(l1);
                                linesBody.Add(l2);
                            }
                        }

                        linesBody = linesBody.Distinct(new CompareLines()).ToList();
                        foreach (var line in linesBody)
                        {
                            line.Create();
                        }
                    }
                    ts.Commit();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
