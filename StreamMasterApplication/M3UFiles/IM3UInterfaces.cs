using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.M3UFiles;

public interface IM3UFileController
{
    Task<ActionResult> AddM3UFile(AddM3UFileRequest request);

    Task<ActionResult> AddM3UFileFromForm([FromForm] AddM3UFileRequest request);

    Task<ActionResult> ChangeM3UFileName(ChangeM3UFileNameRequest request);

    Task<ActionResult> DeleteM3UFile(DeleteM3UFileRequest request);

    Task<ActionResult<M3UFilesDto>> GetM3UFile(int id);

    Task<ActionResult<IEnumerable<M3UFilesDto>>> GetM3UFiles();

    Task<ActionResult> ProcessM3UFile(ProcessM3UFileRequest request);

    Task<ActionResult> RefreshM3UFile(RefreshM3UFileRequest request);

    Task<ActionResult> ScanDirectoryForM3UFiles();

    Task<ActionResult> UpdateM3UFile(UpdateM3UFileRequest request);
}

public interface IM3UFileDB
{
    DbSet<M3UFile> M3UFiles { get; set; }
}

public interface IM3UFileHub
{
    Task AddM3UFile(AddM3UFileRequest request);

    Task ChangeM3UFileName(ChangeM3UFileNameRequest request);

    Task DeleteM3UFile(DeleteM3UFileRequest request);

    Task<M3UFilesDto?> GetM3UFile(int id);

    Task<IEnumerable<M3UFilesDto>> GetM3UFiles();

    Task ProcessM3UFile(ProcessM3UFileRequest request);

    Task RefreshM3UFile(RefreshM3UFileRequest request);

    Task ScanDirectoryForM3UFiles();

    Task UpdateM3UFile(UpdateM3UFileRequest request);
}

public interface IM3UFileTasks
{
    ValueTask ProcessM3UFile(int M3UFileId, bool immediate = false, CancellationToken cancellationToken = default);

    ValueTask ProcessM3UFiles(CancellationToken cancellationToken = default);

    ValueTask ScanDirectoryForM3UFiles(CancellationToken cancellationToken = default);
}

public interface IM3UFileScoped
{ }
