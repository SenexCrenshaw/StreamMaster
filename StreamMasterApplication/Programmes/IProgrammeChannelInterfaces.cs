using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.Programmes.Queries;

using StreamMasterDomain.Entities.EPG;

namespace StreamMasterApplication.Programmes;

public interface IProgrammeChannelDB
{
}

public interface IProgrammeChannelHub
{
    Task<IEnumerable<Programme>?> GetProgramme(string Channel);

    Task<IEnumerable<ProgrammeChannel>> GetProgrammeChannels();

    Task<IEnumerable<ProgrammeName>> GetProgrammeNames();

    Task<IEnumerable<Programme>> GetProgrammes();
}

public interface IProgrammeChannelScoped
{
}

public interface IProgrammeChannelTasks
{
    ValueTask AddProgrammesFromSDRequest(CancellationToken cancellationToken = default);
}

public interface IProgrammeChannelController
{
    Task<ActionResult<IEnumerable<Programme>?>> GetProgramme(string Channel);

    Task<ActionResult<IEnumerable<ProgrammeChannel>>> GetProgrammeChannels();

    Task<ActionResult<IEnumerable<ProgrammeName>>> GetProgrammeNames();

    Task<ActionResult<IEnumerable<Programme>>> GetProgrammes();
}
