using FluentValidation;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.Common.Extensions;

using StreamMasterDomain.Authentication;

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
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetStreamGroupLineUpHandler(IHttpContextAccessor httpContextAccessor, ILogger<GetStreamGroupLineUp> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
  : base(logger, repository, mapper, publisher, sender, hubContext, memoryCache) { _httpContextAccessor = httpContextAccessor; }


    public async Task<string> Handle(GetStreamGroupLineUp request, CancellationToken cancellationToken)
    {
        string requestPath = _httpContextAccessor.GetUrlWithPathValue();
        byte[]? iv = requestPath.GetIVFromPath(128);
        if (iv == null)
        {
            return "";
        }

        string url = _httpContextAccessor.GetUrl();
        List<LineUp> ret = new();

        IEnumerable<VideoStream> videoStreams;
        if (request.StreamGroupNumber > 0)
        {
            StreamGroup? streamGroup = await Repository.StreamGroup
                    .FindAll()
                    .Include(a => a.ChildVideoStreams)
                    .FirstOrDefaultAsync(a => a.StreamGroupNumber == request.StreamGroupNumber, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

            if (streamGroup == null)
            {
                return "";
            }
            videoStreams = streamGroup.ChildVideoStreams.Select(a => a.ChildVideoStream).Where(a => !a.IsHidden);
        }
        else
        {
            videoStreams = Repository.VideoStream.GetVideoStreamsHidden();
        }

        if (!videoStreams.Any())
        {
            return JsonSerializer.Serialize(ret);
        }

        foreach (VideoStream videoStream in videoStreams)
        {

            if (Settings.M3UIgnoreEmptyEPGID &&
            (string.IsNullOrEmpty(videoStream.User_Tvg_ID) || videoStream.User_Tvg_ID.ToLower() == "dummy"))
            {
                continue;
            }


            string videoUrl = videoStream.Url;

            string encodedNumbers = request.StreamGroupNumber.EncodeValues128(videoStream.Id, Settings.ServerKey, iv);

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
