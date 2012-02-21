using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using DroppedBoxx.Enums;
using System.IO;

namespace DroppedBoxx.ViewModels
{
    public class ItemViewModel
    {
        public bool IsFolder { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public DateTime? LastSync { get; set; }
        public SyncResultEnum LastResult { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public ItemViewModel ParentFolder { get; set; }
        
        public List<ItemViewModel> SubFolders { get; set; }
        public List<ItemViewModel> Files { get; set; }

        public ItemViewModel() { }

        public ItemViewModel(DirectoryInfo dir, ItemViewModel parent)
        {
            ParentFolder = parent;
            IsFolder = true;
            Path = dir.FullName;
            Name = dir.Name;

            DirectoryInfo[] subfolders = null;
            try
            {
                subfolders = dir.GetDirectories();
            }
            catch { }

            SubFolders = new List<ItemViewModel>();
            if (subfolders != null)
            {
                foreach (var d in subfolders)
                {
                    SubFolders.Add(new ItemViewModel(d, this));
                }
            }

            FileInfo[] files = null;
            try
            {
                files = dir.GetFiles();
            }
            catch { }

            Files = new List<ItemViewModel>();
            if (files != null)
            {
                foreach (var f in files)
                {
                    Files.Add(new ItemViewModel(f, this));
                }
            }
        }

        public ItemViewModel(FileInfo file, ItemViewModel parent)
        {
            ParentFolder = parent;
            Path = file.FullName;
            Name = file.Name;
            Extension = file.Extension;
        }

        public List<ItemViewModel> GetContents()
        {
            var contents = new List<ItemViewModel>();
            contents.AddRange(SubFolders);
            contents.AddRange(Files);
            return contents;
        }
    }
}
