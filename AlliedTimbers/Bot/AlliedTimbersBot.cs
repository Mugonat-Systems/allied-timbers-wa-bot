using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AlliedTimbers.Bot.Context;
using Mugonat.Bots.Core;
using Mugonat.Bots.Core.Models;
using Mugonat.Bots.WhatsappCloudBot;
using Mugonat.Bots.WhatsappCloudBot.Models;
using Mugonat.Chat.BotEngine;
using Mugonat.Chat.BotEngine.Messages;
using Mugonat.Chat.BotEngine.Plugins;
using Mugonat.Chat.BotEngine.Services;
using Mugonat.Chat.BotJsonEngine;
using Mugonat.Chat.BotLinguisticsEngine.Extensions;
using Mugonat.Utils.Files;
using Mugonat.Utils.Reflection;
using Newtonsoft.Json;
using RestSharp;


namespace AlliedTimbers.Bot;

public partial class AlliedTimbersBot : BotJsonEngine
{
    public AlliedTimbersBot() : base(AppSettings["app.bot.name"])
    {
        ConfigureDevBot();
    }

    protected override BotExecutionContext GetContext()
    {
        return new AlliedTimbersBotContext();
    }

    protected override string GetAppRoot()
    {
        return HttpContext.Current.Server.MapPath("~/");
    }

    protected override string GetMessagesDirectory()
    {
        return Path.Combine(GetAppRoot(), "Bot", "Json");
    }

    protected override string GetLinguisticsDirectory()
    {
        return Path.Combine(GetAppRoot(), "Bot", "Linguistics");
    }

    private async Task<ICollection<BotMessage>> Talk(string threadId, string message)
    {
        if (string.IsNullOrEmpty(message)) return Array.Empty<BotMessage>();

        var thread = Thread(threadId, "default");
        var context = thread.ExecutionContext as AlliedTimbersBotContext ?? throw new Exception("Invalid context");

        // Bail territory

        if (context.Customer.IsLive && RegexPlugin.Match("exit", message))
        {
            context.Customer.LastSeen = DateTime.UtcNow;
            context.Customer.LastAvailableOn = DateTime.UtcNow;
            context.Customer.LastMessage = context.Customer.IsLive ? "Exited live chat" : "Exited bot chat";
            context.Customer.IsLive = false;
            context.Customer.NewMessagesCount = 0;
            context.Database.Messages.Add(new Models.Message
            {
                FromPhone = threadId,
                FromName = context.Customer.Name,
                Platform = thread.Platform,
                SentTimeStamp = DateTime.Now,
                MessageText = "Exited live chat",
                MessageType = "text", //TODO: GET REAL VALUE
                CreatedAt = DateTime.UtcNow,
                IsInLiveChat = false
            });

            await context.Database.SaveChangesAsync();

            context.Session.SetValue("__progress-tracking-key__", 0L);
        }

        // Agent territory

       if (context.Customer.IsLive)
        {
            context.Customer.LastMessage = message;
            context.Customer.LastSeen = DateTime.UtcNow;
            context.Customer.LastAvailableOn = DateTime.UtcNow;
            context.Customer.NewMessagesCount = context.Customer.NewMessagesCount.HasValue
                ? context.Customer.NewMessagesCount + 1 : 1;

            context.Database.Messages.Add(new Models.Message
            {
                FromPhone = threadId,
                FromName = context.Customer.Name,
                Platform = thread.Platform,
                SentTimeStamp = DateTime.UtcNow,
                MessageText = message,
                MessageType = "text", //TODO: GET REAL VALUE
                CreatedAt = DateTime.UtcNow,
                IsInLiveChat = true
            });

            await context.Database.SaveChangesAsync();

            return Array.Empty<BotMessage>();
        }

        // Bot territory
        var replies = await thread.Reply(message);
        
        if (!replies.Any())
            replies.Add(new TextMessage("Sorry, we could not understand your message, please try again"));

        return replies;
    }

    private async Task<ICollection<BotMessage>> Talk(ActualPayload payload)
    {
        return await Talk(payload.GetThreadId(), payload.GetMessage());
    }

   
    private async Task ProcessMessage(ChatService api, CloudPayload payload)
    {
        foreach (var change in payload.Entry.SelectMany(s => s.Changes))
        {
            if (change.Value.Messages == null) continue;
            var threadId = payload.GetThreadId();
            foreach (var message in change.Value.Messages)
            {
                var session = new BotSession(message.From);
                var profileName = change.Value.Contacts?
                    .FirstOrDefault(w => w.WaId == message.From)?
                    .Profile.Name;
                session.Set("customerName", profileName);

                var messages = await Talk(threadId,
                    await GetMessageString(message, api as WhatsappCloudChatService)
                    );

                //var messages = await this.Linguify(replies);

                foreach (var replyMessage in messages)
                {
                    await api.Send(threadId, replyMessage);

                    if (messages.Count > 1) System.Threading.Thread.Sleep(1000);
                }
            }

        }
    }

    private async Task<string> GetMessageString(Message message,
            WhatsappCloudChatService api)
    {
        switch (message.Type)
        {
            case "text":
                return message.Text.Body;
            case "image":
            case "document":
            case "video":
            case "audio":
            case "sticker":
                return await GetMediaId(message, api);
            case "button":
                return message.Button.Text;
        }

        if (message.Type != "interactive") return default;

        string result;
        switch (message.Interactive.Type)
        {
            case "list_reply":
                result = message.Interactive.ListReply.Id ??
                         message.Interactive.ListReply.Description ?? message.Interactive.ListReply.Title;
                break;
            case "button_reply":
                result = message.Interactive.ButtonReply.Id ?? message.Interactive.ButtonReply.Title;
                break;
            default:
                result = default;
                break;
        }
        return result;
    }

    private async Task<string> GetMediaId(
        Message message,
        WhatsappCloudChatService api
    )
    {
        string result;
        switch (message.Type)
        {
            case "image":
                result = await GetMediaUrl(message.Image.Id, api);
                break;
            case "document":
                result = await GetMediaUrl(message.Document.Id, api);
                break;
            case "video":
                result = await GetMediaUrl(message.Video.Id, api);
                break;
            case "audio":
                result = await GetMediaUrl(message.Audio.Id, api);
                break;
            case "sticker":
                result = await GetMediaUrl(message.Sticker.Id, api);
                break;
            default:
                result = default;
                break;
        }
        return result;
    }

    private async Task<string> GetMediaUrl(
        string mediaId,
        WhatsappCloudChatService api
    )
    {
        if (string.IsNullOrEmpty(mediaId)) return default;

        try
        {
            var media = await GetMedia(mediaId, api.AccessToken);

            //string baseUrl = "";
            string url = "";

            url = media != null
            ? FileHandler.Download(media.Url, headers: new List<KeyValuePair<string, string>>
            {
                    new KeyValuePair<string, string>("Authorization", $"Bearer {api.AccessToken}")
            })
            : null;

            /*var request = HttpContext.Current.Request;
            baseUrl = request.Url.GetLeftPart(UriPartial.Authority);*/

            return string.IsNullOrEmpty(url) ? default : $"{url}";

        }
        catch (Exception e)
        {
            MvcApplication.Logger.Exception(e);
        }

        return default;

    }

    public async Task<CloudMedia> GetMedia(string mediaId, string accessToken)
    {
        RestClient restClient = new RestClient("https://graph.facebook.com/v15.0/" + mediaId);
        RestRequest restRequest = new RestRequest("", Method.Get);
        restRequest.AddHeader("authorization", "Bearer " + accessToken);
        
        RestRequest request = restRequest;
        var restResponse = await restClient.ExecuteAsync(request);

        MvcApplication.Logger.Log(restResponse.Content);

        return restResponse.IsSuccessful ? JsonConvert.DeserializeObject<CloudMedia>(restResponse.Content) : null;
    }

}