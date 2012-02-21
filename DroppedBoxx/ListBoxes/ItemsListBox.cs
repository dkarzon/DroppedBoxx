using System;
using System.Collections.Generic;
using System.Text;
using Fluid.Controls;
using System.Drawing;
using DroppedBoxx.Templates;
using DroppedBoxx.Models;
using DroppedBoxx.ViewModels;

namespace DroppedBoxx.ListBoxes
{
    public class ItemsListBox : ListBoxBase
    {
        private ItemTemplate fileTemplate;
        private List<ItemViewModel> _items;

        public bool ShowPath
        {
            get
            {
                return fileTemplate.ShowPath;
            }
            set
            {
                fileTemplate.ShowPath = value;
            }
        }

        public bool IsSyncing
        {
            get
            {
                return fileTemplate.IsSyncing;
            }
            set
            {
                fileTemplate.IsSyncing = value;
                DataSource = DataSource;
            }
        }

        public event EventHandler NavigateBack;

        protected override void InitControl()
        {
            base.InitControl();
            //ShowHeader = false;
            fileTemplate = new ItemTemplate();
            ItemTemplate = fileTemplate;
            ItemHeight = ItemTemplate.Height;
        }

        public List<ItemViewModel> Items
        {
            get { return _items; }
            set
            {
                if (_items != value)
                {
                    _items = value;
                    DataSource = value;
                }
            }
        }

        public override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
            //Do we need this? we dont edit items in the list
            switch (e.KeyChar)
            {
                case '\r': break;
                case '\t': SelectNextItem(); break;
                //default: fileTemplate.InsertKey(e); break;
            }
            base.OnKeyPress(e);
        }

        protected override void OnItemClick(int index)
        {
            base.OnItemClick(index);
        }

        public override bool NavigateBackward()
        {
            NavigateBack(this, EventArgs.Empty);
            return true;
        }

    }
}
