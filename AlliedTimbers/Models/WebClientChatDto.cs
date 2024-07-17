using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlliedTimbers.Models
{
    public class WebClientChatDto
    {
        public DateTime Timestamp { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Message { get; set; }
        public string MessageUrl { get; set; }
        public bool IsClosed { get; set; }
        public string MessageType { get; set; }
        public int Status { get; set; }
        public string Platform { get; set; }
    }
}