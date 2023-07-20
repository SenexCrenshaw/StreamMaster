using AutoMapper;
using AutoMapper.QueryableExtensions;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.Common.Extensions;
using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Attributes;
using StreamMasterDomain.Dto;

namespace StreamMasterApplication.ChannelGroups.Commands;

[RequireAll]
public record UpdateChannelGroupRequest(string GroupName, string? NewGroupName, bool? IsHidden, int? Rank, string? Regex) : IRequest<ChannelGroupDto?>
{
}

public class UpdateChannelGroupRequestValidator : AbstractValidator<UpdateChannelGroupRequest>
{
    public UpdateChannelGroupRequestValidator()
    {
        _ = RuleFor(v => v.GroupName).NotNull().NotEmpty();
        _ = RuleFor(v => v.Rank).NotNull().GreaterThan(0);
    }
}

public class UpdateChannelGroupRequestHandler : IRequestHandler<UpdateChannelGroupRequest, ChannelGroupDto?>
{
    private readonly IAppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    private readonly IPublisher _publisher;

    public UpdateChannelGroupRequestHandler(
        IHttpContextAccessor httpContextAccessor,
         IMapper mapper,
           IPublisher publisher,
        IAppDbContext context
        )
    {
        _httpContextAccessor = httpContextAccessor;
        _publisher = publisher;
        _mapper = mapper;
        _context = context;
    }

    public async Task<ChannelGroupDto?> Handle(UpdateChannelGroupRequest request, CancellationToken cancellationToken)
    {
        ChannelGroup? channelGroup = await _context.ChannelGroups.FirstOrDefaultAsync(a => a.Name.ToLower() == request.GroupName.ToLower(), cancellationToken: cancellationToken).ConfigureAwait(false);

        if (channelGroup == null)
        {
            return null;
        }

        List<VideoStreamDto> beforeResults = _context.VideoStreams
         .Where(a => a.User_Tvg_group != null && a.User_Tvg_group.ToLower() == channelGroup.Name.ToLower())
         .AsNoTracking()
         .ProjectTo<VideoStreamDto>(_mapper.ConfigurationProvider).ToList();

        var beforeRegexStreams = await _context.GetVideoStreamsByNamePatternAsync(channelGroup.RegexMatch, cancellationToken).ConfigureAwait(false);
        if (beforeRegexStreams != null)
        {
            var mapped = _mapper.Map<List<VideoStreamDto>>(beforeRegexStreams);
            beforeResults.AddRange(mapped);
        }

        if (request.Rank != null)
        {
            channelGroup.Rank = (int)request.Rank;
        }

        bool isChanged = false;

        if (request.IsHidden != null)
        {
            channelGroup.IsHidden = (bool)request.IsHidden;

            await _context.VideoStreams
                .Where(a => a.User_Tvg_group != null && a.User_Tvg_group.ToLower() == channelGroup.Name.ToLower())
                .ExecuteUpdateAsync(s => s.SetProperty(b => b.IsHidden, (bool)request.IsHidden), cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            isChanged = true;
        }

        if (!string.IsNullOrEmpty(request.NewGroupName))
        {
            await _context.VideoStreams
               .Where(a => a.User_Tvg_group != null && a.User_Tvg_group.ToLower() == channelGroup.Name.ToLower())
               .ExecuteUpdateAsync(s => s.SetProperty(b => b.User_Tvg_group, request.NewGroupName), cancellationToken: cancellationToken)
               .ConfigureAwait(false);

            channelGroup.Name = request.NewGroupName;
            isChanged = true;
        }

        if (!string.IsNullOrEmpty(request.Regex))
        {
            channelGroup.RegexMatch = request.Regex;
        }

        _context.ChannelGroups.Update(channelGroup);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        ChannelGroupDto cgresult = _mapper.Map<ChannelGroupDto>(channelGroup);
        await _publisher.Publish(new UpdateChannelGroupEvent(cgresult), cancellationToken).ConfigureAwait(false);

        if (!isChanged)
        {
            return cgresult;
        }

        List<VideoStreamDto> afterResults = _context.VideoStreams
           .Where(a => a.User_Tvg_group != null && a.User_Tvg_group.ToLower() == channelGroup.Name.ToLower())
           .AsNoTracking()
           .ProjectTo<VideoStreamDto>(_mapper.ConfigurationProvider).ToList();

        var afterRegexStreams = await _context.GetVideoStreamsByNamePatternAsync(channelGroup.RegexMatch, cancellationToken).ConfigureAwait(false);
        if (afterRegexStreams != null)
        {
            var mapped = _mapper.Map<List<VideoStreamDto>>(afterRegexStreams);
            afterResults.AddRange(mapped);
        }

        List<VideoStreamDto> distinctList = new List<VideoStreamDto>();
        if (beforeResults is not null)
        {
            distinctList = beforeResults;
        }

        if (afterResults is not null)
        {
            var existingsIds = distinctList.Select(a => a.Id).ToList();
            var diff = afterResults.Where(a => !existingsIds.Contains(a.Id)).ToList();
            distinctList = distinctList.Concat(diff).ToList();
        }

        if (distinctList.Any())
        {
            var url = _httpContextAccessor.GetUrl();

            var sgs = await _context.GetStreamGroupsByVideoStreamIdsAsync(distinctList.Select(a => a.Id).ToList(), url, cancellationToken).ConfigureAwait(false);
            if (sgs != null && sgs.Any())
            {
                foreach (var sg in sgs)
                {
                    await _publisher.Publish(new StreamGroupUpdateEvent(sg), cancellationToken).ConfigureAwait(false);
                }
            }

            await _publisher.Publish(new UpdateVideoStreamsEvent(distinctList), cancellationToken).ConfigureAwait(false);
        }

        return cgresult;
    }
}
