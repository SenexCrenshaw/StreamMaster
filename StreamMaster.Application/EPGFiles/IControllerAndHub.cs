using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.EPGFiles.Commands;
using StreamMaster.Application.EPGFiles.Queries;

namespace StreamMaster.Application.EPGFiles
{
    public interface IEPGFilesController
    {        
        Task<ActionResult<List<EPGColorDto>>> GetEPGColors();
        Task<ActionResult<List<EPGFilePreviewDto>>> GetEPGFilePreviewById(int Id);
        Task<ActionResult<int>> GetEPGNextEPGNumber();
        Task<ActionResult<DefaultAPIResponse>> CreateEPGFile(CreateEPGFileRequest request);
        Task<ActionResult<DefaultAPIResponse>> DeleteEPGFile(DeleteEPGFileRequest request);
        Task<ActionResult<DefaultAPIResponse>> ProcessEPGFile(ProcessEPGFileRequest request);
        Task<ActionResult<DefaultAPIResponse>> RefreshEPGFile(RefreshEPGFileRequest request);
        Task<ActionResult<DefaultAPIResponse>> UpdateEPGFile(UpdateEPGFileRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IEPGFilesHub
    {
        Task<List<EPGColorDto>> GetEPGColors();
        Task<List<EPGFilePreviewDto>> GetEPGFilePreviewById(int Id);
        Task<int> GetEPGNextEPGNumber();
        Task<DefaultAPIResponse> CreateEPGFile(CreateEPGFileRequest request);
        Task<DefaultAPIResponse> DeleteEPGFile(DeleteEPGFileRequest request);
        Task<DefaultAPIResponse> ProcessEPGFile(ProcessEPGFileRequest request);
        Task<DefaultAPIResponse> RefreshEPGFile(RefreshEPGFileRequest request);
        Task<DefaultAPIResponse> UpdateEPGFile(UpdateEPGFileRequest request);
    }
}
