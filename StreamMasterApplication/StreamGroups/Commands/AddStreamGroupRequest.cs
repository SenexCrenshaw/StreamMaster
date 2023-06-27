using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;

using StreamMasterDomain.Attributes;
using StreamMasterDomain.Authentication;
using StreamMasterDomain.Configuration;
using StreamMasterDomain.Dto;

using StreamMasterInfrastructure.Extensions;

namespace StreamMasterApplication.StreamGroups.Commands;

[RequireAll]
public record AddStreamGroupRequest(
    string Name,
    int StreamGroupNumber,
    List<int>? VideoStreamIds,
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

        //_ = RuleFor(v => v.StreamGroupVideoStreams).NotNull().NotEmpty();
    }
}

public class AddStreamGroupRequestHandler : IRequestHandler<AddStreamGroupRequest, StreamGroupDto?>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IPublisher _publisher;
    private readonly IConfigFileProvider _configFileProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AddStreamGroupRequestHandler(
        IMapper mapper,
        IConfigFileProvider configFileProvider,
        IPublisher publisher,
         IHttpContextAccessor httpContextAccessor,
        IAppDbContext context
        )
    {
        _publisher = publisher;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _context = context;
        _configFileProvider = configFileProvider;
    }

    public async Task<StreamGroupDto?> Handle(AddStreamGroupRequest command, CancellationToken cancellationToken)
    {
        if (command.StreamGroupNumber<1)
        {
            return null;
        }
            
        int streamGroupNumber=  command.StreamGroupNumber;
        if (_context.StreamGroups.Any(a => a.StreamGroupNumber == streamGroupNumber))
        {
            streamGroupNumber = _context.StreamGroups.Max(a => a.StreamGroupNumber) + 1;
        }

        // List<StreamGroupVideoStream> sgt = _mapper.Map<List<StreamGroupVideoStream>>(command.StreamGroupVideoStreams);
        StreamGroup entity = new()
        {
            Name = command.Name,
            StreamGroupNumber = command.StreamGroupNumber,
          
        };
        
        if ( command.ChannelGroupNames != null && command.ChannelGroupNames.Any())
        {
            var cgs= _context.ChannelGroups.Where(a => command.ChannelGroupNames.Contains(a.Name))  .ToList();
            if (cgs.Any()) {
                entity.ChannelGroups =cgs ; 
            }
        }

        if (command.VideoStreamIds != null && command.VideoStreamIds.Any())
        {
            var vs = _context.VideoStreams.Where(a => command.VideoStreamIds.Contains(a.Id)).ToList();
            if (vs.Any())
            {
                entity.VideoStreams = vs;
            }
        }

        _ = _context.StreamGroups.Add(entity);
        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var url = _httpContextAccessor.GetUrl();

        StreamGroupDto ret = _mapper.Map<StreamGroupDto>(entity);
        var encodedStreamGroupNumber = ret.StreamGroupNumber.EncodeValue128(_configFileProvider.Setting.ServerKey);
        ret.M3ULink = $"{url}/api/streamgroups/m3u/{encodedStreamGroupNumber}.m3u";
        ret.XMLLink = $"{url}/api/streamgroups/epg/{encodedStreamGroupNumber}.xml";
        ret.HDHRLink = $"{url}/api/streamgroups/{encodedStreamGroupNumber}";

        await _publisher.Publish(new StreamGroupUpdateEvent(ret), cancellationToken).ConfigureAwait(false);
        return ret;
    }

}
