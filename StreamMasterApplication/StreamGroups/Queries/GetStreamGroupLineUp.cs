using AutoMapper;
using AutoMapper.QueryableExtensions;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using StreamMasterDomain.Attributes;
using StreamMasterDomain.Authentication;
using StreamMasterDomain.Configuration;
using StreamMasterDomain.Dto;

using System.IO;
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
    private readonly IConfigFileProvider _configFileProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly ISender _sender;
    private readonly SettingDto _setting;

    public GetStreamGroupLineUpHandler(
        IAppDbContext context,
        IMapper mapper,
           IConfigFileProvider configFileProvider,
          IHttpContextAccessor httpContextAccessor,
            ISender sender)
    {
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _configFileProvider = configFileProvider;
        _context = context;
        _sender = sender;        
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

            //var request = _httpContextAccessor.HttpContext.Request;
            //var encodedNumbers = request.Path.ToString().Replace("/lineup.json", "");
            //encodedNumbers = encodedNumbers.Substring(encodedNumbers.LastIndexOf('/'));
            var encodedNumbers = command.StreamGroupNumber.EncodeValues128(videoStream.Id, _configFileProvider.Setting.ServerKey);


            string url = GetUrl();
            var videoUrl = $"{url}/api/streamgroups/stream{encodedNumbers}";

            LineUp lu = new()
            {
                GuideNumber = videoStream.User_Tvg_chno.ToString(),
                GuideName = videoStream.User_Tvg_name,
                URL = videoUrl
            };

            ret.Add(lu);
        }
        string jsonString = JsonSerializer.Serialize(ret, new JsonSerializerOptions { WriteIndented = true });
        return jsonString;
    }
    private string GetUrl()
    {
        var request = _httpContextAccessor.HttpContext.Request;
        var scheme = request.Scheme;
        var host = request.Host;

        var url = $"{scheme}://{host}";

        return url;
    }
}
