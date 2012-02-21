using System;

using System.Collections.Generic;
using System.Text;
using Fluid.Controls;
using System.Drawing;
using DroppedBoxx.Templates;
using DroppedBoxx.Models;
using System.IO;

namespace DroppedBoxx.ListBoxes
{
    public class FileSystemListBox : ListBoxBase
    {
        private List<FileSystemInfo> items;
        public object SelectedItem { get; private set; }
        public event EventHandler ItemSelected;

        protected override void InitControl()
        {
            base.InitControl();
            ItemHeight = 32;

            ItemTemplate = new FileSystemTemplate();
        }
        public List<FileSystemInfo> Items
        {
            get { return items; }
            set
            {
                if (items != value)
                {
                    items = value;
                    DataSource = value;
                }
            }
        }
        protected override void OnItemClick(int index)
        {
            base.OnItemClick(index);
            SelectedItemIndex = index;
            SelectedItem = items[SelectedItemIndex];
            if (ItemSelected != null) ItemSelected(this, EventArgs.Empty);
        }

    }
}
