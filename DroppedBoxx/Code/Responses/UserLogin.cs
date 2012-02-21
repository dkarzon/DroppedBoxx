using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace DroppedBoxx.Code.Responses
{
    public class UserLogin
    {
        public string Error { get; set; }
        public string Token { get; set; }
        public string Secret { get; set; }
    }
}
