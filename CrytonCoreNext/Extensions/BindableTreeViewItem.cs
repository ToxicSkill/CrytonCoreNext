using CrytonCoreNext.Models;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace CrytonCoreNext.Extensions;

public class BindableSelectedItemBehavior : Behavior<TreeView>
{
    public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register(
            "SelectedItem",
            typeof(object),
            typeof(BindableSelectedItemBehavior),
            new UIPropertyMetadata(null, OnSelectedItemChanged));

    public object SelectedItem
    {
        get
        {
            return this.GetValue(SelectedItemProperty);
        }

        set
        {
            this.SetValue(SelectedItemProperty, value);
        }
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        this.AssociatedObject.SelectedItemChanged += this.OnTreeViewSelectedItemChanged;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        if (this.AssociatedObject != null)
        {
            this.AssociatedObject.SelectedItemChanged -= this.OnTreeViewSelectedItemChanged;
        }
    }

    private static Action<int> GetBringIndexIntoView(Panel itemsHostPanel)
    {
        var virtualizingPanel = itemsHostPanel as VirtualizingStackPanel;
        if (virtualizingPanel == null)
        {
            return null;
        }

        var method = virtualizingPanel.GetType().GetMethod(
            "BringIndexIntoView",
            BindingFlags.Instance | BindingFlags.NonPublic,
            Type.DefaultBinder,
            new[] { typeof(int) },
            null);
        if (method == null)
        {
            return null;
        }

        return i => method.Invoke(virtualizingPanel, new object[] { i });
    }

    private static TreeViewItem GetTreeViewItem(ItemsControl container, object item, bool reverse)
    {
        if (container != null)
        {
            if (container.DataContext == item)
            {
                return container as TreeViewItem;
            }

            if (container is TreeViewItem treeViewItem)
            {
                if (treeViewItem.Header is TreeViewItemModel treeViewItemModel)
                {
                    container.SetValue(TreeViewItem.IsExpandedProperty, treeViewItemModel.IsExpanded);
                }
            }
            container.ApplyTemplate();
            var itemsPresenter =
                (ItemsPresenter)container.Template.FindName("ItemsHost", container);
            if (itemsPresenter != null)
            {
                itemsPresenter.ApplyTemplate();
            }
            else
            {
                itemsPresenter = container.GetVisualDescendant<ItemsPresenter>();
                if (itemsPresenter == null)
                {
                    container.UpdateLayout();
                    itemsPresenter = container.GetVisualDescendant<ItemsPresenter>();
                }
            }

            var itemsHostPanel = (Panel)VisualTreeHelper.GetChild(itemsPresenter, 0);

#pragma warning disable 168
            var children = itemsHostPanel.Children;
#pragma warning restore 168

            var bringIndexIntoView = GetBringIndexIntoView(itemsHostPanel);

            if (reverse)
            {
                for (int i = container.Items.Count - 1; i >= 0; i--)
                {
                    TreeViewItem subContainer;
                    if (bringIndexIntoView != null)
                    {
                        bringIndexIntoView(i);
                        subContainer =
                            (TreeViewItem)container.ItemContainerGenerator.
                                                    ContainerFromIndex(i);
                    }
                    else
                    {
                        subContainer =
                            (TreeViewItem)container.ItemContainerGenerator.
                                                    ContainerFromIndex(i);
                        subContainer.BringIntoView();
                    }

                    if (subContainer == null)
                    {
                        continue;
                    }

                    var resultContainer = GetTreeViewItem(subContainer, item, reverse);
                    if (resultContainer != null)
                    {
                        return resultContainer;
                    }
                    subContainer.IsExpanded = false;
                }
            }
            else
            {

                for (int i = 0, count = container.Items.Count; i < count; i++)
                {
                    TreeViewItem subContainer;
                    if (bringIndexIntoView != null)
                    {
                        bringIndexIntoView(i);
                        subContainer =
                            (TreeViewItem)container.ItemContainerGenerator.
                                                    ContainerFromIndex(i);
                    }
                    else
                    {
                        subContainer =
                            (TreeViewItem)container.ItemContainerGenerator.
                                                    ContainerFromIndex(i);

                        subContainer.BringIntoView();
                    }

                    if (subContainer == null)
                    {
                        continue;
                    }

                    var resultContainer = GetTreeViewItem(subContainer, item, reverse);
                    if (resultContainer != null)
                    {
                        return resultContainer;
                    }
                    subContainer.IsExpanded = false;
                }
            }
        }

        return null;
    }

    private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        var item = e.NewValue as TreeViewItem;
        if (item != null)
        {
            item.SetValue(TreeViewItem.IsSelectedProperty, true);
            return;
        }

        var behavior = (BindableSelectedItemBehavior)sender;
        var treeView = behavior.AssociatedObject;
        if (treeView == null)
        {
            return;
        }

        item = GetTreeViewItem(treeView, e.NewValue, false);
        var reversedItem = GetTreeViewItem(treeView, e.NewValue, true);
        if (item != null && item.Equals(reversedItem))
        {
            item.IsSelected = true;
        }
    }

    private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        this.SelectedItem = e.NewValue;
    }
}