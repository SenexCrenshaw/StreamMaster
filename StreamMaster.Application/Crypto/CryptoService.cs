using StreamMaster.Domain.Crypto;

namespace StreamMaster.Application.Crypto;

public class CryptoService(IOptionsMonitor<Setting> intSettings, IStreamGroupService streamGroupService) : ICryptoService
{
    private async Task<(int? streamGroupId, string? groupKey, string? valuesEncryptedString)> GetGroupKeyAsync(string EncodedString)
    {
        Setting settings = intSettings.CurrentValue;
        (int? streamGroupId, string? valuesEncryptedString) = CryptoUtils.DecodeStreamGroupId(EncodedString, settings.ServerKey);
        if (streamGroupId == null || string.IsNullOrEmpty(valuesEncryptedString))
        {
            return (null, null, null);
        }

        StreamGroup? sg = await streamGroupService.GetStreamGroupFromIdAsync(streamGroupId.Value);
        return sg == null || string.IsNullOrEmpty(sg.GroupKey) ? (null, null, null) : (streamGroupId, sg.GroupKey, valuesEncryptedString);
    }
    public async Task<(int? StreamGroupId, int? StreamGroupProfileId, int? SMChannelId)> DecodeProfileIdSMChannelIdFromEncodedAsync(string EncodedString)
    {
        (int? streamGroupId, string? groupKey, string? valuesEncryptedString) = await GetGroupKeyAsync(EncodedString);
        if (streamGroupId == null || groupKey == null || string.IsNullOrEmpty(valuesEncryptedString))
        {
            return (null, null, null);
        }

        string decryptedTextWithGroupKey = AesEncryption.Decrypt(valuesEncryptedString, groupKey);
        string[] values = decryptedTextWithGroupKey.Split(',');
        if (values.Length == 3)
        {
            int streamGroupProfileId = int.Parse(values[1]);
            int smChannelId = int.Parse(values[2]);
            return (streamGroupId, streamGroupProfileId, smChannelId);
        }

        return (null, null, null);
    }


    public async Task<(int? StreamGroupId, string? SMStreamId)> DecodeStreamGroupIdStreamIdAsync(string EncodedString)
    {
        (int? streamGroupId, string? groupKey, string? valuesEncryptedString) = await GetGroupKeyAsync(EncodedString);
        if (streamGroupId == null || groupKey == null || string.IsNullOrEmpty(valuesEncryptedString))
        {
            return (null, null);
        }

        string decryptedTextWithGroupKey = AesEncryption.Decrypt(valuesEncryptedString, groupKey);
        string[] values = decryptedTextWithGroupKey.Split(',');
        return values.Length == 2 ? ((int? StreamGroupId, string? SMStreamId))(streamGroupId, values[1]) : ((int? StreamGroupId, string? SMStreamId))(null, null);
    }

    public async Task<(int? StreamGroupId, int? SMChannelId)> DecodeStreamGroupIdChannelIdAsync(string EncodedString)
    {
        (int? streamGroupId, string? groupKey, string? valuesEncryptedString) = await GetGroupKeyAsync(EncodedString);
        if (streamGroupId == null || groupKey == null || string.IsNullOrEmpty(valuesEncryptedString))
        {
            return (null, null);
        }

        string decryptedTextWithGroupKey = AesEncryption.Decrypt(valuesEncryptedString, groupKey);
        string[] values = decryptedTextWithGroupKey.Split(',');
        if (values.Length == 2)
        {
            int smChannelId = int.Parse(values[1]);
            return (streamGroupId, smChannelId);
        }

        return (null, null);
    }

    public int? DecodeInt(string stringToDecode)
    {
        if (string.IsNullOrEmpty(stringToDecode))
        {
            return null;
        }

        Setting settings = intSettings.CurrentValue;

        string? encryptedString = CryptoUtils.DecodeValue(stringToDecode, settings.ServerKey);
        if (encryptedString == null)
        {
            return null;
        }
        int ret = int.Parse(encryptedString);
        return ret;
    }

    public string? EncodeString(string stringToEncode)
    {
        if (string.IsNullOrEmpty(stringToEncode))
        {
            return null;
        }

        Setting settings = intSettings.CurrentValue;

        string encryptedString = CryptoUtils.EncodeValue(stringToEncode, settings.ServerKey);

        return encryptedString;
    }
    public string? EncodeInt(int intToEncode)
    {
        Setting settings = intSettings.CurrentValue;

        string encryptedString = CryptoUtils.EncodeValue(intToEncode, settings.ServerKey);

        return encryptedString;
    }

    public string? DecodeString(string stringToDecode)
    {
        if (string.IsNullOrEmpty(stringToDecode))
        {
            return null;
        }

        Setting settings = intSettings.CurrentValue;

        string? encryptedString = CryptoUtils.DecodeValue(stringToDecode, settings.ServerKey);

        return encryptedString;
    }


    public async Task<string?> EncodeStreamGroupIdStreamIdAsync(int StreamGroupId, string SMStreamId)
    {
        StreamGroup? StreamGroup = StreamGroupId < 0
            ? await streamGroupService.GetStreamGroupFromNameAsync("ALl")
            : await streamGroupService.GetStreamGroupFromIdAsync(StreamGroupId);
        if (StreamGroup == null)
        {
            return null;
        }

        if (string.IsNullOrEmpty(StreamGroup.GroupKey))
        {
            return null;
        }

        Setting settings = intSettings.CurrentValue;

        string encryptedString = CryptoUtils.EncodeTwoValues(StreamGroup.Id, SMStreamId, settings.ServerKey, StreamGroup.GroupKey);

        return encryptedString;
    }

    public async Task<string?> EncodeStreamGroupIdChannelIdAsync(int StreamGroupId, int SMChannelId)
    {
        StreamGroup? StreamGroup = StreamGroupId < 0
            ? await streamGroupService.GetStreamGroupFromNameAsync("ALl")
            : await streamGroupService.GetStreamGroupFromIdAsync(StreamGroupId);
        if (StreamGroup == null)
        {
            return null;
        }

        if (string.IsNullOrEmpty(StreamGroup.GroupKey))
        {
            return null;
        }

        Setting settings = intSettings.CurrentValue;

        string encryptedString = CryptoUtils.EncodeTwoValues(StreamGroup.Id, SMChannelId, settings.ServerKey, StreamGroup.GroupKey);

        return encryptedString;
    }
}
