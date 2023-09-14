using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.EPGFiles.Commands;

using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.EPGFiles;

public interface IEPGFileController
{
    Task<ActionResult> CreateEPGFile(CreateEPGFileRequest request);

    Task<ActionResult> CreateEPGFileFromForm([FromForm] CreateEPGFileRequest request);

    Task<ActionResult> DeleteEPGFile(DeleteEPGFileRequest request);

    Task<ActionResult<EPGFileDto>> GetEPGFile(int id);

    Task<ActionResult<PagedResponse<EPGFileDto>>> GetPagedEPGFiles(EPGFileParameters parameters);

    Task<ActionResult> ProcessEPGFile(ProcessEPGFileRequest request);

    Task<ActionResult> RefreshEPGFile(RefreshEPGFileRequest request);

    Task<ActionResult> ScanDirectoryForEPGFiles();

    Task<ActionResult> UpdateEPGFile(UpdateEPGFileRequest request);
}

public interface IEPGFileDB
{
}

public interface IEPGFileHub
{
    Task CreateEPGFile(CreateEPGFileRequest request);

    Task DeleteEPGFile(DeleteEPGFileRequest request);

    Task<EPGFileDto?> GetEPGFile(int id);

    Task<PagedResponse<EPGFileDto>> GetPagedEPGFiles(EPGFileParameters parameters);

    Task ProcessEPGFile(ProcessEPGFileRequest request);

    Task RefreshEPGFile(RefreshEPGFileRequest request);

    Task ScanDirectoryForEPGFiles();

    Task UpdateEPGFile(UpdateEPGFileRequest request);
}

public interface IEPGFileScoped
{
}

public interface IEPGFileTasks
{
    ValueTask ProcessEPGFile(int EPGFileId, CancellationToken cancellationToken = default);

    ValueTask ScanDirectoryForEPGFiles(CancellationToken cancellationToken = default);
}