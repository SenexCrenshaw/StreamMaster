using FluentValidation;

using StreamMasterApplication.SchedulesDirectAPI.Commands;

namespace StreamMasterApplication.EPGFiles.Commands;

public record DeleteEPGFileRequest(bool DeleteFile, int Id) : IRequest<int?> { }

public class DeleteEPGFileRequestValidator : AbstractValidator<DeleteEPGFileRequest>
{
    public DeleteEPGFileRequestValidator()
    {
        _ = RuleFor(v => v.Id)
            .NotNull()
            .GreaterThanOrEqualTo(0);
    }
}

public class DeleteEPGFileRequestHandler(ILogger<DeleteEPGFileRequest> logger, ISchedulesDirectData schedulesDirectData, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<DeleteEPGFileRequest, int?>
{
    public async Task<int?> Handle(DeleteEPGFileRequest request, CancellationToken cancellationToken = default)
    {
        EPGFileDto? epgFile = await Repository.EPGFile.DeleteEPGFile(request.Id);

        if (request.DeleteFile && epgFile != null)
        {
            string fullName = Path.Combine(FileDefinitions.EPG.DirectoryLocation, epgFile.Name + FileDefinitions.EPG.FileExtension);
            if (File.Exists(fullName))
            {
                File.Delete(fullName);
                string txtName = Path.Combine(FileDefinitions.EPG.DirectoryLocation, Path.GetFileNameWithoutExtension(epgFile.Source) + ".json");
                if (File.Exists(txtName))
                {
                    File.Delete(txtName);
                }
            }
            else
            {
                //_logger.LogError("DeleteEPGFile File {fulleName} does not exist", fulleName);
            }
        }

        var services = schedulesDirectData.Services.Where(a => a.extras.ContainsKey("epgid") && a.extras["epgid"] == epgFile.Id).ToList();
        foreach (var service in services)
        {
            schedulesDirectData.Services.Remove(service);
        }

        var programs = schedulesDirectData.Programs.Where(a => a.extras.ContainsKey("epgid") && a.extras["epgid"] == epgFile.Id).ToList();
        foreach (var program in programs)
        {
            schedulesDirectData.Programs.Remove(program);
        }

        //var programmes = MemoryCache.Programmes();
        //_ = programmes.RemoveAll(a => a.EPGFileId == epgFile.Id);
        //MemoryCache.SetCache(programmes);

        //List<ProgrammeChannel> channels = MemoryCache.ProgrammeChannels();
        //_ = channels.RemoveAll(a => a.EPGFileId == epgFile.Id);
        //MemoryCache.SetCache(channels);

        //List<ChannelLogoDto> channelLogos = MemoryCache.ChannelLogos();
        //_ = channelLogos.RemoveAll(a => a.EPGFileId == epgFile.Id);
        //MemoryCache.SetCache(channelLogos);

        //List<IconFileDto> programmeIcons = MemoryCache.ProgrammeIcons();
        //_ = programmeIcons.RemoveAll(a => a.FileId == epgFile.Id);
        //MemoryCache.SetProgrammeLogos(programmeIcons);

        _ = await Repository.SaveAsync().ConfigureAwait(false);

        await Sender.Send(new SDSync(), cancellationToken).ConfigureAwait(false);

        await Publisher.Publish(new EPGFileDeletedEvent(epgFile.Id), cancellationToken).ConfigureAwait(false);
        return epgFile.Id;
    }
}
