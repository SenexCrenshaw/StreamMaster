using StreamMaster.Domain.Crypto;

namespace StreamMaster.Application.Crypto.Commands;

[RequireAll]
public record EncodeStreamGroupIdProfileId(int StreamGroupId, int StreamGroupProfileId)
    : IRequest<string?>;
public class EncodeStreamGroupIdProfileIdHandler(IRepositoryWrapper repositoryWrapper, ISender sender, IOptionsMonitor<Setting> intSettings)
    : IRequestHandler<EncodeStreamGroupIdProfileId, string?>
{

    [LogExecutionTimeAspect]
    public async Task<string?> Handle(EncodeStreamGroupIdProfileId request, CancellationToken cancellationToken)
    {

        StreamGroup? StreamGroup = repositoryWrapper.StreamGroup.GetStreamGroup(request.StreamGroupId);
        if (StreamGroup == null)
        {
            return null;
        }
        Setting settings = intSettings.CurrentValue;

        string encryptedString = CryptoUtils.EncodeTwoValues(StreamGroup.Id, request.StreamGroupProfileId, settings.ServerKey, StreamGroup.GroupKey);

        return encryptedString;
    }
}