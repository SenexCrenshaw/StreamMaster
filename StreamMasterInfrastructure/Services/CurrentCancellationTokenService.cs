
using Microsoft.AspNetCore.Http;

using StreamMasterApplication.Common.Interfaces;

namespace StreamMasterInfrastructure.Services;
public class CurrentCancellationTokenService : ICurrentCancellationTokenService
{
    public CurrentCancellationTokenService(IHttpContextAccessor httpContextAccessor)
    {
        CancellationToken = httpContextAccessor.HttpContext!.RequestAborted;
    }
}
