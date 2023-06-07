using AutoMapper;
using AutoMapper.QueryableExtensions;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.StreamGroups.Queries;

public record GetStreamGroupByStreamNumber(int StreamGroupNumber) : IRequest<StreamGroupDto?>;

public class GetStreamGroupByStreamNumberValidator : AbstractValidator<GetStreamGroupByStreamNumber>
{
    public GetStreamGroupByStreamNumberValidator()
    {
        _ = RuleFor(v => v.StreamGroupNumber)
            .NotNull().GreaterThanOrEqualTo(0);
    }
}

internal class GetStreamGroupByStreamNumberHandler : IRequestHandler<GetStreamGroupByStreamNumber, StreamGroupDto?>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;

    public GetStreamGroupByStreamNumberHandler(
         IMapper mapper,
        IAppDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<StreamGroupDto?> Handle(GetStreamGroupByStreamNumber request, CancellationToken cancellationToken = default)
    {
        StreamGroup? streamGroup = await _context.StreamGroups
            .Include(a => a.VideoStreams)
            .Include(a => a.ChannelGroups)
            .FirstOrDefaultAsync(a => a.StreamGroupNumber == request.StreamGroupNumber, cancellationToken: cancellationToken).ConfigureAwait(false);


        if (streamGroup == null)
            return null;

        var ret = _mapper.Map<StreamGroupDto>(streamGroup);

        var existingIds = streamGroup.VideoStreams.Select(a => a.Id).ToList();

        foreach (var channegroup in streamGroup.ChannelGroups)
        {
            var streams = _context.VideoStreams
                .Where(a => !existingIds.Contains(a.Id) && a.User_Tvg_group == channegroup.Name)
                .AsNoTracking()
                .ProjectTo<VideoStreamDto>(_mapper.ConfigurationProvider)
                .ToList();
            foreach (var stream in streams)
            {
                stream.IsReadOnly = true;
            }
            ret.VideoStreams.AddRange(streams);
        }

        return ret;
    }
}
