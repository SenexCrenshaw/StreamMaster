using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.EPGFiles.Commands;
using StreamMaster.Application.EPGFiles.Queries;

namespace StreamMaster.Application.EPGFiles
{
    public interface IEPGFilesController
    {        
        Task<List<EPGColorDto>> GetEPGColors();
        Task<List<EPGFilePreviewDto>> GetEPGFilePreviewById(int Id);
        Task<int> GetEPGNextEPGNumber();
    Task<ActionResult<EPGFileDto?>> CreateEPGFile(CreateEPGFileRequest request);
    Task<ActionResult<int>> DeleteEPGFile(DeleteEPGFileRequest request);
    Task<ActionResult<EPGFileDto?>> ProcessEPGFile(ProcessEPGFileRequest request);
    Task<ActionResult<EPGFileDto?>> RefreshEPGFile(RefreshEPGFileRequest request);
    Task<ActionResult<EPGFileDto?>> UpdateEPGFile(UpdateEPGFileRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IEPGFilesHub
    {
        Task<List<EPGColorDto>> GetEPGColors();
        Task<List<EPGFilePreviewDto>> GetEPGFilePreviewById(int Id);
        Task<int> GetEPGNextEPGNumber();
        Task<EPGFileDto?> CreateEPGFile(CreateEPGFileRequest request);
        Task<int> DeleteEPGFile(DeleteEPGFileRequest request);
        Task<EPGFileDto?> ProcessEPGFile(ProcessEPGFileRequest request);
        Task<EPGFileDto?> RefreshEPGFile(RefreshEPGFileRequest request);
        Task<EPGFileDto?> UpdateEPGFile(UpdateEPGFileRequest request);
    }
}
