namespace StreamMaster.Application.Logos.Commands;

public class AddSMStreamLogosRequest : IRequest<DataResponse<bool>>;

[LogExecutionTimeAspect]
public class AddSMStreamLogosRequestHandler(ILogoService logoService) : IRequestHandler<AddSMStreamLogosRequest, DataResponse<bool>>
{
    public async Task<DataResponse<bool>> Handle(AddSMStreamLogosRequest command, CancellationToken cancellationToken)
    {
        await logoService.AddSMStreamLogosAsync(cancellationToken);
        return DataResponse.True;
    }
}
