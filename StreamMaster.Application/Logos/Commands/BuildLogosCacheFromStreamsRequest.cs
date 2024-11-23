namespace StreamMaster.Application.Logos.Commands;

public class BuildLogosCacheFromStreamsRequest : IRequest<DataResponse<bool>>;

[LogExecutionTimeAspect]
public class BuildLogosCacheFromStreamsRequestHandler(ILogoService logoService)
    : IRequestHandler<BuildLogosCacheFromStreamsRequest, DataResponse<bool>>
{
    public async Task<DataResponse<bool>> Handle(BuildLogosCacheFromStreamsRequest command, CancellationToken cancellationToken)
    {
        await logoService.BuildLogosCacheFromSMStreamsAsync(cancellationToken);

        return DataResponse.True;
    }
}
