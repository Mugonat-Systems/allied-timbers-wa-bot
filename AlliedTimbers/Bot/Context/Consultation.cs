using System;
using AlliedTimbers.Models;
using Mugonat.Chat.BotEngine.Attributes;
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
    

    [InvalidResponse("Please enter a valid date e.g dd/mm/yyyy ")]
    public bool FxValidateDate(string date)
    {
        return IsValidDate(Thread.CurrentMessage);
    }

    private bool IsValidDate(string date) 
    {
        return System.Text.RegularExpressions.Regex.IsMatch(date , @"^(3[01]|[12][0-9]|0[1-9])/(1[0-2]|0[1-9])/[0-9]{4}$");
    }
    
    [InvalidResponse("Please enter a valid time e.g 10:00 ")]
    public bool FxValidateTime(string time)
    {
        return IsValidTime(Thread.CurrentMessage);
    }
    
    private bool IsValidTime(string time) 
    {
        return System.Text.RegularExpressions.Regex.IsMatch(time , @"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$");
    }
}