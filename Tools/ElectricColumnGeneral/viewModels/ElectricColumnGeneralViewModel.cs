using Autodesk.AutoCAD.DatabaseServices;
using CadDev.Utils;
using CadDev.Utils.Compares;
using CadDev.Utils.Selections;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CadDev.Tools.ElectricColumnGeneral.viewModels
{
    public partial class ElectricColumnGeneralViewModel : ObservableObject
    {
        public Transaction Ts { get; }
        public ElectricColumnGeneralViewModel(Transaction ts)
        {
            Ts = ts;
            var mainFaces = ts.SelectObjs<Line>(AC.Editor);
            var sideFaces = ts.SelectObjs<Line>(AC.Editor);
            GetMinLine(sideFaces);
        }
        private void GetMinLine(IEnumerable<Line> lines)
        {
            try
            {
                var lys = lines.OrderBy(x => Math.Min(Math.Round(x.StartPoint.Y), Math.Round(x.EndPoint.Y)))
                    .GroupBy(x => Math.Min(Math.Round(x.StartPoint.Y), Math.Round(x.EndPoint.Y)))
                    .Select(x => x.ToList())
                    .FirstOrDefault();
                if (lys == null) return;
                var lxs = lys
                    .OrderBy(x => Math.Max(Math.Round(x.StartPoint.X), Math.Round(x.EndPoint.X)))
                    .GroupBy(x => Math.Max(Math.Round(x.StartPoint.X), Math.Round(x.EndPoint.X)))
                    .Select(x => x.ToList())
                    .LastOrDefault();
                if (lxs == null) return;
                var l = lxs.FirstOrDefault();

                var ls = lines.Where(x => x.StartPoint.IsSeem(l.StartPoint) || x.StartPoint.IsSeem(l.EndPoint)
                || x.EndPoint.IsSeem(l.StartPoint) || x.EndPoint.IsSeem(l.EndPoint));

                foreach (var item in ls)
                {
                    item.Highlight();
                }

            }
            catch (Exception)
            {
            }
        }
    }
}
