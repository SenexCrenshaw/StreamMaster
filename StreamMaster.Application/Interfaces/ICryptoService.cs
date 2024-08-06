namespace StreamMaster.Application.Interfaces;

public interface ICryptoService
{
    string? EncodeString(string stringToEncode);
    string? EncodeInt(int intToEncode);
    string? DecodeString(string stringToDecode);
    int? DecodeInt(string stringToDecode);
    //Task<(int? StreamGroupId, int? StreamGroupProfileId, int? SMChannelId)> DecodeProfileIdSMChannelIdFromEncodedAsync(string EncodedString);
    //Task<(int? StreamGroupId, int? SMChannelId)> DecodeStreamGroupIdChannelIdAsync(string EncodedString);
    //Task<(int? StreamGroupId, string? SMStreamId)> DecodeStreamGroupIdStreamIdAsync(string EncodedString);
    //Task<string?> EncodeStreamGroupIdChannelIdAsync(int StreamGroupId, int SMChannelId);
    //Task<string?> EncodeStreamGroupIdStreamIdAsync(int StreamGroupId, string SMStreamId);


    //Task<(int? StreamGroupId, int? StreamGroupProfileId, int? SMChannelId)> DecodeStreamGroupIdProfileIdChannelId(string encodedString);
    //Task<(int? StreamGroupId, int? StreamGroupProfileId, string? SMStreamId)> DecodeStreamGroupIdProfileIdStreamId(string encodedString);
    //Task<(int? StreamGroupId, int? StreamGroupProfileId)> DecodeStreamGroupIdProfileIdAsync(string encodedString);
    //Task<string?> EncodeStreamGroupIdProfileId(int StreamGroupId, int StreamGroupProfileId);

    //Task<string?> EncodeStreamGroupIdProfileIdChannelId(int StreamGroupId, int StreamGroupProfileId, int SMChannelId);
    //Task<string?> EncodeStreamGroupIdProfileIdStreamId(int StreamGroupId, int StreamGroupProfileId, string SMStreamId);

    //Task<string?> EncodeStreamGroupProfileIdChannelId(int StreamGroupProfileId, int SMChannelId);
    //Task<string?> EncodeStreamGroupdProfileIdStreamId(int StreamGroupProfileId, string SMStreamId);

    //Task<(int? StreamGroupId, int? StreamGroupProfileId, string? SMStreamId)> DecodeStreamGroupdProfileIdStreamId(string encodedString);
    //Task<(int? StreamGroupId, int? StreamGroupProfileId, int? SMChannelId)> DecodeStreamGroupdProfileIdChannelId(string encodedString);
}
