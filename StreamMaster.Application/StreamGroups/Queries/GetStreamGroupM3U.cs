using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using Microsoft.AspNetCore.Http;

namespace StreamMaster.Application.StreamGroups.Queries;

[RequireAll]
public record GetStreamGroupM3U(int StreamGroupProfileId, bool IsShort) : IRequest<string>;

public class EncodedData
{
    public SMChannel SMChannel { get; set; } = new();
    public string? EncodedString { get; set; }
    public string CleanName { get; set; } = string.Empty;
}

public class GetStreamGroupM3UHandler(IHttpContextAccessor httpContextAccessor,
    IProfileService profileService,
    IStreamGroupService streamGroupService,
    ILogoService logoService,
    IOptionsMonitor<Setting> _settings
    )
    : IRequestHandler<GetStreamGroupM3U, string>
{
    private const string DefaultReturn = "#EXTM3U\r\n";
    private readonly ConcurrentDictionary<int, bool> chNos = new();


    [LogExecutionTimeAspect]
    public async Task<string> Handle(GetStreamGroupM3U request, CancellationToken cancellationToken)
    {
        if (httpContextAccessor.HttpContext?.Request?.Path.Value == null)
        {
            return DefaultReturn;
        }

        Setting settings = _settings.CurrentValue;
        string url = httpContextAccessor.GetUrl();

        StreamGroup? streamGroup = await streamGroupService.GetStreamGroupFromSGProfileIdAsync(request.StreamGroupProfileId);
        if (streamGroup == null)
        {
            return "";
        }

        //List<SMChannel> smChannels = (await Repository.SMChannel.GetSMChannelsFromStreamGroup(streamGroup.Id)).Where(a => !a.IsHidden).ToList();

        //if (smChannels.Count == 0)
        //{
        //    return DefaultReturn;
        //}

        (List<VideoStreamConfig> videoStreamConfigs, StreamGroupProfile streamGroupProfile) = await streamGroupService.GetStreamGroupVideoConfigsAsync(request.StreamGroupProfileId);

        if (videoStreamConfigs is null || streamGroupProfile is null)
        {
            return string.Empty;
        }

        OutputProfileDto outputProfile = profileService.GetOutputProfile(streamGroupProfile.OutputProfileName);

        //ConcurrentBag<EncodedData> encodedData = [];

        //Parallel.ForEach(smChannels, smChannel =>
        //{
        //    string? encodedString = streamGroupService.EncodeStreamGroupIdProfileIdChannelId(streamGroup, streamGroupProfile.Id, smChannel.Id);
        //    if (string.IsNullOrEmpty(encodedString))
        //    {
        //        return;
        //    }
        //    string cleanName = smChannel.Name.ToCleanFileString();

        //    EncodedData data = new()
        //    {
        //        SMChannel = smChannel,
        //        EncodedString = encodedString,
        //        CleanName = cleanName
        //    };

        //    encodedData.Add(data);
        //});

        var videoStreamData = videoStreamConfigs
            .AsParallel()
            .WithDegreeOfParallelism(Environment.ProcessorCount)
            .Select(videoStreamConfig =>
            {
                (int ChNo, string m3uLine) = BuildM3ULineForVideoStream(videoStreamConfig);
                return new
                {
                    ChNo,
                    m3uLine
                };
            })
            .ToList();

        StringBuilder ret = new("#EXTM3U\r\n");
        foreach (var data in videoStreamData.OrderBy(a => a.ChNo))
        {
            if (!string.IsNullOrEmpty(data.m3uLine))
            {
                _ = ret.AppendLine(data.m3uLine);
            }
        }

        return ret.ToString();
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

    private OutputProfile UpdateProfile(VideoStreamConfig videoStreamConfig)
    {
        //semaphore.Wait();

        try
        {
            OutputProfile outputProfile = videoStreamConfig.OutputProfile!.DeepCopy();

            videoStreamConfig.ChannelNumber = GetNextChNo(videoStreamConfig.ChannelNumber);

            UpdateProperty(videoStreamConfig, outputProfile, p => p.Name);
            UpdateProperty(videoStreamConfig, outputProfile, p => p.Group);
            UpdateProperty(videoStreamConfig, outputProfile, p => p.Id);

            return outputProfile;
        }
        finally
        {
            //_ = semaphore.Release();
        }
    }

    private static void UpdateProperty<T>(VideoStreamConfig videoStreamConfig, OutputProfile outputProfile, Expression<Func<OutputProfile, T>> propertySelector)
    {
        if (propertySelector.Body is MemberExpression memberExpression)
        {
            T? profileValue = propertySelector.Compile()(outputProfile);

            if (Enum.TryParse<ValidM3USetting>(profileValue?.ToString(), out ValidM3USetting setting))
            {
                if (setting != ValidM3USetting.NotMapped)
                {
                    PropertyInfo? smChannelProperty = typeof(SMChannel).GetProperty(setting.ToString());
                    if (smChannelProperty != null)
                    {
                        object? newValue = smChannelProperty.GetValue(outputProfile);
                        if (newValue != null)
                        {
                            PropertyInfo? profileProperty = typeof(OutputProfile).GetProperty(memberExpression.Member.Name);
                            profileProperty?.SetValue(outputProfile, newValue.ToString());
                        }
                    }
                }
            }
        }
    }

    private (int ChNo, string m3uLine) BuildM3ULineForVideoStream(VideoStreamConfig videoStreamConfig)
    {
        if (string.IsNullOrEmpty(videoStreamConfig.EncodedString) || string.IsNullOrEmpty(videoStreamConfig.CleanName))
        {
            return (0, "");
        }

        OutputProfile outputProfile = UpdateProfile(videoStreamConfig);

        string logo = logoService.GetLogoUrl(videoStreamConfig.Logo, videoStreamConfig.BaseUrl);
        videoStreamConfig.Logo = logo;

        string videoUrl = videoStreamConfig.IsShort
            ? $"{videoStreamConfig.BaseUrl}/v/{videoStreamConfig.StreamGroupProfileId}/{videoStreamConfig.Id}"
            : $"{videoStreamConfig.BaseUrl}/api/videostreams/stream/{videoStreamConfig.EncodedString}/{videoStreamConfig.CleanName}";

        List<string> fieldList = ["#EXTINF:-1"];

        if (outputProfile.Id != nameof(ValidM3USetting.NotMapped))
        {
            fieldList.Add($"CUID=\"{outputProfile.Id}\"");
            fieldList.Add($"channel-id=\"{outputProfile.Id}\"");
            fieldList.Add($"tvg-id=\"{outputProfile.Id}\"");
        }

        if (outputProfile.Name != nameof(ValidM3USetting.NotMapped))
        {
            fieldList.Add($"tvg-name=\"{outputProfile.Name}\"");
        }

        if (!string.IsNullOrEmpty(videoStreamConfig.StationId))
        {
            fieldList.Add($"tvc-guide-stationid=\"{videoStreamConfig.StationId}\"");
        }

        if (outputProfile.Group != nameof(ValidM3USetting.NotMapped))
        {
            fieldList.Add($"tvg-group=\"{videoStreamConfig.Group}\"");
        }

        if (outputProfile.EnableChannelNumber)
        {
            fieldList.Add($"tvg-chno=\"{videoStreamConfig.ChannelNumber}\"");
            fieldList.Add($"channel-number=\"{videoStreamConfig.ChannelNumber}\"");
        }

        if (outputProfile.EnableGroupTitle)
        {
            if (!string.IsNullOrEmpty(videoStreamConfig.GroupTitle))
            {
                fieldList.Add($"group-title=\"{videoStreamConfig.GroupTitle}\"");
            }
            else
            {
                fieldList.Add($"group-title=\"{outputProfile.Group}\"");
            }
        }

        if (outputProfile.EnableIcon)
        {
            fieldList.Add($"tvg-logo=\"{videoStreamConfig.Logo}\"");
        }

        string lines = string.Join(" ", [.. fieldList.Order()]);
        lines += $",{videoStreamConfig.Name}\r\n";
        lines += $"{videoUrl}";

        return (videoStreamConfig.ChannelNumber, lines);
    }
}