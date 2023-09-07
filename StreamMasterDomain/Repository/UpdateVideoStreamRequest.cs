using MediatR;

using StreamMasterDomain.Dto;

using System.ComponentModel.DataAnnotations;

namespace StreamMasterDomain.Repository;


public class UpdateVideoStreamRequest : VideoStreamBaseRequest, IRequest<VideoStreamDto?>
{
    [Key]
    public string Id { get; set; }
    public bool? IsUserCreated { get; set; }

    public StreamingProxyTypes? StreamProxyType { get; set; }
}
