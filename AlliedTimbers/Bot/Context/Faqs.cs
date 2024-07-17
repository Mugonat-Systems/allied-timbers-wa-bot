using System.Collections.Generic;
using System.Linq;
using AlliedTimbers.Models;
using Mugonat.Chat.BotEngine;
using Mugonat.Chat.BotEngine.Messages;
using Mugonat.Chat.BotEngine.Messages.Internal;
using Mugonat.Chat.BotEngine.Models;
using Mugonat.Utils.Extensions;

namespace AlliedTimbers.Bot.Context;

public partial class AlliedTimbersBotContext
{
    public BotMessageConfig FxDisplayFaqs(BotMessageConfig json)
    {
        return ListFaqs(json, Database.Faqs.ToList());
    }
    
    public BotMessageConfig FxShowFaq(BotMessageConfig json)
    {
         int.TryParse(Thread.CurrentMessage.Replace("faq", ""),
            out var id);
        var faq = Database.Faqs.Find(id);

        if ( faq == default)
        {
            var error = new TextMessage
            {
                Text = "Currently we dont have any FAQs available to read. " +
                "Please check again later"
            };
           
            json.Message = error;
        }
        else
        {
            var display = new TextMessage
            {
                Text = $"{faq.Question}\n" + $"\n{faq.Answer}"
            };
            
            json.Message = display;
        }
        return json;
    }
    
    public BotMessageConfig FxFaqSearchResult(BotMessageConfig json)
    {
        var faqs = Database.Faqs.Search(Thread.CurrentMessage).ToList();

        if (faqs.Count == 0) return ListFaqs(json, faqs, $"{faqs.Count} related question and answers found");

        json.Type = "text";
        json.Message = new TextMessage("FAQ you are looking for cannot be found\n" +
                                       "Do contact your nearest branch for any information you might require");
        return json;
    }

    private BotMessageConfig ListFaqs(BotMessageConfig json, IEnumerable<Faq> faqs, string text = null)
    {
        var list = (ListMenuMessage)json.Message;
        
        if(faqs.Count() <= 10)
        {
            list.Body = text ?? "Find related questions that other users have asked, and get answers quickly";
            list.Items = new List<ChatMessageModels.ListSection>
            {
                new()
                {
                     Title = "FAQs",
                Options = faqs
                    .Select(l => new ChatMessageModels.ListOption
                    {
                        Title = l.Question.ToEllipsis(20),
                        PostbackText = $"faq {l.Id}",
                        Description = l.Answer.ToEllipsis(20)
                    }).ToList()
                }
            };
            return json;

        }
        
        if (!Pagination.Paging(Thread.CurrentMessage, 8)) Pagination.MoveToPage(0);

        list.Body = text ?? "Find related questions that other users have asked, and get answers quickly";
        list.Items = new List<ChatMessageModels.ListSection>
        {
            new()
            {
                Title = "FAQs: Page " + Pagination.CurrentHumanPage,
                Options = Pagination
                    .GetPaged(faqs, 8)
                    .Select(l => new ChatMessageModels.ListOption
                    {
                        Title = l.Question.ToEllipsis(20),
                        PostbackText = $"faq {l.Id}",
                        Description = l.Answer.ToEllipsis(20)
                    }).ToList()
            },
        };

        var pagination = new List<ChatMessageModels.ListOption>();

        if (Pagination.CurrentPage > 0)
        {
            pagination.Add(new("Previous page", "Show previous items", "previous page"));
        }

        if (faqs.Count() > (Pagination.CurrentHumanPage * 8)) // 
        {
            pagination.Add(new("Next page", "Show more items", "next page"));
        }

        list.Items.Add(new ChatMessageModels.ListSection("Navigation", pagination.ToArray()));

        return json;
    }

    public string FxGotoAnswer(BotThread thread)
    {
            return Thread.AliasChatMessages["answer"].Step.ToString();
    }
}