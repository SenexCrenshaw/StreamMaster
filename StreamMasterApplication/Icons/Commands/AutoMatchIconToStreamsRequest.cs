using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Caching.Memory;

using StreamMasterApplication.Icons.Queries;
using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Icons.Commands;

public record AutoMatchIconToStreamsRequest(List<string> Ids) : IRequest<IconFileDto?>
{
}

public class AutoMatchIconToStreamsRequestValidator : AbstractValidator<AutoMatchIconToStreamsRequest>
{
    public AutoMatchIconToStreamsRequestValidator()
    {
        _ = RuleFor(v => v.Ids).NotNull().NotEmpty();
    }
}

public class AutoMatchIconToStreamsRequestHandler : IRequestHandler<AutoMatchIconToStreamsRequest, IconFileDto?>
{
    private readonly IAppDbContext _context;

    private readonly IMapper _mapper;

    private readonly IMemoryCache _memoryCache;

    private readonly IPublisher _publisher;

    private readonly ISender _sender;

    public AutoMatchIconToStreamsRequestHandler(
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

    public static double GetWeightedMatch(string sentence1, string sentence2)
    {
        // Convert sentences to lowercase and remove punctuation
        string normalizedSentence1 = NormalizeString(sentence1);
        string normalizedSentence2 = NormalizeString(sentence2);

        // Split sentences into individual words
        string[] words1 = normalizedSentence1.Split(' ');
        string[] words2 = normalizedSentence2.Split(' ');

        // Calculate the intersection of words between the two sentences
        var wordIntersection = words1.Intersect(words2, StringComparer.OrdinalIgnoreCase);

        // Calculate the weighted match
        double weightedMatch = (double)wordIntersection.Count() / words1.Length;

        return weightedMatch;
    }

    public async Task<IconFileDto?> Handle(AutoMatchIconToStreamsRequest request, CancellationToken cancellationToken)
    {
        if (request.Ids == null || request.Ids.Count == 0)
        {
            return null;
        }
        var icons = await _sender.Send(new GetIcons(), cancellationToken).ConfigureAwait(false);// CacheKeys.Icons(); //await _sender.Send(new GetIcons(), cancellationToken).ConfigureAwait(false);
        //var streams = _context.VideoStreams.Where(a => request.Ids.Contains(a.Id) && string.IsNullOrEmpty(a.User_Tvg_logo)).ToList();
        var streams = _context.VideoStreams.Where(a => request.Ids.Contains(a.Id)).ToList();
        List<VideoStreamDto> videoStreamDtos = new();

        foreach (var stream in streams)
        {
            var icon = icons.FirstOrDefault(a => a.Name.Equals(stream.User_Tvg_name, StringComparison.CurrentCultureIgnoreCase));
            if (icon != null)
            {
                stream.User_Tvg_logo = icon.Source;
                videoStreamDtos.Add(_mapper.Map<VideoStreamDto>(stream));
                continue;
            }

            var topCheckIcon = icons.Where(a => a.Name.ToLower().Contains(stream.User_Tvg_name.ToLower()))
                         .OrderByDescending(a => GetWeightedMatch(stream.User_Tvg_name, a.Name))
                         .Select(a => new { Icon = a, Weight = GetWeightedMatch(stream.User_Tvg_name, a.Name) })
                         .FirstOrDefault();

            if (topCheckIcon != null && topCheckIcon.Weight > 0.5 && stream.User_Tvg_logo != topCheckIcon.Icon.Source)
            {
                stream.User_Tvg_logo = topCheckIcon.Icon.Source;
                var videoStreamDto = _mapper.Map<VideoStreamDto>(stream);
                videoStreamDtos.Add(videoStreamDto);
                break;
            }
        }
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        if (videoStreamDtos.Any())
        {
            await _publisher.Publish(new UpdateVideoStreamsEvent(videoStreamDtos), cancellationToken).ConfigureAwait(false);
        }

        return null;
    }

    private static string NormalizeString(string input)
    {
        // Remove punctuation characters
        string normalized = new string(input.Where(c => !char.IsPunctuation(c)).ToArray());

        // Convert to lowercase
        normalized = normalized.ToLower();

        return normalized;
    }

    private class WeightedMatch
    {
        public string Name { get; set; }
        public double Weight { get; set; }
    }
}
