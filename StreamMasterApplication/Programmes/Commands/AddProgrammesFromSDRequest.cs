using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirect;

namespace StreamMasterApplication.Programmes.Commands;

public record AddProgrammesFromSDRequest() : IRequest { }

public class AddProgrammesFromSDRequestValidator : AbstractValidator<AddProgrammesFromSDRequest>
{
    public AddProgrammesFromSDRequestValidator()
    {
    }
}

public class AddProgrammesFromSDRequestHandler : IRequestHandler<AddProgrammesFromSDRequest>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<AddProgrammesFromSDRequestHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IPublisher _publisher;

    public AddProgrammesFromSDRequestHandler(
        ILogger<AddProgrammesFromSDRequestHandler> logger,
         IMapper mapper,
         IPublisher publisher,
        IAppDbContext context)
    {
        _logger = logger;
        _publisher = publisher;
        _mapper = mapper;
        _context = context;
    }

    public async Task Handle(AddProgrammesFromSDRequest command, CancellationToken cancellationToken)
    {
        Setting setting = FileUtil.GetSetting();

        if (string.IsNullOrEmpty(setting.SDUserName) || string.IsNullOrEmpty(setting.SDPassword))
        {
            _logger.LogDebug("No SD Information, skipping");
            return;
        }

        _logger.LogInformation("Getting Token");

        var sd = new SchedulesDirect("senexcrenshaw", "IPTVR0xR0x");

        _logger.LogInformation("Getting Status");
        var status = await sd.GetStatus(cancellationToken);
        if (status == null || !status.systemStatus.Any())
        {
            _logger.LogWarning("Status is null");
            return;
        }

        var systemStatus = status.systemStatus[0];
        if (systemStatus.status == "Offline")
        {
            _logger.LogWarning($"Status is {systemStatus.status}");
            return;
        }
        _logger.LogInformation("System Is Online");

        var lineup = await sd.GetLineup("USA-OTA-19087", cancellationToken: cancellationToken);
        if (lineup == null)
        {
            _logger.LogWarning($"lineup is null");
            return;
        }

        var wpvi = lineup.Stations.FirstOrDefault(a => a.Callsign.ToLower().Contains("wpvi"));
        if (wpvi != null)
        {
            _logger.LogInformation("Getting Schedules");
            var schedules = await sd.GetSchedules(new List<string> { wpvi.StationID }, cancellationToken: cancellationToken);
            if (schedules != null)
            {
                var progIds = schedules.SelectMany(a => a.Programs).Select(a => a.ProgramID).Distinct().ToList();
                _logger.LogInformation("Getting Programs");

                var programs = await sd.GetPrograms(progIds, cancellationToken: cancellationToken);
            }
        }
    }
}
