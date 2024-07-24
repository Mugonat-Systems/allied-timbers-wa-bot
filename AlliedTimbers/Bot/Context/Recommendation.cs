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
    public BotMessageConfig FxDisplayRecommendations(BotMessageConfig json)
    {
        // Retrieve payments for the customer's PhoneNumber
        var payments = Database.Payments.Where(p => p.PhoneNumber == Customer.PhoneNumber).ToList();
        
        // Use a HashSet to track recommended product IDs to avoid duplicates
        var recommendedProduct = new HashSet<string>();
        var uniqueRecommendations = new List<Payment>();

        foreach (var payment in payments)
        {
            // Check if the product has already been recommended
            if (!recommendedProduct.Contains(payment.Product))
            {
                uniqueRecommendations.Add(payment);
                recommendedProduct.Add(payment.Product);
            }
        }
        // Call ListRecommendations with the unique recommendations
        return ListRecommendations(json, uniqueRecommendations);
        //return ListRecommendations(json, Database.Payments.Where( p => p.PhoneNumber == Customer.PhoneNumber).ToList());
    }
    
    public BotMessageConfig FxShowRecommendations(BotMessageConfig json)
    {
         int.TryParse(Thread.CurrentMessage.Replace("recommendations", ""),
            out var id);
        var recommendation = Database.Payments.Find(id);

        if ( recommendation == default)
        {
            var error = new TextMessage
            {
                Text = "Currently we dont have any product available for recommendations. You can check available products " +
                "Please check again later"
            };
           
            json.Message = error;
        }
        else
        {
            var display = new TextMessage
            {
                Text = $"{recommendation.Product}\n"
            };
            
            json.Message = display;
        }
        return json;
    }
    
    public BotMessageConfig FxRecommendationsSearchResult(BotMessageConfig json)
    {
        var recommendation = Database.Payments.Search(Thread.CurrentMessage).ToList();

        if (recommendation.Count == 0) return ListRecommendations(json, recommendation, $"{recommendation.Count} related recommendations found");

        json.Type = "text";
        json.Message = new TextMessage("Recommendations you are looking for cannot be found.");
        return json;
    }

    private BotMessageConfig ListRecommendations(BotMessageConfig json, List<Payment> recommendations, string text = null)
    {
        var list = (ListMenuMessage)json.Message;

        if (recommendations.Count == 0)
        {
            list.Body = text ?? "Can you kindly purchase some products to get recommendations";
            list.Items = new List<ChatMessageModels.ListSection>
            {
                new()
                {
                    Title = "Recommendations",
                    Options = new List<ChatMessageModels.ListOption>
                    {
                        new()
                        {
                            Title = "Return to Menu",
                            PostbackText = "menu",
                            //Description = "Purchase some products to get recommendations"
                        }
                    }
                }
            };
            return json;
        }
        
        if(recommendations.Count() <= 10)
        {
            list.Body = text ?? "Based on your previous purchases, we recommend products below for your next project";
            list.Items = new List<ChatMessageModels.ListSection>
            {
                new()
                {
                     Title = "Recommendations",
                Options = recommendations
                    .Select(l => new ChatMessageModels.ListOption
                    {
                        Title = l.Product.ToEllipsis(20),
                        PostbackText = $"allProductsAvailable",
                        //Description = l.Product.ToEllipsis(20)
                    }).ToList()
                }
            };
            return json;

        }
        
        if (!Pagination.Paging(Thread.CurrentMessage, 8)) Pagination.MoveToPage(0);

        list.Body = text ?? "Find related recommendations that other users have purchased, and get more insights quickly";
        list.Items = new List<ChatMessageModels.ListSection>
        {
            new()
            {
                Title = "Recommendations: Page " + Pagination.CurrentHumanPage,
                Options = Pagination
                    .GetPaged(recommendations, 8)
                    .Select(l => new ChatMessageModels.ListOption
                    {
                        Title = l.Product.ToEllipsis(20),
                        PostbackText = $"recommendations {l.Id}",
                        Description = l.Product.ToEllipsis(20)
                    }).ToList()
            },
        };

        var pagination = new List<ChatMessageModels.ListOption>();

        if (Pagination.CurrentPage > 0)
        {
            pagination.Add(new("Previous page", "Show previous items", "previous page"));
        }

        if (recommendations.Count() > (Pagination.CurrentHumanPage * 8)) // 
        {
            pagination.Add(new("Next page", "Show more items", "next page"));
        }

        list.Items.Add(new ChatMessageModels.ListSection("Navigation", pagination.ToArray()));

        return json;
    }

    public string FxGotoRecommendationsAnswer(BotThread thread)
    {
            return Thread.AliasChatMessages["RecommendationsAnswer"].Step.ToString();
    }
}