namespace StreamMaster.Application.Logos.Commands;

public class BuildLogosCacheFromStreamsRequest : IRequest<DataResponse<bool>>;

[LogExecutionTimeAspect]
public class BuildIconsCacheFromVideoStreamRequestHandler(ILogoService logoService)
    : IRequestHandler<BuildLogosCacheFromStreamsRequest, DataResponse<bool>>
{
    public Task<DataResponse<bool>> Handle(BuildLogosCacheFromStreamsRequest command, CancellationToken cancellationToken)
    {
        logoService.BuildLogosCacheFromSMStreamsAsync(cancellationToken);

        return Task.FromResult(DataResponse.True);
    }
}
