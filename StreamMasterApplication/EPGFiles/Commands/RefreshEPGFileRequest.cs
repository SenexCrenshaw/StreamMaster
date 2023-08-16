using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Attributes;
using StreamMasterDomain.Cache;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Repository.EPG;

namespace StreamMasterApplication.EPGFiles.Commands;

[RequireAll]
public class RefreshEPGFileRequest : IRequest<EPGFilesDto?>
{
    //public bool ForceDownload { get; set; }
    public int Id { get; set; }
}

public class RefreshEPGFileRequestValidator : AbstractValidator<RefreshEPGFileRequest>
{
    public RefreshEPGFileRequestValidator()
    {
        _ = RuleFor(v => v.Id).NotNull().GreaterThanOrEqualTo(0);
    }
}

public class RefreshEPGFileRequestHandler : BaseMemoryRequestHandler, IRequestHandler<RefreshEPGFileRequest, EPGFilesDto?>
{


    public RefreshEPGFileRequestHandler(ILogger<RefreshEPGFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache) { }


    public async Task<EPGFilesDto?> Handle(RefreshEPGFileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            EPGFile? epgFile = await Repository.EPGFile.GetEPGFileByIdAsync(request.Id).ConfigureAwait(false);
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
                await Repository.SaveAsync().ConfigureAwait(false);

                List<ChannelLogoDto> programmes = MemoryCache.ChannelLogos();
                programmes.RemoveAll(a => a.EPGFileId == epgFile.Id);
                MemoryCache.Set(programmes);

                List<ChannelLogoDto> channels = MemoryCache.ChannelLogos();
                channels.RemoveAll(a => a.EPGFileId == epgFile.Id);
                MemoryCache.Set(channels);

                List<ChannelLogoDto> channelLogos = MemoryCache.ChannelLogos();
                channelLogos.RemoveAll(a => a.EPGFileId == epgFile.Id);
                MemoryCache.Set(channelLogos);

                List<IconFileDto> programmeIcons = MemoryCache.ProgrammeIcons();
                programmeIcons.RemoveAll(a => a.FileId == epgFile.Id);
                MemoryCache.SetProgrammeLogos(programmeIcons);


                EPGFilesDto ret = Mapper.Map<EPGFilesDto>(epgFile);
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
