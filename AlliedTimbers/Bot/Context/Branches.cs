using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Mugonat.Chat.BotEngine.Messages;
using Mugonat.Chat.BotEngine.Models;
using Mugonat.Utils.Extensions;
using static Mugonat.Chat.BotEngine.Messages.Internal.ChatMessageModels;

namespace AlliedTimbers.Bot.Context;

public partial class AlliedTimbersBotContext
{
    public BotMessageConfig FxGetBranches(BotMessageConfig json)
    {
        var items = Database.CompanyBranches.OrderBy(x => x.Name).ToList();
        var list = (ListMenuMessage)json.Message;

        if (items.Count == 0)
        {
            list.Title = "No open branches.";
            list.Items = new List<ListSection>
            {
                new()
                {
                    Title = "No open branches.",
                    Options = new List<ListOption>
                    {
                        new("Menu", "View the main menu", "menu")
                    }
                }
            };
            return json;
        }

        if (items.Count <= 10)
        {
            list.Items = new List<ListSection>
            {
                new()
                {
                    Title = "Branches",
                    Options = items.Select(f => new ListOption
                    {
                        Title = f.Name,
                        PostbackText = $"branch {f.Id}",
                        Description = f.PhoneNumber
                    }).ToList(),
                },
            };
            return json;
        }

        if (!Pagination.Paging(Thread.CurrentMessage, 8)) Pagination.MoveToPage(0);

        list.Items = new List<ListSection>
        {
            new()
            {
                Title = "Branches: Page " + Pagination.CurrentHumanPage,
                Options = Pagination
                    .GetPaged(items, 8)
                    .Select(f => new ListOption
                    {
                        Title = f.Name.ToEllipsis(20),
                        PostbackText = $"branch {f.Id}",
                        Description = f.PhoneNumber.ToEllipsis(20)
                    }).ToList()
            },
        };

        var pagination = new List<ListOption>();

        if (Pagination.CurrentPage > 0)
        {
            pagination.Add(new("Previous page", "Show previous items", "previous page"));
        }

        if (items.Count > (Pagination.CurrentHumanPage * 8)) // 
        {
            pagination.Add(new ListOption("Next page", "Show more items", "next page"));
        }

        list.Items.Add(new ListSection("Navigation", pagination.ToArray()));


        return json;
    }

    //public Dictionary<string, string> FxGetBranch(BotThread thread)
    //{
    //    int.TryParse(Thread.CurrentMessage.Replace("branches", ""), out var id);
    //    var branch =  Database.CompanyBranches.Include(s => s.Address).FirstOrDefault(b => b.Id == id);
    //    string title = "", caption = "";

    //    if (branch == default)
    //    {
    //        title = "Branch closed";
    //        caption = "Do contact your nearest branch";


    //    }
    //    else
    //    {
    //        title = $"{branch.Name}\n {branch.Address.Line1}\n" +
    //                         $"\n{branch.Address.Line2}";
    //        caption = $"{branch.PhoneNumber}\n" +
    //                         $"{branch.Email}\n\n";
    //    }

    //    return new Dictionary<string, string>
    //        {
    //            {"title", title.ToEllipsis(55) },
    //            {"caption", caption }
    //        };
    //}

    public BotMessageConfig FxRetrieveBranch(BotMessageConfig json)
    {
        int.TryParse(Thread.CurrentMessage.Replace("branch ", ""), out var id);
        var branch = Database.CompanyBranches.Include(s => s.Address).FirstOrDefault(b => b.Id == id);

        if (branch == null)
        {
            var error = new TextMessage
            {
                Text = "Branch you are looking for may have been deleted\n" +
                       "Please check again later"
            };
            json.Message = error;
        }
        else
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append($"{branch.Name}\n {branch.Address.Line1}\n" +
                                 $"\n{branch.Address.Line2}\n");
            stringBuilder.Append($"{branch.PhoneNumber}\n" +
                                 $"{branch.Email}\n\n");
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