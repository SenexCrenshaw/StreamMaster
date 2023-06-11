using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Caching.Memory;

using System.Web;

namespace StreamMasterApplication.Icons.Commands;

public class CacheIconsFromVideoStreamsRequest : IRequest<bool>
{
}

public class CacheIconsFromVideoStreamsRequestHandler : IRequestHandler<CacheIconsFromVideoStreamsRequest, bool>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;
    private readonly ISender _sender;

    public CacheIconsFromVideoStreamsRequestHandler(
          IMapper mapper,
           IMemoryCache memoryCache,
         IAppDbContext context, ISender sender)
    {
        _memoryCache = memoryCache;
        _mapper = mapper;
        _context = context;
        _sender = sender;
    }

    public async Task<bool> Handle(CacheIconsFromVideoStreamsRequest command, CancellationToken cancellationToken)
    {
        StreamMasterDomain.Dto.SettingDto _setting = await _sender.Send(new GetSettings(), cancellationToken).ConfigureAwait(false);
        if (!_setting.CacheIcons)
        {
            return false;
        }

        int count = 0;
        var isNew = false;

        List<VideoStream> streams = _context.VideoStreams
            .Where(a =>

                    a.User_Tvg_logo != null &&
                    !a.User_Tvg_logo.Contains(_setting.BaseHostURL) &&
                    a.User_Tvg_logo.Contains("://")

            )
            .ToList();

        if (!streams.Any()) { return false; }

        foreach (VideoStream stream in streams)
        {
            if (cancellationToken.IsCancellationRequested) { return false; }

            ++count;
            if (count % 100 == 0)
            {
                Console.WriteLine($"CacheIconsFromVideoStreamsRequest {count} of {streams.Count}");
            }

            string source = HttpUtility.UrlDecode(stream.Tvg_logo);

            if (source.ToLower().StartsWith("https://github.com/tapiosinn"))
            {
                source = @"https://github.com/tv-logo" + source[28..];
                source = source.Replace("blob", "raw");
                source = source.Replace("master", "main");

                stream.Tvg_logo = source;
                stream.User_Tvg_logo = source;
                _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }

            (_, isNew) = await IconHelper.AddIcon(source, "", stream.User_Tvg_name, _context, _mapper, _setting, FileDefinitions.Icon, cancellationToken).ConfigureAwait(false);
        }

        foreach (VideoStream? channel in _context.VideoStreams.Where(a => a.Tvg_logo.ToLower().StartsWith("https://github.com/tapiosinn")))
        {
            string source = @"https://github.com/tv-logo" + channel.Tvg_logo[28..];
            source = source.Replace("blob", "raw");
            source = source.Replace("master", "main");

            channel.Tvg_logo = source;
            channel.User_Tvg_logo = source;
        }

        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        if (isNew)
        {
            _memoryCache.ClearIcons();
        }

        return true;
    }
}
