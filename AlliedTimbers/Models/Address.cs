using System.ComponentModel;

namespace AlliedTimbers.Models;

public class Address
{
    public int Id { get; set; }

    [DisplayName("Location Name")] 
    public string Name { get; set; }

    [DisplayName("Street Address 1")] 
    public string Line1 { get; set; }

    [DisplayName("Street Address 2")] 
    public string Line2 { get; set; }

    public virtual CompanyBranch CompanyBranch { get; set; }
}