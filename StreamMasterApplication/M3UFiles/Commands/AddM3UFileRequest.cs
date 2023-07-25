using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Dto;

using System.ComponentModel.DataAnnotations;
using System.Web;

namespace StreamMasterApplication.M3UFiles.Commands;

public class AddM3UFileRequest : IRequest<M3UFilesDto?>
{
    public string? Description { get; set; }
    public IFormFile? FormFile { get; set; }

    public int MaxStreamCount { get; set; }

    public string? MetaData { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public int? StartingChannelNumber { get; set; }
    public string? UrlSource { get; set; }
}

public class AddM3UFileRequestValidator : AbstractValidator<AddM3UFileRequest>
{
    public AddM3UFileRequestValidator()
    {
        _ = RuleFor(v => v.Name)
            .MaximumLength(32)
            .NotEmpty();

        _ = RuleFor(v => v.UrlSource).NotEmpty().When(v => v.FormFile == null);
        _ = RuleFor(v => v.FormFile).NotNull().When(v => string.IsNullOrEmpty(v.UrlSource));
    }
}

public class AddM3UFileRequestHandler : IRequestHandler<AddM3UFileRequest, M3UFilesDto?>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<AddM3UFileRequestHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IPublisher _publisher;

    public AddM3UFileRequestHandler(
        ILogger<AddM3UFileRequestHandler> logger,
     IMapper mapper,
         IPublisher publisher,
        IAppDbContext context)
    {
        _logger = logger;
        _publisher = publisher;
        _mapper = mapper;
        _context = context;
    }

    public async Task<M3UFilesDto?> Handle(AddM3UFileRequest command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(command.UrlSource) && command.FormFile != null && command.FormFile.Length <= 0)
        {
            return null;
        }

        Setting setting = FileUtil.GetSetting();
        try
        {
            FileDefinition fd = FileDefinitions.M3U;
            string fullName = Path.Combine(fd.DirectoryLocation, command.Name + fd.FileExtension);

            M3UFile m3UFile = new()
            {
                Description = command.Description ?? "",
                Name = command.Name,
                Source = command.Name + fd.FileExtension,
                StartingChannelNumber = command.StartingChannelNumber == null ? 1 : (int)command.StartingChannelNumber,
            };

            if (command.FormFile != null)
            {
                _logger.LogInformation("Adding M3U From Form: {fullName}", fullName);
                (bool success, Exception? ex) = await FormHelper.SaveFormFileAsync(command.FormFile!, fullName).ConfigureAwait(false);
                if (success)
                {
                    m3UFile.LastDownloaded = File.GetLastWriteTime(fullName);
                    m3UFile.FileExists = true;
                }
                else
                {
                    _logger.LogCritical("Exception M3U From Form {ex}", ex);
                    return null;
                }
            }
            else if (!string.IsNullOrEmpty(command.UrlSource))
            {
                string source = HttpUtility.UrlDecode(command.UrlSource);
                m3UFile.Url = source;
                m3UFile.LastDownloadAttempt = DateTime.Now;

                _logger.LogInformation("Add M3U From URL {command.UrlSource}", command.UrlSource);
                (bool success, Exception? ex) = await FileUtil.DownloadUrlAsync(source, fullName, cancellationToken).ConfigureAwait(false);
                if (success)
                {
                    m3UFile.LastDownloaded = File.GetLastWriteTime(fullName);
                    m3UFile.FileExists = true;
                }
                else
                {
                    ++m3UFile.DownloadErrors;
                    _logger.LogCritical("Exception M3U From URL {ex}", ex);
                }
            }

            m3UFile.MaxStreamCount = command.MaxStreamCount;

            var streams = await m3UFile.GetM3U().ConfigureAwait(false);
            if (streams == null || streams.Count == 0)
            {
                _logger.LogCritical("Exception M3U {fullName} format is not supported", fullName);
                //Bad M3U
                if (File.Exists(fullName))
                {
                    File.Delete(fullName);
                }
                return null;
            }

            if (m3UFile.StationCount != streams.Count)
            {
                m3UFile.StationCount = streams.Count;
            }

            _ = _context.M3UFiles.Add(m3UFile);
            _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            M3UFilesDto ret = _mapper.Map<M3UFilesDto>(m3UFile);
            await _publisher.Publish(new M3UFileAddedEvent(ret), cancellationToken).ConfigureAwait(false);

            return ret;
        }
        catch (Exception exception)
        {
            _logger.LogCritical("Exception M3U From Form {exception}", exception);
        }
        return null;
    }
}
