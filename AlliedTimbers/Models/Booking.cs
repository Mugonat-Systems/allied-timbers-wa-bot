using System;

namespace AlliedTimbers.Models;

public enum BookingStatus
{
    Pending, Confirmed, Cancelled
}
public class Booking
{
    public int Id { get; set; }

    public string Apartment { get; set; }

    public string CheckInDate { get; set; }

    public string CheckOutDate { get; set; }

    public string Guests { get; set; }

    public string Email { get; set; }

    public string Phone { get; set; }

    public string BookingContact { get; set; }
    

    public BookingStatus BookingStatus{ get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
