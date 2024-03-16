using Microsoft.AspNetCore.Http;

using StreamMaster.Application.Common.Extensions;
using StreamMaster.Domain.Authentication;
using StreamMaster.Domain.Pagination;

using System.Text.Json;
using System.Web;

namespace StreamMaster.Application.SMStreams.Queries;


public record GetPagedSMStreams(SMStreamParameters Parameters) : IRequest<PagedResponse<SMStreamDto>>;

internal class GetPagedSMStreamsHandler(IHttpContextAccessor httpContextAccessor, IOptionsMonitor<HLSSettings> inthlssettings, IOptionsMonitor<Setting> intsettings, ILogger<GetPagedSMStreamsHandler> logger, IRepositoryWrapper Repository) : IRequestHandler<GetPagedSMStreams, PagedResponse<SMStreamDto>>
{
    private readonly Setting settings = intsettings.CurrentValue;
    private readonly HLSSettings hlssettings = inthlssettings.CurrentValue;
    public async Task<PagedResponse<SMStreamDto>> Handle(GetPagedSMStreams request, CancellationToken cancellationToken)
    {
        if (request.Parameters.PageSize == 0)
        {
            return Repository.SMStream.CreateEmptyPagedResponse();
        }

        PagedResponse<SMStreamDto> res = await Repository.SMStream.GetPagedSMStreams(request.Parameters, cancellationToken).ConfigureAwait(false);

        string url = httpContextAccessor.GetUrl();
        foreach (SMStreamDto stream in res.Data)
        {
            string videoUrl;

            if (hlssettings.HLSM3U8Enable)
            {
                videoUrl = $"{url}/api/stream/{stream.Id}.m3u8";
            }
            else
            {
                string encodedName = HttpUtility.HtmlEncode(stream.Name).Trim()
                        .Replace("/", "")
                        .Replace(" ", "_");

                string encodedNumbers = 0.EncodeValues128(stream.Id, settings.ServerKey);
                videoUrl = $"{url}/api/videostreams/stream/{encodedNumbers}/{encodedName}";

            }

            string jsonString = JsonSerializer.Serialize(videoUrl);
            stream.RealUrl = jsonString;
        }


        return res;
    }
}