namespace StreamMaster.Streams.Domain.Interfaces;

public interface IStreamLimitsService
{
    bool IsLimited(SMStreamDto smStreamDto);
    bool IsLimited(string smStreamDtoId);
    (int currentStreamCount, int maxStreamCount) GetStreamLimits(string smStreamId);
}