namespace AlliedTimbers.Models;

public class Faq : Timestamp
{
    public string Question { get; set; }
    public string Answer { get; set; }
    public string Tags { get; set; }
}