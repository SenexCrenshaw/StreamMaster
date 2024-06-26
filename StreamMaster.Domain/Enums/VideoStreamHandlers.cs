using Reinforced.Typings.Attributes;

namespace StreamMaster.Domain.Enums;

[TsEnum]
public enum VideoStreamHandlers
{
    SystemDefault = 0,
    None = 1,
    Loop = 2,
}
