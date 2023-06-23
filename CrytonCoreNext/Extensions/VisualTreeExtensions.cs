using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace CrytonCoreNext.Extensions
{
    public static class VisualTreeExtensions
    {
        public static T GetChildOfType<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null) return result;
            }
            return null;
        }

        public static IEnumerable<T> FindVisualChildren<T>([NotNull] this DependencyObject parent) where T : DependencyObject
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            var queue = new Queue<DependencyObject>(new[] { parent });

            while (queue.Any())
            {
                var reference = queue.Dequeue();
                var count = VisualTreeHelper.GetChildrenCount(reference);

                for (var i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(reference, i);
                    if (child is T children)
                        yield return children;

                    queue.Enqueue(child);
                }
            }
        }

        public static void Hide(this IEnumerable<FrameworkElement> elements)
        {
            foreach (var element in elements)
            {
                element.Hide();
            }
        }

        public static void Hide(this FrameworkElement element)
        {
            element.Visibility = Visibility.Hidden;
        }

        public static void Collapse(this IEnumerable<FrameworkElement> elements)
        {
            foreach (var element in elements)
            {
                element.Collapse();
            }
        }

        public static void Collapse(this FrameworkElement element)
        {
            element.Visibility = Visibility.Collapsed;
        }

        public static void Show(this IEnumerable<FrameworkElement> elements)
        {
            foreach (var element in elements)
            {
                element.Show();
            }
        }

        public static void Show(this FrameworkElement element)
        {
            element.Visibility = Visibility.Visible;
        }
    }
}
