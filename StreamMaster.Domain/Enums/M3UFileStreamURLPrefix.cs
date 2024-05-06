using Reinforced.Typings.Attributes;

namespace StreamMaster.Domain.Enums;

[TsEnum]
public enum M3UFileStreamURLPrefix
{
    SystemDefault = 0,
    TS = 1,
    M3U8 = 2,
}
