using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AlliedTimbers.Helpers;
using AlliedTimbers.Models;
using Mugonat.Chat.BotEngine;
using Mugonat.Chat.BotEngine.Attributes;
using Mugonat.Chat.BotEngine.Messages;
using Mugonat.Chat.BotEngine.Messages.Internal;
using Mugonat.Chat.BotEngine.Models;
using Mugonat.Utils.Extensions;
using Webdev.Payments;
using Payment = AlliedTimbers.Models.Payment;


namespace AlliedTimbers.Bot.Context;

public partial class AlliedTimbersBotContext
{
    public BotMessageConfig FxGetAccommodations(BotMessageConfig config)
    {
        var list = (ListMenuMessage)config.Message;

        var items = AccommodationType();

        if (items.Count == 0)
        {
            list.Title = "No accommodations";
            list.Items = new List<ChatMessageModels.ListSection>
            {
                new()
                {
                    Title= "No accommodations.",
                    Options = new List<ChatMessageModels.ListOption>
                    {
                        new ("Menu", "View the main menu", "menu")
                    }
                }
            };

            return config;
        }

        if (items.Count <= 10)
        {
            list.Items = new List<ChatMessageModels.ListSection>
            {
                new()
                {
                    Title = "Accommodations",
                    Options = items.Select(p => new ChatMessageModels.ListOption
                    {

                        Title = p.Name.ToEllipsis(20),
                        PostbackText = $"accommodation {p.Id}",
                        Description = p.Description.ToEllipsis(47)

                    }).ToList()
                }
            };
            return config;

        }

        if (!Pagination.Paging(Thread.CurrentMessage, 8)) Pagination.MoveToPage(0);

        list.Items = new List<ChatMessageModels.ListSection>
        {
            new()
            {
                Title = "Accommodations",
                Options = Pagination
                    .GetPaged( items, 8)
                    .Select(p => new ChatMessageModels.ListOption
                    {
                        Title = p.Name.ToEllipsis(20),
                        PostbackText = $"accommodation {p.Id}",
                        Description = p.Description.ToEllipsis(47)
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

        return config;
    }

    public BotMessageConfig FxGetAccommodation(BotMessageConfig json)
    {
        int.TryParse(Thread.CurrentMessage.Replace("accommodation ", ""),
            out var id);
        var accommodation = Database.Accommodations.Find(id);

        if (accommodation == default)
        {
            json.Messages = new List<BotMessage> {
            new TextMessage("The accommodation you have specified may have been deleted"),
            new TextMessage("Please check again later")
        };
        }
        else
        {

            Session.Set("AccommodationId", accommodation.Id);
            Session.Set("AccommodationName", accommodation.Name);
            var url = AppendToUrl(accommodation.Image1);
            var url2 = AppendToUrl(accommodation.Image2);

            json.Messages = new List<BotMessage>
        {
            new ImageMessage(AppendToUrl(accommodation.Image1), "accommodation image 1"),
            new ImageMessage(AppendToUrl(accommodation.Image2), "accommodation image 2"),
            /*new TextMessage($"{accommodation.Name} \n\n {accommodation.Description} \n\n ${accommodation.Price}"),*/
            new ButtonOptionsMessage("", "Would you like to view other accommodations?",
            new ChatMessageModels.ButtonOption("Reserve"),
            new ChatMessageModels.ButtonOption("Accommodations"), 
            new ChatMessageModels.ButtonOption("Menu")),
        };


        }
        return json;

    }

    public void FxSaveBookingCheckInDate(BotMessageConfig config)
    {
        Session.Set("CheckInDate", Thread.CurrentMessage);
    }
    public void FxSaveBookingCheckOutDate(BotMessageConfig config)
    {
        Session.Set("CheckOutDate", Thread.CurrentMessage);
    }

    public void FxSaveBookingGuests(BotMessageConfig json)
    {
        Session.Set("Guests", Thread.CurrentMessage);
    }

    public void FxSaveBookingContactNumber(BotMessageConfig json)
    {
        Session.Set("ContactNumber", Thread.CurrentMessage);
    }

    public void FxSaveBookingContactEmail(BotMessageConfig json)
    {
        var ContactEmail = new Booking
        {
            Apartment = Session.Get<string>("AccommodationName"),
            CheckInDate = Session.Get<string>("CheckInDate"),
            CheckOutDate = Session.Get<string>("CheckOutDate"),
            Guests = Session.Get<string>("Guests"),
            Phone = Session.Get<string>("ContactNumber"),
            Email = Thread.CurrentMessage,
            BookingContact = Thread.ThreadId,
            BookingStatus = BookingStatus.Pending,
            CreatedAt = DateTime.Now
        };

        Database.Bookings.Add(ContactEmail);
        Database.SaveChanges();
    }
    


}