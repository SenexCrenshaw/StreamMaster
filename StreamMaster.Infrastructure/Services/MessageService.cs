using MediatR;

using Microsoft.Extensions.Logging;

using StreamMaster.Application.SMMessages.Commands;
using StreamMaster.Domain.Services;

namespace StreamMaster.Infrastructure.Services
{
    public class MessageService(ILogger<MessageService> Logger, ISender sender) : IMessageService
    {
        public async Task SendError(string message, string? details = null)
        {
            Logger.LogError(message);
            SendSMErrorRequest request = new(Detail: message, Summary: details);

            await sender.Send(request);
        }

        public async Task SendInfo(string message)
        {
            Logger.LogInformation(message);
            SendSMInfoRequest request = new(Detail: message);

            await sender.Send(request);
        }

        public async Task SendMessage(SMMessage smMessage)
        {
            SendSMMessageRequest request = new(smMessage);
            await sender.Send(request);
        }

        public async Task SendWarn(string message)
        {
            Logger.LogWarning(message);
            SendSMWarnRequest request = new(Detail: message);

            await sender.Send(request);
        }

        public async Task SendSuccess(string message)
        {
            Logger.LogInformation(message);
            SendSuccessRequest request = new(Detail: message);

            await sender.Send(request);
        }
    }
}
