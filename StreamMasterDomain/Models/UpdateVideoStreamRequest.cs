using MediatR;

using StreamMasterDomain.Dto;

using System.ComponentModel.DataAnnotations;

namespace StreamMasterDomain.Models;


public class UpdateVideoStreamRequest : VideoStreamBaseRequest, IRequest<VideoStreamDto?>
{
    [Key]
    public string Id { get; set; }

}
