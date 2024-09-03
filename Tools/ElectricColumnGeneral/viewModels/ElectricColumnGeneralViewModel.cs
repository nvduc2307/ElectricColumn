using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using CadDev.Tools.ElectricColumnGeneral.exceptions;
using CadDev.Tools.ElectricColumnGeneral.models;
using CadDev.Tools.ElectricColumnGeneral.services;
using CadDev.Utils.Cads;
using CadDev.Utils.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CadDev.Tools.ElectricColumnGeneral.viewModels
{
    public partial class ElectricColumnGeneralViewModel : ObservableObject
    {
        public Transaction Ts { get; }
        public ElectricColumnService ElectricColumnService { get; }
        public ElectricColumnUIElementModel UIElement { get; set; }
        public ElectricColumnGeneralModel ElectricColumnGeneralModel { get; set; }
        public ElectricColumnGeneralViewModel(Transaction ts, ElectricColumnGeneralModel electricColumnGeneralModel)
        {
            Ts = ts;
            ElectricColumnGeneralModel = electricColumnGeneralModel;
            UIElement = new ElectricColumnUIElementModel(this);
            ElectricColumnGeneralModel.UIElement = UIElement;
            ElectricColumnService = new ElectricColumnService(this);
        }

        [RelayCommand]
        public void Create3D()
        {
            //UIElement.MainView.Close();
            //create new file cad.
            var saveDl = new SaveFileDialog
            {
                Filter = "*|.dwg",
                FileName = "3d",
                RestoreDirectory = true,
            };
            if (saveDl.ShowDialog() == DialogResult.OK)
            {
                var pathSave = saveDl.FileName;
                CadExt.CreateNewFileCad(pathSave);
                var documentNew = CadExt.OpenDocumentCad(pathSave);
                var editorNew = documentNew.Editor;
                var databaseNew = documentNew.Database;
                using (Transaction ts = databaseNew.TransactionManager.StartTransaction())
                {
                    try
                    {
                        using (DocumentLock documentLock = documentNew.LockDocument())
                        {
                            ElectricColumnService?.CreateElectricColumn3D(ts, databaseNew);
                            UIElement.MainView.Focus();
                        }
                        ts.Commit();
                    }
                    catch (System.Exception ex)
                    {
                        if (ex.Message != ObjectNotFoundException.MessageError) IO.ShowException(ex);
                    }
                }
            }
        }
        [RelayCommand]
        public void Cancel()
        {
            UIElement.MainView.Close();
        }
        [RelayCommand]
        public void CreateDrawing()
        {
            var saveDl = new SaveFileDialog
            {
                Filter = "*|.dwg",
                FileName = "PlanDrawing",
                RestoreDirectory = true,
            };
            if (saveDl.ShowDialog() == DialogResult.OK)
            {
                var pathSave = saveDl.FileName;
                CadExt.CreateNewFileCad(pathSave);
                var documentNew = CadExt.OpenDocumentCad(pathSave);
                var editorNew = documentNew.Editor;
                var databaseNew = documentNew.Database;
                using (Transaction ts = databaseNew.TransactionManager.StartTransaction())
                {
                    try
                    {
                        using (DocumentLock documentLock = documentNew.LockDocument())
                        {
                            var electricColumnGeneralPlan = new ElectricColumnGeneralPlan(
                                ts, databaseNew,
                                ElectricColumnGeneralModel.LinesSouth,
                                ElectricColumnGeneralModel.LinesEarth,
                                ElectricColumnGeneralModel.LinesNorth,
                                ElectricColumnGeneralModel.LinesWest,
                                ElectricColumnGeneralModel.SectionPlanes,
                                ElectricColumnGeneralModel.ElectricColumnSwingModel);

                            UIElement.MainView.Focus();
                        }
                        ts.Commit();
                    }
                    catch (System.Exception ex)
                    {
                        if (ex.Message != ObjectNotFoundException.MessageError) IO.ShowException(ex);
                    }
                }
            }
        }
    }
}
