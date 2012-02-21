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
using Fluid.Classes;
using DroppedBoxx.Sync;
using System.IO;
using DroppedBoxx.ViewModels;
using DroppedBoxx.Code;

namespace DroppedBoxx
{
    public class FoldersPanel : FluidPanel
    {
        private FluidHeader header = new FluidHeader();
        private ItemsListBox lsbFolders = new ItemsListBox();
        private ButtonPanel buttonPanel;
        private SyncPanel syncPanel;

        public ItemViewModel SelectedFolder { get; set; }

        public event EventHandler SelectFolder;
        public event EventHandler AddFolder;
        public event EventHandler BrowseDropbox;

        protected override void InitControl()
        {
            base.InitControl();
            Bounds = new Rectangle(0, 0, 240, 300);
            BackColor = Color.Green;
            Anchor = AnchorAll;
            const int h = 32;
            header.Bounds = new Rectangle(0, 0, 240, h);
            lsbFolders.Bounds = new Rectangle(0, h, 240, 260 - h);
            header.Anchor = AnchorTLR;
            lsbFolders.Anchor = AnchorAll;
            header.Title = "Sync Folders";
            header.BackColor = Theme.Current.HeaderBackColor;
            header.GradientFill = Theme.Current.HeaderGradianted;
            header.ForeColor = Theme.Current.HeaderForeColor;
            header.BackButton.Click += new EventHandler(BackButton_Click);
            header.BackButton.Visible = true;
            header.BackButton.Shape = ButtonShape.Back;
            header.BackButton.TextOffset = new Point(6, 0);
            header.BackButton.Text = "Logout";
            header.BackButton.BackColor = Theme.Current.LogoutButtonBackColor;
            header.BackButton.GradientFill = Theme.Current.ButtonsGradianted;

            FluidButton newFolderButton = new FluidButton("New");
            newFolderButton.Click += new EventHandler(newFolderButton_Click);
            newFolderButton.BackColor = Theme.Current.HeaderSecondaryButtonBackColor;
            newFolderButton.GradientFill = Theme.Current.ButtonsGradianted;
            header.Buttons.Add(newFolderButton);

            lsbFolders.ItemClick += new EventHandler<ListBoxItemEventArgs>(lsbFolders_FolderSelected);
            lsbFolders.NavigateBack += new EventHandler(lsbFolders_Back);

            buttonPanel = new ButtonPanel();
            buttonPanel.Bounds = new Rectangle(0, this.Height - 54, Width, 54);
            buttonPanel.Anchor = AnchorStyles.None;
            buttonPanel.Command += new EventHandler<CommandEventArgs>(buttonPanel_Command);
            
            Controls.Add(header);
            Controls.Add(lsbFolders);
            Controls.Add(buttonPanel);

            lsbFolders.ShowPath = true;
            lsbFolders.Items = Code.Settings.Instance.GetFolders();

            //Update the SyncStatus
            UpdateSyncStatus();
        }

        void BackButton_Click(object sender, EventArgs e)
        {
            //Logout
            Close(ShowTransition.None);
            Form1.Instance.OpenLogin();
        }

        void newFolderButton_Click(object sender, EventArgs e)
        {
            //Create new folder for syncage
            AddFolder(sender, e);
        }

        void lsbFolders_FolderSelected(object sender, ListBoxItemEventArgs e)
        {
            if (!Syncer.Instance.IsSyncing)
            {
                SelectedFolder = e.Item as ItemViewModel;
                //Check if the folder exists
                if (!Directory.Exists(SelectedFolder.Path))
                {
                    var syncFolderMsgBox = new MessageDialog();
                    syncFolderMsgBox.Message = string.Format("Folder No Longer exists{0}Remove From List?", Environment.NewLine);
                    syncFolderMsgBox.OkText = "Yes";
                    syncFolderMsgBox.CancelText = "No";
                    syncFolderMsgBox.Result += new EventHandler<Fluid.Classes.DialogEventArgs>(syncFolderMsgBox_Result);
                    syncFolderMsgBox.Show(ShowTransition.FromBottom);
                }
                else
                {
                    SelectFolder(sender, e);
                }
            }
        }

        private void syncFolderMsgBox_Result(object sender, DialogEventArgs e)
        {
            if (e.Result != DialogResult.OK) return;

            Host.Cursor = Cursors.WaitCursor;
            Code.Settings.Instance.RemoveFolder(SelectedFolder);
            UpdateData();
            Host.Cursor = Cursors.Default;
        }

        void lsbFolders_Back(object sender, EventArgs e)
        {
            //Should this logout? users might acciedently press it...
        }

        public override void Focus()
        {
            lsbFolders.Focus();
        }

        internal void UpdateData()
        {
            lsbFolders.Items = Code.Settings.Instance.GetFolders();
            lsbFolders.Update();
        }

        protected override void OnSizeChanged(System.Drawing.Size oldSize, System.Drawing.Size newSize)
        {
            base.OnSizeChanged(oldSize, newSize);

            //now fix the buttons panel
            if (buttonPanel != null)
            {
                buttonPanel.Bounds = new Rectangle(0, this.Height - ScaleY(buttonPanel.ButtonHeight + buttonPanel.LabelHeight), Width, ScaleY(buttonPanel.ButtonHeight + buttonPanel.LabelHeight));
            }

            if (syncPanel != null)
            {
                syncPanel.Bounds = new Rectangle(0, header.Height, Width, Height - header.Height - buttonPanel.Height);
            }
        }

        void buttonPanel_Command(object sender, CommandEventArgs e)
        {
            switch (e.Command)
            {
                case "A":
                case "1":
                    //Sync Button
                    if (Settings.Instance.IsTrial)
                    {
                        MessageDialog.Show("Sync feature only available in full version.", "OK", null);
                        return;
                    }

                    if (!Syncer.Instance.IsSyncing)
                    {
                        UpdateSyncStatus(true);

                        Syncer.Instance.Sync();

                        MessageDialog.Show("Syncing Started...", "OK", null);
                    }
                    else
                    {
                        MessageDialog.Show("Syncing in progress...", "OK", null);
                    }
                    break;

                case "B":
                case "2":
                    //Browse Dropbox
                    BrowseDropbox(sender, e);
                    break;

                case "C":
                case "3":
                    //Settings...?
                    Form1.Instance.OpenSettings();
                    break;

                case "D":
                case "4":
                    //Info?
                    Form1.Instance.OpenInfo();
                    break;

                case "E":
                case "5":
                    //Exit
                    Form1.Instance.Close();
                    break;
            }
        }

        public void UpdateSyncStatus()
        {
            UpdateSyncStatus(Syncer.Instance.IsSyncing);
        }

        public void UpdateSyncStatus(bool isSyncing)
        {
            if (isSyncing)
            {
                lsbFolders.Enabled = false;
                lsbFolders.IsSyncing = true;
                header.Buttons[0].Visible = false;
                header.Title = "Syncing...";
                buttonPanel.Buttons[0].Image = Resources.arrow_rotate_anticlockwise;

                if (lsbFolders.Visible)
                {
                    syncPanel = new SyncPanel();
                    syncPanel.Bounds = new Rectangle(0, header.Height/2, Width/2, (Height - header.Height - buttonPanel.Height)/2);
                    syncPanel.Show();
                    lsbFolders.Visible = false;
                }

                syncPanel.UpdateStatus(this, null);
            }
            else
            {
                lsbFolders.Enabled = true;
                lsbFolders.IsSyncing = false;
                header.Buttons[0].Visible = true;
                header.Title = "Sync Folders";
                buttonPanel.Buttons[0].Image = Resources.arrow_refresh;

                lsbFolders.Visible = true;
                if (syncPanel != null) syncPanel.Visible = false; //Maybe use Close()?
                
                syncPanel = null;
            }
        }

    }
}
