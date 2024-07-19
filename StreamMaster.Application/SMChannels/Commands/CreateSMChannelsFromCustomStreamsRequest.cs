namespace StreamMaster.Application.SMChannels.Commands;

public record CreateSMChannelsFromCustomStreamsRequest(List<string> StreamIds, int M3UFileId, bool IsCustomPlayList) : IRequest<APIResponse>;

internal class CreateSMChannelsFromCustomRequestHandler(IRepositoryWrapper Repository, IDataRefreshService dataRefreshService) : IRequestHandler<CreateSMChannelsFromCustomStreamsRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CreateSMChannelsFromCustomStreamsRequest request, CancellationToken cancellationToken)
    {
        APIResponse res = await Repository.SMChannel.CreateSMChannelsFromCustomStreams(request.StreamIds, request.M3UFileId, request.IsCustomPlayList);
        if (res.IsError)
        {
            return APIResponse.ErrorWithMessage(res.ErrorMessage);
        }

        await dataRefreshService.RefreshAllSMChannels();


        return APIResponse.Success;
    }
}
