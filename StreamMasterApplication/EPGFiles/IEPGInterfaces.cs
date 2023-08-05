using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.EPGFiles.Commands;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Repository;

namespace StreamMasterApplication.EPGFiles;

public interface IEPGFileController
{
    Task<ActionResult> AddEPGFile(AddEPGFileRequest request);

    Task<ActionResult> AddEPGFileFromForm([FromForm] AddEPGFileRequest request);


    Task<ActionResult> DeleteEPGFile(DeleteEPGFileRequest request);

    Task<ActionResult<EPGFilesDto>> GetEPGFile(int id);

    Task<ActionResult<IEnumerable<EPGFilesDto>>> GetEPGFiles();

    Task<ActionResult> ProcessEPGFile(ProcessEPGFileRequest request);

    Task<ActionResult> RefreshEPGFile(RefreshEPGFileRequest request);

    Task<ActionResult> ScanDirectoryForEPGFiles();

    Task<ActionResult> UpdateEPGFile(UpdateEPGFileRequest request);
}

public interface IEPGFileDB
{
    DbSet<EPGFile> EPGFiles { get; set; }
}

public interface IEPGFileHub
{
    Task AddEPGFile(AddEPGFileRequest request);

    Task DeleteEPGFile(DeleteEPGFileRequest request);

    Task<EPGFilesDto?> GetEPGFile(int id);

    Task<IEnumerable<EPGFilesDto>> GetEPGFiles();

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
