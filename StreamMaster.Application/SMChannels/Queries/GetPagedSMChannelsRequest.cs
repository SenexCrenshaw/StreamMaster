using Microsoft.AspNetCore.Http;

using StreamMaster.Domain.Crypto;

using System.Diagnostics;
using System.Text.Json;

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

        PagedResponse<SMChannelDto> res = await Repository.SMChannel.GetPagedSMChannels(request.Parameters).ConfigureAwait(false);

        string Url = httpContextAccessor.GetUrl();
        string requestPath = httpContextAccessor.GetUrlWithPathValue();

        int sgId = await streamGroupService.GetDefaultSGIdAsync().ConfigureAwait(false);


        foreach (SMChannelDto channel in res.Data)
        {
            List<SMChannelStreamLink> links = [.. Repository.SMChannelStreamLink.GetQuery(true).Where(a => a.SMChannelId == channel.Id)];

            //string videoUrl;
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


            StreamGroupProfile test = await streamGroupService.GetDefaultStreamGroupProfileAsync();

            string? EncodedString = await streamGroupService.EncodeStreamGroupIdProfileIdChannelId(sgId, test.Id, channel.Id);

            //(string EncodedString, string CleanName) = await sender.Send(new EncodeStreamGroupIdProfileIdChannelId(defaultSGProfile.Data.StreamGroupId, defaultSGProfile.Data.Id, channel.Id, channel.Name), cancellationToken);
            if (string.IsNullOrEmpty(EncodedString))
            {
                continue;
            }

            //string videoUrl = $"{url}/m/{encodedString}.m3u8"

            string videoUrl = $"{Url}/api/videostreams/stream/{EncodedString}/{channel.Name.ToCleanFileString()}";
            //videoUrl = $"{Url}/m/{EncodedString}.m3u8";


            string jsonString = JsonSerializer.Serialize(videoUrl);
            channel.StreamUrl = jsonString;
        }

        Debug.WriteLine($"GetPagedSMChannelsRequestHandler returning {res.Data.Count} items");
        return res;
    }
}
