//namespace StreamMaster.Application.SMChannels.Queries;

//[SMAPI]
//[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
//public record GetSMChannelNameLogosRequest() : IRequest<DataResponse<List<NameLogo>>>;

//internal class GetSMChannelNameLogosRequestHandler(IRepositoryWrapper Repository)
//    : IRequestHandler<GetSMChannelNameLogosRequest, DataResponse<List<NameLogo>>>
//{
//    public async Task<DataResponse<List<NameLogo>>> Handle(GetSMChannelNameLogosRequest request, CancellationToken cancellationToken)
//    {
//        List<NameLogo> channelNames = await Repository.SMChannel.GetQuery().OrderBy(a => a.Name).Select(a => new NameLogo(a, SMFileTypes.Logo)).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
//        return DataResponse<List<NameLogo>>.Success(channelNames);
//    }
//}
