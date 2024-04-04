using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.M3UFiles.Commands;
using StreamMaster.Application.M3UFiles.Queries;

namespace StreamMaster.Application.M3UFiles
{
    public interface IM3UFilesController
    {        
    Task<ActionResult<PagedResponse<M3UFileDto>>> GetPagedM3UFiles(QueryStringParameters Parameters);
    Task<ActionResult<DefaultAPIResponse>> CreateM3UFile(CreateM3UFileRequest request);
    Task<ActionResult<DefaultAPIResponse>> DeleteM3UFile(DeleteM3UFileRequest request);
    Task<ActionResult<DefaultAPIResponse>> RefreshM3UFile(RefreshM3UFileRequest request);
    Task<ActionResult<DefaultAPIResponse>> UpdateM3UFile(UpdateM3UFileRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IM3UFilesHub
    {
        Task<PagedResponse<M3UFileDto>> GetPagedM3UFiles(QueryStringParameters Parameters);
        Task<DefaultAPIResponse> CreateM3UFile(CreateM3UFileRequest request);
        Task<DefaultAPIResponse> DeleteM3UFile(DeleteM3UFileRequest request);
        Task<DefaultAPIResponse> ProcessM3UFile(ProcessM3UFileRequest request);
        Task<DefaultAPIResponse> RefreshM3UFile(RefreshM3UFileRequest request);
        Task<DefaultAPIResponse> UpdateM3UFile(UpdateM3UFileRequest request);
    }
}
