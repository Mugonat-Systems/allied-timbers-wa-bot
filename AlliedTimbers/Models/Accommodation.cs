using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AlliedTimbers.Models;

    public class Accommodation
    {
       public int Id { get; set; }

       [Required]
       public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public decimal Price { get; set; }

    public string Image1 { get; set; }
    public string Image2 { get; set; }

    [Required]
    public AccommodationStatus Status { get; set; }
    }

public enum AccommodationStatus
{
    Available, Booked, UnderMaintenance
}
