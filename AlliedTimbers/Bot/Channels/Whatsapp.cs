using System.Threading.Tasks;
using Mugonat.Bots.Core.Models;
using Mugonat.Bots.WhatsappCloudBot.Interfaces;
using Mugonat.Bots.WhatsappCloudBot.Models;

namespace AlliedTimbers.Bot;

public partial class AlliedTimbersBot : IWhatsappCloudBot
{
    public Task OnWhatsappCloudUpdate(ChatEvent<CloudPayload> @event)
    {
        return ProcessMessage(@event.Api, @event.Payload);
    }
}