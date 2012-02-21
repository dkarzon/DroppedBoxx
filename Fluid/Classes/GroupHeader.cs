using System;

using System.Collections.Generic;
using System.Text;

namespace Fluid.Controls
{
    /// <summary>
    /// Implements a default IGroupHeader with a simple title.
    /// </summary>
    public class GroupHeader : IGroupHeader
    {

        public GroupHeader()
            : base()
        {
        }

        public GroupHeader(string title)
            : base()
        {
            this.Title = title;
        }
        #region IGroupHeader Members

        public string Title
        {
            get;
            set;
        }

        #endregion
    }
}
