using Microsoft.AspNetCore.Http;



namespace StreamMaster.Application.M3UFiles.Commands;

[SMAPI]
public record GetPagedM3UFiles(M3UFileParameters Parameters) : IRequest<APIResponse<M3UFileDto>>;

internal class GetPagedM3UFilesRequestHandler(IRepositoryWrapper Repository, IOptionsMonitor<Setting> settings, IOptionsMonitor<HLSSettings> hlsSettings, IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetPagedM3UFiles, APIResponse<M3UFileDto>>
{
    public async Task<APIResponse<M3UFileDto>> Handle(GetPagedM3UFiles request, CancellationToken cancellationToken)
    {
        if (request?.Parameters?.PageSize == null || request.Parameters.PageSize == 0)
        {
            return APIResponseFactory.OkWithData(Repository.M3UFile.CreateEmptyPagedResponse());
        }

        PagedResponse<M3UFileDto> m3uFiles = await Repository.M3UFile.GetPagedM3UFiles(request.Parameters).ConfigureAwait(false);
        return APIResponseFactory.OkWithData(m3uFiles);
    }
}
