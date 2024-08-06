using Microsoft.AspNetCore.Http;

using System.Text.Json;

namespace StreamMaster.Application.SMStreams.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetPagedSMStreamsRequest(QueryStringParameters Parameters) : IRequest<PagedResponse<SMStreamDto>>;

internal class GetPagedSMStreamsRequestHandler(IRepositoryWrapper Repository, IStreamGroupService streamGroupService, IOptionsMonitor<Setting> intSettings, IHttpContextAccessor httpContextAccessor)
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

            string? EncodedString = await streamGroupService.EncodeStreamGroupIdStreamIdAsync(sgId, stream.Id);

            if (string.IsNullOrEmpty(EncodedString))
            {
                continue;
            }
            string videoUrl = $"{Url}/m/{EncodedString}.ts";

            string jsonString = JsonSerializer.Serialize(videoUrl);
            stream.RealUrl = jsonString;
        }
        return res;
    }
}
