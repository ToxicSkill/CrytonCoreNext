using System.Collections.Generic;
using Wpf.Ui.Common;

namespace CrytonCoreNext.Models
{
    public class TreeViewItemModel
    {
        public string Title { get; set; }

        public SymbolRegular Symbol { get; set; }

        public bool IsSelected { get; set; }

        public bool IsExpanded { get; set; }

        public List<TreeViewItemModel> Childs { get; set; }
    }
}
