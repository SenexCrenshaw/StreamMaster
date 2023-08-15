using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Extensions;
using StreamMasterApplication.M3UFiles.Commands;
using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Attributes;
using StreamMasterDomain.Authentication;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.StreamGroups.Commands;

[RequireAll]
public record AddStreamGroupRequest(
    string Name,
    int StreamGroupNumber,
    List<VideoStreamIsReadOnly>? VideoStreams,
    List<string>? ChannelGroupNames
    ) : IRequest<StreamGroupDto?>
{
}

public class AddStreamGroupRequestValidator : AbstractValidator<AddStreamGroupRequest>
{
    public AddStreamGroupRequestValidator()
    {
        _ = RuleFor(v => v.StreamGroupNumber)
            .NotEmpty()
            .GreaterThan(0);

        _ = RuleFor(v => v.Name)
           .MaximumLength(32)
           .NotEmpty();
    }
}

public class AddStreamGroupRequestHandler : BaseMediatorRequestHandler, IRequestHandler<AddStreamGroupRequest, StreamGroupDto?>
{
    protected Setting _setting = FileUtil.GetSetting();
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AddStreamGroupRequestHandler(IHttpContextAccessor httpContextAccessor, ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<StreamGroupDto?> Handle(AddStreamGroupRequest command, CancellationToken cancellationToken)
    {
        if (command.StreamGroupNumber < 0)
        {
            return null;
        }

        int streamGroupNumber = 1;
        if (Repository.StreamGroup.GetAllStreamGroups().Any())
        {
            streamGroupNumber = Repository.StreamGroup.GetAllStreamGroups().Max(a => a.StreamGroupNumber) + 1;
        }

        StreamGroup entity = new()
        {
            Name = command.Name,
            StreamGroupNumber = command.StreamGroupNumber,
        };

        Repository.StreamGroup.CreateStreamGroup(entity);
        await Repository.SaveAsync().ConfigureAwait(false);

        if (command.ChannelGroupNames != null && command.ChannelGroupNames.Any())
        {
            IQueryable<ChannelGroup> cgs = Repository.ChannelGroup.GetAllChannelGroups().Where(a => command.ChannelGroupNames.Contains(a.Name));
            if (cgs.Any())
            {
                foreach (ChannelGroup? cg in cgs)
                {
                    await Repository.StreamGroup.AddChannelGroupToStreamGroupAsync(entity.Id, cg.Id, cancellationToken);
                }
            }
        }

        //if (command.VideoStreamIds != null && command.VideoStreamIds.Any())
        //{
        //    var vss = _context.VideoStreams.Where(a => command.VideoStreamIds.Contains(a.Id)).ToList();
        //    if (vss.Any())
        //    {
        //        for (int index = 0; index < vss.Count; index++)
        //        {
        //            VideoStream? vs = vss[index];
        //            await _context.AddOrUpdatVideoStreamToStreamGroupAsync(entity.Id, vs.Id, false, cancellationToken);
        //        }
        //    }
        //}

        string url = _httpContextAccessor.GetUrl();

        StreamGroupDto ret = Mapper.Map<StreamGroupDto>(entity);
        string encodedStreamGroupNumber = ret.StreamGroupNumber.EncodeValue128(_setting.ServerKey);
        ret.M3ULink = $"{url}/api/streamgroups/m3u/{encodedStreamGroupNumber}.m3u";
        ret.XMLLink = $"{url}/api/streamgroups/epg/{encodedStreamGroupNumber}.xml";
        ret.HDHRLink = $"{url}/api/streamgroups/{encodedStreamGroupNumber}";

        await Publisher.Publish(new StreamGroupUpdateEvent(), cancellationToken).ConfigureAwait(false);

        StreamGroupDto? streamGroup = await Repository.StreamGroup.GetStreamGroupDtoByStreamGroupNumber(ret.Id, url, cancellationToken).ConfigureAwait(false);
        if (streamGroup is not null && streamGroup.ChildVideoStreams.Any())
        {
            await Publisher.Publish(new UpdateVideoStreamsEvent(), cancellationToken).ConfigureAwait(false);
        }

        return ret;
    }
}
