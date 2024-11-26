namespace StreamMaster.Application.Logos.Commands;

public record AddSMChannelLogosRequest : IRequest<DataResponse<bool>>;

[LogExecutionTimeAspect]
public class AddSMChannelLogosRequestHandler(ILogoService logoService) : IRequestHandler<AddSMChannelLogosRequest, DataResponse<bool>>
{
    public async Task<DataResponse<bool>> Handle(AddSMChannelLogosRequest request, CancellationToken cancellationToken)
    {
        await logoService.AddSMChannelLogosAsync(cancellationToken);
        return DataResponse.True;
    }
}
