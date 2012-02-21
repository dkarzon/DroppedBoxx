using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using DroppedBoxx.Code;

namespace DroppedBoxx.Models
{
    public class DropboxFile
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public FileInfo LocalFileInfo { get; set; }

        public void SaveFile(string savePath)
        {
            var saveFile = new FileInfo(savePath);
            if (saveFile.Exists)
            {
                try
                {
                    saveFile.Delete();
                }
                catch { }
            }
            try
            {
                LocalFileInfo.MoveTo(savePath);
            }
            catch { }
        }

        public void Dispose()
        {
            if (LocalFileInfo.Exists)
            {
                try
                {
                    LocalFileInfo.Delete();
                }
                catch { }
            }
        }
    }
}
