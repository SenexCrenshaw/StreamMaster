//using AutoMapper;

//using FluentValidation;

//using MediatR;

//using Microsoft.EntityFrameworkCore;

//using StreamMasterDomain.Dto;

//namespace StreamMasterApplication.StreamGroups.Commands;

//public class StreamGroupVideoStreamRequest
//{
//    public int Tvg_chno { get; set; }
//    public string Tvg_name { get; set; }
//    public int VideoStreamId { get; set; }
//}

//public record UpdateStreamGroupChannelRequest(int StreamGroupId, List<StreamGroupVideoStreamRequest> StreamGroupVideoStreamRequests) : IRequest<StreamGroupVideoStreamDto?>
//{ }

//public class UpdateStreamGroupChannelRequestValidator : AbstractValidator<UpdateStreamGroupChannelRequest>
//{
//    public UpdateStreamGroupChannelRequestValidator()
//    {
//        _ = RuleFor(v => v.StreamGroupId).NotNull().GreaterThan(0);
//        _ = RuleFor(v => v.StreamGroupVideoStreamRequests).NotNull();
//    }
//}

//public class UpdateStreamGroupChannelRequestHandler : IRequestHandler<UpdateStreamGroupChannelRequest, StreamGroupVideoStreamDto?>
//{
//    private readonly IAppDbContext _context;
//    private readonly IMapper _mapper;
//    private readonly IPublisher _publisher;

// public UpdateStreamGroupChannelRequestHandler( IMapper mapper, IPublisher
// publisher, IAppDbContext context) { _publisher = publisher; _mapper = mapper;
// _context = context; }

// public async Task<StreamGroupVideoStreamDto?>
// Handle(UpdateStreamGroupChannelRequest request, CancellationToken
// cancellationToken) { try { if (request.StreamGroupVideoStreamRequests.Count
// == 0) { return null; }

// StreamGroup? streamGroup = await _context.StreamGroups .Include(a =>
// a.StreamGroupVideoStreams) .FirstOrDefaultAsync(a => a.Id ==
// request.StreamGroupId, cancellationToken: cancellationToken) .ConfigureAwait(false);

// if (streamGroup == null) { return null; }

// streamGroup.StreamGroupVideoStreams ??= new List<StreamGroupVideoStream>();

// foreach (var arg in request.StreamGroupVideoStreamRequests) { if
// (streamGroup.StreamGroupVideoStreams != null) { var test =
// streamGroup.StreamGroupVideoStreams.FirstOrDefault(a => a.VideoStreamId ==
// arg.VideoStreamId); if (test != null) { test.Tvg_chno = arg.Tvg_chno;
// test.Tvg_name = arg.Tvg_name; } } else {
// streamGroup.StreamGroupVideoStreams.Add(new StreamGroupVideoStream { Tvg_chno
// = arg.Tvg_chno, Tvg_name = arg.Tvg_name, VideoStreamId = arg.VideoStreamId,
// StreamGroupId = request.StreamGroupId }); } }

// _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

// StreamGroupDto ret = _mapper.Map<StreamGroupDto>(streamGroup);

// await _publisher.Publish(new StreamGroupUpdateEvent(ret),
// cancellationToken).ConfigureAwait(false); return
// _mapper.Map<StreamGroupVideoStreamDto>(ret); } catch (Exception) { }

//        return null;
//    }
//}
