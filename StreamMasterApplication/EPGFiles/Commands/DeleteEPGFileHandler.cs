using FluentValidation;

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

public class DeleteEPGFileRequestHandler(ILogger<DeleteEPGFileRequest> logger, ISchedulesDirect schedulesDirect, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<DeleteEPGFileRequest, int?>
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
        _ = await Repository.SaveAsync().ConfigureAwait(false);

        schedulesDirect.ResetEPGCache();
        MemoryCache.SetSyncForceNextRun(Extra: true);

        await Publisher.Publish(new EPGFileDeletedEvent(epgFile.Id), cancellationToken).ConfigureAwait(false);
        return epgFile.Id;
    }
}
