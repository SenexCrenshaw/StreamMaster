using StreamMaster.Domain.Mappings;
using StreamMaster.Domain.Models;

namespace StreamMaster.Domain.Dto;

public class LogEntryDto : LogEntry, IMapFrom<LogEntry>;
