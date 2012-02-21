using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using DroppedBoxx.Code.Responses;

namespace DroppedBoxx.Helpers
{
    public class ListHelpers
    {
        public static List<MetaData> MakeDirList(MetaData items)
        {
            var list = new List<MetaData>();

            foreach (var item in items.Contents.OrderByDescending(i => i.Is_Dir))
            {
                list.Add(item);
            }
            return list;
        }
    }
}
