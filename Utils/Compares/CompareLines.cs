using Autodesk.AutoCAD.DatabaseServices;
using CadDev.Utils.Lines;

namespace CadDev.Utils.Compares
{
    public class CompareLines : IEqualityComparer<LineCad>
    {
        public bool Equals(LineCad x, LineCad y)
        {
            var dk1 = x.StartP.IsSeem(y.StartP) || x.StartP.IsSeem(y.EndP);
            var dk2 = x.EndP.IsSeem(y.StartP) || x.EndP.IsSeem(y.EndP);
            return dk1 && dk2;
        }

        public int GetHashCode(LineCad obj)
        {
            return 0;
        }
    }
}
