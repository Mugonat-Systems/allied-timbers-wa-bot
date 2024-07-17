using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AlliedTimbers.Models;

public class ProductInfo
{
    public int Id { get; set; }
    [MaxLength(450)] public string Title { get; set; }
    [MaxLength(750)] public string Description { get; set; }

    public bool IsChecked { get; set; } = true;
    //[DisplayName("Upload File")]
    //public string FilePath { get; set; }
    //public decimal Size { get; set; }
   // [DisplayName("File Type")] public string Type { get; set; }

    public int? ProductId  { get; set; }
    public Product Product { get; set; }
}