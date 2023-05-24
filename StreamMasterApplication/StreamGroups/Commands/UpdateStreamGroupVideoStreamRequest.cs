//using AutoMapper;

//using FluentValidation;

//using MediatR;

//using Microsoft.EntityFrameworkCore;

//using StreamMasterDomain.Dto;

//namespace StreamMasterApplication.StreamGroups.Commands;


//public record UpdateStreamGroupVideoStreamRequest(int StreamGroupId, int? Tvg_chno,
//     string? Tvg_name,
//     int VideoStreamId) : IRequest<StreamGroupDto?>
//{
//}

//public class UpdateStreamGroupVideoStreamRequestValidator : AbstractValidator<UpdateStreamGroupVideoStreamRequest>
//{
//    public UpdateStreamGroupVideoStreamRequestValidator()
//    {
//        _ = RuleFor(v => v.StreamGroupId)
//           .NotNull()
//           .GreaterThanOrEqualTo(0);
//    }
//}

//public class UpdateStreamGroupVideoStreamRequestHandler : IRequestHandler<UpdateStreamGroupVideoStreamRequest, StreamGroupDto?>
//{
//    private readonly IAppDbContext _context;
//    private readonly IMapper _mapper;
//    private readonly IPublisher _publisher;

//    public UpdateStreamGroupVideoStreamRequestHandler(
//        IMapper mapper,
//         IPublisher publisher,
//        IAppDbContext context)
//    {
//        _publisher = publisher;
//        _mapper = mapper;
//        _context = context;
//    }

//    public async Task<StreamGroupDto?> Handle(UpdateStreamGroupVideoStreamRequest request, CancellationToken cancellationToken)
//    {
//        try
//        {
//            StreamGroup? streamGroup = _context.StreamGroups
//                .Include(a => a.VideoStreams)
//                .FirstOrDefault(a => a.Id == request.StreamGroupId);

//            if (streamGroup == null || streamGroup.VideoStreams == null || !streamGroup.VideoStreams.Any())
//            {
//                return null;
//            }

//            //var streamGroupStream = streamGroup.VideoStreams.FirstOrDefault(a => a.VideoStreamId == request.VideoStreamId);
//            //if (streamGroupStream == null)
//            //{
//            //    return null;
//            //}

//            var videoStream = _context.VideoStreams.FirstOrDefault(a => a.Id == request.VideoStreamId);
//            if (videoStream == null)
//            {
//                return null;
//            }

//            //if (request.Tvg_chno != null && request.Tvg_chno != videoStream.StreamGroup_Tvg_chno)
//            //{
//            //    videoStream.StreamGroup_Tvg_chno = (int)request.Tvg_chno;
//            //}

//            //if (!string.IsNullOrEmpty(request.Tvg_name) && request.Tvg_name != videoStream.StreamGroup_Tvg_name)
//            //{
//            //    videoStream.StreamGroup_Tvg_name = request.Tvg_name;
//            //}

//            //if (!string.IsNullOrEmpty(sg.Tvg_logo))
//            //{
//            //    stream.StreamGroup_Tvg_logo = sg.Tvg_logo;
//            //}

//            _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

//            StreamGroupDto ret = _mapper.Map<StreamGroupDto>(streamGroup);
//            var retVideoStream = _mapper.Map<VideoStreamDto>(videoStream);

//            //if (!string.IsNullOrEmpty(videoStream.StreamGroup_Tvg_name))
//            //{
//            //    videoStream.User_Tvg_name = videoStream.StreamGroup_Tvg_name;
//            //}

//            //if (videoStream.StreamGroup_Tvg_chno != null)
//            //{
//            //    videoStream.User_Tvg_chno = (int)videoStream.StreamGroup_Tvg_chno;
//            //}

//            await _publisher.Publish(new StreamGroupUpdateEvent(ret), cancellationToken).ConfigureAwait(false);
//            await _publisher.Publish(new UpdateVideoStreamEvent(retVideoStream), cancellationToken).ConfigureAwait(false);
//            return ret;
//        }
//        catch (Exception)
//        {
//        }

//        return null;
//    }
//}
