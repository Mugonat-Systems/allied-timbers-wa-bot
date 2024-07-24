using System;
using AlliedTimbers.Models;
using Mugonat.Chat.BotEngine.Models;

namespace AlliedTimbers.Bot.Context;

public partial class AlliedTimbersBotContext
{
    public void FxSaveConsultationName(BotMessageConfig config)
    {
        Session.Set("Name", Thread.CurrentMessage);
    }
    
    public void FxSaveConsultationEmail(BotMessageConfig config)
    {
        Session.Set("Email", Thread.CurrentMessage);
    }
    
    public void FxSaveConsultationPhone(BotMessageConfig config)
    {
        Session.Set("Phone", Thread.CurrentMessage);
    }

    public void FxSaveConsultationDay(BotMessageConfig config)
    {
        Session.Set("Day", Thread.CurrentMessage);
    }

    public void FxSaveConsultationPeriodOfTime(BotMessageConfig config)
    {
        Session.Set("PeriodOfTime", Thread.CurrentMessage);
    }

    public void FxSaveConsultation(BotMessageConfig config)
    {
        var consultation = new Consultation
        {
            Name = Session.Get<string>("Name"),
            Email = Session.Get<string>("Email"),
            Phone = Session.Get<string>("Phone"),
            Date = Session.Get<string>("Day"),
            Time = Session.Get<string>("PeriodOfTime"),
            ServiceType = Thread.CurrentMessage,
            ConsultationStatus =  ConsultationStatus.Pending,
            CreatedAt = DateTime.Now
        };
        Database.Consultations.Add(consultation);
        Database.SaveChanges();
    }
    
    // public void FxSaveConsultationServiceType(BotMessageConfig config)
    // {
    //     var consultationService = new Consultation
    //     {
    //         Name = Session.Get<string>("Name"),
    //         Email = Session.Get<string>("Email"),
    //         Phone = Session.Get<string>("Phone"),
    //         Date = Session.Get<string>("Day"),
    //         Time = Session.Get<string>("ConsultationTime"),
    //         ServiceType = Thread.CurrentMessage,
    //         ConsultationStatus =  ConsultationStatus.Pending,
    //         CreatedAt = DateTime.Now
    //     };
    //     Database.Consultations.Add(consultationService);
    //     Database.SaveChanges();
    // }
}