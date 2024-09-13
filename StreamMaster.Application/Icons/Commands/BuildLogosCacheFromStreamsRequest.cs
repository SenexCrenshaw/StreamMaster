namespace StreamMaster.Application.Icons.Commands;

public class BuildLogosCacheFromStreamsRequest : IRequest<DataResponse<bool>>;

[LogExecutionTimeAspect]
public class BuildIconsCacheFromVideoStreamRequestHandler(ILogoService logoService)
    : IRequestHandler<BuildLogosCacheFromStreamsRequest, DataResponse<bool>>
{
    public async Task<DataResponse<bool>> Handle(BuildLogosCacheFromStreamsRequest command, CancellationToken cancellationToken)
    {
        //await logoService.BuildLogosCacheFromSMStreamsAsync(cancellationToken);
        _ = Task.Run(() => logoService.BuildLogosCacheFromSMStreamsAsync(cancellationToken), cancellationToken);

        return DataResponse.True;
    }
}
