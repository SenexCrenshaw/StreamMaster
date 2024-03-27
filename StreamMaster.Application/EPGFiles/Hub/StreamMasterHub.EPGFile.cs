using StreamMaster.Application.EPGFiles;
using StreamMaster.Application.EPGFiles.Commands;
using StreamMaster.Application.EPGFiles.Queries;
using StreamMaster.Domain.Pagination;

namespace StreamMaster.Application.Hubs;

public partial class StreamMasterHub : IEPGFileHub
{
    public async Task CreateEPGFile(CreateEPGFileRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task DeleteEPGFile(DeleteEPGFileRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task<List<EPGColorDto>> GetEPGColors(object nothing)
    {
        List<EPGColorDto> a = await Sender.Send(new GetEPGColors()).ConfigureAwait(false);
        return a;
    }

    public async Task<int> GetEPGNextEPGNumber(object nothing)
    {
        var a = await Sender.Send(new GetEPGNextEPGNumber()).ConfigureAwait(false);
        return a;

    }

    public async Task<EPGFileDto?> GetEPGFile(int id)
    {
        return await Sender.Send(new GetEPGFile(id)).ConfigureAwait(false);
    }

    public async Task<List<EPGFilePreviewDto>> GetEPGFilePreviewById(int id)
    {
        return await Sender.Send(new GetEPGFilePreviewById(id)).ConfigureAwait(false);
    }



    public async Task<PagedResponse<EPGFileDto>> GetPagedEPGFiles(EPGFileParameters parameters)
    {
        return await Sender.Send(new GetPagedEPGFiles(parameters)).ConfigureAwait(false);
    }

    public async Task ProcessEPGFile(ProcessEPGFileRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task RefreshEPGFile(RefreshEPGFileRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task ScanDirectoryForEPGFiles()
    {
        await Sender.Send(new ScanDirectoryForEPGFilesRequest()).ConfigureAwait(false);
    }

    public async Task UpdateEPGFile(UpdateEPGFileRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }
}