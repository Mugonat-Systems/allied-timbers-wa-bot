using System;

namespace AlliedTimbers.Models;

public class Feedback
{
    public int Id { get; set; }
    public int rating { get; set; }
    public string Opinion { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}