using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace DroppedBoxx.Code.Responses
{
    public class AccountInfo
    {
        public string Country { get; set; }
        public string Display_Name { get; set; }
        public QuotaInfo Quota_Info { get; set; }
        public string Uid { get; set; }
    }

    public class QuotaInfo
    {
        public long Shared { get; set; }
        public long Quota { get; set; }
        public long Normal { get; set; }
    }
}
