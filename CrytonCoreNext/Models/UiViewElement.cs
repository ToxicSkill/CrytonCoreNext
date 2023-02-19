using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace CrytonCoreNext.Models
{
    public class UiViewElement<T> where T : ContentControl
    {
        public CardControl Card { get; set; }

        public T Object { get; init; }

        public double Height { get; private set; }

        public bool HasHeader { get; set; }

        public double HeaderHeight { get; set; }

        public UiViewElement(T obj, bool hasHeader, double headerHeight)
        {
            Object = obj;
            Height = (obj as T).ActualHeight;
            HasHeader = hasHeader;
            HeaderHeight = headerHeight;
        }

        public UiViewElement(SymbolRegular icon, string title, string description, FrameworkElement content)
        {
            var cardHeader = new StackPanel();
            var titleTextBlock = new TextBlock();
            var descriptionTextBlock = new TextBlock();
            var cardControl = new CardControl();
            cardControl.Header = new StackPanel();

            titleTextBlock.Text = title;
            descriptionTextBlock.Text = description;

            cardControl.Icon = icon;
            cardHeader.Children.Add(titleTextBlock);
            cardHeader.Children.Add(descriptionTextBlock);
            cardControl.Header = cardHeader;
            cardControl.Content = content;
            Card = cardControl;
        }
    }
}
