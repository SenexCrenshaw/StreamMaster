using Microsoft.AspNetCore.Http;

using System.Web;

namespace StreamMaster.Application.SMChannels.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetVideoStreamNamesAndUrlsRequest() : IRequest<DataResponse<List<IdNameUrl>>>;

internal class GetVideoStreamNamesAndUrlsHandler(IRepositoryWrapper Repository, IStreamGroupService streamGroupService, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetVideoStreamNamesAndUrlsRequest, DataResponse<List<IdNameUrl>>>
{
    public async Task<DataResponse<List<IdNameUrl>>> Handle(GetVideoStreamNamesAndUrlsRequest request, CancellationToken cancellationToken)
    {
        string url = httpContextAccessor.GetUrl();

        int defaultStreamGroupId = await streamGroupService.GetDefaultSGIdAsync();
        StreamGroupProfile test = await streamGroupService.GetDefaultStreamGroupProfileAsync();

        List<IdNameUrl> ret = new();

        IOrderedQueryable<SMChannel> Q = Repository.SMChannel.GetQuery().Where(vs => !vs.IsHidden).OrderBy(vs => vs.ChannelNumber);
        foreach (SMChannel? smChannel in Q)
        {
            string Url = await GetVideoStreamUrl(smChannel.Name, smChannel.Id, defaultStreamGroupId, test.Id, url);
            ret.Add(new IdNameUrl(smChannel.Id, smChannel.Name, Url));
        }


        return DataResponse<List<IdNameUrl>>.Success(ret);

    }
    private async Task<string> GetVideoStreamUrl(string name, int smId, int sgId, int sgPId, string url)
    {
        string cleanName = HttpUtility.UrlEncode(name);

        string? encodedString = await streamGroupService.EncodeStreamGroupIdProfileIdChannelIdAsync(sgId, sgPId, smId);

        string videoUrl = $"{url}/api/videostreams/stream/{encodedString}/{cleanName}";

        return videoUrl;
    }
}
