using System;

namespace AlliedTimbers.Models;

public class Message
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public string FromPhone { get; set; }

    public string FromName { get; set; }

    public string Platform { get; set; }

    public string MessageText { get; set; }

    public DateTime SentTimeStamp { get; set; } = DateTime.Now;

    public int Replies { get; set; }
    public int CurrentStep { get; set; }
    public string IntegrationId { get; set; }
    public string IntegrationName { get; set; }
    public string ToPhone { get; set; }
    public string ToName { get; set; }
    public string MessageMediaUrl { get; set; }
    public string MessageType { get; set; }
    public string MessageId { get; set; }
    public bool IsInLiveChat { get; set; } = false;
}