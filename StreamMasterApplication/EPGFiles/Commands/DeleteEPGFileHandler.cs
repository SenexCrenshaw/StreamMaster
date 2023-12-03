using FluentValidation;

using StreamMaster.SchedulesDirectAPI.Domain.EPG;

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

public class DeleteEPGFileRequestHandler : BaseMediatorRequestHandler, IRequestHandler<DeleteEPGFileRequest, int?>
{

    public DeleteEPGFileRequestHandler(ILogger<DeleteEPGFileRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
    : base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { }

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

        var programmes = MemoryCache.Programmes();
        _ = programmes.RemoveAll(a => a.EPGFileId == epgFile.Id);
        MemoryCache.SetCache(programmes);

        List<ProgrammeChannel> channels = MemoryCache.ProgrammeChannels();
        _ = channels.RemoveAll(a => a.EPGFileId == epgFile.Id);
        MemoryCache.SetCache(channels);

        List<ChannelLogoDto> channelLogos = MemoryCache.ChannelLogos();
        _ = channelLogos.RemoveAll(a => a.EPGFileId == epgFile.Id);
        MemoryCache.SetCache(channelLogos);

        List<IconFileDto> programmeIcons = MemoryCache.ProgrammeIcons();
        _ = programmeIcons.RemoveAll(a => a.FileId == epgFile.Id);
        MemoryCache.SetProgrammeLogos(programmeIcons);

        _ = await Repository.SaveAsync().ConfigureAwait(false);

        await Publisher.Publish(new EPGFileDeletedEvent(epgFile.Id), cancellationToken).ConfigureAwait(false);
        return epgFile.Id;
    }
}
