using Mugonat.Bots.Core.DevBot;
using Mugonat.Chat.BotEngine.Services;

namespace AlliedTimbers.Bot;

public partial class AlliedTimbersBot
{
    private void ConfigureDevBot()
    {
        this.OnDevUpdate(
            payload =>
            {
                new BotSession(payload.Phone).Set("userName", payload.Phone);

                return payload;
            },
            onReceived: payload => Talk(payload.Phone, payload.Message).Result

        );
    }
}