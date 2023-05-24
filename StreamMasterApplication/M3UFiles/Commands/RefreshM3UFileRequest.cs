using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterDomain.Attributes;
using StreamMasterDomain.Dto;

namespace StreamMasterApplication.M3UFiles.Commands;

[RequireAll]
public class RefreshM3UFileRequest : IRequest<M3UFilesDto?>
{
    //public bool ForceDownload { get; set; }
    public int M3UFileID { get; set; }
}

public class RefreshM3UFileRequestValidator : AbstractValidator<RefreshM3UFileRequest>
{
    public RefreshM3UFileRequestValidator()
    {
        _ = RuleFor(v => v.M3UFileID).NotNull().GreaterThanOrEqualTo(0);
    }
}

public class RefreshM3UFileRequestHandler : IRequestHandler<RefreshM3UFileRequest, M3UFilesDto?>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<RefreshM3UFileRequest> _logger;
    private readonly IMapper _mapper;
    private readonly IPublisher _publisher;

    public RefreshM3UFileRequestHandler(
        ILogger<RefreshM3UFileRequest> logger,
        IMapper mapper,
        IPublisher publisher,
        IAppDbContext context)
    {
        _publisher = publisher;
        _mapper = mapper;
        _logger = logger;
        _context = context;
    }

    public async Task<M3UFilesDto?> Handle(RefreshM3UFileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            M3UFile? m3uFile = await _context.M3UFiles.FindAsync(new object?[] { request.M3UFileID, cancellationToken }, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (m3uFile == null)
            {
                return null;
            }

            M3UFilesDto m3uFileDto = _mapper.Map<M3UFilesDto>(m3uFile);

            if (m3uFile.LastDownloadAttempt.AddMinutes(m3uFile.MinimumMinutesBetweenDownloads) < DateTime.Now)
            {
                FileDefinition fd = FileDefinitions.M3U;
                string fullName = Path.Combine(fd.DirectoryLocation, m3uFile.Source);

                if (m3uFile.Url != null && m3uFile.Url.Contains("://"))
                {
                    _logger.LogInformation("Refresh M3U From URL {m3uFile.Url}", m3uFile.Url);

                    m3uFile.LastDownloadAttempt = DateTime.Now;

                    (bool success, Exception? ex) = await FileUtil.DownloadUrlAsync(m3uFile.Url, fullName, cancellationToken).ConfigureAwait(false);
                    if (success)
                    {
                        m3uFile.DownloadErrors = 0;
                        m3uFile.LastDownloaded = File.GetLastWriteTime(fullName);
                        m3uFile.FileExists = true;
                    }
                    else
                    {
                        ++m3uFile.DownloadErrors;
                        _logger.LogCritical("Exception M3U From URL {ex}", ex);
                    }
                }

                List<VideoStream>? streams = await m3uFile.GetM3U().ConfigureAwait(false);
                if (streams == null)
                {
                    _logger.LogCritical("Exception M3U {fullName} format is not supported", fullName);
                    //Bad M3U
                    if (File.Exists(fullName))
                    {
                        File.Delete(fullName);
                    }
                    return null;
                }

                if (m3uFile.StationCount != streams.Count)
                {
                    m3uFile.StationCount = streams.Count;
                }

                _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                M3UFilesDto ret = _mapper.Map<M3UFilesDto>(m3uFile);
                await _publisher.Publish(new M3UFileAddedEvent(ret), cancellationToken).ConfigureAwait(false);

                return ret;
            }

            return m3uFileDto;
        }
        catch (Exception)
        {
        }

        return null;
    }
}
