using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CadDev.Extension
{
    public static class Utils
    {
        public static ImageSource GetEmbeddedPng(this string imageName)
        {
            var file = Assembly.GetExecutingAssembly().GetManifestResourceStream(imageName);
            var source = BitmapDecoder.Create(file, BitmapCreateOptions.None, BitmapCacheOption.None);
            return source.Frames[0];
        }
    }
}
