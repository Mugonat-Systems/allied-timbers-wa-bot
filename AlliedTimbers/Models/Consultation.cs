using System;

namespace AlliedTimbers.Models;

public enum ConsultationStatus
{
    Pending, Confirmed, Cancelled
}
public class Consultation
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Date { get; set; }
    public string Time { get; set; }
    public string ServiceType { get; set; }
    
    public ConsultationStatus ConsultationStatus  { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
