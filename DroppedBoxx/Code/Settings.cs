using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using DroppedBoxx.Models;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Drawing;
using DroppedBoxx.Data;
using DroppedBoxx.ViewModels;

namespace DroppedBoxx.Code
{

    public class Settings
    {
        private const string _syncLogKey = "synclog";

        public Dictionary<double, string> CameraRes;

        public static Settings Instance;

        private string _configDirPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "DroppedBoxx";
        private string _settingsPath;

        //Properties
        private bool _rememberMe = false;

        public int Theme { get; set; }
        //Themes: 0 = Default, 1 = DropboxTheme, 2 = BlackAndWhite

        public long MaxSizeMB { get; set; } // 0 = no Limit
        public double CameraResMP { get; set; }
        public string CameraResText
        {
            get
            {
                return CameraRes[CameraResMP];
            }
        }

        public List<string> ExcludeExtentions { get; set; }

        public bool IsTrial { get; set; }
        public string UserToken { get; set; }
        public string UserSecret { get; set; }
        public bool RememberMe
        {
            get
            {
                return _rememberMe;
            }
            set
            {
                if (!value)
                {
                    UserToken = string.Empty;
                    UserSecret = string.Empty;
                }
                _rememberMe = value;
            }
        }

        public List<ItemViewModel> SyncFolders { get; set; }

        public string TempDirectory { get; set; }

        public Settings()
        {
            IsTrial = false;

            _settingsPath = _configDirPath + Path.DirectorySeparatorChar + "settings.config";
            TempDirectory = _configDirPath + Path.DirectorySeparatorChar + "Files" + Path.DirectorySeparatorChar;

            //check if they have saved settings
            FileInfo settingFile = new FileInfo(_settingsPath);
            if (settingFile.Exists)
            {
                //Load the settings from the file
                try
                {
                    var xDoc = XDocument.Load(_settingsPath);
                    //now load the xml settings...
                    LoadFromXml(xDoc);
                }
                catch
                {
                    LoadDefaults();
                }
            }
            else
            {
                //Load defaults
                LoadDefaults();
            }

            //Load the logs
            SyncFolders = JsonStorageProvider.RetrieveObject<List<ItemViewModel>>(_syncLogKey);
            if (SyncFolders == null) SyncFolders = new List<ItemViewModel>();

            CameraRes = new Dictionary<double, string>();
            CameraRes.Add(0, "VGA");
            CameraRes.Add(1, "1 MP");
            CameraRes.Add(2, "2 MP");
            CameraRes.Add(3, "3 MP");
            CameraRes.Add(4, "4 MP");
            CameraRes.Add(5, "5 MP");
            CameraRes.Add(6, "6 MP");
            CameraRes.Add(7, "7 MP");
        }

        private void LoadDefaults()
        {
            UserToken = string.Empty;
            UserSecret = string.Empty;
            SyncFolders = new List<ItemViewModel>();
            try
            {
                SyncFolders.Add(new ItemViewModel(new DirectoryInfo(@"\My Documents\"), null));
            }
            catch { }
        }

        public List<ItemViewModel> GetFolders()
        {
            return SyncFolders.OrderBy(s => s.Name).ToList();
        }

        public void AddFolder(DirectoryInfo dir)
        {
            if (SyncFolders.Any(s => s.Path == dir.FullName && s.Name == dir.Name))
            {
                return;
            }

            var syncFolder = new ItemViewModel(dir, null);

            SyncFolders.Add(syncFolder);
            //should i save here?
            Save();
        }

        public void RemoveFolder(ItemViewModel folder)
        {
            if (!SyncFolders.Contains(folder)) return;

            SyncFolders.Remove(folder);
            //should i save here?
            Save();
        }

        private void LoadFromXml(XDocument xDoc)
        {
            //TODO - Better error handling here...

            try
            {
                var userSizenode = xDoc.Element("droppedboxx").Element("MaxSizeMB");
                if (userSizenode != null) MaxSizeMB = Convert.ToInt64(userSizenode.Value);
            }
            catch { }

            try
            {
                var userCamnode = xDoc.Element("droppedboxx").Element("CameraResMP");
                if (userCamnode != null) CameraResMP = Convert.ToDouble(userCamnode.Value);
            }
            catch { }

            try
            {
                var userThemenode = xDoc.Element("droppedboxx").Element("Theme");
                if (userThemenode != null) Theme = Convert.ToInt32(userThemenode.Value);
            }
            catch { }

            var userTnode = xDoc.Element("droppedboxx").Element("UserToken");
            var userSnode = xDoc.Element("droppedboxx").Element("UserSecret");

            if (userTnode != null) UserToken = userTnode.Value;
            if (userSnode != null) UserSecret = userSnode.Value;

            if (!string.IsNullOrEmpty(UserToken) && !string.IsNullOrEmpty(UserSecret))
            {
                _rememberMe = true;
            }
        }

        private XDocument toXml()
        {
            var doc = new XDocument();
            var root = new XElement("droppedboxx");
            root.Add(new XElement("UserToken", UserToken));
            root.Add(new XElement("UserSecret", UserSecret));
            root.Add(new XElement("MaxSizeMB", MaxSizeMB.ToString()));
            root.Add(new XElement("CameraResMP", CameraResMP.ToString()));
            root.Add(new XElement("Theme", Theme.ToString()));

            doc.Add(root);

            return doc;
        }

        public bool Save()
        {
            //Trials dont need sync folders saved
            if (!IsTrial)
            {
                //Get the logs yo
                JsonStorageProvider.StoreObject<List<ItemViewModel>>(_syncLogKey, SyncFolders);
            }

            try
            {
                var dir = new DirectoryInfo(_configDirPath);
                if (!dir.Exists) dir.Create();

                this.toXml().Save(_settingsPath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes any Temp Files the app has downloaded (files are used by camera upload and email functions)
        /// </summary>
        public void DeleteTempFiles()
        {
            var tempDir = new DirectoryInfo(TempDirectory);
            if (tempDir.Exists)
            {
                foreach (var file in tempDir.GetFiles())
                {
                    try
                    {
                        file.Delete();
                    }
                    catch { }
                }
            }
        }

        public void ThemeUp()
        {
            if (Theme < 2)
            {
                Theme++;
                ReloadTheme();
            }
        }

        public void ThemeDown()
        {
            if (Theme > 0)
            {
                Theme--;
                ReloadTheme();
            }
        }

        public void ReloadTheme()
        {
            switch (Theme)
            {
                case 3:
                    DroppedBoxx.Theme.Current = new Themes.TestTheme();
                    break;
                case 2:
                    DroppedBoxx.Theme.Current = new Themes.BlackWhiteTheme();
                    break;
                case 1:
                    DroppedBoxx.Theme.Current = new Themes.DefaultTheme();
                    break;
                default:
                    DroppedBoxx.Theme.Current = new Themes.DropboxTheme();
                    break;
            }
        }

        public void CameraResUp()
        {
            int index = -1;
            var keyList = CameraRes.Keys.ToList();
            for (int i = 0; i < keyList.Count(); i++)
            {
                if (keyList[i] == CameraResMP)
                {
                    index = i;
                }
            }

            if (index == (keyList.Count() - 1))
            {
                //Max
                return;
            }

            index++;
            CameraResMP = keyList[index];
        }

        public void CameraResDown()
        {
            int index = -1;
            var keyList = CameraRes.Keys.ToList();
            for (int i = 0; i < keyList.Count(); i++)
            {
                if (keyList[i] == CameraResMP)
                {
                    index = i;
                }
            }

            if (index == 0)
            {
                //Min
                return;
            }

            index--;
            CameraResMP = keyList[index];
        }

        public Size GetCameraResolution()
        {
            if (CameraResMP == 0)
            {
                return new Size(640, 480);
            }
            else if (CameraResMP == 1)
            {
                return new Size(1280, 960);
            }
            else if (CameraResMP == 2)
            {
                return new Size(1600, 1200);
            }
            else if (CameraResMP == 3)
            {
                return new Size(2048, 1536);
            }
            else if (CameraResMP == 4)
            {
                return new Size(2289, 1712);
            }
            else if (CameraResMP == 5)
            {
                return new Size(2592, 1944);
            }
            else if (CameraResMP == 6)
            {
                return new Size(3000, 2000);
            }
            else if (CameraResMP == 7)
            {
                return new Size(3240, 2160);
            }
            return new Size(640, 480);
        }

    }
}
