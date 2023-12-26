using Microsoft.AspNetCore.Mvc;

using StreamMaster.SchedulesDirect.Domain.XmltvXml;

namespace StreamMasterApplication.Programmes;

public interface IProgrammeChannelHub
{
    Task<IEnumerable<XmltvProgramme>?> GetProgramme(string Channel);
    Task<IEnumerable<XmltvProgramme>> GetProgrammes();
}

public interface IProgrammeChannelTasks
{
    ValueTask EPGSync(CancellationToken cancellationToken = default);
}

public interface IProgrammeChannelController
{
    Task<ActionResult<IEnumerable<XmltvProgramme>?>> GetProgramme(string Channel);
    Task<ActionResult<IEnumerable<XmltvProgramme>>> GetProgrammes();
}