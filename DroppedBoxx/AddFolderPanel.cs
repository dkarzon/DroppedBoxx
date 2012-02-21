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

namespace DroppedBoxx
{
    public class AddFolderPanel : FluidPanel
    {
        private FluidHeader header = new FluidHeader();
        private AddFolderListBox lsbFolders = new AddFolderListBox();

        public DirectoryInfo SelectedFolder { get; set; }

        private DirectoryInfo CurrentDir;

        public event EventHandler SelectFolder;

        protected override void InitControl()
        {
            base.InitControl();
            Bounds = new Rectangle(0, 0, 240, 300);
            BackColor = Color.Green;
            Anchor = AnchorAll;
            const int h = 32;
            lsbFolders.Bounds = new Rectangle(0, h, 240, 300 - h);
            lsbFolders.Anchor = AnchorAll;
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

            FluidButton addFolderButton = new FluidButton("Add");
            addFolderButton.Click += new EventHandler(addFolderButton_Click);
            addFolderButton.BackColor = Theme.Current.HeaderSecondaryButtonBackColor;
            addFolderButton.GradientFill = Theme.Current.ButtonsGradianted;
            header.Buttons.Add(addFolderButton);

            lsbFolders.FolderSelected += new EventHandler(lsbFolders_FolderSelected);
            
            Controls.Add(header);
            Controls.Add(lsbFolders);

            CurrentDir = new DirectoryInfo("/");

            lsbFolders.Folders = MakeDirList(CurrentDir.GetDirectories());
        }

        private List<DirectoryInfo> MakeDirList(DirectoryInfo[] dirs)
        {
            var list = new List<DirectoryInfo>();
            foreach (DirectoryInfo dir in dirs.OrderBy(d => d.Name))
            {
                list.Add(dir);
            }
            return list;
        }

        void BackButton_Click(object sender, EventArgs e)
        {
            //Check if they are at root
            if (CurrentDir.FullName == CurrentDir.Root.FullName)
            {
                Close(ShowTransition.FromBottom);
            }
            else
            {
                //Goes Up a dir...
                Host.Cursor = Cursors.WaitCursor;
                CurrentDir = CurrentDir.Parent;
                lsbFolders.Folders = MakeDirList(CurrentDir.GetDirectories());
                lsbFolders.SelectedItemIndex = -1;
                header.Title = CurrentDir.Name;
                Host.Cursor = Cursors.Default;
            }
        }

        void addFolderButton_Click(object sender, EventArgs e)
        {
            //Create new folder for syncage
            SelectedFolder = lsbFolders.SelectedFolder;
            SelectFolder(sender, e);
        }

        void lsbFolders_FolderSelected(object sender, EventArgs e)
        {
            Host.Cursor = Cursors.WaitCursor;
            CurrentDir = lsbFolders.SelectedFolder;
            lsbFolders.Folders = MakeDirList(CurrentDir.GetDirectories());
            lsbFolders.SelectedItemIndex = -1;
            header.Title = CurrentDir.Name;
            Host.Cursor = Cursors.Default;
        }

        public override void Focus()
        {
            lsbFolders.Focus();
        }

        internal void UpdateData()
        {
            lsbFolders.Update();
        }
    }
}
