using StreamMasterDomain.Attributes;

namespace StreamMasterDomain.Dto;

[RequireAll]
public record IdName(string Id, string Name);
