using FluentValidation;

using Microsoft.AspNetCore.Http;

using StreamMaster.SchedulesDirect.Domain.Extensions;

using System.Text.Json;
using System.Web;

namespace StreamMaster.Application.StreamGroups.QueriesOld;

[RequireAll]
public record GetStreamGroupLineup(int StreamGroupId) : IRequest<string>;

public class GetStreamGroupLineupValidator : AbstractValidator<GetStreamGroupLineup>
{
    public GetStreamGroupLineupValidator()
    {
        _ = RuleFor(v => v.StreamGroupId)
            .NotNull().GreaterThanOrEqualTo(0);
    }
}

[LogExecutionTimeAspect]
public class GetStreamGroupLineupHandler(IHttpContextAccessor httpContextAccessor, IIconHelper iconHelper, IEPGHelper epgHelper, ISchedulesDirectDataService schedulesDirectDataService, ILogger<GetStreamGroupLineup> logger, IRepositoryWrapper Repository, IOptionsMonitor<HLSSettings> inthlssettings, IOptionsMonitor<Setting> intsettings)
    : IRequestHandler<GetStreamGroupLineup, string>
{
    private readonly Setting settings = intsettings.CurrentValue;
    private readonly HLSSettings hlssettings = inthlssettings.CurrentValue;

    public async Task<string> Handle(GetStreamGroupLineup request, CancellationToken cancellationToken)
    {

        string requestPath = httpContextAccessor.GetUrlWithPathValue();
        byte[]? iv = requestPath.GetIVFromPath(settings.ServerKey, 128);
        if (iv == null)
        {
            return "";
        }

        string url = httpContextAccessor.GetUrl();
        List<SGLineup> ret = [];

        List<SMChannel> smChannels = await Repository.SMChannel.GetSMChannelsFromStreamGroup(request.StreamGroupId);

        if (!smChannels.Any())
        {
            return JsonSerializer.Serialize(ret);
        }

        ISchedulesDirectData dummyData = schedulesDirectDataService.DummyData();
        foreach (SMChannel? smChannel in smChannels.OrderBy(a => a.ChannelNumber))
        {
            if (settings.M3UIgnoreEmptyEPGID && string.IsNullOrEmpty(smChannel.EPGId))
            {
                continue;
            }

            bool isDummy = epgHelper.IsDummy(smChannel.EPGId);

            if (isDummy)
            {
                smChannel.EPGId = $"{EPGHelper.DummyId}-{smChannel.Id}";
                VideoStreamConfig videoStreamConfig = new()
                {
                    Id = smChannel.Id.ToString(),
                    M3UFileId = 1,// smChannel.M3UFileId,
                    User_Tvg_name = smChannel.Name,
                    Tvg_ID = smChannel.EPGId,
                    User_Tvg_ID = smChannel.EPGId,
                    User_Tvg_Logo = smChannel.Logo,
                    User_Tvg_chno = smChannel.ChannelNumber,
                    IsDuplicate = false,
                    IsDummy = false
                };
                dummyData.FindOrCreateDummyService(smChannel.EPGId, videoStreamConfig);
            }

            int epgNumber = EPGHelper.DummyId;
            string stationId;

            if (string.IsNullOrEmpty(smChannel.EPGId))
            {
                stationId = smChannel.Group;
            }
            else
            {
                if (EPGHelper.IsValidEPGId(smChannel.EPGId))
                {
                    (epgNumber, stationId) = smChannel.EPGId.ExtractEPGNumberAndStationId();
                }
                else
                {
                    stationId = smChannel.EPGId;
                }

            }

            string videoUrl;
            if (hlssettings.HLSM3U8Enable)
            {
                videoUrl = $"{url}/api/stream/{smChannel.Id}.m3u8";
            }
            else
            {
                string encodedNumbers = request.StreamGroupId.EncodeValues128(smChannel.Id, settings.ServerKey, iv);

                string encodedName = HttpUtility.HtmlEncode(smChannel.Name).Trim().Replace(" ", "_");
                videoUrl = $"{url}/api/videostreams/stream/{encodedNumbers}/{encodedName}";
            }

            MxfService? service = schedulesDirectDataService.AllServices.GetMxfService(smChannel.EPGId);
            if (service == null)
            {
                continue;
            }
            string graceNote = service?.CallSign ?? stationId;
            string id = graceNote;
            if (settings.M3UUseChnoForId)
            {
                id = smChannel.ChannelNumber.ToString();
            }

            string? logo = "";
            if (service != null && service.mxfGuideImage != null && !string.IsNullOrEmpty(service.mxfGuideImage.ImageUrl))
            {
                logo = service.mxfGuideImage.ImageUrl;
                string _baseUrl = httpContextAccessor.GetUrl();
                logo = iconHelper.GetIconUrl(service.EPGNumber, service.extras["logo"].Url, _baseUrl);
            }

            SGLineup lu = new()
            {
                GuideName = smChannel.Name,
                GuideNumber = id,
                Station = id,
                Logo = logo,
                URL = videoUrl
            };

            ret.Add(lu);
        }

        string jsonString = JsonSerializer.Serialize(ret);
        return jsonString;
    }
}