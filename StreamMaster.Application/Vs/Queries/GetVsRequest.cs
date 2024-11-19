using Microsoft.AspNetCore.Http;

using StreamMaster.Application.StreamGroupSMChannelLinks.Queries;

namespace StreamMaster.Application.Vs.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetVsRequest(int? StreamGroupId, int? StreamGroupProfileId) : IRequest<DataResponse<List<V>>>;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class V
{
    public string BaseUrl { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Id { get; set; }
    public int StreamGroupId { get; set; }
    public string StreamGroupName { get; set; } = string.Empty;
    public int StreamGroupProfileId { get; set; }
    public string StreamGroupProfileName { get; set; } = string.Empty;
    public string DefaultRealUrl { get; set; } = string.Empty;
    public string RealUrl { get; set; } = string.Empty;
}

internal class GetVsRequestHandler(ILogger<GetVsRequest> logger, IStreamGroupService streamGroupService, IHttpContextAccessor httpContextAccessor, ISender sender, IRepositoryWrapper repositoryWrapper)
    : IRequestHandler<GetVsRequest, DataResponse<List<V>>>
{
    public async Task<DataResponse<List<V>>> Handle(GetVsRequest request, CancellationToken cancellationToken)
    {
        string baseUrl = httpContextAccessor.GetUrl();

        if (request.StreamGroupId.HasValue && !request.StreamGroupProfileId.HasValue)
        {
            StreamGroup? sgSG = repositoryWrapper.StreamGroup.GetStreamGroup(request.StreamGroupId.Value);
            if (sgSG == null)
            {
                logger.LogError("GetVsRequest streamGroup not found!");
                throw new ApplicationException("StreamGroup not found!");
            }
            List<StreamGroupProfile> profiles = await repositoryWrapper.StreamGroupProfile.GetStreamGroupProfiles(request.StreamGroupId.Value);
            DataResponse<List<SMChannelDto>> sgSMChannels = await sender.Send(new GetStreamGroupSMChannelsRequest(request.StreamGroupId.Value), cancellationToken);
            if (sgSMChannels.IsError)
            {
                logger.LogError("GetVsRequest streamGroupSMChannelLinks not found!");
                throw new ApplicationException("StreamGroupSMChannelLinks not found!");
            }
            List<V> sgRet = [];
            foreach (StreamGroupProfile sgStreamGroupProfile in profiles)
            {
                if (sgSMChannels.Data is null)
                {
                    continue;
                }

                sgRet.AddRange(sgSMChannels.Data.ConvertAll(a => new V()
                {
                    Id = a.Id,
                    StreamGroupId = request.StreamGroupId.Value,
                    StreamGroupName = sgSG.Name,
                    StreamGroupProfileId = sgStreamGroupProfile.Id,
                    StreamGroupProfileName = sgStreamGroupProfile.ProfileName,
                    Name = a.Name,
                    BaseUrl = baseUrl,
                    DefaultRealUrl = $"{baseUrl}/v/{a.Id}",
                    RealUrl = $"{baseUrl}/v/{sgStreamGroupProfile.Id}/{a.Id}"
                }));
            }

            return DataResponse<List<V>>.Success(sgRet);
        }

        //int? sgId = request.StreamGroupId;
        //if (!sgId.HasValue)
        //{
        //    sgId = await streamGroupService.GetDefaultSGIdAsync().ConfigureAwait(false);
        //}
        //StreamGroupProfile streamGroupProfile = await profileService.GetStreamGroupProfileAsync(request.StreamGroupId, request.StreamGroupProfileId);
        //if (streamGroupProfile == null)
        //{
        //    logger.LogError("GetVsRequest streamGroupProfile not found!");
        //    throw new ApplicationException("StreamGroupProfile not found!");
        //}
        StreamGroup sg = await streamGroupService.GetDefaultSGAsync().ConfigureAwait(false);
        //if (sg == null)
        //{
        //    logger.LogError("GetVsRequest streamGroup not found!");
        //    throw new ApplicationException("StreamGroup not found!");
        //}

        if (sg.Name.EqualsIgnoreCase("ALL"))
        {
            List<SMChannel> allSMChannels = await repositoryWrapper.SMChannel.GetQuery().ToListAsync(cancellationToken: cancellationToken);
            List<V> allRet = allSMChannels.ConvertAll(a => new V()
            {
                Id = a.Id,
                StreamGroupId = sg.Id,
                StreamGroupProfileId = 1,
                StreamGroupProfileName = "TEST",
                StreamGroupName = sg.Name,
                Name = a.Name,
                BaseUrl = baseUrl,
                DefaultRealUrl = $"{baseUrl}/v/{a.Id}",
                RealUrl = $"{baseUrl}/v/{sg.Id}/{a.Id}"
            });
            return DataResponse<List<V>>.Success(allRet);
        }

        DataResponse<List<SMChannelDto>> smChannels = await sender.Send(new GetStreamGroupSMChannelsRequest(sg.Id), cancellationToken);
        if (smChannels.IsError)
        {
            logger.LogError("GetVsRequest streamGroupSMChannelLinks not found!");
            throw new ApplicationException("StreamGroupSMChannelLinks not found!");
        }

        if (smChannels.Data is null)
        {
            return DataResponse<List<V>>.ErrorWithMessage("channel Data not found");
        }

        List<V> ret = smChannels.Data.ConvertAll(a => new V()
        {
            Id = a.Id,
            StreamGroupId = sg.Id,
            StreamGroupName = sg.Name,
            StreamGroupProfileId = 1,
            StreamGroupProfileName = "TEST",
            Name = a.Name,
            BaseUrl = baseUrl,
            DefaultRealUrl = $"{baseUrl}/v/{a.Id}",
            RealUrl = $"{baseUrl}/v/{sg.Id}/{a.Id}"
        });

        return DataResponse<List<V>>.Success(ret);
    }
}