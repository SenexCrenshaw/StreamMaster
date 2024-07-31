namespace StreamMaster.Application.Interfaces;

public interface ICryptoService
{
    string? EncodeString(string stringToEncode);
    string? EncodeInt(int intToEncode);
    string? DecodeString(string stringToDecode);
    int? DecodeInt(string stringToDecode);
    Task<(int? StreamGroupId, int? StreamGroupProfileId, int? SMChannelId)> DecodeProfileIdSMChannelIdFromEncodedAsync(string EncodedString);
    Task<(int? StreamGroupId, int? SMChannelId)> DecodeStreamGroupIdChannelIdAsync(string EncodedString);
    Task<(int? StreamGroupId, string? SMStreamId)> DecodeStreamGroupIdStreamIdAsync(string EncodedString);
    Task<string?> EncodeStreamGroupIdChannelIdAsync(int StreamGroupId, int SMChannelId);
    Task<string?> EncodeStreamGroupIdStreamIdAsync(int StreamGroupId, string SMStreamId);
}
