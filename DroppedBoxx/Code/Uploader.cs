using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using DroppedBoxx.Code.Responses;
using Fluid.Controls;
using System.Threading;

namespace DroppedBoxx.Code
{
    public class Uploader
    {

        public static void BackgroundUpload(FileInfo localFile, MetaData remoteDir)
        {
            var uploader = new Uploader(localFile, remoteDir);
            var uploadThread = new Thread(uploader.UploadFile);
            uploadThread.Start();
        }


        private FileInfo _localFile;
        private MetaData _remoteDir;

        public Uploader(FileInfo localFile, MetaData remoteDir)
        {
            _localFile = localFile;
            _remoteDir = remoteDir;
        }

        public void UploadFile()
        {
            Form1.Instance.DropBox.UploadFile(_localFile, _remoteDir);

            //Delete Temp File
            try
            {
                _localFile.Delete();
            }
            catch { }
        }

    }
}
