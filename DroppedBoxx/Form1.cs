using System;
using System.Drawing;
using System.Windows.Forms;
using DroppedBoxx.Code;
using DroppedBoxx.Sync;
using System.IO;

namespace DroppedBoxx
{
    public partial class Form1 : Form
    {
        public static string Version = "1.7";

        public Screen PrimaryScreen { get; set; }

        public Fluid.Controls.FluidHost Host;
        public LoginPanel Login;
        public DropBox DropBox;

        public static Form1 Instance;

        public FoldersPanel FoldersPanel;
        public AddFolderPanel AddFolderPanel;
        public FileSystemPanel DetailsPanel;
        public DropFolderPanel DropFolderPanel;
        public InfoPanel InfoPanel;
        public SettingsPanel SettingsPanel;

        //this does the keyboard thingy...
        public Microsoft.WindowsCE.Forms.InputPanel InputPanel;

        public Form1()
        {
            this.Text = "DroppedBoxx";

            Settings.Instance = new Settings();

            //bust a theme
            Settings.Instance.ReloadTheme();

            Instance = this;
            InitializeComponent();            

            Host.Bounds = Host.ClientBounds;
            Host.BackColor = Color.Empty;

            DropBox = new DropBox();

            //Login should always be open coz scrolling fails with nothing behind the panel... :S
            OpenLogin();

            if (Settings.Instance.RememberMe)
            {
                DropBox.UserLogin = new DroppedBoxx.Code.Responses.UserLogin { Token = Settings.Instance.UserToken, Secret = Settings.Instance.UserSecret };
                //auto Login
                OpenFoldersPanel();
            }
        }

        /// <summary>
        /// Opens the Login panel
        /// </summary>
        public void OpenLogin()
        {
            //Open the login screen
            Login = new LoginPanel(0, 0, 240, 400);
            Login.Enter += new EventHandler(login_Enter);
            Login.ShowMaximized();
        }

        /// <summary>
        /// Opens the Sync Folders panel
        /// </summary>
        public void OpenFoldersPanel()
        {
            FoldersPanel = new FoldersPanel();
            FoldersPanel.SelectFolder += new EventHandler(folder_Select);
            FoldersPanel.AddFolder += new EventHandler(FoldersPanel_AddFolder);
            FoldersPanel.BrowseDropbox += new EventHandler(FoldersPanel_BrowseDropbox);
            FoldersPanel.ShowMaximized();
        }

        /// <summary>
        /// Opens the Dropbox Browser panel
        /// </summary>
        public void OpenDropBrowser()
        {
            Host.Cursor = Cursors.WaitCursor;
            DropFolderPanel = new DropFolderPanel();
            Host.Cursor = Cursors.Default;
            DropFolderPanel.ShowMaximized();
            FoldersPanel.Close();
        }

        /// <summary>
        /// opens the Info/About panel
        /// </summary>
        public void OpenInfo()
        {
            Host.Cursor = Cursors.WaitCursor;
            InfoPanel = new InfoPanel();
            Host.Cursor = Cursors.Default;
            InfoPanel.ShowMaximized();
            FoldersPanel.Close();
        }

        //Opens the Settings panel
        public void OpenSettings()
        {
            SettingsPanel = new SettingsPanel();
            SettingsPanel.ShowMaximized();
            FoldersPanel.Close();
        }

        void login_Enter(object sender, EventArgs e)
        {
            //on login show the sync folder list
            OpenFoldersPanel();
        }

        void FoldersPanel_AddFolder(object sender, EventArgs e)
        {
            //on Add Folder button click show the folder browser
            Host.Cursor = Cursors.WaitCursor;
            AddFolderPanel = new AddFolderPanel();
            AddFolderPanel.SelectFolder += new EventHandler(folder_Add);
            Host.Cursor = Cursors.Default;
            AddFolderPanel.ShowMaximized();
        }

        void FoldersPanel_BrowseDropbox(object sender, EventArgs e)
        {
            //on Browse Dropbox button click show the Dropbox browser
            OpenDropBrowser();
        }

        void folder_Select(object sender, EventArgs e)
        {
            //on folder select open the details view
            Host.Cursor = Cursors.WaitCursor;

            DetailsPanel = new FileSystemPanel();
            DetailsPanel.ShowSelectButton("Remove");
            DetailsPanel.FolderSelected += DetailsPanel_FolderSelected;
            DetailsPanel.LoadItems(new DirectoryInfo(FoldersPanel.SelectedFolder.Path), true);
            DetailsPanel.ShowMaximized();
            Host.Cursor = Cursors.Default;
        }

        void DetailsPanel_FolderSelected(object sender, EventArgs e)
        {
            Host.Cursor = Cursors.WaitCursor;
            Settings.Instance.RemoveFolder(FoldersPanel.SelectedFolder);
            FoldersPanel.UpdateData();
            DetailsPanel.Close();
            Host.Cursor = Cursors.Default;
        }

        void folder_Add(object sender, EventArgs e)
        {
            if (AddFolderPanel.SelectedFolder != null)
            {
                //on new folder add, add the folder to settings...
                Host.Cursor = Cursors.WaitCursor;
                Settings.Instance.AddFolder(AddFolderPanel.SelectedFolder);
                //something something...
                FoldersPanel.UpdateData();
                AddFolderPanel.Close();
                Host.Cursor = Cursors.Default;
            }
        }

        private void Form1_Closed(object sender, EventArgs e)
        {
            Settings.Instance.Save();
            // Maybe this should be done as the files are used...?
            Settings.Instance.DeleteTempFiles();
            if (Syncer.Instance.syncThreadThread != null)
            {
                Syncer.Instance.syncThreadThread.Abort();
            }
            Close();
        }

        protected delegate void ShowSyncStatusDelegate();

        public void ShowSyncStatus()
        {
            if (this.InvokeRequired)
            {
                ShowSyncStatusDelegate dlg = new ShowSyncStatusDelegate(this.ShowSyncStatus);
                this.Invoke(dlg);
                return;
            }

            //do something with the GUI control here
            if (FoldersPanel.Visible)
            {
                //update the sync status stuff...
                FoldersPanel.UpdateSyncStatus();
            }
        }

        private delegate void DoActionDelegate(string message);

        public void DoAction(Action action)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(action);
            }
            else
            {
                action();
            }
        }
    }
}