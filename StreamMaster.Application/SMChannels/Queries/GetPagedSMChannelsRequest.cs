using Microsoft.AspNetCore.Http;

using StreamMaster.Application.Crypto.Commands;
using StreamMaster.Application.StreamGroups.Queries;

using System.Diagnostics;
using System.Text.Json;

namespace StreamMaster.Application.SMChannels.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetPagedSMChannelsRequest(QueryStringParameters Parameters) : IRequest<PagedResponse<SMChannelDto>>;

internal class GetPagedSMChannelsRequestHandler(IRepositoryWrapper Repository, ISender sender, IOptionsMonitor<Setting> intSettings, IHttpContextAccessor httpContextAccessor)
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

        DataResponse<StreamGroupProfile> defaultSGProfile = await sender.Send(new GetDefaultStreamGroupProfileIdRequest()).ConfigureAwait(false);
        foreach (SMChannelDto channel in res.Data)
        {
            List<SMChannelStreamLink> links = [.. Repository.SMChannelStreamLink.GetQuery(true).Where(a => a.SMChannelId == channel.Id)];

            string videoUrl;
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


            //string encodedName = HttpUtility.HtmlEncode(channel.Name).Trim()
            //                    .Replace("/", "")
            //                    .Replace(" ", "_");

            //string encodedNumbers = 1.EncodeValues128(1, channel.Id, intSettings.CurrentValue.ServerKey);
            //string? EncodedString = await sender.Send(new EncodeFromStreamGroupIdProfileIdRequest(1, 1));
            //if (EncodedString == null || CleanName == null)
            //{
            //    continue;
            //}
            (string EncodedString, string CleanName) = await sender.Send(new EncodeStreamGroupIdProfileIdChannelId(defaultSGProfile.Data.StreamGroupId, defaultSGProfile.Data.Id, channel.Id, channel.Name), cancellationToken);
            if (string.IsNullOrEmpty(EncodedString) || string.IsNullOrEmpty(CleanName))
            {
                continue;
            }

            videoUrl = $"{Url}/api/videostreams/stream/{EncodedString}/{CleanName}";


            string jsonString = JsonSerializer.Serialize(videoUrl);
            channel.StreamUrl = jsonString;
        }

        Debug.WriteLine($"GetPagedSMChannelsRequestHandler returning {res.Data.Count} items");
        return res;
    }
}
