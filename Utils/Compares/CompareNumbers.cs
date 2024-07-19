namespace CadDev.Utils.Compares
{
    public static class CompareNumbers
    {
        public static bool IsEqual(this double a, double b, double too = 0.001)
        {
            return Math.Abs(Math.Round(a, 4) - Math.Round(b, 4)) <= Math.Abs(too);
        }
        public static bool IsGreateOrEqual(this double a, double b, double too = 0.001)
        {
            return Math.Round(a, 4) - Math.Round(b, 4) >= -Math.Abs(too);
        }
        public static bool IsGreate(this double a, double b, double too = 0.001)
        {
            return Math.Round(a, 4) - Math.Round(b, 4) > -Math.Abs(too);
        }
        public static bool IsLess(this double a, double b)
        {
            return !Math.Round(a, 4).IsGreateOrEqual(Math.Round(b, 4));
        }
        public static bool IsLessOrEqual(this double a, double b, double too = 0.001)
        {
            try
            {
                return Math.Round(a, 4) - Math.Round(b, 4) <= Math.Abs(too);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
