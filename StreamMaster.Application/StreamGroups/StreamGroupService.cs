using Microsoft.Extensions.DependencyInjection;

namespace StreamMaster.Application.StreamGroups;

public class StreamGroupService(ILogger<StreamGroupService> logger, IMemoryCache _memoryCache, IProfileService profileService, IServiceProvider _serviceProvider)
    : IStreamGroupService
{
    private const string DefaultStreamGroupName = "all";
    private const string CacheKey = "DefaultStreamGroup";
    public async Task<int> GetDefaultSGIdAsync()
    {
        StreamGroup sg = await GetDefaultSGAsync();
        return sg.Id;
    }

    public async Task<(List<VideoStreamConfig> videoStreamConfigs, StreamGroupProfile streamGroupProfile)> GetStreamGroupVideoConfigs(int StreamGroupId, int StreamGroupProfileId)
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
        IRepositoryWrapper repositoryWrapper = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();

        StreamGroup? streamGroup = repositoryWrapper.StreamGroup.GetStreamGroup(StreamGroupId);
        if (streamGroup == null)
        {
            StreamGroupProfile d = await GetDefaultStreamGroupProfileAsync();
            return ([], d);
        }

        List<SMChannel> smChannels = (StreamGroupId < 2 ?
             await repositoryWrapper.SMChannel.GetQuery().ToListAsync() :
             await repositoryWrapper.SMChannel.GetSMChannelsFromStreamGroup(StreamGroupId)).Where(a => !a.IsHidden).ToList();

        if (smChannels.Count == 0)
        {
            StreamGroupProfile d = await GetDefaultStreamGroupProfileAsync();
            return ([], d);
        }

        StreamGroupProfile StreamGroupProfile = await GetStreamGroupProfileAsync(StreamGroupId, StreamGroupProfileId);

        CommandProfileDto commandProfile = profileService.GetCommandProfile(StreamGroupProfile.CommandProfileName);
        OutputProfileDto outputProfile = profileService.GetOutputProfile(StreamGroupProfile.OutputProfileName);

        List<VideoStreamConfig> videoStreamConfigs = [];

        logger.LogInformation("GetStreamGroupVideoConfigsHandler: Handling {Count} channels", smChannels.Count);

        foreach (SMChannel? smChannel in smChannels.Where(a => !a.IsHidden))
        {
            SMStream? stream = smChannel.SMStreams.FirstOrDefault(a => a.Rank == 0)?.SMStream;

            videoStreamConfigs.Add(new VideoStreamConfig
            {
                Id = smChannel.Id,
                Name = smChannel.Name,
                EPGId = smChannel.EPGId,
                Logo = smChannel.Logo,
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

        return (videoStreamConfigs, StreamGroupProfile);
    }
    public async Task<StreamGroupProfile> GetDefaultStreamGroupProfileAsync()
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
        IRepositoryWrapper repositoryWrapper = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();

        StreamGroup defaultSg = await GetDefaultSGAsync();


        StreamGroupProfile defaultPolicy = await repositoryWrapper.StreamGroupProfile.GetQuery().FirstAsync(a => a.StreamGroupId == defaultSg.Id && a.ProfileName.ToLower() == "default");
        return defaultPolicy;
    }

    public async Task<StreamGroupProfile> GetStreamGroupProfileAsync(int? StreamGroupId = null, int? StreamGroupProfileId = null)
    {
        if (!StreamGroupId.HasValue && !StreamGroupProfileId.HasValue)
        {
            StreamGroupProfile defaultSG = await GetDefaultStreamGroupProfileAsync();
            return defaultSG;
        }

        using IServiceScope scope = _serviceProvider.CreateScope();
        IRepositoryWrapper repositoryWrapper = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
        if (StreamGroupProfileId.HasValue)
        {
            StreamGroupProfile? profile = repositoryWrapper.StreamGroupProfile.GetQuery().FirstOrDefault(a => a.Id == StreamGroupProfileId);
            if (profile != null)
            {
                return profile;
            }
        }

        StreamGroupProfile? sgProfile = repositoryWrapper.StreamGroupProfile.GetQuery().FirstOrDefault(a => a.StreamGroupId == StreamGroupId && a.ProfileName == "Default");
        if (sgProfile != null)
        {
            return sgProfile;
        }

        StreamGroupProfile def = await GetDefaultStreamGroupProfileAsync();
        return def;
    }

    public async Task<StreamGroup?> GetStreamGroupFromIdAsync(int streamGroupId)
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
        IRepositoryWrapper repositoryWrapper = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
        return await repositoryWrapper.StreamGroup.FirstOrDefaultAsync(a => a.Id == streamGroupId).ConfigureAwait(false);
    }

    public async Task<StreamGroup?> GetStreamGroupFromNameAsync(string streamGroupName)
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
        IRepositoryWrapper repositoryWrapper = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
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

        StreamGroup sg = await GetStreamGroupFromNameAsync(DefaultStreamGroupName).ConfigureAwait(false) ?? throw new Exception("StreamGroup 'All' not found");

        _memoryCache.Set(CacheKey, sg, new MemoryCacheEntryOptions
        {
            Priority = CacheItemPriority.NeverRemove
        });

        return sg;
    }
}
