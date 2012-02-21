using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace DroppedBoxx.Web
{
    /// <summary>
    /// Representation of an HTTP header
    /// </summary>
    public class HttpHeader
    {
        /// <summary>
        /// Name of the header
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Value of the header
        /// </summary>
        public string Value { get; set; }
    }
}
