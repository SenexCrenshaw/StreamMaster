using Microsoft.AspNetCore.Http;

namespace StreamMaster.Application.M3UFiles.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetPagedM3UFilesRequest(QueryStringParameters Parameters) : IRequest<APIResponse<M3UFileDto>>;

internal class GetPagedM3UFilesRequestHandler(IRepositoryWrapper Repository, IOptionsMonitor<Setting> settings, IOptionsMonitor<HLSSettings> hlsSettings, IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetPagedM3UFilesRequest, APIResponse<M3UFileDto>>
{
    public async Task<APIResponse<M3UFileDto>> Handle(GetPagedM3UFilesRequest request, CancellationToken cancellationToken)
    {
        if (request?.Parameters?.PageSize == null || request.Parameters.PageSize == 0)
        {
            return DefaultAPIResponse.OkWithData(Repository.M3UFile.CreateEmptyPagedResponse());
        }

        PagedResponse<M3UFileDto> m3uFiles = await Repository.M3UFile.GetPagedM3UFiles(request.Parameters).ConfigureAwait(false);
        return DefaultAPIResponse.OkWithData(m3uFiles);
    }
}
