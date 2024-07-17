using System;
using System.Collections.Generic;
using System.Linq;

namespace AlliedTimbers.Models;

public class ChatHistoryReport : Timestamp
{
    public ChatHistoryReport(ApplicationDbContext db, DateTime startDate, DateTime endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
        var identityDb = ApplicationDbContext.Create();
        var dateBetween = new Func<DateTime?, bool>(d =>
            startDate <= d.GetValueOrDefault() && endDate >= d.GetValueOrDefault());
        var dateBefore = new Func<DateTime?, bool>(d => startDate > d.GetValueOrDefault());
        var users = identityDb.Users.ToList();
        var messages = db.Messages.Where(a => !string.IsNullOrEmpty(a.MessageText))
            .ToList().Where(m => dateBetween(m.SentTimeStamp)).ToList();
        var customers = db.Customers.ToList();
        var maxDateBack = DateTime.Now.Subtract(new TimeSpan(2, 0, 0, 0));

        Totals = new Dictionary<string, string>
        {
            ["Total clients"] = customers.Count(a => dateBetween(a.LastAvailableOn)).ToString(),
            ["Total chat messages"] = messages.Count.ToString(),
            ["Total messages by clients"] = messages.Count(m => double.TryParse(m.FromPhone, out _)).ToString(),
            ["Total messages by agents"] = messages.Count(m => m.FromPhone?.Contains("@") ?? false).ToString(),
            ["Recently missed clients"] = customers.Count(a =>
                    a.LastRepliedOn > maxDateBack && dateBefore(a.LastRepliedOn) && a.IsClosed != true)
                .ToString(),
            // ["Total missed clients"] = customers.Count(a => dateBefore(a.LastRepliedOn) && a.IsClosed != true)
            //     .ToString(),
            ["Total applications"] = db.LoanApplications.Count().ToString(),
            ["Total blocked"] = customers.Count(y => y.ChancesLeft < 0).ToString()
        };

        Ratings = new Dictionary<string, string>
        {
            ["Ratings : Very happy"] =
                customers.Count(x => x.LastRating == 5).ToString(),

            ["Ratings : Satisfied"] =
                customers.Count(x => x.LastRating == 4).ToString(),

            ["Ratings : Neutral"] = customers.Count(x => x.LastRating == 3).ToString(),

            ["Ratings : Dissatisfied"] = customers.Count(x => x.LastRating == 2).ToString(),

            ["Ratings : Very Disappointed"] = customers.Count(x => x.LastRating == 1).ToString(),
        };

        UserChatsClosed = users.ToDictionary(u => $"Number of chats closed : {u.Email}",
            u => messages
                .Count(a => a.MessageText == "Closed by " + u.Email)
                .ToString()
        );

        UserActivity = new Dictionary<string, string>
        {
            ["ACTIVE"] = "Interacted with user during the period: " + users
                .Select(item =>
                    messages
                        .Where(a => a.FromPhone == item.Email)
                        .OrderByDescending(a => a.SentTimeStamp).FirstOrDefault())
                .Where(val => val != null)
                .Count(val => dateBetween(val.SentTimeStamp))
        };

        foreach (var item in users)
        {
            var val = messages.Where(a => a.FromPhone == item.Email)
                .OrderByDescending(a => a.SentTimeStamp).FirstOrDefault();
            if (val == null) continue;

            UserActivity.Add(item.Email, $"Last active : on {val.SentTimeStamp:dddd, dd MMMM yyyy HH:mm tt}");
        }

        FrequentChats = messages
            .Where(u => u.MessageText == "End of chat ....")
            .Where(u => !string.IsNullOrEmpty(u.ToPhone))
            .GroupBy(m => m.ToPhone).Take(10).ToDictionary(m => m.Key, m => m.Count().ToString());
    }

    public DateTime StartDate { get; }
    public DateTime EndDate { get; }
    public Dictionary<string, string> Totals { get; }
    public Dictionary<string, string> Ratings { get; }
    public Dictionary<string, string> UserChatsClosed { get; }
    public Dictionary<string, string> FrequentChats { get; }
    public Dictionary<string, string> UserActivity { get; }
}