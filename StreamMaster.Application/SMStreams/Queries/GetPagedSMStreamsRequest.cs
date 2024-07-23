using Microsoft.AspNetCore.Http;

using StreamMaster.Application.Crypto.Commands;
using StreamMaster.Application.StreamGroups.Queries;

using System.Text.Json;

namespace StreamMaster.Application.SMStreams.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetPagedSMStreamsRequest(QueryStringParameters Parameters) : IRequest<PagedResponse<SMStreamDto>>;

internal class GetPagedSMStreamsRequestHandler(IRepositoryWrapper Repository, ISender sender, IOptionsMonitor<Setting> intSettings, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetPagedSMStreamsRequest, PagedResponse<SMStreamDto>>
{
    public async Task<PagedResponse<SMStreamDto>> Handle(GetPagedSMStreamsRequest request, CancellationToken cancellationToken)
    {
        if (request.Parameters.PageSize == 0)
        {
            return Repository.SMStream.CreateEmptyPagedResponse();
        }

        _ = intSettings.CurrentValue;
        _ = httpContextAccessor.GetUrlWithPathValue();

        PagedResponse<SMStreamDto> res = await Repository.SMStream.GetPagedSMStreams(request.Parameters, CancellationToken.None).ConfigureAwait(false);


        string Url = httpContextAccessor.GetUrl();
        DataResponse<StreamGroupProfile> defaultSGProfile = await sender.Send(new GetDefaultStreamGroupProfileIdRequest()).ConfigureAwait(false);

        foreach (SMStreamDto stream in res.Data)
        {
            string videoUrl;


            //string encodedName = HttpUtility.HtmlEncode(stream.Name).Trim()
            //        .Replace("/", "")
            //        .Replace(" ", "_");

            //string encodedNumbers = 0.EncodeValues128(stream.Id, settings.CurrentValue.ServerKey);
            //videoUrl = $"{Url}/api/videostreams/stream/{encodedNumbers}/{encodedName}";

            (string EncodedString, string CleanName) = await sender.Send(new EncodeStreamGroupIdProfileIdStreamId(defaultSGProfile.Data.StreamGroupId, defaultSGProfile.Data.Id, stream.Id, stream.Name), cancellationToken);
            if (string.IsNullOrEmpty(EncodedString) || string.IsNullOrEmpty(CleanName))
            {
                continue;
            }
            videoUrl = $"{Url}/api/videostreams/stream/{EncodedString}/{CleanName}";


            string jsonString = JsonSerializer.Serialize(videoUrl);
            stream.RealUrl = jsonString;
        }
        return res;
    }
}
