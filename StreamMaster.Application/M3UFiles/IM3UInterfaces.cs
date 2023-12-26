using Microsoft.AspNetCore.Mvc;

using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Pagination;

using StreamMaster.Application.M3UFiles.Commands;



namespace StreamMaster.Application.M3UFiles;

public interface IM3UFileController
{
    Task<ActionResult<List<string>>> GetM3UFileNames();
    Task<ActionResult> CreateM3UFile(CreateM3UFileRequest request);

    Task<ActionResult> CreateM3UFileFromForm([FromForm] CreateM3UFileRequest request);

    Task<ActionResult> ChangeM3UFileName(ChangeM3UFileNameRequest request);

    Task<ActionResult> DeleteM3UFile(DeleteM3UFileRequest request);

    Task<ActionResult<M3UFileDto>> GetM3UFile(int id);

    Task<ActionResult<PagedResponse<M3UFileDto>>> GetPagedM3UFiles(M3UFileParameters Parameters);

    Task<ActionResult> ProcessM3UFile(ProcessM3UFileRequest request);

    Task<ActionResult> RefreshM3UFile(RefreshM3UFileRequest request);

    Task<ActionResult> ScanDirectoryForM3UFiles();

    Task<ActionResult> UpdateM3UFile(UpdateM3UFileRequest request);
}

public interface IM3UFileDB
{
}

public interface IM3UFileHub
{
    Task<List<string>> GetM3UFileNames();
    Task CreateM3UFile(CreateM3UFileRequest request);

    Task ChangeM3UFileName(ChangeM3UFileNameRequest request);

    Task DeleteM3UFile(DeleteM3UFileRequest request);

    Task<M3UFileDto?> GetM3UFile(int id);

    Task<PagedResponse<M3UFileDto>> GetPagedM3UFiles(M3UFileParameters Parameters);

    Task ProcessM3UFile(ProcessM3UFileRequest request);

    Task RefreshM3UFile(RefreshM3UFileRequest request);

    Task ScanDirectoryForM3UFiles();

    Task UpdateM3UFile(UpdateM3UFileRequest request);
}

public interface IM3UFileTasks
{
    ValueTask ProcessM3UFile(int Id, bool immediate = false, CancellationToken cancellationToken = default);

    ValueTask ProcessM3UFiles(CancellationToken cancellationToken = default);

    ValueTask ScanDirectoryForM3UFiles(CancellationToken cancellationToken = default);
}

public interface IM3UFileScoped
{ }