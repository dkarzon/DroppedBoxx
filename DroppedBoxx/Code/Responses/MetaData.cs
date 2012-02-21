using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace DroppedBoxx.Code.Responses
{
    public class MetaData
    {
        public string error { get; set; }
        //public string Hash { get; set; }
        //public bool Thumb_Exists { get; set; }
        public long Bytes { get; set; }
        public DateTime Modified { get; set; }
        public string Path { get; set; }
        public bool Is_Dir { get; set; }
        public List<MetaData> Contents { get; set; }
        public MetaData Parent { get; set; }

        public string Name
        {
            get
            {
                if (Path.LastIndexOf("/") == -1)
                {
                    return string.Empty;
                }
                else
                {
                    return string.IsNullOrEmpty(Path) ? "root" : Path.Substring(Path.LastIndexOf("/") + 1);
                }
            }
        }
        public string Extension
        {
            get
            {
                if (Path.LastIndexOf(".") == -1)
                {
                    return string.Empty;
                }
                else
                {
                    return Is_Dir ? string.Empty : Path.Substring(Path.LastIndexOf("."));
                }
            }
        }
    }

}
