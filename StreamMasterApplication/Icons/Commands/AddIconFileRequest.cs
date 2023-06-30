using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

using StreamMasterDomain.Dto;

using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Web;

namespace StreamMasterApplication.Icons.Commands;

public class AddIconFileRequest : IRequest<IconFileDto?>
{
    public string? Description { get; set; }

    public IFormFile? FormFile { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? UrlSource { get; set; }
}

public class AddIconFileRequestValidator : AbstractValidator<AddIconFileRequest>
{
    public AddIconFileRequestValidator()
    {
        _ = RuleFor(v => v.Name)
            .MaximumLength(32)
            .NotEmpty();

        _ = RuleFor(v => v.UrlSource).NotEmpty().When(v => v.FormFile == null);
        _ = RuleFor(v => v.FormFile).NotNull().When(v => string.IsNullOrEmpty(v.UrlSource));
    }
}

public class AddIconFileRequestHandler : IRequestHandler<AddIconFileRequest, IconFileDto?>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;
    private readonly IPublisher _publisher;
    private readonly ISender _sender;

    public AddIconFileRequestHandler(
         IMapper mapper,
         IPublisher publisher,
         ISender sender,
          IMemoryCache memoryCache,
        IAppDbContext context)
    {
        _memoryCache = memoryCache;
        _publisher = publisher;
        _mapper = mapper;
        _context = context;
        _sender = sender;
    }

    public async Task<IconFileDto?> Handle(AddIconFileRequest command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(command.UrlSource) && command.FormFile != null && command.FormFile.Length <= 0)
        {
            return null;
        }

        try
        {
            FileDefinition fd = FileDefinitions.Icon;

            IconFile iconFile;

            if (command.FormFile != null)
            {
                if (!command.FormFile.ContentType.Contains("image/"))
                {
                    return null;
                }
                string ext = "." + command.FormFile.ContentType.Replace("image/", string.Empty);
                string fullName = Path.Combine(fd.DirectoryLocation, command.Name + ext);                

                var nameWithExtension = command.Name + ext;
                
                iconFile = new()
                {
                    Name = command.Name,
                    Source = nameWithExtension,
                    OriginalSource = nameWithExtension,
                    Url = command.Name + ext,
                    ContentType = command.FormFile.ContentType,
                    FileExtension = ext[1..]
                };
                (bool success, Exception? ex) = await FormHelper.SaveFormFileAsync(command.FormFile!, fullName).ConfigureAwait(false);
                if (success)
                {
                    iconFile.LastDownloaded = DateTime.Now;
                    iconFile.FileExists = true;
                }
                else
                {
                    ++iconFile.DownloadErrors;
                }
                _ = _context.Icons.Add(iconFile);
                _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            else if (!string.IsNullOrEmpty(command.UrlSource))
            {
                SettingDto _setting = await _sender.Send(new GetSettings(), cancellationToken).ConfigureAwait(false);

                (iconFile, var isNew) = await IconHelper.AddIcon(command.UrlSource, "", command.Name, _context, _mapper, _setting, FileDefinitions.Icon, cancellationToken).ConfigureAwait(false);
                if (isNew)
                {
                    _memoryCache.ClearIcons();
                }
            }
            else
            {
                return null;
            }

            IconFileDto ret = _mapper.Map<IconFileDto>(iconFile);
            await _publisher.Publish(new IconFileAddedEvent(ret), cancellationToken).ConfigureAwait(false);

            return ret;
        }
        catch (Exception)
        {
        }

        return null;
    }
}
