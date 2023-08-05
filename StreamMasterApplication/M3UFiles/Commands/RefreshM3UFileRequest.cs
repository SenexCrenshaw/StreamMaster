using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterDomain.Attributes;
using StreamMasterDomain.Dto;

namespace StreamMasterApplication.M3UFiles.Commands;

[RequireAll]
public class RefreshM3UFileRequest : IRequest<M3UFile?>
{
    public int Id { get; set; }
}

public class RefreshM3UFileRequestValidator : AbstractValidator<RefreshM3UFileRequest>
{
    public RefreshM3UFileRequestValidator()
    {
        _ = RuleFor(v => v.Id).NotNull().GreaterThanOrEqualTo(0);
    }
}

public class RefreshM3UFileRequestHandler : BaseMediatorRequestHandler, IRequestHandler<RefreshM3UFileRequest, M3UFile?>
{
    public RefreshM3UFileRequestHandler(ILogger<RefreshM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task<M3UFile?> Handle(RefreshM3UFileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var m3uFile = await Repository.M3UFile.GetM3UFileByIdAsync(request.Id).ConfigureAwait(false);
            if (m3uFile == null)
            {
                return null;
            }

            if (m3uFile.LastDownloadAttempt.AddMinutes(m3uFile.MinimumMinutesBetweenDownloads) < DateTime.Now)
            {
                FileDefinition fd = FileDefinitions.M3U;
                string fullName = Path.Combine(fd.DirectoryLocation, m3uFile.Source);

                if (m3uFile.Url != null && m3uFile.Url.Contains("://"))
                {
                    Logger.LogInformation("Refresh M3U From URL {m3uFile.Url}", m3uFile.Url);

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
                        Logger.LogCritical("Exception M3U From URL {ex}", ex);
                    }
                }

                List<VideoStream>? streams = await m3uFile.GetM3U().ConfigureAwait(false);
                if (streams == null)
                {
                    Logger.LogCritical("Exception M3U {fullName} format is not supported", fullName);
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

                Repository.M3UFile.UpdateM3UFile(m3uFile);
                await Repository.SaveAsync().ConfigureAwait(false);

                M3UFileDto ret = Mapper.Map<M3UFileDto>(m3uFile);
                await Publisher.Publish(new M3UFileAddedEvent(ret), cancellationToken).ConfigureAwait(false);
            }

            return m3uFile;
        }
        catch (Exception)
        {
        }

        return null;
    }
}
