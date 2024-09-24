using Microsoft.AspNetCore.Http;

using StreamMaster.Application.Common.Models;
using StreamMaster.Application.StreamGroups.Queries;
using StreamMaster.Domain.Crypto;
using StreamMaster.SchedulesDirect.Domain.Extensions;

using System.Collections.Concurrent;
using System.Text.Json;
using System.Xml.Serialization;

using static StreamMaster.Domain.Common.GetStreamGroupEPGHandler;
namespace StreamMaster.Application.StreamGroups;

public class StreamGroupService(ILogger<StreamGroupService> _logger, ILogoService logoService, ICacheManager CacheManager, IMapper _mapper, IRepositoryWrapper repositoryWrapper, ISchedulesDirectDataService _schedulesDirectDataService, IOptionsMonitor<CommandProfileDict> _commandProfileSettings, IOptionsMonitor<Setting> _settings, IMemoryCache _memoryCache, IProfileService _profileService)
    : IStreamGroupService
{
    private const string DefaultStreamGroupName = "all";
    private const string CacheKey = "DefaultStreamGroup";

    #region CRYPTO

    private async Task<(int? streamGroupId, string? groupKey, string? valuesEncryptedString)> GetGroupKeyFromEncodeAsync(string EncodedString)
    {
        Setting settings = _settings.CurrentValue;
        (int? streamGroupId, string? valuesEncryptedString) = CryptoUtils.DecodeStreamGroupId(EncodedString, settings.ServerKey);
        if (streamGroupId == null || string.IsNullOrEmpty(valuesEncryptedString))
        {
            return (null, null, null);
        }

        StreamGroup? sg = await GetStreamGroupFromIdAsync(streamGroupId.Value);
        return sg == null || string.IsNullOrEmpty(sg.GroupKey) ? (null, null, null) : (streamGroupId, sg.GroupKey, valuesEncryptedString);
    }

    private async Task<string?> GetStreamGroupKeyFromIdAsync(int StreamGroupId)
    {
        //StreamGroup? StreamGroup = StreamGroupId < 0
        //    ? await GetStreamGroupFromNameAsync(DefaultStreamGroupName)
        //    : await GetStreamGroupFromIdAsync(StreamGroupId);
        //return StreamGroup == null ? null : string.IsNullOrEmpty(StreamGroup.GroupKey) ? null : StreamGroup.GroupKey;
        // Try to get the value from the cache first
        if (CacheManager.StreamGroupKeyCache.TryGetValue(StreamGroupId, out string? cachedGroupKey))
        {
            return cachedGroupKey;
        }

        // Value is not in the cache, proceed to retrieve it
        StreamGroup? streamGroup = StreamGroupId < 0
            ? await GetStreamGroupFromNameAsync(DefaultStreamGroupName)
            : await GetStreamGroupFromIdAsync(StreamGroupId);

        string? groupKey = streamGroup == null || string.IsNullOrEmpty(streamGroup.GroupKey)
            ? null
            : streamGroup.GroupKey;

        // Add the value to the cache
        CacheManager.StreamGroupKeyCache.TryAdd(StreamGroupId, groupKey);

        return groupKey;
    }

    public async Task<(int? StreamGroupId, int? StreamGroupProfileId, int? SMChannelId)> DecodeProfileIdSMChannelIdFromEncodedAsync(string EncodedString)
    {
        (int? streamGroupId, string? groupKey, string? valuesEncryptedString) = await GetGroupKeyFromEncodeAsync(EncodedString);
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

    public async Task<(int? StreamGroupId, int? StreamGroupProfileId, int? SMChannelId)> DecodeStreamGroupIdProfileIdChannelId(string encodedString)
    {
        (int? streamGroupId, string? groupKey, string? valuesEncryptedString) = await GetGroupKeyFromEncodeAsync(encodedString);
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

    public async Task<(int? StreamGroupId, int? StreamGroupProfileId, string? SMStreamId)> DecodeStreamGroupIdProfileIdStreamId(string encodedString)
    {
        (int? streamGroupId, string? groupKey, string? valuesEncryptedString) = await GetGroupKeyFromEncodeAsync(encodedString);
        if (streamGroupId == null || groupKey == null || string.IsNullOrEmpty(valuesEncryptedString))
        {
            return (null, null, null);
        }

        string decryptedTextWithGroupKey = AesEncryption.Decrypt(valuesEncryptedString, groupKey);
        string[] values = decryptedTextWithGroupKey.Split(',');
        if (values.Length == 3)
        {
            int streamGroupProfileId = int.Parse(values[1]);
            string smStreamId = values[2];
            return (streamGroupId, streamGroupProfileId, smStreamId);
        }

        return (null, null, null);
    }

    public string EncodeStreamGroupIdProfileIdChannelId(StreamGroup streamGroup, int StreamGroupProfileId, int SMChannelId)
    {
        Setting settings = _settings.CurrentValue;

        string encryptedString = CryptoUtils.EncodeThreeValues(streamGroup.Id, StreamGroupProfileId, SMChannelId, settings.ServerKey, streamGroup.GroupKey);

        return encryptedString;
    }

    public async Task<string?> EncodeStreamGroupIdProfileIdChannelIdAsync(int StreamGroupId, int StreamGroupProfileId, int SMChannelId)
    {
        string? groupKey = await GetStreamGroupKeyFromIdAsync(StreamGroupId);
        if (string.IsNullOrEmpty(groupKey))
        {
            return null;
        }

        Setting settings = _settings.CurrentValue;

        string encryptedString = CryptoUtils.EncodeThreeValues(StreamGroupId, StreamGroupProfileId, SMChannelId, settings.ServerKey, groupKey);

        return encryptedString;
    }

    public async Task<string?> EncodeStreamGroupIdProfileIdStreamId(int StreamGroupId, int StreamGroupProfileId, string SMStreamId)
    {
        string? groupKey = await GetStreamGroupKeyFromIdAsync(StreamGroupId);
        if (string.IsNullOrEmpty(groupKey))
        {
            return null;
        }

        Setting settings = _settings.CurrentValue;

        string encryptedString = CryptoUtils.EncodeThreeValues(StreamGroupId, StreamGroupProfileId, SMStreamId, settings.ServerKey, groupKey);

        return encryptedString;
    }

    public async Task<string?> EncodeStreamGroupProfileIdChannelId(int StreamGroupProfileId, int SMChannelId)
    {
        StreamGroup? sg = await GetStreamGroupFromSGProfileIdAsync(StreamGroupProfileId);
        if (sg == null)
        {
            return null;
        }

        string? encryptedString = await EncodeStreamGroupIdProfileIdChannelIdAsync(sg.Id, StreamGroupProfileId, SMChannelId);

        return encryptedString;
    }

    //public async Task<string?> EncodeStreamGroupProfileIdChannelId(StreamGroup streamGroup,int StreamGroupProfileId, int SMChannelId)
    //{
    //    string? encryptedString =  EncodeStreamGroupIdProfileIdChannelIdAsync(streamGroup, StreamGroupProfileId, SMChannelId);

    //    return encryptedString;
    //}

    public async Task<(int? StreamGroupId, int? StreamGroupProfileId, int? SMChannelId)> DecodeStreamGroupdProfileIdChannelId(string encodedString)
    {
        (int? streamGroupId, string? groupKey, string? valuesEncryptedString) = await GetGroupKeyFromEncodeAsync(encodedString);
        if (streamGroupId == null || groupKey == null || string.IsNullOrEmpty(valuesEncryptedString))
        {
            return (null, null, null);
        }

        string decryptedTextWithGroupKey = AesEncryption.Decrypt(valuesEncryptedString, groupKey);
        string[] values = decryptedTextWithGroupKey.Split(',');
        if (values.Length == 2)
        {
            int streamGroupProfileId = int.Parse(values[1]);
            int SMChannelId = int.Parse(values[2]);
            return (streamGroupId, streamGroupProfileId, SMChannelId);
        }

        return (null, null, null);
    }
    public string? EncodeStreamGroupIdStreamId(StreamGroup StreamGroup, string SMStreamId)
    {
        string encryptedString = CryptoUtils.EncodeTwoValues(StreamGroup.Id, SMStreamId, _settings.CurrentValue.ServerKey, StreamGroup.GroupKey);

        return encryptedString;
    }

    public async Task<string?> EncodeStreamGroupIdStreamIdAsync(int StreamGroupId, string SMStreamId)
    {
        string? groupKey = await GetStreamGroupKeyFromIdAsync(StreamGroupId);
        if (string.IsNullOrEmpty(groupKey))
        {
            return null;
        }

        string encryptedString = CryptoUtils.EncodeTwoValues(StreamGroupId, SMStreamId, _settings.CurrentValue.ServerKey, groupKey);

        return encryptedString;
    }

    #endregion

    public async Task<List<StreamGroupDto>> GetStreamGroups()
    {
        //using IServiceScope scope = _serviceProvider.CreateScope();
        //IRepositoryWrapper repositoryWrapper = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
        //return await repositoryWrapper.StreamGroup.ProjectTo<StreamGroupDto>(_mapper.ConfigurationProvider).ToListAsync();
        return await repositoryWrapper.StreamGroup.GetQuery().ProjectTo<StreamGroupDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task<int> GetDefaultSGIdAsync()
    {
        StreamGroup sg = await GetDefaultSGAsync();
        return sg.Id;
    }

    public string GetStreamGroupLineupStatus()
    {
        string jsonString = JsonSerializer.Serialize(new LineupStatus(), BuildInfo.JsonIndentOptions);

        return jsonString;
    }

    public async Task<string> GetStreamGroupLineup(int StreamGroupProfileId, HttpRequest httpRequest, bool isShort)
    {
        string requestPath = httpRequest.GetUrlWithPathValue();
        string url = httpRequest.GetUrl();
        List<SGLineup> ret = [];

        StreamGroup? streamGroup = await GetStreamGroupFromSGProfileIdAsync(StreamGroupProfileId);
        if (streamGroup == null)
        {
            return JsonSerializer.Serialize(ret);
        }

        List<SMChannel> smChannels = (await repositoryWrapper.SMChannel.GetSMChannelsFromStreamGroup(streamGroup.Id)).Where(a => !a.IsHidden).ToList();

        if (smChannels.Count == 0)
        {
            return JsonSerializer.Serialize(ret);
        }

        (List<VideoStreamConfig> videoStreamConfigs, StreamGroupProfile streamGroupProfile) = await GetStreamGroupVideoConfigs(StreamGroupProfileId);

        if (videoStreamConfigs is null || streamGroupProfile is null)
        {
            return string.Empty;
        }

        ConcurrentBag<EncodedData> encodedData = [];

        Parallel.ForEach(smChannels, smChannel =>
        {
            string? encodedString = EncodeStreamGroupIdProfileIdChannelId(streamGroup, streamGroupProfile.Id, smChannel.Id);
            if (string.IsNullOrEmpty(encodedString))
            {
                return;
            }
            string cleanName = smChannel.Name.ToCleanFileString();

            EncodedData data = new()
            {
                SMChannel = smChannel,
                EncodedString = encodedString,
                CleanName = cleanName
            };

            encodedData.Add(data);
        });

        await Parallel.ForEachAsync(encodedData, (encodedData, _) =>
        {
            //if (string.IsNullOrEmpty(videoStreamConfig?.EPGId))
            //{
            //    return ValueTask.CompletedTask;
            //}
            VideoStreamConfig? videoStreamConfig = videoStreamConfigs.Find(a => a.Id == encodedData.SMChannel.Id);
            if (videoStreamConfig == null)
            {
                return ValueTask.CompletedTask;
            }

            //bool isDummy = _epgHelper.IsDummy(videoStreamConfig.EPGId);

            //if (isDummy)
            //{
            //    videoStreamConfig.EPGId = $"{EPGHelper.DummyId}-{videoStreamConfig.Id}";

            //    videoStreamConfig = new()
            //    {
            //        Id = videoStreamConfig.Id,
            //        Name = videoStreamConfig.Name,
            //        EPGId = videoStreamConfig.EPGId,
            //        Logo = videoStreamConfig.Logo,
            //        ChannelNumber = videoStreamConfig.ChannelNumber,
            //        IsDuplicate = false,
            //        IsDummy = false
            //    };
            //    //_ = dummyData.FindOrCreateDummyService(videoStreamConfig2.EPGId, videoStreamConfig2);
            //    //return ValueTask.CompletedTask;
            //}

            //int epgNumber = EPGHelper.DummyId;
            string stationId;

            if (string.IsNullOrEmpty(videoStreamConfig.EPGId))
            {
                stationId = videoStreamConfig.Group;
            }
            else
            {
                if (EPGHelper.IsValidEPGId(videoStreamConfig.EPGId))
                {
                    (int _, stationId) = videoStreamConfig.EPGId.ExtractEPGNumberAndStationId();
                }
                else
                {
                    stationId = videoStreamConfig.EPGId;
                }
            }

            string? encodedString = EncodeStreamGroupIdProfileIdChannelId(streamGroup, StreamGroupProfileId, videoStreamConfig.Id);
            string videoUrl = isShort
                ? $"{url}/v/{StreamGroupProfileId}/{videoStreamConfig.Id}"
                : $"{url}/api/videostreams/stream/{encodedString}/{videoStreamConfig.Name.ToCleanFileString()}";

            MxfService? service = _schedulesDirectDataService.AllServices.GetMxfService(videoStreamConfig.EPGId);
            if (service == null)
            {
                return ValueTask.CompletedTask;
            }
            string graceNote = service?.CallSign ?? stationId;

            //string logo;
            //if (service?.mxfGuideImage != null && !string.IsNullOrEmpty(service.mxfGuideImage.ImageUrl))
            //{
            //    //logo = service.mxfGuideImage.ImageUrl;
            //    //string baseUrl = url;
            //    //logo = _iconHelper.GetIconUrl(service.EPGNumber, service.extras["logo"].Url, baseUrl);
            //    logo = logoService.GetLogoUrl(videoStreamConfig.Logo, url);
            //}
            //else
            //{
            //    logo = GetIconUrl(videoStreamConfig.Logo, httpRequest);
            //    string logoUrl = logoService.GetLogoUrl(videoStreamConfig.Logo, url);
            //}

            string logo = logoService.GetLogoUrl(videoStreamConfig.Logo, url);

            string id = videoStreamConfig.ChannelNumber.ToString();

            if (videoStreamConfig.OutputProfile.Id != nameof(ValidM3USetting.NotMapped))
            {
                switch (videoStreamConfig.OutputProfile.Id)
                {
                    case nameof(ValidM3USetting.Group):
                        if (videoStreamConfig.OutputProfile != null && !string.IsNullOrEmpty(videoStreamConfig.OutputProfile.Group))
                        {
                            id = videoStreamConfig.Group;
                        }
                        break;
                    case nameof(ValidM3USetting.ChannelNumber):
                        id = videoStreamConfig.ChannelNumber.ToString();
                        break;

                    case nameof(ValidM3USetting.Name):
                        id = videoStreamConfig.Name.ToCleanFileString();
                        break;
                }
            }

            SGLineup lu = new()
            {
                GuideName = videoStreamConfig.Name,
                GuideNumber = videoStreamConfig.ChannelNumber.ToString(),
                Station = id,
                Logo = logo,
                URL = videoUrl
            };

            lock (ret)
            {
                ret.Add(lu);
            }

            return ValueTask.CompletedTask;
        });

        string jsonString = JsonSerializer.Serialize(ret.OrderBy(a => a.GuideNumber));
        return jsonString;
    }

    public async Task<string> GetStreamGroupDiscover(int StreamGroupProfileId, HttpRequest request)
    {
        //using IServiceScope scope = _serviceProvider.CreateScope();
        //IRepositoryWrapper repositoryWrapper = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();

        string url = request.GetUrlWithPath();
        int maxTuners = await repositoryWrapper.M3UFile.GetM3UMaxStreamCount();

        StreamGroup? streamGroup = await GetStreamGroupFromSGProfileIdAsync(StreamGroupProfileId);
        if (streamGroup == null)
        {
            return "";
        }

        Discover discover = new(url, streamGroup.DeviceID + "-" + StreamGroupProfileId, maxTuners);

        string jsonString = JsonSerializer.Serialize(discover, BuildInfo.JsonIndentOptions);
        return jsonString;
    }

    public async Task<string> GetStreamGroupCapability(int StreamGroupProfileId, HttpRequest request)
    {
        StreamGroup? streamGroup = await GetStreamGroupFromSGProfileIdAsync(StreamGroupProfileId).ConfigureAwait(false);
        if (streamGroup == null)
        {
            return "";
        }

        string url = request.GetUrlWithPath();
        Capability capability = new(url, $"{streamGroup.DeviceID}-{StreamGroupProfileId}");

        await using Utf8StringWriter textWriter = new();
        XmlSerializer serializer = new(typeof(Capability));
        serializer.Serialize(textWriter, capability);

        return textWriter.ToString();
    }

    public async Task<CommandProfileDto> GetProfileFromSGIdsCommandProfileNameAsync(int? streamGroupId, int streamGroupProfileId, string commandProfileName)
    {
        CommandProfileDto? commandProfileDto = null;
        if (!commandProfileName.Equals("Default", StringComparison.InvariantCultureIgnoreCase))
        {
            commandProfileDto = _commandProfileSettings.CurrentValue.GetProfileDto(commandProfileName);
        }

        if (commandProfileDto?.ProfileName.Equals("Default", StringComparison.InvariantCultureIgnoreCase) != false)
        {
            StreamGroupProfile streamGroupProfile = await GetStreamGroupProfileAsync(streamGroupId, streamGroupProfileId);
            commandProfileDto = !streamGroupProfile.CommandProfileName.Equals("Default", StringComparison.InvariantCultureIgnoreCase)
                ? _commandProfileSettings.CurrentValue.GetProfileDto(streamGroupProfile.CommandProfileName)
                : _commandProfileSettings.CurrentValue.GetProfileDto(_settings.CurrentValue.DefaultCommandProfileName);
        }
        return commandProfileDto;
    }

    public async Task<(List<VideoStreamConfig> videoStreamConfigs, StreamGroupProfile streamGroupProfile)> GetStreamGroupVideoConfigs(int streamGroupProfileId)
    {
        StreamGroup? streamGroup = await GetStreamGroupFromSGProfileIdAsync(streamGroupProfileId);
        if (streamGroup == null)
        {
            StreamGroupProfile d = await GetDefaultStreamGroupProfileAsync();
            return (new List<VideoStreamConfig>(), d);
        }

        List<SMChannel> smChannels = (streamGroup.Id < 2
            ? await repositoryWrapper.SMChannel.GetQuery().ToListAsync()
            : await repositoryWrapper.SMChannel.GetSMChannelsFromStreamGroup(streamGroup.Id))
            .Where(a => !a.IsHidden).ToList();

        if (smChannels.Count == 0)
        {
            StreamGroupProfile d = await GetDefaultStreamGroupProfileAsync();
            return (new List<VideoStreamConfig>(), d);
        }

        StreamGroupProfile streamGroupProfile = await GetStreamGroupProfileAsync(streamGroup.Id, streamGroupProfileId);
        CommandProfileDto commandProfile = _profileService.GetCommandProfile(streamGroupProfile.CommandProfileName);
        OutputProfileDto outputProfile = _profileService.GetOutputProfile(streamGroupProfile.OutputProfileName);

        List<VideoStreamConfig> videoStreamConfigs = [];
        _logger.LogInformation("GetStreamGroupVideoConfigsHandler: Handling {Count} channels", smChannels.Count);

        foreach (SMChannel? smChannel in smChannels.Where(a => !a.IsHidden))
        {
            SMStream? stream = smChannel.SMStreams.FirstOrDefault(a => a.Rank == 0)?.SMStream;

            videoStreamConfigs.Add(new VideoStreamConfig
            {
                Id = smChannel.Id,
                Name = smChannel.Name,
                EPGId = smChannel.EPGId,
                Logo = smChannel.Logo,
                Group = smChannel.Group,
                ChannelNumber = smChannel.ChannelNumber,
                TimeShift = smChannel.TimeShift,
                IsDuplicate = false,
                IsDummy = false,
                M3UFileId = stream?.M3UFileId ?? 0,
                FilePosition = stream?.FilePosition ?? 0,
                CommandProfile = commandProfile,
                OutputProfile = outputProfile
            });
        }
        videoStreamConfigs = [.. videoStreamConfigs.OrderBy(a => a.ChannelNumber)];

        return (videoStreamConfigs, streamGroupProfile);
    }

    public async Task<StreamGroupProfile> GetDefaultStreamGroupProfileAsync()
    {
        //using IServiceScope scope = _serviceProvider.CreateScope();
        //IRepositoryWrapper repositoryWrapper = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
        StreamGroup defaultSg = await GetDefaultSGAsync();

        StreamGroupProfile defaultPolicy = await repositoryWrapper.StreamGroupProfile.GetQuery()
            .FirstAsync(a => a.StreamGroupId == defaultSg.Id && a.ProfileName.ToLower() == "default");
        return defaultPolicy;
    }

    public async Task<StreamGroupProfile> GetStreamGroupProfileAsync(int? streamGroupId = null, int? streamGroupProfileId = null)
    {
        if (!streamGroupId.HasValue && !streamGroupProfileId.HasValue)
        {
            StreamGroupProfile defaultSG = await GetDefaultStreamGroupProfileAsync();
            return defaultSG;
        }

        //using IServiceScope scope = _serviceProvider.CreateScope();
        //IRepositoryWrapper repositoryWrapper = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
        if (streamGroupProfileId.HasValue)
        {
            StreamGroupProfile? sgProfile = repositoryWrapper.StreamGroupProfile.GetQuery()
                .FirstOrDefault(a => a.Id == streamGroupProfileId);
            if (sgProfile != null)
            {
                return sgProfile;
            }
        }

        if (streamGroupId.HasValue)
        {
            StreamGroupProfile? profile = repositoryWrapper.StreamGroupProfile.GetQuery()
                .FirstOrDefault(a => a.StreamGroupId == streamGroupId && a.ProfileName == "Default");
            if (profile != null)
            {
                return profile;
            }
        }

        StreamGroupProfile def = await GetDefaultStreamGroupProfileAsync();
        return def;
    }

    public async Task<StreamGroup?> GetStreamGroupFromNameAsync(string streamGroupName)
    {
        //using IServiceScope scope = _serviceProvider.CreateScope();
        //IRepositoryWrapper repositoryWrapper = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
        return await repositoryWrapper.StreamGroup.FirstOrDefaultAsync(a => a.Name == streamGroupName).ConfigureAwait(false);
    }

    public async Task<StreamGroup> GetDefaultSGAsync()
    {
        if (_memoryCache.TryGetValue(CacheKey, out StreamGroup? streamGroup))
        {
            if (streamGroup != null)
            {
                return streamGroup;
            }
        }

        StreamGroup sg = await GetStreamGroupFromNameAsync(DefaultStreamGroupName).ConfigureAwait(false)
            ?? throw new Exception("StreamGroup 'All' not found");

        _memoryCache.Set(CacheKey, sg, new MemoryCacheEntryOptions
        {
            Priority = CacheItemPriority.NeverRemove
        });

        return sg;
    }

    public async Task<int> GetStreamGroupIdFromSGProfileIdAsync(int? streamGroupProfileId)
    {
        if (streamGroupProfileId.HasValue)
        {
            StreamGroup? sg = await GetStreamGroupFromSGProfileIdAsync(streamGroupProfileId.Value);
            return sg?.Id ?? await GetDefaultSGIdAsync();
        }
        return await GetDefaultSGIdAsync();
    }

    public async Task<StreamGroup?> GetStreamGroupFromSGProfileIdAsync(int streamGroupProfileId)
    {
        //IRepositoryWrapper repositoryWrapper = ResolveScopedService<IRepositoryWrapper>();
        //using IServiceScope scope = _serviceProvider.CreateScope();
        //IRepositoryWrapper repositoryWrapper = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
        StreamGroupProfile? test = await repositoryWrapper.StreamGroupProfile.FirstOrDefaultAsync(a => a.Id == streamGroupProfileId);
        if (test == null)
        {
            return null;
        }
        StreamGroup? sg = await GetStreamGroupFromIdAsync(test.StreamGroupId);
        return sg;
    }

    public async Task<StreamGroup?> GetStreamGroupFromIdAsync(int streamGroupId)
    {
        //IRepositoryWrapper repositoryWrapper = ResolveScopedService<IRepositoryWrapper>();
        //using IServiceScope scope = _serviceProvider.CreateScope();
        //IRepositoryWrapper repositoryWrapper = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
        return await repositoryWrapper.StreamGroup.FirstOrDefaultAsync(a => a.Id == streamGroupId).ConfigureAwait(false);
    }
}
