﻿using System.ComponentModel.DataAnnotations;

namespace StreamMaster.Application.M3UFiles.CommandsOrig;

public class BaseFileRequest
{
    public bool? AutoUpdate { get; set; }
    public int? HoursToUpdate { get; set; }

    public string? Description { get; set; }

    [Required]
    public int Id { get; set; }

    public string? MetaData { get; set; }
    public string? Name { get; set; }
    public string? Url { get; set; }
}