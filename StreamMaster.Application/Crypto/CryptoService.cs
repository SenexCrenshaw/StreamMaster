using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Application.Interfaces;
using StreamMaster.Domain.Crypto;

using System.Web;

namespace StreamMaster.Application.Crypto;

public class CryptoService(IOptionsMonitor<Setting> intSettings, IServiceProvider serviceProvider) : ICryptoService
{
    public (int? StreamGroupId, int? SMChannelId) DecodeSMChannelIdFromEncoded(string EncodedString)
    {
        Setting settings = intSettings.CurrentValue;
        (int? streamGroupId, string? valuesEncryptedString) = CryptoUtils.DecodeStreamGroupId(EncodedString, settings.ServerKey);
        if (streamGroupId == null || string.IsNullOrEmpty(valuesEncryptedString))
        {
            return (null, null);
        }
        using IServiceScope scope = serviceProvider.CreateScope();
        IRepositoryWrapper repositoryWrapper = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
        StreamGroup? sg = repositoryWrapper.StreamGroup.GetStreamGroup(streamGroupId.Value);
        if (sg == null || string.IsNullOrEmpty(sg.GroupKey))
        {
            return (null, null);
        }

        string decryptedTextWithGroupKey = AesEncryption.Decrypt(valuesEncryptedString, sg.GroupKey);
        string[] values = decryptedTextWithGroupKey.Split(',');
        if (values.Length == 2)
        {
            int smChannelId = int.Parse(values[1]);
            return (sg.Id, smChannelId);
        }

        return (null, null);
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

    public (string? EncodedString, string? CleanName) EncodeStreamGroupIdChannelId(int StreamGroupId, int SMChannelId, string SMChannelName)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        IRepositoryWrapper repositoryWrapper = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
        StreamGroup? StreamGroup = repositoryWrapper.StreamGroup.GetStreamGroup(StreamGroupId);
        if (StreamGroup == null)
        {
            return (null, null);
        }

        if (string.IsNullOrEmpty(StreamGroup.GroupKey))
        {
            return (null, null);
        }

        Setting settings = intSettings.CurrentValue;

        string encryptedString = CryptoUtils.EncodeTwoValues(StreamGroup.Id, SMChannelId, settings.ServerKey, StreamGroup.GroupKey);
        string cleanName = HttpUtility.HtmlEncode(SMChannelName).Trim()
       .Replace("/", "")
       .Replace(" ", "_");

        return (encryptedString, cleanName);
    }
}
