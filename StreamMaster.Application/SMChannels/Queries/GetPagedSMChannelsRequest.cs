using System.Web;

using Microsoft.AspNetCore.Http;

namespace StreamMaster.Application.SMChannels.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetPagedSMChannelsRequest(QueryStringParameters Parameters) : IRequest<PagedResponse<SMChannelDto>>;

internal class GetPagedSMChannelsRequestHandler(IRepositoryWrapper Repository, IStreamGroupService streamGroupService, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetPagedSMChannelsRequest, PagedResponse<SMChannelDto>>
{
    public async Task<PagedResponse<SMChannelDto>> Handle(GetPagedSMChannelsRequest request, CancellationToken cancellationToken)
    {
        //Debug.WriteLine("GetPagedSMChannelsRequestHandler");

        if (request.Parameters.PageSize == 0)
        {
            return Repository.SMChannel.CreateEmptyPagedResponse();
        }

        if (string.IsNullOrEmpty(request.Parameters.OrderBy))
        {
            request.Parameters.OrderBy = "Id";
        }

        PagedResponse<SMChannelDto> res = await Repository.SMChannel.GetPagedSMChannels(request.Parameters).ConfigureAwait(false);

        string Url = httpContextAccessor.GetUrl();

        int sgId = await streamGroupService.GetDefaultSGIdAsync().ConfigureAwait(false);

        StreamGroupProfile streamGroupProfile = await streamGroupService.GetDefaultStreamGroupProfileAsync();

        if (res.Data is null)
        {
            return res;
        }
        foreach (SMChannelDto channel in res.Data)
        {
            await Repository.SMChannelStreamLink.UpdateSMChannelDtoRanks(channel);
            await Repository.SMChannelChannelLink.UpdateSMChannelDtoRanks(channel);

            //SMStreamTypeEnum sType = channel.M3UFileId == EPGHelper.CustomPlayListId ? SMStreamTypeEnum.CustomPlayList : SMStreamTypeEnum.Regular;

            ////channel.ChannelLogo = channel.ChannelLogo;// logoSerice.GetLogoUrl(channel.ChannelLogo, "", sType);
            channel.SMStreamDtos = [.. channel.SMStreamDtos.OrderBy(a => a.Rank)];
            channel.SMChannelDtos = [.. channel.SMChannelDtos.OrderBy(a => a.Rank)];
            channel.StreamGroupIds = [.. channel.StreamGroups.Select(a => a.StreamGroupId)];

            string videoUrl = await GetVideoStreamUrlAsync(channel.Name, channel.Id, sgId, streamGroupProfile.Id, Url);// $"{Url}/api/videostreams/stream/{EncodedString}/{channel.Name.ToCleanFileString()}";
            channel.StreamUrl = videoUrl;// JsonSerializer.Serialize(videoUrl);
        }

        //Debug.WriteLine($"GetPagedSMChannelsRequestHandler returning {res.Data.Count} items");
        return res;
    }

    private async Task<string> GetVideoStreamUrlAsync(string name, int smChannelId, int sgId, int sgPId, string url)
    {
        string cleanName = HttpUtility.UrlEncode(name);

        string? encodedString = await streamGroupService.EncodeStreamGroupIdProfileIdChannelIdAsync(sgId, sgPId, smChannelId);

        string videoUrl = $"{url}{BuildInfo.PATH_BASE}/api/videostreams/stream/{encodedString}/{cleanName}";

        return videoUrl;
    }
}
