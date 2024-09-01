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
    public BotMessageConfig FxShowNews(BotMessageConfig json)
    {
        var list = (ListMenuMessage)json.Message;
        var items = Database.News.OrderByDescending(r => r.Date)
            .ToList();

        if (items.Count == 0)
        {
            list.Title = "No news available";
            
            list.Items = new List<ChatMessageModels.ListSection>
            {
                new()
                {
                    Title= "No news available.",
                    

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
                     Title = "News",
                    Options = items.Select(g => new ChatMessageModels.ListOption
                        {
                            Title = g.Title.ToEllipsis(20),
                            PostbackText = $"news {g.Id}",
                            Description = $"{g.Date.ToString("MM/dd/yyyy")}\n"
                            //stringBuilder.Append(news.Date.ToString("MM/dd/yyyy") + "\n\n");
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
                Title = "News",
                Options = Pagination.GetPaged(items, 8)
                    .Select(g => new ChatMessageModels.ListOption
                    {
                        Title = g.Title.ToEllipsis(20),
                        PostbackText = $"news {g.Id}",
                        Description = $"{g.Date}\n"
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

    public BotMessageConfig FxGetNews( BotMessageConfig json)
    {
        int.TryParse(Thread.CurrentMessage.Replace("news", ""),
            out var id);

        var news = Database.News.Find(id);
        

        if(news == null)
        {
            var error = new TextMessage
            {
                Text = "News you are looking for may have been deleted\n" +
                "Please check again later"
            };
            json.Message = error;
        }
        else
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append($"Date: {news.Date.ToString("MM/dd/yyyy")}" + "\n\n");
            stringBuilder.Append($"Title: {news.Title}" + "\n\n");
            stringBuilder.Append($"Description: {news.Description}" + "\n\n");
            
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