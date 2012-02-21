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
using DroppedBoxx.Code.Responses;
using Microsoft.WindowsMobile.Forms;
using Microsoft.WindowsMobile.PocketOutlook;
using DroppedBoxx.Code;
using Fluid.Classes;
using Microsoft.Win32;
using DroppedBoxx.Helpers;

namespace DroppedBoxx
{
    public class DropFolderPanel : FluidPanel
    {
        private FluidHeader header = new FluidHeader();
        private DropItemsListBox lsbDropbox = new DropItemsListBox();
        private FileOptionButtonPanel fileButtonPanel;
        private DropOptionButtonPanel buttonPanel;

        //For saving files
        private FileSystemPanel lsbSave;
        private FileSystemPanel lsbUpload;

        private TextInputPanel textInputPanel;

        private MetaData _rootItem;
        private MetaData _selectedItem;
        private MetaData _clipboard; 

        protected override void InitControl()
        {
            base.InitControl();
            Bounds = new Rectangle(0, 0, 240, 300);
            BackColor = Color.Green;
            Anchor = AnchorAll;
            const int h = 32;
            header.Bounds = new Rectangle(0, 0, 240, h);
            lsbDropbox.Bounds = new Rectangle(0, h, 240, 254 - h);
            header.Anchor = AnchorTLR;
            lsbDropbox.Anchor = AnchorAll;
            header.Title = "/";
            header.BackColor = Theme.Current.HeaderBackColor;
            header.ForeColor = Theme.Current.HeaderForeColor;
            header.GradientFill = Theme.Current.HeaderGradianted;
            header.BackButton.Click += new EventHandler(BackButton_Click);
            header.BackButton.BackColor = Theme.Current.HeaderBackButtonBackColor;
            header.BackButton.Visible = true;
            header.BackButton.Text = "Back";
            header.BackButton.GradientFill = Theme.Current.ButtonsGradianted;

            //Maybe make this an upload button...?
            FluidButton btnUpload = new FluidButton("Upload");
            btnUpload.Click += new EventHandler(btnUpload_Click);
            btnUpload.BackColor = Theme.Current.HeaderSecondaryButtonBackColor;
            btnUpload.GradientFill = Theme.Current.ButtonsGradianted;
            header.Buttons.Add(btnUpload);

            Controls.Add(header);

            lsbDropbox.ItemClick += new EventHandler<ListBoxItemEventArgs>(lsbDropbox_ItemClick);
            lsbDropbox.NavigateBack += new EventHandler(BackButton_Click);

            Controls.Add(lsbDropbox);

            //Options Panel
            buttonPanel = new DropOptionButtonPanel();
            buttonPanel.Bounds = new Rectangle(0, this.Height - 54, Width, 54);
            buttonPanel.Anchor = AnchorStyles.None;
            buttonPanel.Command += new EventHandler<CommandEventArgs>(buttonPanel_Command);
            Controls.Add(buttonPanel);

            //File Options Panel
            fileButtonPanel = new FileOptionButtonPanel();
            fileButtonPanel.Bounds = new Rectangle(0, this.Height - 54, Width, 54);
            fileButtonPanel.Anchor = AnchorStyles.None;
            fileButtonPanel.Command += new EventHandler<CommandEventArgs>(fileButtonPanel_Command);
            fileButtonPanel.Hide();
            Controls.Add(fileButtonPanel);

            _rootItem = Form1.Instance.DropBox.GetItems();
            if (_rootItem == null)
            {
                //Failed
                Form1.Instance.OpenFoldersPanel();
                Close(ShowTransition.FromBottom);

                MessageDialog.Show("Failed to Get Dropbox Folder", null, "OK");
            }
            else
            {
                _selectedItem = _rootItem;

                lsbDropbox.Items = ListHelpers.MakeDirList(_rootItem);
            }
        }

        void BackButton_Click(object sender, EventArgs e)
        {
            //if a file is selected deselect it and reselect the folder
            if (!_selectedItem.Is_Dir) _selectedItem = _selectedItem.Parent;

            //Check if they are at root
            if (_selectedItem.Parent == null)
            {
                if (fileButtonPanel != null)
                {
                    fileButtonPanel.Close();
                }
                Form1.Instance.OpenFoldersPanel();
                Close(ShowTransition.FromBottom);
            }
            else
            {
                //Goes Up a dir...
                Host.Cursor = Cursors.WaitCursor;
                if (fileButtonPanel != null)
                {
                    fileButtonPanel.Close(ShowTransition.FromBottom); //hide file buttons
                }
                _selectedItem = _selectedItem.Parent;
                // Cached...?
                if (_selectedItem.Parent == null)
                {
                    //At the root
                    header.BackButton.Text = "Back";
                }
                else
                {
                    header.BackButton.Text = "Up";
                }
                //lsbFolders.Items = MakeDirList(Form1.Instance.DropBox.GetItems(_selectedItem));
                lsbDropbox.Items = ListHelpers.MakeDirList(_selectedItem);
                lsbDropbox.SelectedItemIndex = -1;
                header.Title = string.IsNullOrEmpty(_selectedItem.Name) ? "/" : _selectedItem.Name;
                Host.Cursor = Cursors.Default;
            }
        }

        void btnUpload_Click(object sender, EventArgs e)
        {
            //Close file Menu if its open
            if (fileButtonPanel != null)
            {
                fileButtonPanel.Hide();
            }
            //Upload a File
            lsbUpload = new FileSystemPanel();
            lsbUpload.LoadItems(null, true);
            lsbUpload.FileSelected += new EventHandler(lsbUpload_SelectedFile);
            //lsbUpload.Show(ShowTransition.FromBottom);
            Controls.Add(lsbUpload);
            lsbUpload.Bounds = new Rectangle(0, 0, Width, Height);
        }

        private void lsbUpload_SelectedFile(object sender, EventArgs e)
        {
            Host.Cursor = Cursors.WaitCursor;
            //If a File is selected select the folder its in
            if (!_selectedItem.Is_Dir) _selectedItem = _selectedItem.Parent;
            if (Form1.Instance.DropBox.UploadFile(lsbUpload.SelectedItem, _selectedItem))
            {
                //Reload current folder...?
                lsbDropbox.Items = ListHelpers.MakeDirList(Form1.Instance.DropBox.GetItems(_selectedItem));
                Host.Cursor = Cursors.Default;
            }
            else
            {
                Host.Cursor = Cursors.Default;
                MessageDialog.Show("Failed to Upload File!", null, "OK");
            }
        }

        void lsbDropbox_ItemClick(object sender, ListBoxItemEventArgs e)
        {
            //if a file is selected deselect it and reselect the folder
            if (!_selectedItem.Is_Dir) _selectedItem = _selectedItem.Parent;

            var selected = e.Item as MetaData;
            if (selected.Is_Dir)
            {
                Host.Cursor = Cursors.WaitCursor;
                //hide file buttons
                fileButtonPanel.Hide();
                //Get the selected folders contents
                var newSelected = Form1.Instance.DropBox.GetItems(selected);
                if (newSelected == null)
                {
                    Host.Cursor = Cursors.Default;
                    MessageDialog.Show("Failed to Get Dropbox Folder" + Environment.NewLine + "Check Data Connection.", null, "OK");
                }
                else
                {
                    header.BackButton.Text = "Up";
                    newSelected.Parent = _selectedItem;
                    _selectedItem = newSelected;
                    lsbDropbox.Items = ListHelpers.MakeDirList(_selectedItem);
                    lsbDropbox.SelectedItemIndex = -1;
                    header.Title = _selectedItem.Name;
                    Host.Cursor = Cursors.Default;
                }
            }
            else
            {
                selected.Parent = _selectedItem;
                _selectedItem = selected;

                fileButtonPanel.Show(ShowTransition.FromBottom);
            }
        }

        public override void Focus()
        {
            lsbDropbox.Focus();
        }

        internal void UpdateData()
        {
            lsbDropbox.Update();
        }

        protected override void OnSizeChanged(System.Drawing.Size oldSize, System.Drawing.Size newSize)
        {
            base.OnSizeChanged(oldSize, newSize);

            //now fix the buttons panel
            if (buttonPanel != null)
            {
                buttonPanel.Bounds = new Rectangle(0, this.Height - ScaleY(buttonPanel.ButtonHeight + buttonPanel.LabelHeight), Width, ScaleY(buttonPanel.ButtonHeight + buttonPanel.LabelHeight));
            }
            if (fileButtonPanel != null)
            {
                fileButtonPanel.Bounds = new Rectangle(0, this.Height - ScaleY(fileButtonPanel.ButtonHeight + 12), Width, ScaleY(fileButtonPanel.ButtonHeight + 12));
            }
            //if (fileButtonPanel != null) fileButtonPanel.Bounds = new Rectangle(0, this.Height - 48, Width, 48);

            if (lsbUpload != null) lsbUpload.Bounds = new Rectangle(0, 0, Width, Height);
            if (lsbSave != null) lsbSave.Bounds = new Rectangle(0, 0, Width, Height);
        }


        void buttonPanel_Command(object sender, CommandEventArgs e)
        {
            switch (e.Command)
            {
                case "A":
                case "1":
                    //Camera
                    DoCameraMagic();
                    break;
                case "B":
                case "2":
                    //New Folder
                    textInputPanel = new TextInputPanel("Folder Name:", 0, 40, 240, 100);
                    textInputPanel.Result += new EventHandler<Fluid.Classes.DialogEventArgs>(newFolder_submit);
                    break;
                case "C":
                case "3":
                    //Delete Folder
                    //deselect the file and select the containing folder
                    if (!_selectedItem.Is_Dir) _selectedItem = _selectedItem.Parent;

                    //Check if is root...
                    if (_selectedItem.Parent == null)
                    {
                        //is root
                        return;
                    }

                    var deleteFolderMsgBox = new MessageDialog();
                    deleteFolderMsgBox.Message = string.Format("Really Delete Folder?{0}{1}", Environment.NewLine, _selectedItem.Name);
                    deleteFolderMsgBox.OkText = "Yes";
                    deleteFolderMsgBox.CancelText = "No";
                    deleteFolderMsgBox.Result += new EventHandler<Fluid.Classes.DialogEventArgs>(deleteFolderMsgBox_Result);
                    deleteFolderMsgBox.Show(ShowTransition.FromBottom);
                    break;
                case "D":
                case "4":
                    //Paste
                    //see if there is something in the clipboard

                    if (_clipboard == null)
                    {
                        MessageDialog.Show("There is nothing to Paste.", null, "OK");
                    }
                    else
                    {
                        var copyMsgBox = new MessageDialog();
                        copyMsgBox.Message = string.Format("Pasting{0}{1}", Environment.NewLine, _clipboard.Name);
                        copyMsgBox.OkText = "Copy";
                        copyMsgBox.CancelText = "Move";
                        copyMsgBox.Result += new EventHandler<Fluid.Classes.DialogEventArgs>(copyMsgBox_Result);
                        copyMsgBox.Show(ShowTransition.FromBottom);
                    }
                    break;
                case "E":
                case "5":
                    //Back
                    if (fileButtonPanel != null)
                    {
                        fileButtonPanel.Close();
                    }
                    Form1.Instance.OpenFoldersPanel();
                    Close(ShowTransition.FromBottom);
                    break;
            }
        }

        private void DoCameraMagic()
        {
            //check droppedboxx folder exists
            var dropfolder = new DirectoryInfo(Settings.Instance.TempDirectory);
            if (!dropfolder.Exists) dropfolder.Create();

            string originalFileName;
            using (CameraCaptureDialog dlg = new CameraCaptureDialog())
            {
                dlg.Mode = CameraCaptureMode.Still;
                dlg.InitialDirectory = dropfolder.FullName;
                dlg.StillQuality = CameraCaptureStillQuality.High;
                dlg.Resolution = Settings.Instance.GetCameraResolution();
                dlg.Title = "Take a Photo";
                DialogResult res;
                try
                {
                    res = dlg.ShowDialog();
                    if (res != DialogResult.OK) return;
                }
                catch (Exception ex)
                {
                }

                this.Refresh();
                originalFileName = dlg.FileName;
            }
            //Now save the file!
            Host.Cursor = Cursors.WaitCursor;
            //If a File is selected select the folder its in
            if (!_selectedItem.Is_Dir) _selectedItem = _selectedItem.Parent;

            var tempFile = new FileInfo(originalFileName);

            MessageDialog.Show("Photo Uploading...", "OK", null);

            Uploader.BackgroundUpload(tempFile, _selectedItem);

            Host.Cursor = Cursors.Default;

        }

        private void newFolder_submit(object sender, DialogEventArgs e)
        {
            if (e.Result != DialogResult.OK) return;

            //If a File is selected select the folder its in
            if (!_selectedItem.Is_Dir) _selectedItem = _selectedItem.Parent;

            var folderName = textInputPanel.Data;

            Host.Cursor = Cursors.WaitCursor;
            var newSelected = Form1.Instance.DropBox.CreateFolder(_selectedItem, folderName);
            if (newSelected != null)
            {
                newSelected.Parent = _selectedItem;
                _selectedItem.Contents.Add(newSelected);
                _selectedItem = newSelected;
                lsbDropbox.Items = ListHelpers.MakeDirList(_selectedItem);
                lsbDropbox.SelectedItemIndex = -1;
                header.Title = _selectedItem.Name;
                Host.Cursor = Cursors.Default;
            }
            else
            {
                //show error...?
                Host.Cursor = Cursors.Default;
                MessageDialog.Show("Create Folder Failed!", null, "OK");
            }
        }

        private void deleteFolderMsgBox_Result(object sender, DialogEventArgs e)
        {
            if (e.Result != DialogResult.OK) return;

            //Delete Selected folder...
            Host.Cursor = Cursors.WaitCursor;
            //Delete Folder
            if (Form1.Instance.DropBox.Delete(_selectedItem))
            {
                //Now load the parent Directory
                _selectedItem = _selectedItem.Parent;
                lsbDropbox.Items = ListHelpers.MakeDirList(Form1.Instance.DropBox.GetItems(_selectedItem));
                lsbDropbox.SelectedItemIndex = -1;
                header.Title = _selectedItem.Name;
                Host.Cursor = Cursors.Default;
            }
            else
            {
                Host.Cursor = Cursors.Default;
                MessageDialog.Show("Failed to Delete!", null, "OK");
            }
        }

        private void copyMsgBox_Result(object sender, DialogEventArgs e)
        {
            //If a File is selected select the folder its in
            if (!_selectedItem.Is_Dir) _selectedItem = _selectedItem.Parent;

            Host.Cursor = Cursors.WaitCursor;

            if (e.Result == DialogResult.OK)
            {
                //copy
                if (!Form1.Instance.DropBox.CopyFile(_clipboard, _selectedItem))
                {
                    Host.Cursor = Cursors.Default;
                    MessageDialog.Show("Failed to Copy!", null, "OK");
                }
            }
            else if (e.Result == DialogResult.Cancel)
            {
                //move
                if (!Form1.Instance.DropBox.MoveFile(_clipboard, _selectedItem))
                {
                    Host.Cursor = Cursors.Default;
                    MessageDialog.Show("Failed to Move!", null, "OK");
                }
            }

            lsbDropbox.Items = ListHelpers.MakeDirList(Form1.Instance.DropBox.GetItems(_selectedItem));
            Host.Cursor = Cursors.Default;
            _clipboard = null;
        }


        void fileButtonPanel_Command(object sender, CommandEventArgs e)
        {
            switch (e.Command)
            {
                case "A":
                case "1":
                    //Download - Maybe set somewhere to save to...?
                    DoFileSave();
                    break;

                case "B":
                case "2":
                    //Email Attach
                    DoEmailMagic();
                    break;

                case "C":
                case "3":
                    //Copy
                    _clipboard = _selectedItem;
                    MessageDialog.Show("File in Clipboard" + Environment.NewLine + "Now Paste in a Folder", "OK", null);
                    fileButtonPanel.Hide();
                    break;

                case "D":
                case "4":
                    //Delete?
                    var deleteMsgBox = new MessageDialog();
                    deleteMsgBox.Message = string.Format("Really Delete?{0}{1}", Environment.NewLine, _selectedItem.Name);
                    deleteMsgBox.OkText = "Yes";
                    deleteMsgBox.CancelText = "No";
                    deleteMsgBox.Result += new EventHandler<Fluid.Classes.DialogEventArgs>(deleteMsgBox_Result);
                    deleteMsgBox.Show(ShowTransition.FromBottom);
                    break;

                case "E":
                case "5":
                    //Close Menu
                    fileButtonPanel.Hide();
                    break;
            }
        }

        private void DoFileSave()
        {
            //Close file Menu if its open
            if (fileButtonPanel != null) fileButtonPanel.Hide();
            //open the filesystem panel to select a folder to save to.
            lsbSave = new FileSystemPanel();
            lsbSave.ShowSelectButton("Select");
            lsbSave.LoadItems(null, false);
            lsbSave.FolderSelected += new EventHandler(lsbSave_FolderSelected);
            //lsbSave.Show(ShowTransition.FromBottom);
            Controls.Add(lsbSave);
            lsbSave.Bounds = new Rectangle(0, 0, Width, Height);
        }

        private void lsbSave_FolderSelected(object sender, EventArgs e)
        {
            Host.Cursor = Cursors.WaitCursor;
            var downloadFile = Form1.Instance.DropBox.GetFile(_selectedItem);
            if (downloadFile != null)
            {
                //save file
                downloadFile.SaveFile(string.Format("{0}/{1}", lsbSave.CurrentDir.FullName, downloadFile.Name));
                fileButtonPanel.Hide();
                Host.Cursor = Cursors.Default;
                var fileDownloadDiag = new MessageDialog();
                fileDownloadDiag.Message = "File Downloaded!" + Environment.NewLine + "Open with Default?";
                fileDownloadDiag.OkText = "Yes";
                fileDownloadDiag.CancelText = "No";
                //fileDownloadDiag.Result += new EventHandler<Fluid.Classes.DialogEventArgs>(fileDownloadDiag_Result);
                fileDownloadDiag.Result += (diagSender, result2) =>
                {
                    if (result2.Result == DialogResult.OK)
                    {
                        OpenFileWithDefault(string.Format("{0}/{1}", lsbSave.CurrentDir.FullName, downloadFile.Name));
                    }
                };
                fileDownloadDiag.Show(ShowTransition.FromBottom);
            }
            else
            {
                //show error...?
                Host.Cursor = Cursors.Default;
                MessageDialog.Show("File Download Failed!", null, "OK");
            }
        }

        private void OpenFileWithDefault(string filename)
        {
            try
            {
                //open it
                System.Diagnostics.Process.Start(filename, "");

                //string AppName = string.Empty;
                //string ShellAppName = string.Empty;
                //if (GetRegisteredApplication(filename, filename.Substring(filename.LastIndexOf(".")).ToLower(), ref AppName, ref ShellAppName))
                //{
                //}
                //else
                //{
                //    throw new NotImplementedException();
                //}
            }
            catch (Exception ex)
            {
                MessageDialog.Show("Unable to open file with default application", null, "OK");
            }
        }

        private void deleteMsgBox_Result(object sender, Fluid.Classes.DialogEventArgs e)
        {
            if (e.Result == DialogResult.OK)
            {
                Host.Cursor = Cursors.WaitCursor;
                //Delete File
                if (Form1.Instance.DropBox.Delete(_selectedItem))
                {
                    //Reload current folder...?
                    fileButtonPanel.Hide();
                    lsbDropbox.Items = ListHelpers.MakeDirList(Form1.Instance.DropBox.GetItems(_selectedItem.Parent));
                    Host.Cursor = Cursors.Default;
                }
                else
                {
                    Host.Cursor = Cursors.Default;
                    MessageDialog.Show("Failed to Delete!", null, "OK");
                }
            }
        }


        private void DoEmailMagic()
        {
            Host.Cursor = Cursors.WaitCursor;

            //check droppedboxx folder exists
            var dropfolder = new DirectoryInfo(Settings.Instance.TempDirectory);
            if (!dropfolder.Exists) dropfolder.Create();

            var downloadFile = Form1.Instance.DropBox.GetFile(_selectedItem);
            if (downloadFile != null)
            {
                //Attach file

                var outlook = new OutlookSession();
                var message = new EmailMessage();

                message.Subject = "Sending File: " + _selectedItem.Name;
                message.Attachments.Add(new Attachment(downloadFile.LocalFileInfo.FullName));

                MessagingApplication.DisplayComposeForm(message);

                outlook.Dispose();
                Host.Cursor = Cursors.Default;
            }
            else
            {
                //show error...?
                Host.Cursor = Cursors.Default;
                MessageDialog.Show("File Download Failed!", null, "OK");
            }
        }


    }
}
