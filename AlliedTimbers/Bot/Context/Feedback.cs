using System;
using AlliedTimbers.Models;
using Mugonat.Chat.BotEngine.Models;

namespace AlliedTimbers.Bot.Context;

public partial class AlliedTimbersBotContext
{
    public void FxSaveRating(BotMessageConfig config)
    {
        Session.Set("rating", Thread.CurrentMessage);
    }
    
    
    public void FxSaveFeedback(BotMessageConfig json)
    {
        var feedback = new Feedback
        {
            rating = Session.Get<int>("rating"),
            Opinion = Thread.CurrentMessage,
            CreatedAt = DateTime.Now
        };
        Database.Feedbacks.Add(feedback);
        Database.SaveChanges();
    }
}