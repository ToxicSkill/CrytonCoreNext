using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CrytonCoreNext.Views
{
    /// <summary>
    /// Interaction logic for FilesSelectorListingView.xaml
    /// </summary>
    public partial class FilesSelectorListingView : UserControl
    {
        public FilesSelectorListingView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IncomingItemProperty =
            DependencyProperty.Register("IncomingItem", typeof(object), typeof(FilesSelectorListingView),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object IncomingItem
        {
            get { return (object)GetValue(IncomingItemProperty); }
            set { SetValue(IncomingItemProperty, value); }
        }

        public static readonly DependencyProperty RemovedItemProperty =
            DependencyProperty.Register("RemovedItem", typeof(object), typeof(FilesSelectorListingView),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object RemovedItem
        {
            get { return (object)GetValue(RemovedItemProperty); }
            set { SetValue(RemovedItemProperty, value); }
        }

        public static readonly DependencyProperty ItemDropCommandProperty =
            DependencyProperty.Register("ItemDropCommand", typeof(ICommand), typeof(FilesSelectorListingView),
                new PropertyMetadata(null));

        public ICommand ItemDropCommand
        {
            get { return (ICommand)GetValue(ItemDropCommandProperty); }
            set { SetValue(ItemDropCommandProperty, value); }
        }

        public static readonly DependencyProperty ItemRemovedCommandProperty =
            DependencyProperty.Register("ItemRemovedCommand", typeof(ICommand), typeof(FilesSelectorListingView),
                new PropertyMetadata(null));

        public ICommand ItemRemovedCommand
        {
            get { return (ICommand)GetValue(ItemRemovedCommandProperty); }
            set { SetValue(ItemRemovedCommandProperty, value); }
        }

        public static readonly DependencyProperty ItemInsertedCommandProperty =
            DependencyProperty.Register("ItemInsertedCommand", typeof(ICommand), typeof(FilesSelectorListingView),
                new PropertyMetadata(null));

        public ICommand ItemInsertedCommand
        {
            get { return (ICommand)GetValue(ItemInsertedCommandProperty); }
            set { SetValue(ItemInsertedCommandProperty, value); }
        }

        public static readonly DependencyProperty InsertedItemProperty =
            DependencyProperty.Register("InsertedItem", typeof(object), typeof(FilesSelectorListingView),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object InsertedItem
        {
            get { return (object)GetValue(InsertedItemProperty); }
            set { SetValue(InsertedItemProperty, value); }
        }

        public static readonly DependencyProperty TargetItemProperty =
            DependencyProperty.Register("TargetItem", typeof(object), typeof(FilesSelectorListingView),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object TargetItem
        {
            get { return (object)GetValue(TargetItemProperty); }
            set { SetValue(TargetItemProperty, value); }
        }

        private void Item_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed &&
                sender is FrameworkElement frameworkElement)
            {
                object Item = frameworkElement.DataContext;

                DragDropEffects dragDropResult = DragDrop.DoDragDrop(frameworkElement,
                    new DataObject(DataFormats.Serializable, Item),
                    DragDropEffects.Move);

                if (dragDropResult == DragDropEffects.None)
                {
                    AddItem(Item);
                }
            }
        }

        private void Item_DragOver(object sender, DragEventArgs e)
        {
            if (ItemInsertedCommand?.CanExecute(null) ?? false)
            {
                if (sender is FrameworkElement element)
                {
                    TargetItem = element.DataContext;
                    InsertedItem = e.Data.GetData(DataFormats.Serializable);

                    ItemInsertedCommand?.Execute(null);
                }
            }
        }

        private void ItemList_DragOver(object sender, DragEventArgs e)
        {
            object Item = e.Data.GetData(DataFormats.Serializable);
            AddItem(Item);
        }

        private void AddItem(object Item)
        {
            if (ItemDropCommand?.CanExecute(null) ?? false)
            {
                IncomingItem = Item;
                ItemDropCommand?.Execute(null);
            }
        }

        private void ItemList_DragLeave(object sender, DragEventArgs e)
        {
            HitTestResult result = VisualTreeHelper.HitTest(lvItems, e.GetPosition(lvItems));

            if (result == null)
            {
                if (ItemRemovedCommand?.CanExecute(null) ?? false)
                {
                    RemovedItem = e.Data.GetData(DataFormats.Serializable);
                    ItemRemovedCommand?.Execute(null);
                }
            }
        }
    }
}
