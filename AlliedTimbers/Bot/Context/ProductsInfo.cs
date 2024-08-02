using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading;
using AlliedTimbers.Models;
using Jil;
using Microsoft.Net.Http.Headers;
using Mugonat.Chat.BotEngine.Messages;
using Mugonat.Chat.BotEngine.Messages.Internal;
using Mugonat.Chat.BotEngine.Models;

namespace AlliedTimbers.Bot.Context;

public partial class AlliedTimbersBotContext
{
    public BotMessageConfig FxGetProductsInfo(BotMessageConfig json)
    {
        var list = (ListMenuMessage)json.Message;

        list.Items = new List<ChatMessageModels.ListSection>
        {
            new()
            {
                Title = "Products, Promotions and Recommendations",
                Options = new List<ChatMessageModels.ListOption>
                {
                    new("Product Categories", "Products in our catalog", "categories"),
                    new("Promotions", "View running promotions", "viewPromotions"),
                    new("Products Recommendations", "Products we highly recommend you to purchase.", "productRecommendations"),
                }
            },
        };

        return json;
    }
}