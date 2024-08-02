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
    public BotMessageConfig FxGetNewsEvents(BotMessageConfig json)
    {
        var list = (ListMenuMessage)json.Message;

        list.Items = new List<ChatMessageModels.ListSection>
        {
            new()
            {
                Title = "News and Events",
                Options = new List<ChatMessageModels.ListOption>
                {
                    new("News Updates", "View news and updates", "viewNews"),
                    new("Events Invitations", "View upcoming events, trade shows, and exhibitions", "viewEvents"),
                    new("Educational Contents", "View educational content", "viewEducation"),
                }
            },
        };

        return json;
    }
}