using System;

namespace AlliedTimbers.Models
{
    public class Audit
    {
        public int Id  { get; set; }
        public string UserName { get; set; }
        public string AreaAccessed { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Action { get; set; }
    }
}