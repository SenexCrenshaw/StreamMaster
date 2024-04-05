using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Web;

namespace StreamMaster.Application.SMStreams.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetPagedSMStreamsRequest(QueryStringParameters Parameters) : IRequest<PagedResponse<SMStreamDto>>;

internal class GetPagedSMStreamsRequestHandler(IRepositoryWrapper Repository, IOptionsMonitor<Setting> settings, IOptionsMonitor<HLSSettings> hlsSettings, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetPagedSMStreamsRequest, PagedResponse<SMStreamDto>>
{
    public async Task<PagedResponse<SMStreamDto>> Handle(GetPagedSMStreamsRequest request, CancellationToken cancellationToken)
    {
        if (request.Parameters.PageSize == 0)
        {
            return Repository.SMStream.CreateEmptyPagedResponse();
        }

        PagedResponse<SMStreamDto> res = await Repository.SMStream.GetPagedSMStreams(request.Parameters, CancellationToken.None).ConfigureAwait(false);

        string url = httpContextAccessor.GetUrl();
        foreach (SMStreamDto stream in res.Data)
        {
            string videoUrl;

            if (hlsSettings.CurrentValue.HLSM3U8Enable)
            {
                videoUrl = $"{url}/api/stream/{stream.Id}.m3u8";
            }
            else
            {
                string encodedName = HttpUtility.HtmlEncode(stream.Name).Trim()
                        .Replace("/", "")
                        .Replace(" ", "_");

                string encodedNumbers = 0.EncodeValues128(stream.Id, settings.CurrentValue.ServerKey);
                videoUrl = $"{url}/api/videostreams/stream/{encodedNumbers}/{encodedName}";

            }

            string jsonString = JsonSerializer.Serialize(videoUrl);
            stream.RealUrl = jsonString;
        }
        return res;
    }
}
