using Microsoft.AspNetCore.Mvc;

using StreamMaster.SchedulesDirectAPI.Domain.XmltvXml;

using StreamMasterApplication.Programmes.Queries;

using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.Programmes;

public interface IProgrammeChannelDB
{
}

public interface IProgrammeChannelHub
{
    Task<IEnumerable<XmltvProgramme>?> GetProgramme(string Channel);


    Task<IEnumerable<XmltvProgramme>> GetProgrammes();
}

public interface IProgrammeChannelScoped
{
}

public interface IProgrammeChannelTasks
{
    ValueTask SDSync(CancellationToken cancellationToken = default);
}

public interface IProgrammeChannelController
{
    Task<ActionResult<IEnumerable<XmltvProgramme>?>> GetProgramme(string Channel);


    Task<ActionResult<IEnumerable<XmltvProgramme>>> GetProgrammes();

}