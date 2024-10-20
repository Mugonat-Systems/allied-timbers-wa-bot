﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mugonat.Chat.BotEngine;
using Mugonat.Chat.BotEngine.Messages;
using Mugonat.Chat.BotEngine.Messages.Internal;
using Mugonat.Chat.BotEngine.Models;
using Mugonat.Utils.Extensions;

namespace AlliedTimbers.Bot.Context;

public partial class AlliedTimbersBotContext
{
    public BotMessageConfig FxShowEvents(BotMessageConfig json)
    {
        var list = (ListMenuMessage)json.Message;
        var items = Database.Events.OrderByDescending(r => r.EndDate)
            .ToList();

        if (items.Count == 0)
        {
            list.Title = "No upcoming events";
            
            list.Items = new List<ChatMessageModels.ListSection>
            {
                new()
                {
                    Title= "No upcoming events.",
                    

                    Options = new List<ChatMessageModels.ListOption>
                    {
                        new ("Menu", "View the main menu", "menu")
                    }
                }
            };

            return json;
        }

        if(items.Count <  9)
        {
            list.Items = new List<ChatMessageModels.ListSection>
            {
                new()
                {
                     Title = "Events",
                    Options = items.Select(g => new ChatMessageModels.ListOption
                        {
                            Title = g.Name.ToEllipsis(21),
                            PostbackText = $"event {g.Id}",
                            Description = g.Description.ToEllipsis(21)
                        }).ToList()
                }
            };
            return json;
        }
       
        if (!Pagination.Paging(Thread.CurrentMessage, 8)) Pagination.MoveToPage(0);

        list.Items = new List<ChatMessageModels.ListSection>
        {
            new()
            {
                Title = "Events",
                Options = Pagination.GetPaged(items, 8)
                    .Select(g => new ChatMessageModels.ListOption
                    {
                        Title = g.Name.ToEllipsis(21),
                        PostbackText = $"event {g.Id}",
                        Description = g.Description.ToEllipsis(21)
                    }).ToList()
            },
            new()
            {
                Title = "Navigation",
                Options = new List<ChatMessageModels.ListOption>
                {
                    new("Next page", "Show more items", "next page"),
                    new("Previous page", "Show previous items", "previous page")
                }
            }
        };
        

        return json;
    }

    public BotMessageConfig FxGetEvent( BotMessageConfig json)
    {
        int.TryParse(Thread.CurrentMessage.Replace("event", ""),
            out var id);

        var events = Database.Events.Find(id);
        

        if(events == null)
        {
            var error = new TextMessage
            {
                Text = "Event you are looking for may have been deleted\n" +
                "Please check again later"
            };
            json.Message = error;
        }
        else
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append($"Title: {events.Name}" + "\n\n");
            stringBuilder.Append($"Description: {events.Description}" + "\n\n");
            stringBuilder.Append($"Venue: {events.Venue}" + "\n\n");
            stringBuilder.Append($"Date: {events.StartDate:M/d/yy}" + " - " +
                                    $"{events.EndDate:M/d/yy}\n\n");
            stringBuilder.Append($"Time: {events.Time}" + "\n\n");
            stringBuilder.Append("Type hi to go back to the main menu");

            var display = new TextMessage
            {
                Text = stringBuilder.ToString(),
            };
            json.Message = display;

        }

        return json;
    }
}