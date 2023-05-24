namespace StreamMasterDomain.Dto;

public class StreamingServiceStatusDto
{
    public Guid Id { get; set; }
    public int MaxClientBufferSingleSendInBytes { get; set; }
    public int MaxStreamBufferSingleReadInBytes { get; set; }
    public int TotalClients { get; set; }
    public double TotalInboundBps { get; set; }
    public int TotalStreamWorkers { get; set; }
}
