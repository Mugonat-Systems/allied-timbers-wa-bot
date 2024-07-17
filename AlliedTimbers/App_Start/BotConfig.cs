using Mugonat.Bots.Core;
using Mugonat.Bots.WebBot;
using Mugonat.Bots.WhatsappCloudBot;
using System.Web.Http;
using AlliedTimbers.Bot;

namespace AlliedTimbers;

public static class BotConfig
{
    public static string AppName => AppSettings["bot.name"];
    public static string SourceNumber => AppSettings["bot.name"];
    public static string WhatsappCloudPhoneNumberId => AppSettings["bot.whatsappCloud.phoneNumberId"];
    public static string WhatsappCloudAccessToken => AppSettings["bot.whatsappCloud.accessToken"];

    public static void Register(HttpConfiguration config)
    {
        if (AppSettings["app.bot.dev"] == "true") 
            config.EnableDeveloperBot<AlliedTimbersBot>();
        if (AppSettings["app.bot.whatsapp"] == "true") 
            config.EnableWhatsappCloudBot<AlliedTimbersBot>();
       


    }

}