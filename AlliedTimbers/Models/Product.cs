using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlliedTimbers.Models;

public class Product
{
    
    public int Id { get; set; }

    [MaxLength(500)]  public string Name { get; set; }

    [MaxLength(1000)]  public string Description { get; set; }
    
    public decimal Price { get; set; }

    public string Image { get; set; }

    public bool IsTrusses { get; set; }
    public bool IsTimber { get; set; }
    public bool IsBoards { get; set; }
    public bool IsPoles { get; set; }
    public bool IsDoors { get; set; }


    public virtual ICollection<ProductInfo> Information { get; set; } = new HashSet<ProductInfo>();

    [DisplayName("ApplicationForms ")]
    public virtual ICollection<ProductFile> Files { get; set; } = new HashSet<ProductFile>();


    [NotMapped]
    public int[] FileIds { get; set; }
    [NotMapped]
    public int[] InfoIds { get; set; }
}