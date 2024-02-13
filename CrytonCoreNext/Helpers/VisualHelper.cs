using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace CrytonCoreNext.Helpers
{
    public static class VisualHelper
    {
        public static IEnumerable<T> FindVisualChilds<T>(DependencyObject depObj, bool deeper) where T : DependencyObject
        {
            if (depObj == null) yield return (T)Enumerable.Empty<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject ithChild = VisualTreeHelper.GetChild(depObj, i);
                if (ithChild == null) continue;
                if (ithChild is T t) yield return t;
                if (deeper)
                {
                    foreach (T childOfChild in FindVisualChilds<T>(ithChild, deeper)) yield return childOfChild;
                }
            }
        }
    }
}
