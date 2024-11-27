using System.Text;

namespace StreamMaster.Application.StreamGroups.Queries;

[RequireAll]
public record GetStreamGroupM3U(int StreamGroupProfileId, bool IsShort) : IRequest<string>;

public class EncodedData
{
    public SMChannel SMChannel { get; set; } = new();
    public string? EncodedString { get; set; }
    public string CleanName { get; set; } = string.Empty;
}

public class GetStreamGroupM3UHandler(IStreamGroupService streamGroupService, IOptionsMonitor<Setting> _settings)
    : IRequestHandler<GetStreamGroupM3U, string>
{
    //private const string DefaultReturn = "#EXTM3U\r\n";

    [LogExecutionTimeAspect]
    public async Task<string> Handle(GetStreamGroupM3U request, CancellationToken cancellationToken)
    {
        Setting settings = _settings.CurrentValue;

        StreamGroup? streamGroup = await streamGroupService.GetStreamGroupFromSGProfileIdAsync(request.StreamGroupProfileId);
        if (streamGroup == null)
        {
            return "";
        }

        (List<VideoStreamConfig> videoStreamConfigs, StreamGroupProfile streamGroupProfile) = await streamGroupService.GetStreamGroupVideoConfigsAsync(request.StreamGroupProfileId);

        if (videoStreamConfigs is null || streamGroupProfile is null)
        {
            return string.Empty;
        }

        //OutputProfileDto outputProfile = profileService.GetOutputProfile(streamGroupProfile.OutputProfileName);

        var videoStreamData = videoStreamConfigs
            .AsParallel()
            .WithDegreeOfParallelism(Environment.ProcessorCount)
            .Select(videoStreamConfig =>
            {
                (int ChNo, string m3uLine) = BuildM3ULineForVideoStream(videoStreamConfig, request.IsShort);
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

    private static (int ChNo, string m3uLine) BuildM3ULineForVideoStream(VideoStreamConfig videoStreamConfig, bool IsShort)
    {
        if (videoStreamConfig.OutputProfile is null || string.IsNullOrEmpty(videoStreamConfig.EncodedString) || string.IsNullOrEmpty(videoStreamConfig.CleanName))
        {
            return (0, "");
        }

        //string logo = logoService.GetLogoUrl(videoStreamConfig.Logo, videoStreamConfig.BaseUrl, SMStreamTypeEnum.Regular);
        //videoStreamConfig.Logo = logo;

        string videoUrl = IsShort
            ? $"{videoStreamConfig.BaseUrl}/v/{videoStreamConfig.StreamGroupProfileId}/{videoStreamConfig.Id}"
            : $"{videoStreamConfig.BaseUrl}/api/videostreams/stream/{videoStreamConfig.EncodedString}/{videoStreamConfig.CleanName}";

        List<string> fieldList = ["#EXTINF:-1"];

        if (videoStreamConfig.OutputProfile.Id != nameof(ValidM3USetting.NotMapped))
        {
            fieldList.Add($"CUID=\"{videoStreamConfig.OutputProfile.Id}\"");
            fieldList.Add($"channel-id=\"{videoStreamConfig.OutputProfile.Id}\"");
            fieldList.Add($"tvg-id=\"{videoStreamConfig.OutputProfile.Id}\"");
        }

        if (videoStreamConfig.OutputProfile.Name != nameof(ValidM3USetting.NotMapped))
        {
            fieldList.Add($"tvg-name=\"{videoStreamConfig.OutputProfile.Name}\"");
        }

        if (!string.IsNullOrEmpty(videoStreamConfig.StationId))
        {
            fieldList.Add($"tvc-guide-stationid=\"{videoStreamConfig.StationId}\"");
        }

        if (videoStreamConfig.OutputProfile.Group != nameof(ValidM3USetting.NotMapped))
        {
            fieldList.Add($"tvg-group=\"{videoStreamConfig.Group}\"");
        }

        if (videoStreamConfig.OutputProfile.EnableChannelNumber)
        {
            fieldList.Add($"tvg-chno=\"{videoStreamConfig.ChannelNumber}\"");
            fieldList.Add($"channel-number=\"{videoStreamConfig.ChannelNumber}\"");
        }

        if (videoStreamConfig.OutputProfile.EnableGroupTitle)
        {
            if (!string.IsNullOrEmpty(videoStreamConfig.GroupTitle))
            {
                fieldList.Add($"group-title=\"{videoStreamConfig.GroupTitle}\"");
            }
            else
            {
                fieldList.Add($"group-title=\"{videoStreamConfig.OutputProfile.Group}\"");
            }
        }

        if (videoStreamConfig.OutputProfile.EnableIcon)
        {
            fieldList.Add($"tvg-logo=\"{videoStreamConfig.Logo}\"");
        }

        string lines = string.Join(" ", [.. fieldList.Order()]);
        lines += $",{videoStreamConfig.Name}\r\n";
        lines += $"{videoUrl}";

        return (videoStreamConfig.ChannelNumber, lines);
    }
}