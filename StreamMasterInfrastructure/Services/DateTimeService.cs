using StreamMasterApplication.Common.Interfaces;

namespace StreamMasterInfrastructure.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}