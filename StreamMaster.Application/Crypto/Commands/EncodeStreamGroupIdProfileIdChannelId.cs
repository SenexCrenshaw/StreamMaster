using System.Web;

namespace StreamMaster.Application.Crypto.Commands;

[RequireAll]
public record EncodeStreamGroupIdProfileIdChannelId(int StreamGroupId, int StreamGroupProfileId, int SMChannelId, string SMChannelName)
    : IRequest<(string? EncodedString, string? CleanName)>;
public class GetEncodedFromStreamGroupIdProfileIdChannelIdNameHandler(IOptionsMonitor<Setting> intSettings, IRepositoryWrapper repositoryWrapper)
    : IRequestHandler<EncodeStreamGroupIdProfileIdChannelId, (string? EncodedString, string? CleanName)>
{

    [LogExecutionTimeAspect]
    public async Task<(string? EncodedString, string? CleanName)> Handle(EncodeStreamGroupIdProfileIdChannelId request, CancellationToken cancellationToken)
    {
        StreamGroup? StreamGroup = repositoryWrapper.StreamGroup.GetStreamGroup(request.StreamGroupId);
        if (StreamGroup == null)
        {
            return (null, null);
        }

        if (string.IsNullOrEmpty(StreamGroup.GroupKey))
        {
            return (null, null);
        }

        Setting settings = intSettings.CurrentValue;

        string encryptedString = CryptoUtils.EncodeThreeValues(StreamGroup.Id, request.StreamGroupProfileId, request.SMChannelId, settings.ServerKey, StreamGroup.GroupKey);
        string cleanName = HttpUtility.HtmlEncode(request.SMChannelName).Trim()
       .Replace("/", "")
       .Replace(" ", "_");

        return (encryptedString, cleanName);
    }
}