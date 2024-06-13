namespace CadDev.Utils.CanvasUtils
{
    public static class ScaleInCanvas
    {
        public static double Scale(double maximumLengthInRevit, double maximumLengthInCanvas)
        {
            return maximumLengthInCanvas / maximumLengthInRevit;
        }
    }
}
