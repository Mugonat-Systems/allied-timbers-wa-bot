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
    public BotMessageConfig FxGetFeedbackConsultations(BotMessageConfig json)
    {
            var list = (ListMenuMessage)json.Message;

            list.Items = new List<ChatMessageModels.ListSection>
            {
                new()
                {
                    Title = "Feedback and Consultations",
                    Options = new List<ChatMessageModels.ListOption>
                    {
                        new("Feedback", "Give us feedback on how you find our services", "rating"),
                        new("Consultations", "Request a consultation", "consultationBooking"),
                        new("Visit Appointment", "Book an appointment", "appointmentBooking")
                    }
                },
            };

            return json;
    }
}