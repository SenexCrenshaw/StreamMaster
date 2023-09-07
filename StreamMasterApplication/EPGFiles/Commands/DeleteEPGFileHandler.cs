using FluentValidation;

using StreamMasterDomain.Dto;

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

public class DeleteEPGFileRequestHandler : BaseMemoryRequestHandler, IRequestHandler<DeleteEPGFileRequest, int?>
{

    public DeleteEPGFileRequestHandler(ILogger<DeleteEPGFileRequest> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
    : base(logger, repository, mapper, publisher, sender, hubContext, memoryCache) { }

    public async Task<int?> Handle(DeleteEPGFileRequest request, CancellationToken cancellationToken = default)
    {
        EPGFile? epgFile = await Repository.EPGFile.GetEPGFileByIdAsync(request.Id).ConfigureAwait(false);
        if (epgFile == null)
        {
            return null;
        }

        Repository.EPGFile.DeleteEPGFile(epgFile);


        if (request.DeleteFile)
        {
            string fullName = Path.Combine(FileDefinitions.EPG.DirectoryLocation, epgFile.Name + FileDefinitions.EPG.FileExtension);
            if (File.Exists(fullName))
            {
                File.Delete(fullName);
                string txtName = Path.Combine(FileDefinitions.EPG.DirectoryLocation, Path.GetFileNameWithoutExtension(epgFile.Source) + ".url");
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

        List<ChannelLogoDto> programmes = MemoryCache.ChannelLogos();
        _ = programmes.RemoveAll(a => a.EPGFileId == epgFile.Id);
        MemoryCache.Set(programmes);

        List<ChannelLogoDto> channels = MemoryCache.ChannelLogos();
        _ = channels.RemoveAll(a => a.EPGFileId == epgFile.Id);
        MemoryCache.Set(channels);

        List<ChannelLogoDto> channelLogos = MemoryCache.ChannelLogos();
        _ = channelLogos.RemoveAll(a => a.EPGFileId == epgFile.Id);
        MemoryCache.Set(channelLogos);

        List<IconFileDto> programmeIcons = MemoryCache.ProgrammeIcons();
        _ = programmeIcons.RemoveAll(a => a.FileId == epgFile.Id);
        MemoryCache.SetProgrammeLogos(programmeIcons);

        _ = await Repository.SaveAsync().ConfigureAwait(false);

        await Publisher.Publish(new EPGFileDeletedEvent(epgFile.Id), cancellationToken).ConfigureAwait(false);
        return epgFile.Id;
    }
}
