using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Fluid.Controls;
using System.Drawing;
using System.Windows.Forms;
using DroppedBoxx.ListBoxes;
using DroppedBoxx.Models;
using DroppedBoxx.Properties;
using System.IO;
using DroppedBoxx.Helpers;

namespace DroppedBoxx
{
    public class FileSystemPanel : FluidPanel
    {
        private FluidHeader header = new FluidHeader();
        private FileSystemListBox lsbItems = new FileSystemListBox();
        private FluidButton selectButton;

        public FileInfo SelectedItem { get; set; }
        public DirectoryInfo CurrentDir { get; set; }
        public DirectoryInfo RootDir { get; set; }

        public event EventHandler FileSelected;
        public event EventHandler FolderSelected;

        private bool _showFiles;

        public string ButtonText
        {
            set
            {
                if (selectButton != null) selectButton.Text = value;
            }
        }

        public FileSystemPanel()
        {
        }

        public void LoadItems(DirectoryInfo startingDir, bool showFiles)
        {
            _showFiles = showFiles;

            CurrentDir = startingDir ?? new DirectoryInfo("/");
            RootDir = CurrentDir;
            LoadItems();
        }

        public void ShowSelectButton(string buttonText)
        {
            selectButton = new FluidButton(string.IsNullOrEmpty(buttonText) ? "Select" : buttonText);
            selectButton.Click += new EventHandler(selectButton_Click);
            selectButton.BackColor = Theme.Current.HeaderSecondaryButtonBackColor;
            selectButton.GradientFill = Theme.Current.ButtonsGradianted;
            header.Buttons.Add(selectButton);
        }

        protected override void InitControl()
        {
            base.InitControl();

            Anchor = AnchorAll;

            Bounds = new Rectangle(0, 0, 240, 300);
            BackColor = Color.Green;
            Anchor = AnchorAll;
            const int h = 32;
            lsbItems.Bounds = new Rectangle(0, h, 240, Height - h);
            lsbItems.Anchor = AnchorAll;
            header.Bounds = new Rectangle(0, 0, 240, h);
            header.Anchor = AnchorTLR;
            header.Title = "/";
            header.BackColor = Theme.Current.HeaderBackColor;
            header.ForeColor = Theme.Current.HeaderForeColor;
            header.GradientFill = Theme.Current.HeaderGradianted;
            header.BackButton.Click += new EventHandler(BackButton_Click);
            header.BackButton.BackColor = Theme.Current.HeaderBackButtonBackColor;
            header.BackButton.Visible = true;
            header.BackButton.Text = "Back";
            header.BackButton.GradientFill = Theme.Current.ButtonsGradianted;

            lsbItems.ItemSelected += new EventHandler(lsbItems_ItemSelected);

            Controls.Add(header);
            Controls.Add(lsbItems);
        }

        private void LoadItems()
        {
            if (_showFiles)
            {
                var itemList = new List<FileSystemInfo>();
                itemList.AddRange(CurrentDir.GetDirectories().OrderBy(d => d.Name).ToArray());
                itemList.AddRange(CurrentDir.GetFiles().OrderBy(f => f.Name).ToArray());
                lsbItems.Items = itemList;
            }
            else
            {
                var itemList = new List<FileSystemInfo>();
                itemList.AddRange(CurrentDir.GetDirectories().OrderBy(d => d.Name).ToArray());
                lsbItems.Items = itemList;
            }
        }

        void BackButton_Click(object sender, EventArgs e)
        {
            //Check if they are at root
            if (CurrentDir.FullName == CurrentDir.Root.FullName)
            {
                Close(ShowTransition.FromBottom);
            }
            else if (CurrentDir.FullName == RootDir.FullName)
            {
                Close(ShowTransition.FromBottom);
            }
            else
            {
                //Goes Up a dir...
                Host.Cursor = Cursors.WaitCursor;
                CurrentDir = CurrentDir.Parent;
                LoadItems();
                lsbItems.SelectedItemIndex = -1;
                header.Title = CurrentDir.Name;
                Host.Cursor = Cursors.Default;
            }
        }

        void selectButton_Click(object sender, EventArgs e)
        {
            //Folder Selected
            if (FolderSelected != null) FolderSelected(sender, e);
            this.Hide();
        }

        void lsbItems_ItemSelected(object sender, EventArgs e)
        {
            if (lsbItems.SelectedItem is DirectoryInfo)
            {
                Host.Cursor = Cursors.WaitCursor;
                CurrentDir = lsbItems.SelectedItem as DirectoryInfo;
                LoadItems();
                lsbItems.SelectedItemIndex = -1;
                header.Title = CurrentDir.Name;
                Host.Cursor = Cursors.Default;
            }
            else if (lsbItems.SelectedItem is FileInfo)
            {
                SelectedItem = lsbItems.SelectedItem as FileInfo;
                if (FileSelected != null)
                {
                    FileSelected(sender, e);
                    this.Hide();
                }
            }
        }

        public override void Focus()
        {
            lsbItems.Focus();
        }

        internal void UpdateData()
        {
            lsbItems.Update();
        }

        protected override void OnSizeChanged(System.Drawing.Size oldSize, System.Drawing.Size newSize)
        {
            base.OnSizeChanged(oldSize, newSize);
        }

    }
}
