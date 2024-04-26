using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.EPGFiles.Commands;
using StreamMaster.Application.EPGFiles.Queries;

namespace StreamMaster.Application.EPGFiles
{
    public interface IEPGFilesController
    {        
        Task<ActionResult<List<EPGFilePreviewDto>>> GetEPGFilePreviewById(GetEPGFilePreviewByIdRequest request);
        Task<ActionResult<List<EPGFileDto>>> GetEPGFiles();
        Task<ActionResult<int>> GetEPGNextEPGNumber();
        Task<ActionResult<PagedResponse<EPGFileDto>>> GetPagedEPGFiles(QueryStringParameters Parameters);
        Task<ActionResult<APIResponse>> CreateEPGFile(CreateEPGFileRequest request);
        Task<ActionResult<APIResponse>> DeleteEPGFile(DeleteEPGFileRequest request);
        Task<ActionResult<APIResponse>> ProcessEPGFile(ProcessEPGFileRequest request);
        Task<ActionResult<APIResponse>> RefreshEPGFile(RefreshEPGFileRequest request);
        Task<ActionResult<APIResponse>> UpdateEPGFile(UpdateEPGFileRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IEPGFilesHub
    {
        Task<List<EPGFilePreviewDto>> GetEPGFilePreviewById(GetEPGFilePreviewByIdRequest request);
        Task<List<EPGFileDto>> GetEPGFiles();
        Task<int> GetEPGNextEPGNumber();
        Task<PagedResponse<EPGFileDto>> GetPagedEPGFiles(QueryStringParameters Parameters);
        Task<APIResponse> CreateEPGFile(CreateEPGFileRequest request);
        Task<APIResponse> DeleteEPGFile(DeleteEPGFileRequest request);
        Task<APIResponse> ProcessEPGFile(ProcessEPGFileRequest request);
        Task<APIResponse> RefreshEPGFile(RefreshEPGFileRequest request);
        Task<APIResponse> UpdateEPGFile(UpdateEPGFileRequest request);
    }
}
