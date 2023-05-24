using AutoMapper;
using AutoMapper.QueryableExtensions;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using StreamMasterDomain.Attributes;
using StreamMasterDomain.Dto;

using System.Text.Json;

namespace StreamMasterApplication.StreamGroups.Queries;

[RequireAll]
public record GetStreamGroupLineUp(int StreamGroupNumber) : IRequest<string>;

public class GetStreamGroupLineUpValidator : AbstractValidator<GetStreamGroupLineUp>
{
    public GetStreamGroupLineUpValidator()
    {
        _ = RuleFor(v => v.StreamGroupNumber)
            .NotNull().GreaterThanOrEqualTo(0);
    }
}

public class GetStreamGroupLineUpHandler : IRequestHandler<GetStreamGroupLineUp, string>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly ISender _sender;
    private readonly SettingDto _setting;

    public GetStreamGroupLineUpHandler(
        IAppDbContext context,
        IMapper mapper,
            ISender sender)
    {
        _mapper = mapper;
        _context = context;
        _sender = sender;
        _setting = sender.Send(new GetSettings()).Result;
    }

    public async Task<string> Handle(GetStreamGroupLineUp command, CancellationToken cancellationToken)
    {
        List<LineUp> ret = new();

        List<VideoStreamDto> videoStreams = new();
        if (command.StreamGroupNumber > 0)
        {
            StreamGroupDto? sg = await _sender.Send(new GetStreamGroupByStreamNumber(command.StreamGroupNumber), cancellationToken).ConfigureAwait(false);
            if (sg == null)
            {
                return "";
            }
            videoStreams = sg.VideoStreams.Where(a => !a.IsHidden).ToList();
        }
        else
        {
            videoStreams = _context.VideoStreams
                .Where(a => !a.IsHidden)
                .AsNoTracking()
                .ProjectTo<VideoStreamDto>(_mapper.ConfigurationProvider)
                .ToList();
        }

        if (!videoStreams.Any())
        {
            return JsonSerializer.Serialize(ret);
        }

        foreach (var videoStream in videoStreams)
        {
            //VideoStream? videoStream = await _context.VideoStreams
            //    .AsNoTracking()
            //    .FirstOrDefaultAsync(a => !a.IsHidden && a.Id == streamGroupVideoStream.Id)
            //    .ConfigureAwait(false);

            //if (videoStream == null)
            //{
            //    continue;
            //}

            // var channel = await _context.IPTVChannels.Include(a =>
            // a.M3UStreamRanks).FirstOrDefaultAsync(a => !a.IsHidden && a.Id ==
            // sgChannel.IPTVChannelId, cancellationToken).ConfigureAwait(false);

            //VideoStreamDto videoStreamDto = _mapper.Map<VideoStreamDto>(videoStream);
            //videoStreamDto.MergeStreamGroupChannel();

            LineUp lu = new()
            {
                GuideNumber = videoStream.User_Tvg_chno.ToString(),
                GuideName = videoStream.User_Tvg_name,
                URL = $"{_setting.BaseHostURL}api/streamgroups/{command.StreamGroupNumber}/stream/{videoStream.Id}"
            };

            ret.Add(lu);
        }
        string jsonString = JsonSerializer.Serialize(ret, new JsonSerializerOptions { WriteIndented = true });
        return jsonString;
    }
}
