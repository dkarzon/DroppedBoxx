using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace DroppedBoxx.Code.Exceptions
{
    class DropException : Exception
    {
        public DropException() { }
        public DropException(string message) : base(message) { }
    }
}
