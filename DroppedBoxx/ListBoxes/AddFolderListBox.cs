using System;

using System.Collections.Generic;
using System.Text;
using Fluid.Controls;
using System.Drawing;
using DroppedBoxx.Templates;
using DroppedBoxx.Models;
using System.IO;
using DroppedBoxx.Code.Responses;

namespace DroppedBoxx.ListBoxes
{
    public class AddFolderListBox : ListBoxBase
    {
        private List<DirectoryInfo> _folders;
        public DirectoryInfo SelectedFolder { get; private set; }
        public event EventHandler FolderSelected;

        protected override void InitControl()
        {
            base.InitControl();
            ItemHeight = 32;

            ItemTemplate = new AddFolderTemplate();
        }
        public List<DirectoryInfo> Folders
        {
            get { return _folders; }
            set
            {
                if (_folders != value)
                {
                    _folders = value;
                    DataSource = value;
                }
            }
        }
        protected override void OnItemClick(int index)
        {
            base.OnItemClick(index);
            SelectedItemIndex = index;
            NavigateForward();
        }
        public override bool NavigateForward()
        {
            var folder = _folders[SelectedItemIndex] as DirectoryInfo;

            SelectedFolder = folder;
            if (folder != null)
            {
                if (FolderSelected != null) FolderSelected(this, EventArgs.Empty);
            }
            return true;
        }
    }
}
