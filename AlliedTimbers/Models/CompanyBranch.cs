using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlliedTimbers.Models;

public class CompanyBranch
{
    
    [Key] [ForeignKey("Address")] 
    public int Id { get; set; }

    public string Name { get; set; }

    [EmailAddress] 
    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public virtual Address Address { get; set; }
}