using StreamMaster.Domain.Crypto;

namespace StreamMaster.Application.Crypto;

public class CryptoService(IOptionsMonitor<Setting> intSettings) : ICryptoService
{
    //private async Task<(int? streamGroupId, string? groupKey, string? valuesEncryptedString)> GetGroupKeyFromEncodeAsync(string EncodedString)
    //{
    //    Setting settings = intSettings.CurrentValue;
    //    (int? streamGroupId, string? valuesEncryptedString) = CryptoUtils.DecodeStreamGroupId(EncodedString, settings.ServerKey);
    //    if (streamGroupId == null || string.IsNullOrEmpty(valuesEncryptedString))
    //    {
    //        return (null, null, null);
    //    }

    //    StreamGroup? sg = await streamGroupService.GetStreamGroupFromIdAsync(streamGroupId.Value);
    //    return sg == null || string.IsNullOrEmpty(sg.GroupKey) ? (null, null, null) : (streamGroupId, sg.GroupKey, valuesEncryptedString);
    //}

    //private async Task<string?> GetStreamGroupKeyFromIdAsync(int StreamGroupId)
    //{
    //    StreamGroup? StreamGroup = StreamGroupId < 0
    //        ? await streamGroupService.GetStreamGroupFromNameAsync("ALL")
    //        : await streamGroupService.GetStreamGroupFromIdAsync(StreamGroupId);
    //    return StreamGroup == null ? null : string.IsNullOrEmpty(StreamGroup.GroupKey) ? null : StreamGroup.GroupKey;
    //}

    //public async Task<(int? StreamGroupId, int? StreamGroupProfileId, int? SMChannelId)> DecodeProfileIdSMChannelIdFromEncodedAsync(string EncodedString)
    //{
    //    (int? streamGroupId, string? groupKey, string? valuesEncryptedString) = await GetGroupKeyFromEncodeAsync(EncodedString);
    //    if (streamGroupId == null || groupKey == null || string.IsNullOrEmpty(valuesEncryptedString))
    //    {
    //        return (null, null, null);
    //    }

    //    string decryptedTextWithGroupKey = AesEncryption.Decrypt(valuesEncryptedString, groupKey);
    //    string[] values = decryptedTextWithGroupKey.Split(',');
    //    if (values.Length == 3)
    //    {
    //        int streamGroupProfileId = int.Parse(values[1]);
    //        int smChannelId = int.Parse(values[2]);
    //        return (streamGroupId, streamGroupProfileId, smChannelId);
    //    }

    //    return (null, null, null);
    //}

    //public async Task<(int? StreamGroupId, string? SMStreamId)> DecodeStreamGroupIdStreamIdAsync(string EncodedString)
    //{
    //    (int? streamGroupId, string? groupKey, string? valuesEncryptedString) = await GetGroupKeyFromEncodeAsync(EncodedString);
    //    if (streamGroupId == null || groupKey == null || string.IsNullOrEmpty(valuesEncryptedString))
    //    {
    //        return (null, null);
    //    }

    //    string decryptedTextWithGroupKey = AesEncryption.Decrypt(valuesEncryptedString, groupKey);
    //    string[] values = decryptedTextWithGroupKey.Split(',');
    //    return values.Length == 2 ? ((int? StreamGroupId, string? SMStreamId))(streamGroupId, values[1]) : ((int? StreamGroupId, string? SMStreamId))(null, null);
    //}

    //public async Task<(int? StreamGroupId, int? SMChannelId)> DecodeStreamGroupIdChannelIdAsync(string EncodedString)
    //{
    //    (int? streamGroupId, string? groupKey, string? valuesEncryptedString) = await GetGroupKeyFromEncodeAsync(EncodedString);
    //    if (streamGroupId == null || groupKey == null || string.IsNullOrEmpty(valuesEncryptedString))
    //    {
    //        return (null, null);
    //    }

    //    string decryptedTextWithGroupKey = AesEncryption.Decrypt(valuesEncryptedString, groupKey);
    //    string[] values = decryptedTextWithGroupKey.Split(',');
    //    if (values.Length == 2)
    //    {
    //        int smChannelId = int.Parse(values[1]);
    //        return (streamGroupId, smChannelId);
    //    }

    //    return (null, null);
    //}

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

    public string? EncodeInt(int intToEncode)
    {
        Setting settings = intSettings.CurrentValue;

        string encryptedString = CryptoUtils.EncodeValue(intToEncode, settings.ServerKey);

        return encryptedString;
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

    //public async Task<string?> EncodeStreamGroupIdStreamIdAsync(int StreamGroupId, string SMStreamId)
    //{

    //    string? groupKey = await GetStreamGroupKeyFromIdAsync(StreamGroupId);
    //    if (string.IsNullOrEmpty(groupKey))
    //    {
    //        return null;
    //    }

    //    Setting settings = intSettings.CurrentValue;

    //    string encryptedString = CryptoUtils.EncodeTwoValues(StreamGroupId, SMStreamId, settings.ServerKey, groupKey);

    //    return encryptedString;
    //}

    //public async Task<string?> EncodeStreamGroupIdChannelIdAsync(int StreamGroupId, int SMChannelId)
    //{
    //    string? groupKey = await GetStreamGroupKeyFromIdAsync(StreamGroupId);
    //    if (string.IsNullOrEmpty(groupKey))
    //    {
    //        return null;
    //    }

    //    Setting settings = intSettings.CurrentValue;

    //    string encryptedString = CryptoUtils.EncodeTwoValues(StreamGroupId, SMChannelId, settings.ServerKey, groupKey);

    //    return encryptedString;
    //}

    //public async Task<(int? StreamGroupId, int? StreamGroupProfileId, int? SMChannelId)> DecodeStreamGroupIdProfileIdChannelId(string encodedString)
    //{
    //    (int? streamGroupId, string? groupKey, string? valuesEncryptedString) = await GetGroupKeyFromEncodeAsync(encodedString);
    //    if (streamGroupId == null || groupKey == null || string.IsNullOrEmpty(valuesEncryptedString))
    //    {
    //        return (null, null, null);
    //    }

    //    string decryptedTextWithGroupKey = AesEncryption.Decrypt(valuesEncryptedString, groupKey);
    //    string[] values = decryptedTextWithGroupKey.Split(',');
    //    if (values.Length == 3)
    //    {
    //        int streamGroupProfileId = int.Parse(values[1]);
    //        int smChannelId = int.Parse(values[2]);
    //        return (streamGroupId, streamGroupProfileId, smChannelId);
    //    }

    //    return (null, null, null);
    //}

    //public async Task<(int? StreamGroupId, int? StreamGroupProfileId, string? SMStreamId)> DecodeStreamGroupIdProfileIdStreamId(string encodedString)
    //{
    //    (int? streamGroupId, string? groupKey, string? valuesEncryptedString) = await GetGroupKeyFromEncodeAsync(encodedString);
    //    if (streamGroupId == null || groupKey == null || string.IsNullOrEmpty(valuesEncryptedString))
    //    {
    //        return (null, null, null);
    //    }

    //    string decryptedTextWithGroupKey = AesEncryption.Decrypt(valuesEncryptedString, groupKey);
    //    string[] values = decryptedTextWithGroupKey.Split(',');
    //    if (values.Length == 3)
    //    {
    //        int streamGroupProfileId = int.Parse(values[1]);
    //        string smStreamId = values[2];
    //        return (streamGroupId, streamGroupProfileId, smStreamId);
    //    }

    //    return (null, null, null);
    //}

    //public async Task<string?> EncodeStreamGroupIdProfileIdChannelIdAsync(int StreamGroupId, int StreamGroupProfileId, int SMChannelId)
    //{
    //    string? groupKey = await GetStreamGroupKeyFromIdAsync(StreamGroupId);
    //    if (string.IsNullOrEmpty(groupKey))
    //    {
    //        return null;
    //    }

    //    Setting settings = intSettings.CurrentValue;

    //    string encryptedString = CryptoUtils.EncodeThreeValues(StreamGroupId, StreamGroupProfileId, SMChannelId, settings.ServerKey, groupKey);

    //    return encryptedString;
    //}

    //public async Task<string?> EncodeStreamGroupIdProfileIdStreamId(int StreamGroupId, int StreamGroupProfileId, string SMStreamId)
    //{
    //    string? groupKey = await GetStreamGroupKeyFromIdAsync(StreamGroupId);
    //    if (string.IsNullOrEmpty(groupKey))
    //    {
    //        return null;
    //    }

    //    Setting settings = intSettings.CurrentValue;

    //    string encryptedString = CryptoUtils.EncodeThreeValues(StreamGroupId, StreamGroupProfileId, SMStreamId, settings.ServerKey, groupKey);

    //    return encryptedString;
    //}

    //public async Task<(int? StreamGroupId, int? StreamGroupProfileId)> DecodeStreamGroupIdProfileIdAsync(string encodedString)
    //{
    //    (int? streamGroupId, string? groupKey, string? valuesEncryptedString) = await GetGroupKeyFromEncodeAsync(encodedString);
    //    if (streamGroupId == null || groupKey == null || string.IsNullOrEmpty(valuesEncryptedString))
    //    {
    //        return (null, null);
    //    }

    //    string decryptedTextWithGroupKey = AesEncryption.Decrypt(valuesEncryptedString, groupKey);
    //    string[] values = decryptedTextWithGroupKey.Split(',');
    //    if (values.Length == 2)
    //    {
    //        int streamGroupProfileId = int.Parse(values[1]);
    //        return (streamGroupId, streamGroupProfileId);
    //    }

    //    return (null, null);
    //}

    //public async Task<string?> EncodeStreamGroupIdProfileId(int StreamGroupId, int StreamGroupProfileId)
    //{
    //    string? groupKey = await GetStreamGroupKeyFromIdAsync(StreamGroupId);
    //    if (string.IsNullOrEmpty(groupKey))
    //    {
    //        return null;
    //    }

    //    Setting settings = intSettings.CurrentValue;

    //    string encryptedString = CryptoUtils.EncodeTwoValues(StreamGroupId, StreamGroupProfileId, settings.ServerKey, groupKey);

    //    return encryptedString;
    //}

    //public async Task<string?> EncodeStreamGroupProfileIdChannelId(int StreamGroupProfileId, int SMChannelId)
    //{
    //    StreamGroup? sg = await streamGroupService.GetStreamGroupFromSGProfileIdAsync(StreamGroupProfileId);
    //    if (sg == null)
    //    {
    //        return null;
    //    }

    //    string? encryptedString = await EncodeStreamGroupIdProfileIdChannelIdAsync(sg.Id, StreamGroupProfileId, SMChannelId);

    //    return encryptedString;
    //}

    //public async Task<(int? StreamGroupId, int? StreamGroupProfileId, int? SMChannelId)> DecodeStreamGroupdProfileIdChannelId(string encodedString)
    //{
    //    (int? streamGroupId, string? groupKey, string? valuesEncryptedString) = await GetGroupKeyFromEncodeAsync(encodedString);
    //    if (streamGroupId == null || groupKey == null || string.IsNullOrEmpty(valuesEncryptedString))
    //    {
    //        return (null, null, null);
    //    }

    //    string decryptedTextWithGroupKey = AesEncryption.Decrypt(valuesEncryptedString, groupKey);
    //    string[] values = decryptedTextWithGroupKey.Split(',');
    //    if (values.Length == 2)
    //    {
    //        int streamGroupProfileId = int.Parse(values[1]);
    //        int SMChannelId = int.Parse(values[2]);
    //        return (streamGroupId, streamGroupProfileId, SMChannelId);
    //    }

    //    return (null, null, null);
    //}

    //public async Task<string?> EncodeStreamGroupdProfileIdStreamId(int StreamGroupProfileId, string SMStreamId)
    //{
    //    StreamGroup? sg = await streamGroupService.GetStreamGroupFromSGProfileIdAsync(StreamGroupProfileId);
    //    if (sg == null)
    //    {
    //        return null;
    //    }

    //    string? encryptedString = await EncodeStreamGroupIdProfileIdStreamId(sg.Id, StreamGroupProfileId, SMStreamId);

    //    return encryptedString;
    //}
}
