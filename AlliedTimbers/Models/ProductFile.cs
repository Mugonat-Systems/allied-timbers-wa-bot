using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace AlliedTimbers.Models;

public class ProductFile 
{
    public int Id { get; set; }

    //[Required(ErrorMessage= "Please specify name of the file e.g Loan Application Form")]
    public string Name { get; set; }

    [DisplayName("Upload File")] public string Path { get; set; }

    public decimal Size { get; set; }

    public bool IsChecked { get; set; } = true;

    //[Required(ErrorMessage ="Please enter the file type e.g image/document")]
    [DisplayName("File Type")] public string Type { get; set; }

    public int? ProductId { get; set; }
    public Product Product { get; set; }

    //[NotMapped] public HttpPostedFileBase ImageFile { get; set; }
    
}