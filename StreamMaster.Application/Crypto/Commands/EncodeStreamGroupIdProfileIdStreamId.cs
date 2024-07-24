using System.Web;
using StreamMaster.Domain.Crypto;

namespace StreamMaster.Application.Crypto.Commands;

[RequireAll]
public record EncodeStreamGroupIdProfileIdStreamId(int StreamGroupId, int StreamGroupProfileId, string SMStreamId, string SMStreamName, byte[]? Iv = null)
    : IRequest<(string? EncodedString, string? CleanName)>;
public class GetEncodedFromStreamGroupIdProfileIdStreamIdNameHandler(IOptionsMonitor<Setting> intSettings, IRepositoryWrapper repositoryWrapper)
    : IRequestHandler<EncodeStreamGroupIdProfileIdStreamId, (string? EncodedString, string? CleanName)>
{

    [LogExecutionTimeAspect]
    public async Task<(string? EncodedString, string? CleanName)> Handle(EncodeStreamGroupIdProfileIdStreamId request, CancellationToken cancellationToken)
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

        string encryptedString = CryptoUtils.EncodeThreeValues(StreamGroup.Id, request.StreamGroupProfileId, request.SMStreamName, settings.ServerKey, StreamGroup.GroupKey);
        string cleanName = HttpUtility.HtmlEncode(request.SMStreamName).Trim()
       .Replace("/", "")
       .Replace(" ", "_");

        return (encryptedString, cleanName);
    }
}