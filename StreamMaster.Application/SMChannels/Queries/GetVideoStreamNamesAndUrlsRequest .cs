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
        StreamGroupProfile streamGroupProfile = await streamGroupService.GetDefaultStreamGroupProfileAsync();

        List<IdNameUrl> ret = [];

        IOrderedQueryable<SMChannel> Q = Repository.SMChannel.GetQuery()
         .Where(vs => !vs.IsHidden)
         .OrderBy(vs => vs.ChannelNumber);

        List<Task<IdNameUrl>> tasks = [];

        foreach (SMChannel smChannel in Q)
        {
            string Url = await GetVideoStreamUrlAsync(smChannel.Name, smChannel.Id, defaultStreamGroupId, streamGroupProfile.Id, url);
            IdNameUrl idNameUrl = new(smChannel.Id, smChannel.Name, Url);
            ret.Add(idNameUrl);
        }
        //{
        //    string Url = await GetVideoStreamUrlAsync(smChannel.Name, smChannel.Id, defaultStreamGroupId, streamGroupProfile.Id, url);
        //    IdNameUrl idNameUrl = new(smChannel.Id, smChannel.Name, Url);
        //    lock (tasks)
        //    {
        //        tasks.Add(Task.FromResult(idNameUrl));
        //    }
        //});

        //ret.AddRange(await Task.WhenAll(tasks));

        return DataResponse<List<IdNameUrl>>.Success(ret);
    }
    private async Task<string> GetVideoStreamUrlAsync(string name, int smId, int sgId, int sgPId, string url)
    {
        string cleanName = HttpUtility.UrlEncode(name);

        string? encodedString = await streamGroupService.EncodeStreamGroupIdProfileIdChannelIdAsync(sgId, sgPId, smId);

        string videoUrl = $"{url}/api/videostreams/stream/{encodedString}/{cleanName}";

        return videoUrl;
    }
}
