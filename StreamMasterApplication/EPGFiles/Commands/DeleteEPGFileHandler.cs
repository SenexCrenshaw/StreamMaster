using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Cache;

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

public class DeleteEPGFileHandler : BaseMemoryRequestHandler, IRequestHandler<DeleteEPGFileRequest, int?>
{

    public DeleteEPGFileHandler(ILogger<DeleteEPGFileHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache) { }

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

        List<StreamMasterDomain.Dto.ChannelLogoDto> programmes = MemoryCache.ChannelLogos();
        programmes.RemoveAll(a => a.EPGFileId == epgFile.Id);
        MemoryCache.Set(programmes);

        List<StreamMasterDomain.Dto.ChannelLogoDto> channels = MemoryCache.ChannelLogos();
        channels.RemoveAll(a => a.EPGFileId == epgFile.Id);
        MemoryCache.Set(channels);

        List<StreamMasterDomain.Dto.ChannelLogoDto> channelLogos = MemoryCache.ChannelLogos();
        channelLogos.RemoveAll(a => a.EPGFileId == epgFile.Id);
        MemoryCache.Set(channelLogos);

        List<StreamMasterDomain.Dto.IconFileDto> programmeIcons = MemoryCache.ProgrammeIcons();
        programmeIcons.RemoveAll(a => a.FileId == epgFile.Id);
        MemoryCache.SetProgrammeLogos(programmeIcons);

        await Repository.SaveAsync().ConfigureAwait(false);

        await Publisher.Publish(new EPGFileDeletedEvent(epgFile.Id), cancellationToken).ConfigureAwait(false);
        return epgFile.Id;
    }
}
