using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.Programmes.Queries;

using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.Programmes;

public interface IProgrammeChannelDB
{
}

public interface IProgrammeChannelHub
{
    Task<IEnumerable<EPGProgramme>?> GetProgramme(string Channel);

    Task<IEnumerable<ProgrammeChannel>> GetProgrammeChannels();

    Task<PagedResponse<ProgrammeNameDto>> GetPagedProgrammeNameSelections(ProgrammeParameters Parameters);

    Task<ProgrammeNameDto?> GetProgrammeFromDisplayName(GetProgrammeFromDisplayNameRequest request);

    Task<List<ProgrammeNameDto>> GetProgrammsSimpleQuery(ProgrammeParameters Parameters);
    Task<IEnumerable<string>> GetProgrammeNames();

    Task<IEnumerable<EPGProgramme>> GetProgrammes();
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
    Task<ActionResult<IEnumerable<EPGProgramme>?>> GetProgramme(string Channel);

    Task<ActionResult<IEnumerable<ProgrammeChannel>>> GetProgrammeChannels();

    Task<ActionResult<List<ProgrammeNameDto>>> GetProgrammsSimpleQuery(ProgrammeParameters Parameters);
    Task<ActionResult<PagedResponse<ProgrammeNameDto>>> GetPagedProgrammeNameSelections(ProgrammeParameters Parameters);
    Task<ActionResult<IEnumerable<string>>> GetProgrammeNames();

    Task<ActionResult<IEnumerable<EPGProgramme>>> GetProgrammes();

    Task<ActionResult<ProgrammeNameDto?>> GetProgrammeFromDisplayName(GetProgrammeFromDisplayNameRequest request);
}