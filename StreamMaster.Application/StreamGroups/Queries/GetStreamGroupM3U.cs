using FluentValidation;

using Microsoft.AspNetCore.Http;

using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;

namespace StreamMaster.Application.StreamGroups.Queries;

[RequireAll]
public record GetStreamGroupM3U(int StreamGroupId, int? StreamGroupProfileId) : IRequest<string>;

public class GetStreamGroupM3UValidator : AbstractValidator<GetStreamGroupM3U>
{
    public GetStreamGroupM3UValidator()
    {
        _ = RuleFor(v => v.StreamGroupId)
            .NotNull().GreaterThanOrEqualTo(0);
    }
}

public class GetStreamGroupM3UHandler(IHttpContextAccessor httpContextAccessor,
    ISchedulesDirectDataService schedulesDirectDataService,
    ISender sender,
    IRepositoryWrapper Repository,
    IOptionsMonitor<Setting> intSettings,
    IOptionsMonitor<HLSSettings> inthlssettings
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
    private byte[] iv = [];

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
        byte[]? iv = requestPath.GetIVFromPath(settings.ServerKey, 128);
        if (iv == null)
        {
            return DefaultReturn;
        }
        this.iv = iv;

        List<SMChannel> smChannels = (await Repository.SMChannel.GetSMChannelsFromStreamGroup(request.StreamGroupId)).Where(a => !a.IsHidden).ToList();

        if (smChannels.Count == 0)
        {
            return DefaultReturn;
        }

        StreamGroupProfileDto sgProfile;
        int? sgProfileId = request.StreamGroupProfileId;
        if (sgProfileId.HasValue)
        {
            sgProfile = await Repository.StreamGroupProfile.GetDefaultStreamGroupProfile(sgProfileId.Value);
        }
        else
        {
            sgProfile = await Repository.StreamGroupProfile.GetDefaultStreamGroupProfile(request.StreamGroupId);
            sgProfileId = sgProfile.Id;
        }


        (List<VideoStreamConfig> videoStreamConfigs, OutputProfile profile) = await sender.Send(new GetStreamGroupVideoConfigs(request.StreamGroupId, sgProfileId.Value), cancellationToken);


        // Retrieve necessary data in parallel
        var videoStreamData = smChannels
 .AsParallel()
 .WithDegreeOfParallelism(Environment.ProcessorCount)
 .Select((smChannel, index) =>
 {
     (int ChNo, string m3uLine) = BuildM3ULineForVideoStream(smChannel, url, request, profile, sgProfileId.Value, index, settings, videoStreamConfigs);
     return new
     {
         ChNo,
         m3uLine
     };
 }).ToList();


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

        // Wait for the semaphore
        //semaphore.Wait();

        try
        {
            // Find the next available unique channel number starting from baseChNo
            while (chNos.ContainsKey(newChNo))
            {
                newChNo++; // Increment to find the next available number
            }

            // Add the new channel number to the collection
            chNos[newChNo] = true;
        }
        finally
        {
            // Release the semaphore
            //semaphore.Release();
        }

        return newChNo;
    }

    private void UpdateProfile(OutputProfile profile, SMChannel smChannel)
    {
        // Wait for the semaphore
        semaphore.Wait();

        try
        {
            // Use the generic method to update each property
            UpdateProperty(profile, smChannel, p => p.Name);
            UpdateProperty(profile, smChannel, p => p.Group);
            UpdateProperty(profile, smChannel, p => p.EPGId);
            //UpdateProperty(profile, smChannel, p => p.ChannelNumber.ToString());

            smChannel.ChannelNumber = GetNextChNo(smChannel.ChannelNumber);


        }
        finally
        {

            _ = semaphore.Release();
        }

    }

    private static void UpdateProperty<T>(OutputProfile profile, SMChannel smChannel, Expression<Func<OutputProfile, T>> propertySelector)
    {
        // Extract the property name from the expression
        if (propertySelector.Body is MemberExpression memberExpression)
        {
            // Retrieve the property value from the OutputProfile
            T? profileValue = propertySelector.Compile()(profile);

            // Check if the property value is a valid M3U setting
            if (Enum.TryParse<ValidM3USetting>(profileValue?.ToString(), out ValidM3USetting setting))
            {
                // Only update if the setting is not NotMapped
                if (setting != ValidM3USetting.NotMapped)
                {
                    // Use reflection to get the corresponding property from SMChannel
                    PropertyInfo? smChannelProperty = typeof(SMChannel).GetProperty(memberExpression.Member.Name);
                    if (smChannelProperty != null)
                    {
                        // Get the value from SMChannel
                        object? newValue = smChannelProperty.GetValue(smChannel);

                        // Update the OutputProfile property with the new value
                        PropertyInfo? profileProperty = typeof(OutputProfile).GetProperty(memberExpression.Member.Name);
                        profileProperty?.SetValue(profile, newValue);
                    }
                }
            }
        }
    }

    private (int ChNo, string m3uLine) BuildM3ULineForVideoStream(SMChannel smChannel, string url, GetStreamGroupM3U request, OutputProfile profile, int sgProfileId, int index, Setting setting, List<VideoStreamConfig> videoStreamConfigs)
    {
        UpdateProfile(profile, smChannel);
        //if (request.StreamGroupId == 1 && profile.ChannelNumber == "0")//ALL
        //{
        //    profile.ChannelNumber = cid.ToString();
        //}

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
                //if (setting.M3UUseCUIDForChannelID)
                //{
                //    tvgID = epgChannelId;
                //    channelId = smChannel.Id.ToString();
                //}
                //else
                //{
                tvgID = service?.CallSign ?? epgChannelId;
                channelId = tvgID;
                //}
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

        //if (setting.M3UIgnoreEmptyEPGID)
        //{
        //    showM3UFieldTvgId = !(string.IsNullOrEmpty(videoStream.EPGId) && string.IsNullOrEmpty(smChannelDto.EPGId));
        //}




        string logo = GetIconUrl(smChannel.Logo, setting);
        smChannel.Logo = logo;
        HLSSettings hlssettings = inthlssettings.CurrentValue;
        string videoUrl = "";
        //if (request.UseSMChannelId)
        //{
        //    videoUrl = $"{url}/v/v/{smChannel.ShortSMChannelId}";
        //}
        //else
        //{
        if (hlssettings.HLSM3U8Enable)
        {
            videoUrl = $"{url}/api/stream/{smChannel.Id}.m3u8";
        }
        else
        {
            string encodedName = HttpUtility.HtmlEncode(smChannel.Name).Trim()
                     .Replace("/", "")
                     .Replace(" ", "_");

            string encodedNumbers = request.StreamGroupId.EncodeValues128(sgProfileId, smChannel.Id, setting.ServerKey, iv);
            videoUrl = $"{url}/api/videostreams/stream/{encodedNumbers}/{encodedName}";
        }

        //}

        //if (setting.M3UUseChnoForId)
        //{
        //    tvgID = smChannel.ChannelNumber.ToString();
        //    if (!setting.M3UUseCUIDForChannelID)
        //    {
        //        channelId = tvgID;
        //    }
        //}

        List<string> fieldList = ["#EXTINF:0"];

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

        //if (setting.M3UStationId)
        //{
        //    string toDisplay = string.IsNullOrEmpty(smChannel.StationId) ? epgChannelId : smChannel.StationId;
        //    fieldList.Add($"tvc-guide-stationid=\"{toDisplay}\"");
        //}

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