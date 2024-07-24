using System;

namespace AlliedTimbers.Models;

public class Educational
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Content { get; set; }
    public DateTime Date { get; set; } = DateTime.Now.Date;
}