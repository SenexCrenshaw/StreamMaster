using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.M3UFiles.Commands;

namespace StreamMaster.Application.M3UFiles
{
    public interface IM3UFilesController
    {        
    Task<ActionResult<DefaultAPIResponse>> CreateM3UFile(CreateM3UFileRequest request);
    Task<ActionResult<DefaultAPIResponse>> DeleteM3UFile(DeleteM3UFileRequest request);
    Task<ActionResult<APIResponse<M3UFileDto>>> GetPagedM3UFiles(QueryStringParameters Parameters);
    Task<ActionResult<DefaultAPIResponse>> RefreshM3UFile(RefreshM3UFileRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IM3UFilesHub
    {
        Task<DefaultAPIResponse> CreateM3UFile(CreateM3UFileRequest request);
        Task<DefaultAPIResponse> DeleteM3UFile(DeleteM3UFileRequest request);
        Task<APIResponse<M3UFileDto>> GetPagedM3UFiles(QueryStringParameters Parameters);
        Task<DefaultAPIResponse> ProcessM3UFile(ProcessM3UFileRequest request);
        Task<DefaultAPIResponse> RefreshM3UFile(RefreshM3UFileRequest request);
    }
}
