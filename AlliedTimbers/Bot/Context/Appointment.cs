using System;
using AlliedTimbers.Models;
using Mugonat.Chat.BotEngine.Models;

namespace AlliedTimbers.Bot.Context;

public partial class AlliedTimbersBotContext
{
    public void FxSaveAppointmentName(BotMessageConfig config)
    {
        Session.Set("Name", Thread.CurrentMessage);
    }
    
    public void FxSaveAppointmentEmail(BotMessageConfig config)
    {
        Session.Set("Email", Thread.CurrentMessage);
    }
    
    public void FxSaveAppointmentPhone(BotMessageConfig config)
    {
        Session.Set("Phone", Thread.CurrentMessage);
    }

    public void FxSaveAppointmentDay(BotMessageConfig config)
    {
        Session.Set("Day", Thread.CurrentMessage);
    }

    public void FxSaveAppointmentTime(BotMessageConfig config)
    {
        Session.Set("Time", Thread.CurrentMessage);
    }
    
    public void FxSaveServiceType(BotMessageConfig config)
    {
        var serviceType = new Appointment
        {
            Name = Session.Get<string>("Name"),
            Email = Session.Get<string>("Email"),
            Phone = Session.Get<string>("Phone"),
            Date = Session.Get<string>("Day"),
            Time = Session.Get<string>("Time"),
            ServiceType = Thread.CurrentMessage,
            AppointmentStatus = AppointmentStatus.Pending,
            CreatedAt = DateTime.Now
        };
        Database.Appointments.Add(serviceType);
        Database.SaveChanges();
    }
}