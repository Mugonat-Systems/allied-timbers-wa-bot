using System;

namespace AlliedTimbers.Models;

public class Event
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Venue { get; set; }
    public DateTime StartDate { get; set; } = DateTime.Now.Date;
    
    public string Time { get; set; } 
    public DateTime EndDate { get; set; }
}