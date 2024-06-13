using System.Windows.Media;
using System.Windows;

namespace CadDev.Utils.CanvasUtils.Utils
{
    public static class VisualTreeHelperExtensions
    {
        public static T FindParent<T>(this DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null)
                return null;

            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }
        public static DependencyObject GetFirstChild(this DependencyObject parent)
        {
            if (parent == null)
                return null;

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            if (childCount > 0)
                return VisualTreeHelper.GetChild(parent, 0);
            else
                return null;
        }
    }
}
