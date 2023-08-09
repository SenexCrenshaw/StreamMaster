using AutoMapper;
using AutoMapper.QueryableExtensions;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Extensions;
using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Attributes;
using StreamMasterDomain.Authentication;
using StreamMasterDomain.Dto;

using System.Text.Json;
using System.Web;

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

public class GetStreamGroupLineUpHandler : BaseMemoryRequestHandler, IRequestHandler<GetStreamGroupLineUp, string>
{
    protected Setting _setting = FileUtil.GetSetting();

    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetStreamGroupLineUpHandler(IHttpContextAccessor httpContextAccessor, ILogger<DeleteM3UFileHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> Handle(GetStreamGroupLineUp request, CancellationToken cancellationToken)
    {
        string requestPath = _httpContextAccessor.HttpContext.Request.Path.Value.ToString();
        byte[]? iv = requestPath.GetIVFromPath(128);
        if (iv == null)
        {
            return "";
        }

        string url = _httpContextAccessor.GetUrl();
        List<LineUp> ret = new();

        IEnumerable<VideoStreamDto> videoStreams;
        if (request.StreamGroupNumber > 0)
        {
            StreamGroupDto? streamGroup = await Repository.StreamGroup.GetStreamGroupDtoByStreamGroupNumber(request.StreamGroupNumber, url, cancellationToken).ConfigureAwait(false);
            if (streamGroup == null)
            {
                return "";
            }
            videoStreams = streamGroup.ChildVideoStreams.Where(a => !a.IsHidden);
        }
        else
        {
            videoStreams = Repository.VideoStream.GetVideoStreamsHidden()
                .ProjectTo<VideoStreamDto>(Mapper.ConfigurationProvider);
        }

        if (!videoStreams.Any())
        {
            return JsonSerializer.Serialize(ret);
        }

        foreach (VideoStreamDto videoStream in videoStreams)
        {

            if (_setting.M3UIgnoreEmptyEPGID &&
            (string.IsNullOrEmpty(videoStream.User_Tvg_ID) || videoStream.User_Tvg_ID.ToLower() == "dummy"))
            {
                continue;
            }


            string videoUrl = videoStream.Url;

            string encodedNumbers = request.StreamGroupNumber.EncodeValues128(videoStream.Id, _setting.ServerKey, iv);

            string encodedName = HttpUtility.HtmlEncode(videoStream.User_Tvg_name).Trim().Replace(" ", "_");
            videoUrl = $"{url}/api/videostreams/stream/{encodedNumbers}/{encodedName}";

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
