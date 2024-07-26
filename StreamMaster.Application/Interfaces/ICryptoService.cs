namespace StreamMaster.Application.Interfaces;

public interface ICryptoService
{
    string? EncodeString(string stringToEncode);
    string? DecodeString(string stringToDecode);
    (int? StreamGroupId, int? SMChannelId) DecodeSMChannelIdFromEncoded(string EncodedString);
    (string? EncodedString, string? CleanName) EncodeStreamGroupIdChannelId(int StreamGroupId, int SMChannelId, string SMChannelName);
}
