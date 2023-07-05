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

using StreamMasterInfrastructure.Extensions;

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
    protected Setting _setting = FileUtil.GetSetting();

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly ISender _sender;

    public GetStreamGroupLineUpHandler(
        IAppDbContext context,
        IMapper mapper,
          IHttpContextAccessor httpContextAccessor,
            ISender sender)
    {
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;

        _context = context;
        _sender = sender;
    }

    public async Task<string> Handle(GetStreamGroupLineUp command, CancellationToken cancellationToken)
    {
        var requestPath = _httpContextAccessor.HttpContext.Request.Path.Value.ToString();
        var iv = requestPath.GetIVFromPath(128);
        if (iv == null)
        {
            return "";
        }

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
            string videoUrl = videoStream.Url;

            string url = _httpContextAccessor.GetUrl();

            var encodedNumbers = command.StreamGroupNumber.EncodeValues128(videoStream.Id, _setting.ServerKey, iv);

            videoUrl = $"{url}/api/streamgroups/stream/{encodedNumbers}/{videoStream.User_Tvg_name.Replace(" ", "_")}";

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
}