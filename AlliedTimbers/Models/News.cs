using System;

namespace AlliedTimbers.Models;

public class News
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; } = DateTime.Now.Date;
    
}