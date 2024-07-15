using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.M3UFiles.Commands;
using StreamMaster.Application.M3UFiles.Queries;

namespace StreamMaster.Application.M3UFiles
{
    public interface IM3UFilesController
    {        
        Task<ActionResult<List<string>>> GetM3UFileNames();
        Task<ActionResult<List<M3UFileDto>>> GetM3UFiles();
        Task<ActionResult<PagedResponse<M3UFileDto>>> GetPagedM3UFiles(QueryStringParameters Parameters);
        Task<ActionResult<APIResponse>> CreateM3UFile(CreateM3UFileRequest request);
        Task<ActionResult<APIResponse>> DeleteM3UFile(DeleteM3UFileRequest request);
        Task<ActionResult<APIResponse>> RefreshM3UFile(RefreshM3UFileRequest request);
        Task<ActionResult<APIResponse>> SyncChannels(SyncChannelsRequest request);
        Task<ActionResult<APIResponse>> UpdateM3UFile(UpdateM3UFileRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IM3UFilesHub
    {
        Task<List<string>> GetM3UFileNames();
        Task<List<M3UFileDto>> GetM3UFiles();
        Task<PagedResponse<M3UFileDto>> GetPagedM3UFiles(QueryStringParameters Parameters);
        Task<APIResponse> CreateM3UFile(CreateM3UFileRequest request);
        Task<APIResponse> DeleteM3UFile(DeleteM3UFileRequest request);
        Task<APIResponse> ProcessM3UFile(ProcessM3UFileRequest request);
        Task<APIResponse> RefreshM3UFile(RefreshM3UFileRequest request);
        Task<APIResponse> SyncChannels(SyncChannelsRequest request);
        Task<APIResponse> UpdateM3UFile(UpdateM3UFileRequest request);
    }
}
