using FluentValidation;

using StreamMasterDomain.EPG;
using StreamMasterDomain.Models;

namespace StreamMasterApplication.EPGFiles.Commands;

[RequireAll]
public record RefreshEPGFileRequest(int Id) : IRequest<EPGFileDto?> { }

public class RefreshEPGFileRequestValidator : AbstractValidator<RefreshEPGFileRequest>
{
    public RefreshEPGFileRequestValidator()
    {
        _ = RuleFor(v => v.Id).NotNull().GreaterThanOrEqualTo(0);
    }
}

public class RefreshEPGFileRequestHandler : BaseMediatorRequestHandler, IRequestHandler<RefreshEPGFileRequest, EPGFileDto?>
{

    public RefreshEPGFileRequestHandler(ILogger<RefreshEPGFileRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { }

    public async Task<EPGFileDto?> Handle(RefreshEPGFileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            EPGFile? epgFile = await Repository.EPGFile.GetEPGFileById(request.Id).ConfigureAwait(false);
            if (epgFile == null)
            {
                return null;
            }

            if (epgFile.LastDownloadAttempt.AddMinutes(epgFile.MinimumMinutesBetweenDownloads) < DateTime.Now)
            {
                FileDefinition fd = FileDefinitions.EPG;
                string fullName = Path.Combine(fd.DirectoryLocation, epgFile.Source);

                if (epgFile.Url != null && epgFile.Url.Contains("://"))
                {
                    Logger.LogInformation("Refresh EPG From URL {epgFile.Url}", epgFile.Url);

                    epgFile.LastDownloadAttempt = DateTime.Now;

                    (bool success, Exception? ex) = await FileUtil.DownloadUrlAsync(epgFile.Url, fullName, cancellationToken).ConfigureAwait(false);
                    if (success)
                    {
                        epgFile.DownloadErrors = 0;
                        epgFile.LastDownloaded = File.GetLastWriteTime(fullName);
                        epgFile.FileExists = true;
                    }
                    else
                    {
                        ++epgFile.DownloadErrors;
                        Logger.LogCritical("Exception EPG From URL {ex}", ex);
                    }
                }

                Tv? tv = await epgFile.GetTV().ConfigureAwait(false);
                if (tv == null)
                {
                    Logger.LogCritical("Exception EPG {fullName} format is not supported", fullName);
                    //Bad EPG
                    if (File.Exists(fullName))
                    {
                        File.Delete(fullName);
                    }
                    return null;
                }

                if (tv != null)
                {
                    epgFile.ChannelCount = tv.Channel != null ? tv.Channel.Count : 0;
                    epgFile.ProgrammeCount = tv.Programme != null ? tv.Programme.Count : 0;

                }
                Repository.EPGFile.UpdateEPGFile(epgFile);
                _ = await Repository.SaveAsync().ConfigureAwait(false);

                List<Programme> programmes = MemoryCache.Programmes();
                _ = programmes.RemoveAll(a => a.EPGFileId == epgFile.Id);
                MemoryCache.Set(programmes);

                List<ProgrammeChannel> programmeChannels = MemoryCache.ProgrammeChannels();
                _ = programmeChannels.RemoveAll(a => a.EPGFileId == epgFile.Id);
                MemoryCache.Set(programmeChannels);

                List<IconFileDto> programmeIcons = MemoryCache.ProgrammeIcons();
                _ = programmeIcons.RemoveAll(a => a.FileId == epgFile.Id);
                MemoryCache.SetProgrammeLogos(programmeIcons);


                EPGFileDto ret = Mapper.Map<EPGFileDto>(epgFile);
                await Publisher.Publish(new EPGFileAddedEvent(ret), cancellationToken).ConfigureAwait(false);
                return ret;
            }
        }
        catch (Exception)
        {
            return null;
        }
        return null;
    }
}
