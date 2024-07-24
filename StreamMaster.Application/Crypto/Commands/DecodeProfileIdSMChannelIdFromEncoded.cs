using StreamMaster.Domain.Crypto;

namespace StreamMaster.Application.Crypto.Commands;

[RequireAll]
public record DecodeProfileIdSMChannelIdFromEncoded(string EncodedIds) : IRequest<(int? streamGroupId, int? StreamGroupProfileId, int? SMChannelId)>;
public class DecodeProfileIdSMChannelIdFromEncodedHandler(IRepositoryWrapper repositoryWrapper, IOptionsMonitor<Setting> intSettings)
    : IRequestHandler<DecodeProfileIdSMChannelIdFromEncoded, (int? StreamGroupId, int? StreamGroupProfileId, int? SMChannelId)>
{
    /// <summary>
    /// Handles the decoding of profile ID and SM Channel ID from encoded IDs.
    /// </summary>
    /// <param name="request">The request containing the encoded IDs.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The decoded profile ID and SM Channel ID.</returns>
    [LogExecutionTimeAspect]
    public async Task<(int? StreamGroupId, int? StreamGroupProfileId, int? SMChannelId)> Handle(DecodeProfileIdSMChannelIdFromEncoded request, CancellationToken cancellationToken)
    {
        Setting settings = intSettings.CurrentValue;
        (int? streamGroupId, string? valuesEncryptedString) = CryptoUtils.DecodeStreamGroupId(request.EncodedIds, settings.ServerKey);
        if (streamGroupId == null || string.IsNullOrEmpty(valuesEncryptedString))
        {
            return (null, null, null);
        }

        // Assuming repositoryWrapper and StreamGroup class are available
        StreamGroup? sg = repositoryWrapper.StreamGroup.GetStreamGroup(streamGroupId.Value);
        if (sg == null || string.IsNullOrEmpty(sg.GroupKey))
        {
            return (null, null, null);
        }

        string decryptedTextWithGroupKey = AesEncryption.Decrypt(valuesEncryptedString, sg.GroupKey);
        string[] values = decryptedTextWithGroupKey.Split(',');
        if (values.Length == 3)
        {
            int streamGroupProfileId = int.Parse(values[1]);
            int smChannelId = int.Parse(values[2]);
            return (streamGroupId, streamGroupProfileId, smChannelId);
        }

        return (null, null, null);
    }
}
