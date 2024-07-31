using Microsoft.AspNetCore.Http;

using System.Text.Json;

namespace StreamMaster.Application.SMStreams.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetPagedSMStreamsRequest(QueryStringParameters Parameters) : IRequest<PagedResponse<SMStreamDto>>;

internal class GetPagedSMStreamsRequestHandler(IRepositoryWrapper Repository, IStreamGroupService streamGroupService, ICryptoService cryptoService, IOptionsMonitor<Setting> intSettings, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetPagedSMStreamsRequest, PagedResponse<SMStreamDto>>
{
    public async Task<PagedResponse<SMStreamDto>> Handle(GetPagedSMStreamsRequest request, CancellationToken cancellationToken)
    {
        if (request.Parameters.PageSize == 0)
        {
            return Repository.SMStream.CreateEmptyPagedResponse();
        }

        _ = intSettings.CurrentValue;
        _ = httpContextAccessor.GetUrlWithPathValue();

        PagedResponse<SMStreamDto> res = await Repository.SMStream.GetPagedSMStreams(request.Parameters, CancellationToken.None).ConfigureAwait(false);


        string Url = httpContextAccessor.GetUrl();
        int sgId = await streamGroupService.GetDefaultSGIdAsync().ConfigureAwait(false);
        foreach (SMStreamDto stream in res.Data)
        {
            string videoUrl;


            //string encodedName = HttpUtility.HtmlEncode(stream.Name).Trim()
            //        .Replace("/", "")
            //        .Replace(" ", "_");

            //string encodedNumbers = 0.EncodeValues128(stream.Id, settings.CurrentValue.ServerKey);
            //videoUrl = $"{Url}/api/videostreams/stream/{encodedNumbers}/{encodedName}";

            //(string EncodedString, string CleanName) = await sender.Send(new EncodeStreamGroupIdProfileIdStreamId(defaultSGProfile.Data.StreamGroupId, defaultSGProfile.Data.Id, stream.Id, stream.Name), cancellationToken);
            string? EncodedString = await cryptoService.EncodeStreamGroupIdStreamIdAsync(sgId, stream.Id);

            if (string.IsNullOrEmpty(EncodedString))
            {
                continue;
            }
            videoUrl = $"{Url}/m/{EncodedString}.ts";


            string jsonString = JsonSerializer.Serialize(videoUrl);
            stream.RealUrl = jsonString;
        }
        return res;
    }
}
