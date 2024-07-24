using System;
using System.ComponentModel.DataAnnotations;

namespace AlliedTimbers.Models;

public class Payment
{
    public int Id { get; set; }
    
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; }
    public string Status { get; set; }
    
    public string Product { get; set; }
    public string Quantity { get; set; }
    [Display(Name = "Payment Id")]
    public string PaymentId { get; set; }
    [Display(Name = "payment Method")]
    public string PaymentMethod { get; set; }
    public decimal Amount { get; set; }
    [Display(Name = "Customer Id")]
    public int CustomerId { get; set; }
    
    [Display(Name = "Branch")]
    public string BranchName { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
}