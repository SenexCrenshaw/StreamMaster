using MediatR;

using System.ComponentModel.DataAnnotations;

namespace StreamMasterDomain.Repository;

public class UpdateVideoStreamRequest : VideoStreamBaseRequest, IRequest<bool>
{
    [Key]
    public string Id { get; set; }
    public bool? IsUserCreated { get; set; }

    public StreamingProxyTypes? StreamProxyType { get; set; }
}
