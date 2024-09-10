using Microsoft.AspNetCore.Http;

using System.Diagnostics;
using System.Web;

namespace StreamMaster.Application.SMChannels.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetPagedSMChannelsRequest(QueryStringParameters Parameters) : IRequest<PagedResponse<SMChannelDto>>;

internal class GetPagedSMChannelsRequestHandler(IRepositoryWrapper Repository, IStreamGroupService streamGroupService, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetPagedSMChannelsRequest, PagedResponse<SMChannelDto>>
{
    public async Task<PagedResponse<SMChannelDto>> Handle(GetPagedSMChannelsRequest request, CancellationToken cancellationToken)
    {
        Debug.WriteLine("GetPagedSMChannelsRequestHandler");

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
        string requestPath = httpContextAccessor.GetUrlWithPathValue();

        int sgId = await streamGroupService.GetDefaultSGIdAsync().ConfigureAwait(false);


        StreamGroupProfile streamGroupProfile = await streamGroupService.GetDefaultStreamGroupProfileAsync();

        foreach (SMChannelDto channel in res.Data)
        {
            List<SMChannelStreamLink> links = [.. Repository.SMChannelStreamLink.GetQuery(true).Where(a => a.SMChannelId == channel.Id)];


            foreach (SMStreamDto stream in channel.SMStreams)
            {
                SMChannelStreamLink? link = links.Find(a => a.SMStreamId == stream.Id);

                if (link != null)
                {
                    stream.Rank = link.Rank;
                }
            }

            channel.SMStreams = [.. channel.SMStreams.OrderBy(a => a.Rank)];
            channel.StreamGroupIds = channel.StreamGroups.Select(a => a.StreamGroupId).ToList();

            string videoUrl = await GetVideoStreamUrlAsync(channel.Name, channel.Id, sgId, streamGroupProfile.Id, Url);// $"{Url}/api/videostreams/stream/{EncodedString}/{channel.Name.ToCleanFileString()}";
            channel.StreamUrl = videoUrl;// JsonSerializer.Serialize(videoUrl);
        }

        Debug.WriteLine($"GetPagedSMChannelsRequestHandler returning {res.Data.Count} items");
        return res;
    }

    private async Task<string> GetVideoStreamUrlAsync(string name, int smChannelId, int sgId, int sgPId, string url)
    {
        string cleanName = HttpUtility.UrlEncode(name);

        string? encodedString = await streamGroupService.EncodeStreamGroupIdProfileIdChannelIdAsync(sgId, sgPId, smChannelId);

        string videoUrl = $"{url}/api/videostreams/stream/{encodedString}/{cleanName}";

        return videoUrl;
    }
}
