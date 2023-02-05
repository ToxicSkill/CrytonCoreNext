using System.Windows.Controls;

namespace CrytonCoreNext.Models
{
    public class UiViewElement<T> where T : ContentControl
    {
        public T Object { get; init; }

        public double Height { get; private set; }

        public bool HasHeader { get; set; }

        public double HeaderHeight { get; set; }

        public UiViewElement(T obj,  bool hasHeader, double headerHeight)
        {
            Object = obj;
            Height = (obj as T).ActualHeight;
            HasHeader = hasHeader;
            HeaderHeight = headerHeight;
        }
    }
}
