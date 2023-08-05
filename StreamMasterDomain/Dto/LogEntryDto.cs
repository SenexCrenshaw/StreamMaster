using StreamMasterDomain.Mappings;
using StreamMasterDomain.Repository;

namespace StreamMasterDomain.Dto;

public class LogEntryDto : LogEntry, IMapFrom<LogEntry>
{
}
