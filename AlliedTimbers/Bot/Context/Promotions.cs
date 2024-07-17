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
    public BotMessageConfig FxShowPromotions(BotMessageConfig json)
    {
        var list = (ListMenuMessage)json.Message;
        var items = Database.Promotions.OrderByDescending(r => r.EndDate)
            .ToList();

        if (items.Count == 0)
        {
            list.Title = "No promotions running";
            
            list.Items = new List<ChatMessageModels.ListSection>
            {
                new()
                {
                    Title= "No promotions running.",
                    

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
                     Title = "Promotions",
                    Options = items.Select(g => new ChatMessageModels.ListOption
                        {
                            Title = g.Name,
                            PostbackText = $"promotion {g.Id}",
                            Description = $"{g.StartDate:MM/dd/yyyy}" + " - " + $"{g.EndDate:MM/dd/yyyy}"
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
                    Title = "Promotions",
                    Options = Pagination.GetPaged(items, 8)
                        .Select(g => new ChatMessageModels.ListOption
                        {
                            Title = g.Name,
                            PostbackText = $"promotion {g.Id}",
                            Description = $"{g.StartDate:MM/dd/yyyy}" + " - " + $"{g.EndDate:MM/dd/yyyy}"
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

    //Fix Viewing of promotions
    //public Dictionary<string, string> FxGetPromotion(BotThread thread)
    //{
    //    int.TryParse(Thread.CurrentMessage.Replace("promotion", ""),
    //        out var id);
    //    var promotion =  Database.Promotions.Find(id);
    //    string title = "", caption = "";

    //    if ( promotion == default)
    //    {
    //        title = "Currently we dont have any running promotions";
    //        caption = "Please check again later";
    //    }

    //    else
    //    {
    //        title = $"{promotion.Name.ToEllipsis(25)}\n " +
    //            $"{promotion.Description.ToEllipsis(30)}\n";
    //        caption = $"{promotion.StartDate:M/d/yy}" + " - " +
    //                        $"{promotion.EndDate:M/d/yy}";
    //    }

    //    return new Dictionary<string, string>
    //        {
    //            {"title", title },
    //            {"caption", caption }
    //        };

    //}

    public BotMessageConfig FxGetPromotion( BotMessageConfig json)
    {
        int.TryParse(Thread.CurrentMessage.Replace("promotion", ""),
            out var id);

        var promotion = Database.Promotions.Find(id);
        

        if(promotion == null)
        {
            var error = new TextMessage
            {
                Text = "Promotion you are looking for may have been deleted\n" +
                "Please check again later"
            };
            json.Message = error;
        }
        else
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append(promotion.Name + "\n");
            stringBuilder.Append(promotion.Description + "\n");
            stringBuilder.Append($"{promotion.StartDate:M/d/yy}" + " - " +
                                    $"{promotion.EndDate:M/d/yy}\n\n");
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