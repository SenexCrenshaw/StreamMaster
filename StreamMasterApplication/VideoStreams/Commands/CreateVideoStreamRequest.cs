using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;
using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.VideoStreams.Commands;

public record CreateVideoStreamRequest(
     string Tvg_name,
    int? Tvg_chno,
    string? Tvg_group,
    string? Tvg_ID,
    string? Tvg_logo,
    string? Url,
    int? IPTVChannelHandler,
    bool? createChannel,
      List<ChildVideoStreamDto>? ChildVideoStreams
    ) : IRequest<VideoStreamDto?>
{
}

public class CreateVideoStreamRequestValidator : AbstractValidator<CreateVideoStreamRequest>
{
    public CreateVideoStreamRequestValidator()
    {
        _ = RuleFor(v => v.Tvg_name).NotNull().NotEmpty();
    }
}

public class CreateVideoStreamRequestHandler : BaseMediatorRequestHandler, IRequestHandler<CreateVideoStreamRequest, VideoStreamDto?>
{

    public CreateVideoStreamRequestHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task<VideoStreamDto?> Handle(CreateVideoStreamRequest request, CancellationToken cancellationToken)
    {
        Setting setting = FileUtil.GetSetting();

        string group = string.IsNullOrEmpty(request.Tvg_group) ? "(None)" : request.Tvg_group;
        string epgId = string.IsNullOrEmpty(request.Tvg_ID) ? "" : request.Tvg_ID;

        VideoStream videoStream = new()
        {
            Id = IdConverter.GetID(),
            IsUserCreated = true,

            Tvg_chno = request.Tvg_chno is null ? 0 : (int)request.Tvg_chno,
            User_Tvg_chno = request.Tvg_chno is null ? 0 : (int)request.Tvg_chno,

            Tvg_group = group,
            User_Tvg_group = group,

            Tvg_ID = epgId,
            User_Tvg_ID = epgId,

            Tvg_logo = request.Tvg_logo is null ? setting.StreamMasterIcon : request.Tvg_logo,
            User_Tvg_logo = request.Tvg_logo is null ? setting.StreamMasterIcon : request.Tvg_logo,

            Tvg_name = request.Tvg_name,
            User_Tvg_name = request.Tvg_name,

            Url = request.Url ?? string.Empty,
            User_Url = request.Url ?? string.Empty
        };

        Repository.VideoStream.Create(videoStream);
        await Repository.SaveAsync().ConfigureAwait(false);

        if (request.ChildVideoStreams != null)
        {
            await Repository.VideoStream.SynchronizeChildRelationships(videoStream, request.ChildVideoStreams, cancellationToken).ConfigureAwait(false);
        }

        VideoStreamDto ret = Mapper.Map<VideoStreamDto>(videoStream);


        await Publisher.Publish(new CreateVideoStreamEvent(ret), cancellationToken).ConfigureAwait(false);
        return ret;
    }
}
