namespace StreamMaster.Streams.Domain.Interfaces;

public interface IStreamLimitsService
{
    Task<bool> IsLimitedAsync(string smStreamDtoId);
}