using AutoMapper;
using AutoMapper.QueryableExtensions;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.Common.Extensions;

using StreamMasterDomain.Attributes;
using StreamMasterDomain.Authentication;
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
    protected Setting _setting = FileUtil.GetSetting();

    private readonly IAppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public GetStreamGroupLineUpHandler(
        IAppDbContext context,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor
        )
    {
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _context = context;
    }

    public async Task<string> Handle(GetStreamGroupLineUp request, CancellationToken cancellationToken)
    {
        var requestPath = _httpContextAccessor.HttpContext.Request.Path.Value.ToString();
        var iv = requestPath.GetIVFromPath(128);
        if (iv == null)
        {
            return "";
        }

        string url = _httpContextAccessor.GetUrl();
        List<LineUp> ret = new();

        List<VideoStreamDto> videoStreams = new();
        if (request.StreamGroupNumber > 0)
        {
            var streamGroup = await _context.GetStreamGroupDtoByStreamGroupNumber(request.StreamGroupNumber, url, cancellationToken).ConfigureAwait(false);
            if (streamGroup == null)
            {
                return "";
            }
            videoStreams = streamGroup.ChildVideoStreams.Where(a => !a.IsHidden).ToList();
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

            var encodedNumbers = request.StreamGroupNumber.EncodeValues128(videoStream.Id, _setting.ServerKey, iv);

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
