using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace CadDev.Utils.Selections
{
    public static class Selections
    {
        public static IEnumerable<Entity> SelectObjs(this Transaction ts, Editor editor)
        {
            var results = new List<Entity>();
            var selection = editor.GetSelection().Value;
            try
            {
                foreach (SelectedObject item in selection)
                {
                    if (item != null)
                    {
                        // Open the selected object for write
                        Entity acEnt = ts.GetObject(item.ObjectId, OpenMode.ForWrite) as Entity;
                        if (acEnt != null) results.Add(acEnt);
                    }
                }
            }
            catch (Exception)
            {
            }
            return results;
        }

        public static IEnumerable<T> SelectObjs<T>(this Transaction ts, Editor editor)
        {
            var results = new List<T>();
            try
            {
                var selection = editor.GetSelection().Value;
                foreach (SelectedObject item in selection)
                {
                    if (item != null)
                    {
                        // Open the selected object for write
                        Entity acEnt = ts.GetObject(item.ObjectId, OpenMode.ForWrite) as Entity;
                        if (acEnt != null) if (acEnt is T t) results.Add(t);
                    }
                }
            }
            catch (Exception)
            {
            }
            return results;
        }

        public static IEnumerable<T> SelectObjsWithLayer<T>(this Transaction ts, Editor editor, LayerTableRecord layerTableRecord)
        {
            var results = new List<T>();
            try
            {
                var selection = editor.GetSelection().Value;
                foreach (SelectedObject item in selection)
                {
                    if (item != null)
                    {
                        // Open the selected object for write
                        Entity acEnt = ts.GetObject(item.ObjectId, OpenMode.ForWrite) as Entity;
                        if (acEnt != null)
                        {
                            if (acEnt.LayerId == layerTableRecord.Id) if (acEnt is T t) results.Add(t);

                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return results;
        }

        public static Entity PickObject(this Transaction ts, Editor editor, string promptMessage = "")
        {
            Entity element = null;
            try
            {
                ObjectId selection;
                PromptEntityOptions options = new PromptEntityOptions(promptMessage)
                {
                    AllowNone = false,
                };
                PromptEntityResult result = editor.GetEntity(options);
                if (result.Status == PromptStatus.OK)
                {
                    selection = result.ObjectId;
                }
                else
                {
                    editor.WriteMessage("\n Selection failure.");
                    selection = ObjectId.Null;
                }
                if (selection != ObjectId.Null)
                {
                    element = ts.GetObject(selection, OpenMode.ForWrite) as Entity;
                }
            }
            catch (Exception)
            {
            }
            return element;
        }

        public static Point3d PickPoint(this Editor editor, string promptMessage = "Pick point")
        {
            var promptPoint = editor.GetPoint(promptMessage);
            if (promptPoint.Status == PromptStatus.OK)
            {
                return promptPoint.Value;
            }
            else
            {
                throw new Exception("Cannot pick point");
            }
        }

        public static Point3dCollection PickPoints(this Editor editor, string promptMessage = "Pick point")
        {
            Point3dCollection result = new Point3dCollection();
            PromptPointResult promptPoint;
            do
            {
                promptPoint = editor.GetPoint(promptMessage);
                if (promptPoint.Status == PromptStatus.OK)
                {
                    result.Add(promptPoint.Value);
                }
                else
                {
                    break;
                }

            } while (promptPoint.Status == PromptStatus.OK);
            return result;
        }
    }
}
