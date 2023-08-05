using FluentValidation;

using MediatR;

using Microsoft.Extensions.Caching.Memory;

using StreamMasterDomain.Repository;

namespace StreamMasterApplication.EPGFiles.Commands;

public class DeleteEPGFileRequest : IRequest<int?>
{
    public bool DeleteFile { get; set; }
    public int Id { get; set; }
}

public class DeleteEPGFileRequestValidator : AbstractValidator<DeleteEPGFileRequest>
{
    public DeleteEPGFileRequestValidator()
    {
        _ = RuleFor(v => v.Id)
            .NotNull()
            .GreaterThanOrEqualTo(0);
    }
}

public class DeleteEPGFileHandler : IRequestHandler<DeleteEPGFileRequest, int?>
{
    private readonly IAppDbContext _context;
    private readonly IMemoryCache _memoryCache;
    private readonly IPublisher _publisher;

    public DeleteEPGFileHandler(IPublisher publisher, IAppDbContext context, IMemoryCache memoryCache)
    {
        _publisher = publisher;
        _memoryCache = memoryCache;
        _context = context;
    }

    public async Task<int?> Handle(DeleteEPGFileRequest request, CancellationToken cancellationToken = default)
    {
        EPGFile? EPGFile = await _context.EPGFiles.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (EPGFile == null)
        {
            return null;
        }

        _ = _context.EPGFiles.Remove(EPGFile);

        if (request.DeleteFile)
        {
            string fullName = Path.Combine(FileDefinitions.EPG.DirectoryLocation, EPGFile.Name + FileDefinitions.EPG.FileExtension);
            if (File.Exists(fullName))
            {
                File.Delete(fullName);
                string txtName = Path.Combine(FileDefinitions.EPG.DirectoryLocation, Path.GetFileNameWithoutExtension(EPGFile.Source) + ".url");
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

        var programmes = _memoryCache.ChannelLogos();
        programmes.RemoveAll(a => a.EPGFileId == EPGFile.Id);
        _memoryCache.Set(programmes);

        var channels = _memoryCache.ChannelLogos();
        channels.RemoveAll(a => a.EPGFileId == EPGFile.Id);
        _memoryCache.Set(channels);

        var channelLogos = _memoryCache.ChannelLogos();
        channelLogos.RemoveAll(a => a.EPGFileId == EPGFile.Id);
        _memoryCache.Set(channelLogos);

        var programmeIcons = _memoryCache.ProgrammeIcons();
        programmeIcons.RemoveAll(a => a.FileId == EPGFile.Id);
        _memoryCache.SetProgrammeLogos(programmeIcons);        

        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        await _publisher.Publish(new EPGFileDeletedEvent(EPGFile.Id), cancellationToken).ConfigureAwait(false);
        return EPGFile.Id;
    }
}
