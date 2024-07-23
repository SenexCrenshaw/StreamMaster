namespace StreamMaster.Application.Crypto.Commands;

[RequireAll]
public record DecodeProfileIdFromEncoded(string EncodedString) : IRequest<(int? streamGroupId, int? streamGroupProfileId)>;
public class DecodeProfileIdFromEncodedHandler(IRepositoryWrapper repositoryWrapper, IOptionsMonitor<Setting> intSettings)
    : IRequestHandler<DecodeProfileIdFromEncoded, (int? streamGroupId, int? streamGroupProfileId)>
{


    [LogExecutionTimeAspect]
    public async Task<(int? streamGroupId, int? streamGroupProfileId)> Handle(DecodeProfileIdFromEncoded request, CancellationToken cancellationToken)
    {
        Setting settings = intSettings.CurrentValue;
        (int? streamGroupId, string? valuesEncryptedString) = CryptoUtils.DecodeStreamGroupId(request.EncodedString, settings.ServerKey);
        if (streamGroupId == null || string.IsNullOrEmpty(valuesEncryptedString))
        {
            return (null, null);
        }

        // Assuming repositoryWrapper and StreamGroup class are available
        StreamGroup? sg = repositoryWrapper.StreamGroup.GetStreamGroup(streamGroupId.Value);
        if (sg == null || string.IsNullOrEmpty(sg.GroupKey))
        {
            return (null, null);
        }

        string decryptedTextWithGroupKey = AesEncryption.Decrypt(valuesEncryptedString, sg.GroupKey);
        string[] values = decryptedTextWithGroupKey.Split(',');
        if (values.Length == 2)
        {
            int streamGroupProfileId = int.Parse(values[1]);
            return (streamGroupId, streamGroupProfileId);
        }

        return (null, null);
    }
}