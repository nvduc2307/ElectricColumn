namespace CadDev.Utils.Compares
{
    public static class CompareNumbers
    {
        public static bool IsEqual(this double a, double b, double too = 0.01)
        {
            return Math.Abs(Math.Round(a, 4) - Math.Round(b, 4)) <= Math.Abs(too);
        }
        public static bool IsGreateOrEqual(this double a, double b, double too = 0.01)
        {
            return Math.Round(a, 4) - Math.Round(b, 4) >= -Math.Abs(too);
        }
        public static bool IsGreate(this double a, double b, double too = 0.01)
        {
            return Math.Round(a, 4) - Math.Round(b, 4) > -Math.Abs(too);
        }
        public static bool IsLess(this double a, double b)
        {
            return !Math.Round(a, 2).IsGreateOrEqual(Math.Round(b, 2));
        }
        public static bool IsLessOrEqual(this double a, double b, double too = 0.01)
        {
            try
            {
                return Math.Round(a, 2) - Math.Round(b, 2) <= Math.Abs(too);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
