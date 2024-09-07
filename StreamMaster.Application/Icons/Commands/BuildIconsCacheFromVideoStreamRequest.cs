namespace StreamMaster.Application.Icons.Commands;

public class BuildIconsCacheFromVideoStreamRequest : IRequest<DataResponse<bool>>;

[LogExecutionTimeAspect]
public class BuildIconsCacheFromVideoStreamRequestHandler(ILogoService logoService)
    : IRequestHandler<BuildIconsCacheFromVideoStreamRequest, DataResponse<bool>>
{
    public async Task<DataResponse<bool>> Handle(BuildIconsCacheFromVideoStreamRequest command, CancellationToken cancellationToken)
    {
        await logoService.BuildLogosCacheFromSMStreamsAsync(cancellationToken);
        return DataResponse.True;
    }
}
