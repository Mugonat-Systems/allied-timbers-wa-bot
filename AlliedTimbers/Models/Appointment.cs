using System;

namespace AlliedTimbers.Models;

public enum AppointmentStatus
{
    Pending, Confirmed, Cancelled
}
public class Appointment
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string Email { get; set; }
    
    public string Phone { get; set; }
    public string Date { get; set; }
    public string Time { get; set; }
    
    public string ServiceType { get; set; }
    
    public AppointmentStatus AppointmentStatus { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}


