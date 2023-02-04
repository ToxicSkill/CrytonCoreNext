using System;
using System.Windows.Controls;

namespace CrytonCoreNext.Models
{
    public class UiViewElement<T> where T : ContentControl
    {
        public T Object { get; init; }

        public int Height { get; private set; }

        public bool HasHeader { get; set; }

        public int HeaderHeight { get; set; }

        public UiViewElement(T obj,  bool hasHeader, int headerHeight)
        {
            Object = obj;
            Height = (int)Math.Ceiling((obj as T).ActualHeight);
            HasHeader = hasHeader;
            HeaderHeight = headerHeight;
        }
    }
}
