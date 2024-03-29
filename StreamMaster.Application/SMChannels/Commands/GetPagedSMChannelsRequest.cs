﻿using Microsoft.AspNetCore.Http;

using System.Diagnostics;
using System.Text.Json;
using System.Web;

namespace StreamMaster.Application.SMChannels.Commands;


[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetPagedSMChannelsRequest(QueryStringParameters Parameters) : IRequest<APIResponse<SMChannelDto>>;

internal class GetPagedSMChannelsRequestHandler(IRepositoryWrapper Repository, IOptionsMonitor<Setting> settings, IOptionsMonitor<HLSSettings> hlsSettings, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetPagedSMChannelsRequest, APIResponse<SMChannelDto>>
{
    public async Task<APIResponse<SMChannelDto>> Handle(GetPagedSMChannelsRequest request, CancellationToken cancellationToken)
    {
        Debug.WriteLine("GetPagedSMChannelsRequestHandler");
        APIResponse<SMChannelDto> ret = new();
        if (request.Parameters.PageSize == 0)
        {
            ret.PagedResponse = Repository.SMChannel.CreateEmptyPagedResponse();
            Debug.WriteLine("GetPagedSMChannelsRequestHandler no params");
            return ret;
        }

        PagedResponse<SMChannelDto> res = await Repository.SMChannel.GetPagedSMChannels(request.Parameters).ConfigureAwait(false);

        string url = httpContextAccessor.GetUrl();

        foreach (SMChannelDto channel in res.Data)
        {
            List<SMChannelStreamLink> links = Repository.SMChannelStreamLink.GetQuery(true).Where(a => a.SMChannelId == channel.Id).ToList();

            string videoUrl;
            foreach (SMStreamDto stream in channel.SMStreams)
            {
                SMChannelStreamLink? link = links.FirstOrDefault(a => a.SMStreamId == stream.Id);

                if (link != null)
                {
                    stream.Rank = link.Rank;
                }
            }

            if (hlsSettings.CurrentValue.HLSM3U8Enable)
            {
                videoUrl = $"{url}/api/stream/{channel.Id}.m3u8";
            }
            else
            {
                string encodedName = HttpUtility.HtmlEncode(channel.Name).Trim()
                        .Replace("/", "")
                        .Replace(" ", "_");

                string encodedNumbers = 0.EncodeValues128(channel.Id, settings.CurrentValue.ServerKey);
                videoUrl = $"{url}/api/videostreams/stream/{encodedNumbers}/{encodedName}";

            }

            string jsonString = JsonSerializer.Serialize(videoUrl);
            channel.RealUrl = jsonString;
        }

        Debug.WriteLine($"GetPagedSMChannelsRequestHandler returning {res.Data.Count} items");
        ret.PagedResponse = res;
        return ret;
    }
}