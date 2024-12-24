using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Xml.Serialization;

using Microsoft.AspNetCore.Http;

using StreamMaster.Application.Common.Models;
using StreamMaster.Domain.Crypto;

using static StreamMaster.Domain.Common.GetStreamGroupEPGHandler;

namespace StreamMaster.Application.StreamGroups;

public partial class StreamGroupService(IHttpContextAccessor httpContextAccessor, ILogoService logoService, IOptionsMonitor<Setting> settings, IOptionsMonitor<CommandProfileDict> _commandProfileSettings, ICacheManager cacheManager, IRepositoryWrapper repositoryWrapper, IProfileService profileService)
    : IStreamGroupService
{
    private readonly ConcurrentDictionary<int, bool> chNos = new();

    #region CRYPTO

    private async Task<(int? streamGroupId, string? groupKey, string? valuesEncryptedString)> GetGroupKeyFromEncodeAsync(string encodedString)
    {
        Setting settingsValue = settings.CurrentValue;
        (int? streamGroupId, string? valuesEncryptedString) = CryptoUtils.DecodeStreamGroupId(encodedString, settingsValue.ServerKey);
        if (streamGroupId == null || string.IsNullOrEmpty(valuesEncryptedString))
        {
            return (null, null, null);
        }

        StreamGroup? streamGroup = await GetStreamGroupFromIdAsync(streamGroupId.Value).ConfigureAwait(false);
        return streamGroup == null || string.IsNullOrEmpty(streamGroup.GroupKey) ? (null, null, null) : (streamGroupId, streamGroup.GroupKey, valuesEncryptedString);
    }

    private async Task<string?> GetStreamGroupKeyFromIdAsync(int streamGroupId)
    {
        if (cacheManager.StreamGroupKeyCache.TryGetValue(streamGroupId, out string? cachedGroupKey))
        {
            return cachedGroupKey;
        }

        StreamGroup? streamGroup = streamGroupId < 0
            ? await GetStreamGroupFromNameAsync(BuildInfo.DefaultStreamGroupName).ConfigureAwait(false)
            : await GetStreamGroupFromIdAsync(streamGroupId).ConfigureAwait(false);

        if (streamGroup == null || string.IsNullOrEmpty(streamGroup.GroupKey))
        {
            return null;
        }

        _ = cacheManager.StreamGroupKeyCache.TryAdd(streamGroupId, streamGroup.GroupKey);
        return streamGroup.GroupKey;
    }

    public async Task<(int? StreamGroupId, int? StreamGroupProfileId, int? SMChannelId)> DecodeProfileIdSMChannelIdFromEncodedAsync(string encodedString)
    {
        (int? streamGroupId, string? groupKey, string? valuesEncryptedString) = await GetGroupKeyFromEncodeAsync(encodedString).ConfigureAwait(false);
        if (streamGroupId == null || groupKey == null || string.IsNullOrEmpty(valuesEncryptedString))
        {
            return (null, null, null);
        }

        string decryptedTextWithGroupKey = AesEncryption.Decrypt(valuesEncryptedString, groupKey);
        string[] values = decryptedTextWithGroupKey.Split(',');
        if (values.Length == 3)
        {
            if (int.TryParse(values[1], out int streamGroupProfileId) && int.TryParse(values[2], out int smChannelId))
            {
                return (streamGroupId, streamGroupProfileId, smChannelId);
            }
        }

        return (null, null, null);
    }

    public async Task<(int? StreamGroupId, int? StreamGroupProfileId)> DecodeStreamGroupIdProfileIdFromEncodedAsync(string encodedString)
    {
        (int? streamGroupId, string? groupKey, string? valuesEncryptedString) = await GetGroupKeyFromEncodeAsync(encodedString).ConfigureAwait(false);
        if (streamGroupId == null || groupKey == null || string.IsNullOrEmpty(valuesEncryptedString))
        {
            return (null, null);
        }

        string decryptedTextWithGroupKey = AesEncryption.Decrypt(valuesEncryptedString, groupKey);
        string[] values = decryptedTextWithGroupKey.Split(',');
        if (values.Length == 2)
        {
            if (int.TryParse(values[1], out int streamGroupProfileId))
            {
                return (streamGroupId, streamGroupProfileId);
            }
        }

        return (null, null);
    }

    public async Task<string?> EncodeStreamGroupIdProfileIdChannelIdAsync(int streamGroupId, int streamGroupProfileId, int smChannelId)
    {
        string? groupKey = await GetStreamGroupKeyFromIdAsync(streamGroupId).ConfigureAwait(false);
        if (string.IsNullOrEmpty(groupKey))
        {
            return null;
        }

        Setting settingsValue = settings.CurrentValue;
        return CryptoUtils.EncodeThreeValues(streamGroupId, streamGroupProfileId, smChannelId, settingsValue.ServerKey, groupKey);
    }

    public async Task<string?> EncodeStreamGroupIdProfileIdAsync(int streamGroupId, int streamGroupProfileId)
    {
        string? groupKey = await GetStreamGroupKeyFromIdAsync(streamGroupId).ConfigureAwait(false);
        return string.IsNullOrEmpty(groupKey)
            ? null
            : CryptoUtils.EncodeTwoValues(streamGroupId, streamGroupProfileId, settings.CurrentValue.ServerKey, groupKey);
    }

    public string EncodeProfileIds(List<int> streamGroupProfileIds)
    {
        string json = JsonSerializer.Serialize(streamGroupProfileIds);
        string encryptedString = CryptoUtils.EncodeValue(json, settings.CurrentValue.ServerKey);
        return encryptedString;
    }

    public List<int>? DecodeProfileIds(string encryptedString)
    {
        string? json = CryptoUtils.DecodeValue(encryptedString, settings.CurrentValue.ServerKey);
        if (string.IsNullOrEmpty(json))
        {
            return [];
        }
        List<int>? ret = JsonSerializer.Deserialize<List<int>>(json);
        return ret;
    }

    #endregion CRYPTO

    public async Task SyncSTRMFilesAsync(CancellationToken cancellationToken)
    {
        Dictionary<string, StreamGroup> sgs = await repositoryWrapper.StreamGroup.GetQuery().ToDictionaryAsync(a => a.Name, a => a, cancellationToken: cancellationToken);
        foreach (StreamGroup? sg in sgs.Values)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            await SyncSGSTRMFilesAsync(sg, cancellationToken);
        }

        DirectoryInfo dirInfo = new(BuildInfo.OnDemandFolder);
        foreach (DirectoryInfo subDir in dirInfo.GetDirectories())
        {
            if (sgs.ContainsKey(subDir.Name))
            {
                continue;
            }
            Directory.Delete(subDir.FullName, true);
        }
    }

    public async Task SyncSGSTRMFilesAsync(StreamGroup streamGroup, CancellationToken cancellationToken)
    {
        if (streamGroup.Name.EqualsIgnoreCase("all"))
        {
            return;
        }

        ConcurrentDictionary<string, bool> strmFiles = new();

        if (streamGroup.CreateSTRM)
        {
            (List<VideoStreamConfig> videoStreamConfigs, StreamGroupProfile streamGroupProfile) = await GetStreamGroupVideoConfigsAsync(streamGroup.Id);

            ParallelOptions parallelOptions = new()
            {
                MaxDegreeOfParallelism = 8,
                CancellationToken = cancellationToken
            };

            Parallel.ForEach(videoStreamConfigs, parallelOptions, videoStreamConfig =>
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if (strmFiles.TryGetValue(videoStreamConfig.Name, out _))
                {
                    return;
                }

                string directory = GetSTRMDirectory(streamGroup, videoStreamConfig);
                if (!DirectoryHelper.EnsureDirectoryExists(directory))
                {
                    return;
                }

                WriteSTRMFile(streamGroup, videoStreamConfig);
                strmFiles.TryAdd(directory, true);
            });
        }

        if (settings.CurrentValue.DeleteOldSTRMFiles)
        {
            DeleteExtraneousDirs(streamGroup.Name, strmFiles);
        }
    }

    private static void DeleteExtraneousDirs(string StreamGroupName, ConcurrentDictionary<string, bool> validFiles)
    {
        string directory = Path.Combine(BuildInfo.OnDemandFolder, StreamGroupName);
        if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
        {
            return;
        }

        if (validFiles.IsEmpty)
        {
            Directory.Delete(directory, true);
            return;
        }

        foreach (string subDir in Directory.GetDirectories(directory))
        {
            if (!validFiles.ContainsKey(subDir))
            {
                if (!Directory.Exists(subDir))
                {
                    continue;
                }
                Directory.Delete(subDir, true);
            }
        }
    }

    private void WriteSTRMFile(StreamGroup streamGroup, VideoStreamConfig videoStreamConfig, bool IsShort = true)
    {
        //FIX
        string baseUrl = settings.CurrentValue.STRMBaseURL;
        string strmFullName = GetSTRMPath(streamGroup, videoStreamConfig);

        string videoUrl = IsShort
                 ? $"{baseUrl}/v/{videoStreamConfig.StreamGroupProfileId}/{videoStreamConfig.Id}"
                 : $"{baseUrl}/v/{videoStreamConfig.EncodedString}";

        File.WriteAllText(strmFullName, videoUrl);
    }

    private static string GetSTRMDirectory(StreamGroup streamGroup, VideoStreamConfig config)
    {
        string sanitizedStreamGroupName = SanitizeForPath(streamGroup.Name);
        string sanitizedConfigName = SanitizeForPath(config.Name);
        return Path.Combine(BuildInfo.OnDemandFolder, sanitizedStreamGroupName, sanitizedConfigName);
    }

    private static string GetSTRMPath(StreamGroup streamGroup, VideoStreamConfig config)
    {
        string sanitizedDirectory = GetSTRMDirectory(streamGroup, config);
        string sanitizedConfigName = SanitizeForPath(config.Name);
        return Path.Combine(sanitizedDirectory, sanitizedConfigName + ".strm");
    }

    /// <summary>
    /// Removes invalid characters and unwanted prefixes from a string, making it safe and formatted for use in file and directory paths.
    /// </summary>
    /// <param name="input">The input string to sanitize.</param>
    /// <returns>A sanitized and formatted string safe for use in paths.</returns>
    private static string SanitizeForPath(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("Input cannot be null or whitespace.", nameof(input));
        }

        // Step 1: Remove prefixes ending with ":".
        input = RemovePrefixes(input);

        // Step 2: Capitalize the first letter of each word.
        input = CapitalizeFirstLetter(input);

        // Step 3: Remove invalid path and file name characters.
        char[] invalidChars = [.. Path.GetInvalidFileNameChars().Concat(Path.GetInvalidPathChars()).Distinct()];
        foreach (char invalidChar in invalidChars)
        {
            input = input.Replace(invalidChar.ToString(), "_");
        }

        return input;
    }

    /// <summary>
    /// Removes prefixes from a string if they end with ":" followed by a space.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>A string with the prefixes removed.</returns>
    private static string RemovePrefixes(string input)
    {
        int colonIndex = input.IndexOf(':');
        if (colonIndex > -1 && colonIndex < input.Length - 1 && input[colonIndex + 1] == ' ')
        {
            input = input[(colonIndex + 2)..].Trim();
        }
        return input;
    }

    /// <summary>
    /// Capitalizes the first letter of each word in a string.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>A string with each word's first letter capitalized.</returns>
    private static string CapitalizeFirstLetter(string input)
    {
        return string.Join(" ", input.Split(' ')
            .Where(word => !string.IsNullOrWhiteSpace(word)) // Ignore empty words
            .Select(word => char.ToUpper(word[0]) + word[1..]));
    }

    public async Task<StreamGroup> GetDefaultSGAsync()
    {
        if (cacheManager.DefaultSG != null)
        {
            return cacheManager.DefaultSG;
        }

        cacheManager.DefaultSG = await GetStreamGroupFromNameAsync(BuildInfo.DefaultStreamGroupName).ConfigureAwait(false)
            ?? throw new Exception("StreamGroup '" + BuildInfo.DefaultStreamGroupName + "' not found");

        return cacheManager.DefaultSG;
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

    public string GetStreamGroupLineupStatus()
    {
        string jsonString = JsonSerializer.Serialize(new LineupStatus(), BuildInfo.JsonIndentOptions);

        return jsonString;
    }

    public async Task<CommandProfileDto> GetProfileFromSGIdsCommandProfileNameAsync(int? streamGroupId, int streamGroupProfileId, string commandProfileName)
    {
        // Check if the commandProfileName is not "Default" and get the corresponding profile DTO
        CommandProfileDto? commandProfileDto = !string.Equals(commandProfileName, "Default", StringComparison.InvariantCultureIgnoreCase)
            ? _commandProfileSettings.CurrentValue.GetProfileDto(commandProfileName)
            : null;

        // If commandProfileDto is null or "Default", proceed to fetch StreamGroupProfile and decide on profile DTO
        if (commandProfileDto == null || string.Equals(commandProfileDto.ProfileName, "Default", StringComparison.InvariantCultureIgnoreCase))
        {
            StreamGroupProfile streamGroupProfile = await GetStreamGroupProfileAsync(streamGroupId, streamGroupProfileId).ConfigureAwait(false);

            string profileToFetch = !string.Equals(streamGroupProfile.CommandProfileName, "Default", StringComparison.InvariantCultureIgnoreCase)
                ? streamGroupProfile.CommandProfileName
                : settings.CurrentValue.DefaultCommandProfileName;

            commandProfileDto = _commandProfileSettings.CurrentValue.GetProfileDto(profileToFetch);
        }

        return commandProfileDto;
    }

    public async Task<int> GetDefaultSGIdAsync()
    {
        StreamGroup sg = await GetDefaultSGAsync();
        return sg.Id;
    }

    public async Task<string> GetStreamGroupCapabilityAsync(int streamGroupProfileId, HttpRequest request)
    {
        StreamGroup? streamGroup = await GetStreamGroupFromSGProfileIdAsync(streamGroupProfileId).ConfigureAwait(false);
        if (streamGroup == null)
        {
            return "";
        }

        string url = request.GetUrlWithPath();
        Capability capability = new(url, $"{streamGroup.DeviceID}-{streamGroupProfileId}");

        await using Utf8StringWriter textWriter = new();
        XmlSerializer serializer = new(typeof(Capability));
        serializer.Serialize(textWriter, capability);

        return textWriter.ToString();
    }

    public async Task<string> GetStreamGroupDiscoverAsync(int streamGroupProfileId, HttpRequest request)
    {
        string url = request.GetUrlWithPath();
        int maxTuners = await repositoryWrapper.M3UFile.GetM3UMaxStreamCount();

        StreamGroup? streamGroup = await GetStreamGroupFromSGProfileIdAsync(streamGroupProfileId).ConfigureAwait(false);
        if (streamGroup == null)
        {
            return "";
        }

        Discover discover = new(url, streamGroup.DeviceID + "-" + streamGroupProfileId, maxTuners);

        return JsonSerializer.Serialize(discover, BuildInfo.JsonIndentOptions);
    }

    public async Task<string> GetStreamGroupLineupAsync(int streamGroupProfileId, bool IsShort)
    {
        //string url = httpRequest.GetUrl();
        StreamGroup? streamGroup = await GetStreamGroupFromSGProfileIdAsync(streamGroupProfileId).ConfigureAwait(false);
        if (streamGroup == null)
        {
            return JsonSerializer.Serialize(new List<SGLineup>());
        }

        (List<VideoStreamConfig> videoStreamConfigs, StreamGroupProfile streamGroupProfile) = await GetStreamGroupVideoConfigsAsync(streamGroupProfileId).ConfigureAwait(false);
        if (videoStreamConfigs == null || streamGroupProfile == null)
        {
            return string.Empty;
        }

        var baseUrl = string.IsNullOrEmpty(BuildInfo.PATH_BASE) 
            ? httpContextAccessor.GetUrl()
            : httpContextAccessor.GetUrl() + BuildInfo.PATH_BASE;
        ConcurrentBag<SGLineup> ret = [];
        _ = Parallel.ForEach(videoStreamConfigs, (videoStreamConfig, _) =>
        {
            string videoUrl = IsShort
     ? $"{videoStreamConfig.BaseUrl}/v/{videoStreamConfig.StreamGroupProfileId}/{videoStreamConfig.Id}"
     : $"{videoStreamConfig.BaseUrl}/v/{videoStreamConfig.EncodedString}";

            if (videoStreamConfig != null)
            {
                ret.Add(new SGLineup
                {
                    GuideName = videoStreamConfig.Name,
                    GuideNumber = videoStreamConfig.ChannelNumber.ToString(),
                    Station = videoStreamConfig.ChannelNumber.ToString(),
                    Logo = videoStreamConfig.Logo,
                    URL = videoUrl
                });
            }
        });

        return JsonSerializer.Serialize(ret.OrderBy(a => a.GuideNumber));
    }

    private static void UpdateProperty<T>(VideoStreamConfig videoStreamConfig, Expression<Func<OutputProfile, T>> propertySelector)
    {
        if (videoStreamConfig.OutputProfile is null)
        {
            return;
        }

        if (propertySelector.Body is MemberExpression memberExpression)
        {
            T? profileValue = propertySelector.Compile()(videoStreamConfig.OutputProfile);

            if (Enum.TryParse<ValidM3USetting>(profileValue?.ToString(), out ValidM3USetting setting))
            {
                if (setting != ValidM3USetting.NotMapped)
                {
                    PropertyInfo? smChannelProperty = typeof(VideoStreamConfig).GetProperty(setting.ToString());
                    if (smChannelProperty != null)
                    {
                        object? newValue = smChannelProperty.GetValue(videoStreamConfig);
                        if (newValue != null)
                        {
                            PropertyInfo? profileProperty = typeof(OutputProfile).GetProperty(memberExpression.Member.Name);
                            profileProperty?.SetValue(videoStreamConfig.OutputProfile, newValue.ToString());
                        }
                    }
                }
            }
        }
    }

    public async Task<StreamGroupProfile> GetStreamGroupProfileAsync(int? streamGroupId = null, int? streamGroupProfileId = null)
    {
        if (!streamGroupId.HasValue && !streamGroupProfileId.HasValue)
        {
            return await GetDefaultStreamGroupProfileAsync().ConfigureAwait(false);
        }

        if (streamGroupProfileId.HasValue)
        {
            StreamGroupProfile? sgProfile = await repositoryWrapper.StreamGroupProfile.GetQuery()
                .FirstOrDefaultAsync(a => a.Id == streamGroupProfileId).ConfigureAwait(false);
            if (sgProfile != null)
            {
                return sgProfile;
            }
        }

        if (streamGroupId.HasValue)
        {
            StreamGroupProfile? profile = await repositoryWrapper.StreamGroupProfile.GetQuery()
                .FirstOrDefaultAsync(a => a.StreamGroupId == streamGroupId && a.ProfileName == "Default").ConfigureAwait(false);
            if (profile != null)
            {
                return profile;
            }
        }

        return await GetDefaultStreamGroupProfileAsync().ConfigureAwait(false);
    }

    public string EncodeStreamGroupIdProfileIdChannelId(StreamGroup streamGroup, int streamGroupProfileId, int smChannelId)
    {
        Setting settingsValue = settings.CurrentValue;
        return CryptoUtils.EncodeThreeValues(streamGroup.Id, streamGroupProfileId, smChannelId, settingsValue.ServerKey, streamGroup.GroupKey);
    }

    //private string GetLogoUrl(string baseUrl, SMChannel smChannel)
    //{
    //    return settings.CurrentValue.LogoCache || !smChannel.ChannelLogo.StartsWithIgnoreCase("http")
    //        ? $"{baseUrl}/api/files/sm/{smChannel.Id}"
    //        : smChannel.ChannelLogo;
    //}

    public async Task<(List<VideoStreamConfig> videoStreamConfigs, StreamGroupProfile streamGroupProfile)> GetStreamGroupVideoConfigsAsync(int streamGroupProfileId)
    {
        chNos.Clear();

        StreamGroup? streamGroup = await GetStreamGroupFromSGProfileIdAsync(streamGroupProfileId).ConfigureAwait(false);
        if (streamGroup == null)
        {
            StreamGroupProfile defaultProfile = await GetDefaultStreamGroupProfileAsync().ConfigureAwait(false);
            return (new List<VideoStreamConfig>(), defaultProfile);
        }

        int defaultSGID = await GetDefaultSGIdAsync().ConfigureAwait(false);
        List<SMChannel> smChannels = [.. (streamGroup.Id == defaultSGID
            ? await repositoryWrapper.SMChannel.GetQuery().ToListAsync().ConfigureAwait(false)
            : await repositoryWrapper.SMChannel.GetSMChannelsFromStreamGroup(streamGroup.Id).ConfigureAwait(false))
            .Where(a => !a.IsHidden)];

        if (smChannels.Count == 0)
        {
            StreamGroupProfile defaultProfile = await GetDefaultStreamGroupProfileAsync().ConfigureAwait(false);
            return (new List<VideoStreamConfig>(), defaultProfile);
        }

        StreamGroupProfile streamGroupProfile = await GetStreamGroupProfileAsync(streamGroup.Id, streamGroupProfileId).ConfigureAwait(false);
        CommandProfileDto commandProfile = profileService.GetCommandProfile(streamGroupProfile.CommandProfileName);
        OutputProfileDto originalOutputProfile = profileService.GetOutputProfile(streamGroupProfile.OutputProfileName);

        string baseUrl = httpContextAccessor.GetUrl();

        List<VideoStreamConfig> videoStreamConfigs = [];

        foreach (SMChannel smChannel in smChannels)
        {
            //SMStream? stream = smChannel.SMStreams.FirstOrDefault(a => a.Rank == 0)?.SMStream;
            string? encodedString = EncodeStreamGroupIdProfileIdChannelId(streamGroup, streamGroupProfile.Id, smChannel.Id);
            if (string.IsNullOrEmpty(encodedString))
            {
                continue;
            }
            OutputProfileDto outputProfile = originalOutputProfile.DeepCopy();

            string cleanName = smChannel.Name.ToCleanFileString();
            string logo = logoService.GetLogoUrl(smChannel, baseUrl);
            string epgId = string.IsNullOrEmpty(smChannel.EPGId) ? EPGHelper.DummyId + "-Dummy" : smChannel.EPGId;

            StationChannelName? match = cacheManager.StationChannelNames
                .SelectMany(kvp => kvp.Value)
                .FirstOrDefault(stationchannel => stationchannel.Id == smChannel.EPGId || stationchannel.Channel == smChannel.EPGId);

            if (match is not null && epgId != match.Id)
            {
                epgId = match.Id;
            }

            (int epgNumber, string stationId) = epgId.ExtractEPGNumberAndStationId();

            VideoStreamConfig videoStreamConfig = new()
            {
                Id = smChannel.Id,
                BaseUrl = baseUrl,
                Name = smChannel.Name,
                CleanName = cleanName,
                EncodedString = encodedString,
                EPGNumber = epgNumber,
                StreamGroupProfileId = streamGroupProfile.Id,
                EPGId = stationId,
                Logo = logo,
                Group = smChannel.Group,
                ChannelNumber = smChannel.ChannelNumber,
                TimeShift = smChannel.TimeShift,
                IsDuplicate = false,
                M3UFileId = epgNumber,
                //FilePosition = stream?.FilePosition ?? dummyPosition++,
                StationId = smChannel.StationId,
                GroupTitle = smChannel.GroupTitle,
                CommandProfile = commandProfile,
                OutputProfile = outputProfile
            };

            videoStreamConfig.ChannelNumber = GetNextChNo(videoStreamConfig.ChannelNumber);

            UpdateProperty(videoStreamConfig, p => p.Name);
            UpdateProperty(videoStreamConfig, p => p.Group);
            UpdateProperty(videoStreamConfig, p => p.Id);

            videoStreamConfigs.Add(videoStreamConfig);
        }
        return (videoStreamConfigs.OrderBy(a => a.ChannelNumber).ToList(), streamGroupProfile);
    }

    private int GetNextChNo(int baseChNo)
    {
        int newChNo = baseChNo;

        while (chNos.ContainsKey(newChNo))
        {
            newChNo++;
        }

        chNos[newChNo] = true;

        return newChNo;
    }

    public async Task<StreamGroup?> GetStreamGroupFromIdAsync(int streamGroupId)
    {
        return await repositoryWrapper.StreamGroup.FirstOrDefaultAsync(a => a.Id == streamGroupId).ConfigureAwait(false);
    }

    public async Task<StreamGroup?> GetStreamGroupFromNameAsync(string streamGroupName)
    {
        return await repositoryWrapper.StreamGroup.FirstOrDefaultAsync(a => a.Name == streamGroupName).ConfigureAwait(false);
    }

    public async Task<bool> StreamGroupExistsAsync(int streamGroupProfileId)
    {
        // Check if the StreamGroupProfile exists and has a valid StreamGroupId.
        StreamGroupProfile? streamGroupProfile = await repositoryWrapper.StreamGroupProfile
            .FirstOrDefaultAsync(a => a.Id == streamGroupProfileId)
            .ConfigureAwait(false);

        if (streamGroupProfile == null)
        {
            return false; // StreamGroupProfile does not exist
        }

        // Check if the StreamGroup associated with the StreamGroupId exists
        return await repositoryWrapper.StreamGroup.GetQuery(tracking: false).AnyAsync(sg => sg.Id == streamGroupProfile.StreamGroupId)
            .ConfigureAwait(false);
    }

    public async Task<StreamGroup?> GetStreamGroupFromSGProfileIdAsync(int streamGroupProfileId)
    {
        StreamGroupProfile? streamGroupProfile = await repositoryWrapper.StreamGroupProfile.FirstOrDefaultAsync(a => a.Id == streamGroupProfileId).ConfigureAwait(false);
        return streamGroupProfile == null ? null : await GetStreamGroupFromIdAsync(streamGroupProfile.StreamGroupId).ConfigureAwait(false);
    }

    public async Task<StreamGroupProfile> GetDefaultStreamGroupProfileAsync()
    {
        StreamGroup? defaultStreamGroup = await GetStreamGroupFromNameAsync(BuildInfo.DefaultStreamGroupName).ConfigureAwait(false);
        return defaultStreamGroup == null
            ? throw new Exception("Default stream group not found.")
            : await repositoryWrapper.StreamGroupProfile.GetQuery()
            .FirstOrDefaultAsync(a => a.StreamGroupId == defaultStreamGroup.Id && a.ProfileName == "Default").ConfigureAwait(false) ?? throw new Exception("Default stream group not found.");
    }

    public async Task<Dictionary<int, SGFS>> GetSMFS(List<int> sgProfileIds, bool isShort, CancellationToken cancellationToken)
    {
        ConcurrentDictionary<int, SGFS> sgFiles = [];

        if (sgProfileIds.Contains(0))
        {
            int defaultSGId = await GetDefaultSGIdAsync();
            sgProfileIds = await repositoryWrapper.StreamGroupProfile.GetQuery().Where(a => a.Id != defaultSGId).Select(a => a.Id).ToListAsync(cancellationToken: cancellationToken);
        }

        foreach (int sgProfileId in sgProfileIds)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return sgFiles.ToDictionary();
            }

            (List<VideoStreamConfig> videoStreamConfigs, StreamGroupProfile streamGroupProfile) = await GetStreamGroupVideoConfigsAsync(sgProfileId);

            StreamGroupDto? streamGroup = await repositoryWrapper.StreamGroup.GetStreamGroupByIdAsync(streamGroupProfile.StreamGroupId);
            if (streamGroup is null)
            {
                continue;
            }

            foreach (VideoStreamConfig videoStreamConfig in videoStreamConfigs)
            {
                string videoUrl = isShort
                    ? $"{videoStreamConfig.BaseUrl}/v/{videoStreamConfig.StreamGroupProfileId}/{videoStreamConfig.Id}"
                    : $"{videoStreamConfig.BaseUrl}/v/{videoStreamConfig.EncodedString}";

                if (sgFiles.TryGetValue(sgProfileId, out SGFS? sgFS))
                {
                    sgFS.SMFS.Add(new SMFile(CleanUpName(videoStreamConfig.Name), videoUrl));
                }
                else
                {
                    sgFiles.TryAdd(sgProfileId, new SGFS(CleanUpName(streamGroup.Name), !isShort ? streamGroup.HDHRLink : streamGroup.ShortHDHRLink, [new SMFile(CleanUpName(videoStreamConfig.Name), videoUrl)]));
                }
                if (cancellationToken.IsCancellationRequested)
                {
                    return sgFiles.ToDictionary();
                }
            }
        }
        return sgFiles.ToDictionary();
    }
    private static string CleanUpName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return name; // Return as-is if null or empty
        }

        // Remove everything up to and including the first colon if it exists
        int colonIndex = name.IndexOf(':');
        if (colonIndex >= 0)
        {
            name = name[(colonIndex + 1)..];
        }

        // Remove the "24x7" prefix if it exists (case-insensitive)
        string[] prefixesToRemove = ["24x7", "24/7"];
        // Remove any matching prefix
        foreach (string prefix in prefixesToRemove)
        {
            if (name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                name = name[prefix.Length..].Trim();
                break; // Exit loop after removing a prefix
            }
        }

        // Return the cleaned-up name
        return name;
    }
}