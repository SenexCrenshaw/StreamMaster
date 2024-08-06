using Microsoft.AspNetCore.Http;

using StreamMaster.Domain.Crypto;
using StreamMaster.SchedulesDirect.Domain.Extensions;

using System.Text.Json;

namespace StreamMaster.Application.StreamGroups.QueriesOld;

[RequireAll]
public record GetStreamGroupLineup(int StreamGroupProfileId, bool IsShort) : IRequest<string>;


[LogExecutionTimeAspect]
public class GetStreamGroupLineupHandler(IHttpContextAccessor httpContextAccessor, IStreamGroupService streamGroupService, ICryptoService cryptoService, IIconHelper iconHelper, IEPGHelper epgHelper, ISchedulesDirectDataService schedulesDirectDataService, IRepositoryWrapper Repository, IOptionsMonitor<Setting> intSettings)
    : IRequestHandler<GetStreamGroupLineup, string>
{
    private readonly Setting settings = intSettings.CurrentValue;

    public async Task<string> Handle(GetStreamGroupLineup request, CancellationToken cancellationToken)
    {

        string requestPath = httpContextAccessor.GetUrlWithPathValue();
        //byte[]? iv = requestPath.GetIVFromPath(settings.ServerKey, 128);
        //if (iv == null)
        //{
        //    return "";
        //}

        string url = httpContextAccessor.GetUrl();
        List<SGLineup> ret = [];

        StreamGroup streamGroup = await streamGroupService.GetStreamGroupFromSGProfileIdAsync(request.StreamGroupProfileId);
        List<SMChannel> smChannels = await Repository.SMChannel.GetSMChannelsFromStreamGroup(streamGroup.Id);

        if (!smChannels.Any())
        {
            return JsonSerializer.Serialize(ret);
        }

        ISchedulesDirectData dummyData = schedulesDirectDataService.DummyData();
        foreach (SMChannel? smChannel in smChannels.OrderBy(a => a.ChannelNumber))
        {
            //settings.M3UIgnoreEmptyEPGID &&

            if (string.IsNullOrEmpty(smChannel.EPGId))
            {
                continue;
            }

            bool isDummy = epgHelper.IsDummy(smChannel.EPGId);

            if (isDummy)
            {
                smChannel.EPGId = $"{EPGHelper.DummyId}-{smChannel.Id}";

                VideoStreamConfig videoStreamConfig = new()
                {
                    Id = smChannel.Id,
                    Name = smChannel.Name,
                    EPGId = smChannel.EPGId,
                    Logo = smChannel.Logo,
                    ChannelNumber = smChannel.ChannelNumber,
                    IsDuplicate = false,
                    IsDummy = false
                };
                _ = dummyData.FindOrCreateDummyService(smChannel.EPGId, videoStreamConfig);
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

            //string? EncodedString = await cryptoService.EncodeStreamGroupIdChannelIdAsync(request.StreamGroupId, smChannel.Id);


            string? EncodedString = await cryptoService.EncodeStreamGroupIdProfileIdChannelId(streamGroup.Id, request.StreamGroupProfileId, smChannel.Id);


            //(string EncodedString, string CleanName) = await sender.Send(new EncodeStreamGroupIdProfileIdChannelId(request.StreamGroupId, request.StreamGroupProfileId, smChannel.Id, smChannel.Name), cancellationToken);
            //if (string.IsNullOrEmpty(EncodedString))
            //{
            //    continue;
            //}
            //videoUrl = $"{url}/m/{EncodedString}.m3u8";

            //if (smChannel.Name.Contains("comedy", StringComparison.CurrentCultureIgnoreCase))
            //{
            //    videoUrl = $"{url}/m/comedy.ts";
            //}

            //if (smChannel.Name.Contains("intros", StringComparison.CurrentCultureIgnoreCase))
            //{
            //    videoUrl = $"{url}/m/intros.ts";
            //}
            //// : $"{url}/api/videostreams/stream/{encodedString}/{cleanName}";
            //object sgProfileId = Repository.StreamGroupProfile.GetStreamGroupProfile(request.StreamGroupId, "default");
            videoUrl = request.IsShort
                ? $"{url}/v/{request.StreamGroupProfileId}/{smChannel.Id}"
                : $"{url}/api/videostreams/stream/{EncodedString}/{smChannel.Name.ToCleanFileString()}";



            MxfService? service = schedulesDirectDataService.AllServices.GetMxfService(smChannel.EPGId);
            if (service == null)
            {
                continue;
            }
            string graceNote = service?.CallSign ?? stationId;
            string id = graceNote;

            //if (settings.M3UUseChnoForId)
            //{
            //    id = smChannel.ChannelNumber.ToString();
            //}

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