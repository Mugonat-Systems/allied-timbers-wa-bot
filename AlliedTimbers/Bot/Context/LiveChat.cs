using System;
using System.Collections.Generic;
using AlliedTimbers.Models;
using Mugonat.Chat.BotEngine;
using Mugonat.Chat.BotEngine.Messages;
using Mugonat.Chat.BotEngine.Messages.Internal;
using Mugonat.Chat.BotEngine.Models;
using Mugonat.Chat.BotEngine.Plugins;

namespace AlliedTimbers.Bot.Context;

public partial class AlliedTimbersBotContext
{
    public void FxEnterLiveChat(BotMessageConfig config)
    {
        if (!RegexPlugin.Match("start", Thread.CurrentMessage)) return;

        Customer.IsLive = true;
        Customer.LastSeen = DateTime.Now;

        Session.Set("customerId", Customer.Id);

        Database.SaveChanges();
    }

    public string FxExitLiveChat(BotThread thread)
    {
        return Thread.AliasChatMessages["menu"].Step.ToString();

    }
    public void FxRateLiveChat(BotMessageConfig config)
    {
        if (!int.TryParse(Thread.CurrentMessage, out var rating)) return;

        Customer.LastRating = rating;
      
        Database.SaveChanges();
    }

    public BotMessageConfig FxGetRating(BotMessageConfig config)
    {
        var list = (ListMenuMessage)config.Message;

        list.Title = "Rate Chat Experience";
        list.Items = new List<ChatMessageModels.ListSection>
        {
            new()
            {
                Title = "Ratings",
                Options = new List<ChatMessageModels.ListOption>
                {
                    new("Very happy", "⭐⭐⭐⭐⭐ (5 Stars)", "5"),
                    new("Satisfied",  "⭐⭐⭐⭐ (4 Stars)", "4"),
                    new("Neutral",  "⭐⭐⭐ (3 Stars)", "3"),
                    new("Dissatisfied",  "⭐⭐ (2 Star)", "2"),
                    new("Very Disappointed", "⭐ (1 Star)", "1")
                }
               
            }
        };

        return config;
    }
}