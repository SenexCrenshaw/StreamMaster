using FluentValidation;

using Microsoft.AspNetCore.Http;

using StreamMaster.Domain.Crypto;

using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;

namespace StreamMaster.Application.StreamGroups.Queries;

[RequireAll]
public record GetStreamGroupM3U(int StreamGroupProfileId, bool IsShort) : IRequest<string>;

public class GetStreamGroupM3UHandler(IHttpContextAccessor httpContextAccessor,
    ICryptoService cryptoService,
    IProfileService profileService,
    IStreamGroupService streamGroupService,
    ISchedulesDirectDataService schedulesDirectDataService,
    IRepositoryWrapper Repository,
    IOptionsMonitor<Setting> intSettings
    )
    : IRequestHandler<GetStreamGroupM3U, string>
{

    public string GetIconUrl(string iconOriginalSource, Setting setting)
    {
        string url = httpContextAccessor.GetUrl();

        if (string.IsNullOrEmpty(iconOriginalSource))
        {
            iconOriginalSource = $"{url}{setting.DefaultIcon}";
            return iconOriginalSource;
        }

        string originalUrl = iconOriginalSource;

        if (iconOriginalSource.StartsWith('/'))
        {
            iconOriginalSource = iconOriginalSource[1..];
        }

        if (iconOriginalSource.StartsWith("images/"))
        {
            iconOriginalSource = $"{url}/{iconOriginalSource}";
        }
        else if (!iconOriginalSource.StartsWith("http"))
        {
            iconOriginalSource = GetApiUrl(SMFileTypes.TvLogo, originalUrl);
        }
        else if (setting.CacheIcons)
        {
            iconOriginalSource = GetApiUrl(SMFileTypes.Icon, originalUrl);
        }

        return iconOriginalSource;
    }

    private const string DefaultReturn = "#EXTM3U\r\n";
    private readonly ConcurrentDictionary<int, bool> chNos = new();
    private readonly SemaphoreSlim semaphore = new(1, 1); // Allow only one thread at a time


    [LogExecutionTimeAspect]
    public async Task<string> Handle(GetStreamGroupM3U request, CancellationToken cancellationToken)
    {
        if (httpContextAccessor.HttpContext?.Request?.Path.Value == null)
        {
            return DefaultReturn;
        }

        Setting settings = intSettings.CurrentValue;
        string url = httpContextAccessor.GetUrl();
        string requestPath = httpContextAccessor.HttpContext.Request.Path.Value!.ToString();
        //byte[]? iv = requestPath.GetIVFromPath(settings.ServerKey, 128);
        //if (iv == null)
        //{
        //    return DefaultReturn;
        //}

        StreamGroup? sg = await streamGroupService.GetStreamGroupFromSGProfileIdAsync(request.StreamGroupProfileId);
        if (sg == null)
        {
            return "";
        }

        List<SMChannel> smChannels = (await Repository.SMChannel.GetSMChannelsFromStreamGroup(sg.Id)).Where(a => !a.IsHidden).ToList();

        if (smChannels.Count == 0)
        {
            return DefaultReturn;
        }

        (List<VideoStreamConfig> videoStreamConfigs, StreamGroupProfile streamGroupProfile) = await streamGroupService.GetStreamGroupVideoConfigs(request.StreamGroupProfileId);

        if (videoStreamConfigs is null || streamGroupProfile is null)
        {
            return string.Empty;
        }

        var encodedData = await Task.WhenAll(smChannels.Select(async smChannel =>
        {
            string? encodedString = await streamGroupService.EncodeStreamGroupProfileIdChannelId(streamGroupProfile.Id, smChannel.Id).ConfigureAwait(false);

            string cleanName = smChannel.Name.ToCleanFileString();

            return new
            {
                smChannel,
                EncodedString = encodedString,
                CleanName = cleanName
            };
        }));

        OutputProfileDto outputProfile = profileService.GetOutputProfile(streamGroupProfile.OutputProfileName);

        var videoStreamData = encodedData
        .AsParallel()
        .WithDegreeOfParallelism(Environment.ProcessorCount)
        .Select((data, _) =>
        {
            (int ChNo, string m3uLine) = BuildM3ULineForVideoStream(data.smChannel, url, request, outputProfile, settings, videoStreamConfigs, data.EncodedString, data.CleanName);
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


        try
        {

            while (chNos.ContainsKey(newChNo))
            {
                newChNo++;
            }


            chNos[newChNo] = true;
        }
        finally
        {

        }

        return newChNo;
    }

    private void UpdateProfile(OutputProfile profile, SMChannel smChannel)
    {

        semaphore.Wait();

        try
        {

            UpdateProperty(profile, smChannel, p => p.Name);
            UpdateProperty(profile, smChannel, p => p.Group);
            UpdateProperty(profile, smChannel, p => p.EPGId);

            smChannel.ChannelNumber = GetNextChNo(smChannel.ChannelNumber);


        }
        finally
        {

            _ = semaphore.Release();
        }

    }

    private static void UpdateProperty<T>(OutputProfile profile, SMChannel smChannel, Expression<Func<OutputProfile, T>> propertySelector)
    {

        if (propertySelector.Body is MemberExpression memberExpression)
        {

            T? profileValue = propertySelector.Compile()(profile);


            if (Enum.TryParse<ValidM3USetting>(profileValue?.ToString(), out ValidM3USetting setting))
            {

                if (setting != ValidM3USetting.NotMapped)
                {

                    PropertyInfo? smChannelProperty = typeof(SMChannel).GetProperty(memberExpression.Member.Name);
                    if (smChannelProperty != null)
                    {

                        object? newValue = smChannelProperty.GetValue(smChannel);


                        PropertyInfo? profileProperty = typeof(OutputProfile).GetProperty(memberExpression.Member.Name);
                        profileProperty?.SetValue(profile, newValue);
                    }
                }
            }
        }
    }

    private (int ChNo, string m3uLine) BuildM3ULineForVideoStream(SMChannel smChannel, string url, GetStreamGroupM3U request, OutputProfile profile, Setting settings, List<VideoStreamConfig> videoStreamConfigs, string encodedString, string cleanName)
    {
        if (string.IsNullOrEmpty(encodedString) || string.IsNullOrEmpty(cleanName))
        {
            return (0, "");
        }

        UpdateProfile(profile, smChannel);

        string epgChannelId;

        string channelId = string.Empty;
        string tvgID = string.Empty;

        if (string.IsNullOrEmpty(smChannel.EPGId))
        {
            epgChannelId = smChannel.Group;
        }

        else
        {
            if (EPGHelper.IsValidEPGId(smChannel.EPGId))
            {
                (_, epgChannelId) = smChannel.EPGId.ExtractEPGNumberAndStationId();
                MxfService? service = schedulesDirectDataService.GetService(smChannel.EPGId);

                tvgID = service?.CallSign ?? epgChannelId;
                channelId = tvgID;

            }
            else
            {
                epgChannelId = tvgID = channelId = smChannel.EPGId;
            }
        }
        VideoStreamConfig videoStreamConfig = videoStreamConfigs.First(a => a.Id == smChannel.Id);
        if (profile.EnableChannelNumber)
        {
            channelId = videoStreamConfig.ChannelNumber.ToString();
        }

        string name = smChannel.Name;

        string logo = GetIconUrl(smChannel.Logo, settings);
        smChannel.Logo = logo;


        string videoUrl = request.IsShort
            ? $"{url}/v/{request.StreamGroupProfileId}/{smChannel.Id}"
            : $"{url}/api/videostreams/stream/{encodedString}/{cleanName}";



        List<string> fieldList = ["#EXTINF:-1"];

        if (profile.EnableId)
        {
            fieldList.Add($"CUID =\"{smChannel.Id}\"");
            fieldList.Add($"channel-id=\"{channelId}\"");
        }

        if (profile.Name != nameof(ValidM3USetting.NotMapped))
        {
            fieldList.Add($"tvg-name=\"{name}\"");
        }

        if (profile.EPGId != nameof(ValidM3USetting.NotMapped))
        {
            fieldList.Add($"tvg-id=\"{tvgID}\"");
        }

        if (profile.Group != nameof(ValidM3USetting.NotMapped))
        {
            fieldList.Add($"tvg-group=\"{profile.Group}\"");
        }

        if (profile.EnableChannelNumber)
        {
            fieldList.Add($"tvg-chno=\"{videoStreamConfig.ChannelNumber}\"");
            fieldList.Add($"channel-number=\"{videoStreamConfig.ChannelNumber}\"");
        }

        if (profile.EnableGroupTitle)
        {
            if (!string.IsNullOrEmpty(smChannel.GroupTitle))
            {
                fieldList.Add($"group-title=\"{smChannel.GroupTitle}\"");
            }
            else
            {
                fieldList.Add($"group-title=\"{profile.Group}\"");
            }
        }


        if (profile.EnableIcon)
        {
            fieldList.Add($"tvg-logo=\"{smChannel.Logo}\"");
        }

        string lines = string.Join(" ", [.. fieldList.Order()]);
        lines += $",{smChannel.Name}\r\n";
        lines += $"{videoUrl}";

        return (smChannel.ChannelNumber, lines);
    }

    private string GetApiUrl(SMFileTypes path, string source)
    {
        string url = httpContextAccessor.GetUrl();
        return $"{url}/api/files/{(int)path}/{WebUtility.UrlEncode(source)}";
    }

}