﻿using MessagePack;

using Reinforced.Typings.Attributes;

using StreamMaster.Domain.Attributes;

using System.Text.Json.Serialization;

namespace StreamMaster.Domain.Dto;

[RequireAll]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class LogoFileDto : IMapFrom<IconFile>
{
    [JsonIgnore]
    [IgnoreMember]
    public string Extension { get; set; } = string.Empty;

    [JsonIgnore]
    [IgnoreMember]
    public int FileId { get; set; }

    public string Id => Source;

    public string Name { get; set; } = string.Empty;

    [JsonIgnore]
    public SMFileTypes SMFileType { get; set; }

    public string Source { get; set; } = string.Empty;
}
