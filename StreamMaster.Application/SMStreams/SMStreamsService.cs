using Microsoft.AspNetCore.Http;

using System.Text.Json;
using System.Web;

namespace StreamMaster.Application.SMStreams;

public partial class SMStreamsService(IRepositoryWrapper repository, IHttpContextAccessor httpContextAccessor, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IOptionsMonitor<HLSSettings> inthlssettings, IOptionsMonitor<Setting> intsettings)
: ISMStreamsService
{
    private readonly Setting settings = intsettings.CurrentValue;
    private readonly HLSSettings hlssettings = inthlssettings.CurrentValue;

    [SMAPI]
    public async Task<APIResponse<SMStreamDto>> GetPagedSMStreams(SMStreamParameters Parameters)
    {
        APIResponse<SMStreamDto> ret = new();
        if (Parameters.PageSize == 0)
        {
            ret.PagedResponse = repository.SMStream.CreateEmptyPagedResponse();
            return ret;
        }

        PagedResponse<SMStreamDto> res = await repository.SMStream.GetPagedSMStreams(Parameters, CancellationToken.None).ConfigureAwait(false);

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

        ret.PagedResponse = res;
        return ret;
    }

    [SMAPI]
    public async Task<DefaultAPIResponse> ToggleSMStreamVisibleById(string id)
    {
        SMStreamDto? stream = await repository.SMStream.ToggleSMStreamVisibleById(id, CancellationToken.None).ConfigureAwait(false);
        if (stream == null)
        {
            return APIResponseFactory.NotFound();
        }

        FieldData fd = new(nameof(SMStreamDto), stream.Id, "isHidden", stream.IsHidden);

        await hubContext.Clients.All.SetField(fd).ConfigureAwait(false);
        return APIResponseFactory.Ok();
    }
}
