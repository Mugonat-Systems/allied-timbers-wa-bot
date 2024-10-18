using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace AlliedTimbers.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string Image { get; set; }
        public string Requirements { get; set; }
        
        public decimal Price { get; set; }
        //public IEnumerable<int> FileIds { get; set; }
        public IEnumerable<int> InfoIds { get; set; }
        public bool IsLoan { get; set; }
        //[DisplayName("ImageVerificationRequired")]
        public bool IsImageRequired { get; set; }
        public bool IsMukando { get; set; }
        public bool IsSolar { get; set; }
        
        public bool IsTrusses { get; set; }
        public bool IsTimber { get; set; }
        public bool IsBoards { get; set; }
        public bool IsPoles { get; set; }
        public bool IsDoors { get; set; }
    }
}