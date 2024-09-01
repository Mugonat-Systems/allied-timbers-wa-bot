using System;
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
    public BotMessageConfig FxShowEducational(BotMessageConfig json)
    {
        var list = (ListMenuMessage)json.Message;
        var items = Database.Educationals.OrderByDescending(r => r.Date)
            .ToList();

        if (items.Count == 0)
        {
            list.Title = "No educational content";
            
            list.Items = new List<ChatMessageModels.ListSection>
            {
                new()
                {
                    Title= "No educational content",
                    

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
                     Title = "Educational content",
                    Options = items.Select(g => new ChatMessageModels.ListOption
                        {
                            Title = g.Title.ToEllipsis(20),
                            PostbackText = $"educational {g.Id}",
                            Description = $"{g.Description.ToEllipsis(30)}\n"
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
                Title = "Educational content",
                Options = Pagination.GetPaged(items, 8)
                    .Select(g => new ChatMessageModels.ListOption
                    {
                        Title = g.Title.ToEllipsis(20),
                        PostbackText = $"event {g.Id}",
                        Description = $"{g.Description.ToEllipsis(55)}\n"
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

    public BotMessageConfig FxGetEducational( BotMessageConfig json)
    {
        int.TryParse(Thread.CurrentMessage.Replace("educational", ""),
            out var id);

        var educational = Database.Educationals.Find(id);
        

        if(educational == null)
        {
            var error = new TextMessage
            {
                Text = "Educational content you are looking for may have been deleted\n" +
                "Please check again later"
            };
            json.Message = error;
        }
        else
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append($"Title: {educational.Title}" + "\n\n");
            stringBuilder.Append($"Description: {educational.Description}"+ "\n\n");
            stringBuilder.Append($"Content: {educational.Content.ToEllipsis(1021)}" + "\n\n");
            stringBuilder.Append($"Date: {educational.Date:M/d/yy} \n\n");
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