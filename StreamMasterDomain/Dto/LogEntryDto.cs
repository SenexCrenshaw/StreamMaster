using StreamMasterDomain.Mappings;
using StreamMasterDomain.Models;

namespace StreamMasterDomain.Dto;

public class LogEntryDto : LogEntry, IMapFrom<LogEntry>
{
}
