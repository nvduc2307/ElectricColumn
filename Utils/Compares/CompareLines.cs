using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
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

    public class CompareLinesOnPlane : IEqualityComparer<LineCad>
    {
        public bool Equals(LineCad x, LineCad y)
        {
            var dk1 = x.StartP.Z.IsEqual(y.StartP.Z) && x.StartP.Z.IsEqual(y.EndP.Z);
            var dk2 = x.EndP.Z.IsEqual(y.StartP.Z) && x.EndP.Z.IsEqual(y.EndP.Z);
            return  dk1 && dk2;
        }

        public int GetHashCode(LineCad obj)
        {
            return 0;
        }
    }
}
