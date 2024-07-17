using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlliedTimbers.Models
{
    public class QuickResponse : Timestamp
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string UserId { get; set; }
    }
}