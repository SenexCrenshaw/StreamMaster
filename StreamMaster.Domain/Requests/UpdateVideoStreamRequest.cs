﻿using MediatR;

using System.ComponentModel.DataAnnotations;

namespace StreamMaster.Domain.Requests;


public class UpdateVideoStreamRequest : VideoStreamBaseRequest, IRequest<VideoStreamDto?>
{
    [Key]
    public string Id { get; set; }

}
