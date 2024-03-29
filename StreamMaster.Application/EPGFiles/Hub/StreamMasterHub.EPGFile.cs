﻿using StreamMaster.Application.EPGFiles;
using StreamMaster.Application.EPGFiles.Commands;
using StreamMaster.Application.EPGFiles.Queries;
using StreamMaster.Domain.Pagination;

namespace StreamMaster.Application.Hubs;

public partial class StreamMasterHub : IEPGFileHub
{
    public async Task CreateEPGFile(CreateEPGFileRequest request)
    {
        await mediator.Send(request).ConfigureAwait(false);
    }

    public async Task DeleteEPGFile(DeleteEPGFileRequest request)
    {
        await mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<List<EPGColorDto>> GetEPGColors(object nothing)
    {
        List<EPGColorDto> a = await mediator.Send(new GetEPGColors()).ConfigureAwait(false);
        return a;
    }

    public async Task<int> GetEPGNextEPGNumber(object nothing)
    {
        var a = await mediator.Send(new GetEPGNextEPGNumber()).ConfigureAwait(false);
        return a;

    }

    public async Task<EPGFileDto?> GetEPGFile(int id)
    {
        return await mediator.Send(new GetEPGFile(id)).ConfigureAwait(false);
    }

    public async Task<List<EPGFilePreviewDto>> GetEPGFilePreviewById(int id)
    {
        return await mediator.Send(new GetEPGFilePreviewById(id)).ConfigureAwait(false);
    }



    public async Task<PagedResponse<EPGFileDto>> GetPagedEPGFiles(EPGFileParameters parameters)
    {
        return await mediator.Send(new GetPagedEPGFiles(parameters)).ConfigureAwait(false);
    }

    public async Task ProcessEPGFile(ProcessEPGFileRequest request)
    {
        await mediator.Send(request).ConfigureAwait(false);
    }

    public async Task RefreshEPGFile(RefreshEPGFileRequest request)
    {
        await mediator.Send(request).ConfigureAwait(false);
    }

    public async Task ScanDirectoryForEPGFiles()
    {
        await mediator.Send(new ScanDirectoryForEPGFilesRequest()).ConfigureAwait(false);
    }

    public async Task UpdateEPGFile(UpdateEPGFileRequest request)
    {
        await mediator.Send(request).ConfigureAwait(false);
    }
}