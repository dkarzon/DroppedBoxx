using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using DroppedBoxx.Code;
using DroppedBoxx.Models;
using DroppedBoxx.Code.Responses;

namespace DroppedBoxx.Sync
{
    public enum SyncStatus
    {
        Checking,
        Uploading,
        Downloading,
        Nothing
    }


    public class Syncer
    {
        public static Syncer Instance = new Syncer();

        public bool IsSyncing { get; set; }
        public DateTime LastSync { get; set; }

        //public List<SyncFile> FilesUp { get; set; }
        //public List<SyncFile> FilesDown { get; set; }
        //public List<MetaData> FilesNew { get; set; }
        //public List<MetaData> FilesSkippedD { get; set; }
        //public List<SyncFile> FilesSkipped { get; set; }

        public long BytesSent { get; set; }
        public long BytesRecieved { get; set; }

        //Status Related
        public SyncStatus SyncStatus { get; set; }
        public string CurrentFolderPath { get; set; }
        public string CurrentFileName { get; set; }

        public Thread syncThreadThread;

        //DropBox Client
        public DropBox DropBox;

        public Syncer()
        {
            IsSyncing = false;
            SyncStatus = SyncStatus.Nothing;
        }

        public void Sync()
        {
            IsSyncing = true;

            SyncStatus = SyncStatus.Checking;

            //FilesUp = new List<SyncFile>();
            //FilesDown = new List<SyncFile>();
            //FilesNew = new List<MetaData>();
            //FilesSkipped = new List<SyncFile>();
            //FilesSkippedD = new List<MetaData>();

            DropBox = new DropBox();
            DropBox.UserLogin = Form1.Instance.DropBox.UserLogin;

            var syncThread = new SyncThread();
            syncThreadThread = new Thread(syncThread.Go);
            syncThreadThread.Start();
        }

        public void SyncFinished()
        {
            IsSyncing = false;
            LastSync = DateTime.Now;
            SyncStatus = SyncStatus.Nothing;

            Form1.Instance.DropBox.BytesRecieved += DropBox.BytesRecieved;
            Form1.Instance.DropBox.BytesSent += DropBox.BytesSent;

            syncThreadThread = null;
        }
    }

}
