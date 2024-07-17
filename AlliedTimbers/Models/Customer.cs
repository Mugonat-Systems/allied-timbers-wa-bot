using Microsoft.Owin.Security;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlliedTimbers.Models;

public class Customer
{
    public int Id { get; set; }

    [MaxLength(250)] public string Name { get; set; }

    [Index(IsUnique = true)]
    [MaxLength(50)]
    public string PhoneNumber { get; set; }

    public bool IsLive { get; set; }
    public DateTime LastSeen { get; set; } =   DateTime.UtcNow;
    public int LastRating { get; set; }

    [MaxLength(500)] public string LastMessage { get; set; }

    public string CustomerNumber { get; set; }
    public string NationalId { get; set; }
    public string BankingPhone { get; set; }
    public string Type { get; set; }
    public bool? Status { get; set; }
    public int? Language { get; set; }
    public bool? Approved { get; set; }
    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedOn { get; set; }
    public DateTime? AppliedOn { get; set; }
    public DateTime? LastAvailableOn { get; set; }
    public int? NewMessagesCount { get; set; }
    public DateTime? LastRepliedOn { get; set; }
    public string LastRepliedBy { get; set; }
    public bool? IsClosed { get; set; }
    public string CurrentIssue { get; set; }
    public int IsRegistered { get; set; }

    public int? ChancesLeft { get; set; }
  

    public bool IsInChatMode()
    {
        return IsClosed.HasValue && !IsClosed.Value;
    }
}