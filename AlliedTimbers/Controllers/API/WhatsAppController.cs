using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AlliedTimbers.Bot;
using AlliedTimbers.Models;
using Mugonat.Bots.WhatsappCloudBot;
using Mugonat.Chat.BotEngine;
using Mugonat.Chat.BotEngine.Messages;
using Mugonat.Chat.BotEngine.Services;
using PagedList;
using static Mugonat.Chat.BotEngine.Messages.Internal.ChatMessageModels;

namespace AlliedTimbers.Controllers.API
{
    public class WhatsAppController : ApiController
    {
        private readonly ApplicationDbContext _db = new();

        //TODO increment when user sends to +1
        #region UsersSnapshot

        [HttpGet, Route("api/whatsapp-web/snapshot")]
        [ResponseType(typeof(IEnumerable<Customer>))]
        public IHttpActionResult GetUsersSnapshot()
        {
            var chatbotUsers = _db.Customers
                .Where(a => DbFunctions.DiffHours(a.LastSeen, DateTime.Now) <= 24 && !string.IsNullOrEmpty(a.LastMessage))
                .OrderByDescending(a => a.LastSeen)
                .Select(u => new
                {
                    u.Name,
                    phone = u.PhoneNumber,
                    platform = "whatsapp",
                    u.Language,
                    u.CurrentIssue,
                    u.NewMessagesCount,
                    last_available_on = u.LastAvailableOn,
                    u.LastRepliedOn,
                    u.LastRepliedBy,
                    u.LastMessage

                });

            return Ok(chatbotUsers.ToList());
        }

        #endregion

        #region UserActiveChats

        [HttpGet, Route("api/whatsapp-web/{phone}")]
        [ResponseType(typeof(IEnumerable<Message>))]
        public IHttpActionResult GetUserActiveChats(string phone, bool live = true)
        {
            var active = new List<Message>();
            var reply = GetUserSystemThreadActive(phone)
                .Where(a => !live || a.IsInLiveChat == true);

            foreach (var item in reply)
            {
                if (item.MessageText?.Contains("Exited live chat") ?? false)
                    active.Clear();

                active.Add(item);
            }

            return Ok(active.ToList());
        }

        #endregion

        #region UserDateChats

        [HttpGet, Route("api/whatsapp-web/history/{phone}")]
        [ResponseType(typeof(IPagedList<Message>))]
        public IHttpActionResult GetHistory(string phone, string date, int? page = 1)
        {
            return Ok(
                GetUserSystemThread(phone)
                    .Where(a => a.SentTimeStamp.ToShortDateString() == DateTime.Parse(date).ToShortDateString())
                    .OrderBy(a => a.SentTimeStamp)
                    .ToPagedList(page ?? 1, 50)
            );
        }

        [HttpGet, Route("api/whatsapp-web/dates/{phone}")]
        [ResponseType(typeof(IEnumerable<string>))]
        public IHttpActionResult GetDates(string phone)
        {
            return Ok(
                GetUserSystemThread(phone)
                    .Select(a => a.SentTimeStamp.ToShortDateString())
                    .Distinct()
            );
        }

        #endregion

        #region AgentReplies

        [HttpPost, Route("api/whatsapp-web/send")]
        public async Task<IHttpActionResult> Send([FromBody] WebClientChatDto send)
        {
            if (send.IsClosed)
            {
                var buttonGroup = new ButtonOptionsMessage(
                    "Chat resolved", "Would you like to rate the chat?",
                    new ButtonOption("Rate"),
                    new ButtonOption("Menu")
                );

                await SendMessage(send.To, buttonGroup);
            }
            else
            {
                await SendMessage(send.Platform, send.To, send.Message);
            }

            await HandleAgentMessages(send);


            return GetUserActiveChats(send.To);
        }

        private async Task HandleAgentMessages(WebClientChatDto send)
        {
            var user = _db.Customers.FirstOrDefault(a => a.PhoneNumber == send.To);

            // Update user for chat snapshot
            if (user != null)
            {
                user.LastMessage = "Chat closed by " + User.Identity.Name;
                user.LastRepliedBy = User.Identity.Name;
                user.LastRepliedOn = DateTime.Now;
                user.NewMessagesCount = 0;
                user.IsLive = !send.IsClosed;

                await _db.SaveChangesAsync();
            }

            // Save agent message
            _db.Messages.Add(new Message
            {
                IntegrationId = BotConfig.SourceNumber,
                IntegrationName = BotConfig.AppName,
                FromPhone = User.Identity.Name,
                FromName = User.Identity.Name,
                ToPhone = send.To,
                ToName = user?.Name,
                SentTimeStamp = DateTime.Now,
                MessageText = send.IsClosed ? $"Closed by {User.Identity.Name}" : send.Message,
                MessageMediaUrl = send.MessageUrl,
                MessageType = send.MessageType,
                IsInLiveChat = true,
                CurrentStep = send.IsClosed ? 0 : -1,
            });

            // Save user chat as closed
            if (send.IsClosed)
            {
                _db.Messages.Add(new Message
                {
                    IntegrationId = BotConfig.SourceNumber,
                    IntegrationName = BotConfig.AppName,
                    ToPhone = BotConfig.SourceNumber,
                    ToName = BotConfig.AppName,
                    FromPhone = send.To,
                    FromName = user?.Name,
                    SentTimeStamp = DateTime.Now,
                    MessageText = "End of chat ....",
                    MessageType = "text",
                    CurrentStep = -1,
                    IsInLiveChat = true
                });
            }

            await _db.SaveChangesAsync();
        }

        #endregion
        private IEnumerable<Message> GetUserSystemThreadActive(string phone)
        {
            return _db.Messages
                .Where(m => m.FromPhone == phone || m.ToPhone == phone)
                .Where(m => m.IsInLiveChat == true)
                .Where(m => DbFunctions.DiffHours(m.SentTimeStamp, DateTime.Now) <= 24)
                .ToList();
        }

        private IEnumerable<Message> GetUserSystemThread(string phone)
        {
            return _db.Messages.Where(m => m.FromPhone == phone || m.ToPhone == phone).ToList();
        }

        private static async Task SendMessage(string platform, string number, string text)
        {
            var WAservice = new WhatsappCloudChatService(
                BotConfig.WhatsappCloudPhoneNumberId,
                BotConfig.WhatsappCloudAccessToken,
                new AlliedTimbersBot()
            );

            await WAservice.Send(number, new TextMessage(text));

        }

        private static async Task SendMessage(string number, ButtonOptionsMessage buttonOptions)
        {
            var WAservice = new WhatsappCloudChatService(BotConfig.WhatsappCloudPhoneNumberId, BotConfig.WhatsappCloudAccessToken, BotDependency.Get<BotEngine>());
            await WAservice.Send(number, buttonOptions);
        }
    }
}