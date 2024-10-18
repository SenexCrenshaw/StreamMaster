using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.EPGFiles.Commands;

namespace StreamMaster.Application.EPGFiles;

public interface IEPGFileController
{

    Task<ActionResult<int>> GetEPGNextEPGNumber();
    Task<ActionResult<List<EPGColorDto>>> GetEPGColors();
    Task<ActionResult<List<EPGFilePreviewDto>>> GetEPGFilePreviewById(int id);
    Task<ActionResult> CreateEPGFile(CreateEPGFileRequest request);

    Task<ActionResult> CreateEPGFileFromForm([FromForm] CreateEPGFileRequest request);

    Task<ActionResult> DeleteEPGFile(DeleteEPGFileRequest request);

    Task<ActionResult<EPGFileDto>> GetEPGFile(int id);

    Task<ActionResult> ProcessEPGFile(ProcessEPGFileRequest request);

    Task<ActionResult> RefreshEPGFile(RefreshEPGFileRequest request);

    Task<ActionResult> ScanDirectoryForEPGFiles();

    Task<ActionResult> UpdateEPGFile(UpdateEPGFileRequest request);
}

public interface IEPGFileHub
{
    Task<int> GetEPGNextEPGNumber(object nothing);
    Task<List<EPGColorDto>> GetEPGColors(object nothing);
    Task<List<EPGFilePreviewDto>> GetEPGFilePreviewById(int id);
    Task CreateEPGFile(CreateEPGFileRequest request);

    Task DeleteEPGFile(DeleteEPGFileRequest request);

    Task<EPGFileDto?> GetEPGFile(int id);

    Task ProcessEPGFile(ProcessEPGFileRequest request);

    Task RefreshEPGFile(RefreshEPGFileRequest request);

    Task ScanDirectoryForEPGFiles();

    Task UpdateEPGFile(UpdateEPGFileRequest request);
}

public interface IEPGFileTasks
{
    ValueTask EPGSync(CancellationToken cancellationToken = default);
    ValueTask ProcessEPGFile(int EPGFileId, CancellationToken cancellationToken = default);

    ValueTask ScanDirectoryForEPGFiles(CancellationToken cancellationToken = default);
}