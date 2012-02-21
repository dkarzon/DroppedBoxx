using System;
using System.Collections.Generic;
using System.Text;
using Fluid.Controls;
using System.Drawing;
using DroppedBoxx.Templates;
using DroppedBoxx.Models;
using DroppedBoxx.Code.Responses;

namespace DroppedBoxx.ListBoxes
{
    public class DropItemsListBox : ListBoxBase
    {
        private DropItemTemplate fileTemplate;
        private List<MetaData> _items;

        public event EventHandler NavigateBack;

        protected override void InitControl()
        {
            base.InitControl();
            //ShowHeader = false;
            fileTemplate = new DropItemTemplate();
            ItemTemplate = fileTemplate;
            ItemHeight = ItemTemplate.Height;
        }

        public List<MetaData> Items
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

        public override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            //TODO - Should possibly change this as well
            switch (e.KeyCode)
            {
                case System.Windows.Forms.Keys.Enter:
                    fileTemplate.Bind(SelectedItemIndex);
                    fileTemplate.Focus();
                    e.Handled = true;
                    break;
            }
            base.OnKeyDown(e);
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
