namespace CadDev.Utils.Compares
{
    public static class CompareNumbers
    {
        public static bool IsEqual(this double a, double b, double too = 0.001)
        {
            return Math.Abs(a - b) <= Math.Abs(too);
        }
    }
}
