using System;

namespace AlliedTimbers.Models;

public abstract class Timestamp
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; }
}