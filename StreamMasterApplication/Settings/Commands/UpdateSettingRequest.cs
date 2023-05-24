using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Hubs;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Settings.Commands;

public class UpdateSettingRequest : IRequest<SettingDto>
{
    public bool? CacheIcons { get; set; }
    public string? DeviceID { get; set; }
    public string? FFMPegExecutable { get; set; }
    public long? FirstFreeNumber { get; set; }
    public int? MaxStreamMissedErrors { get; set; }
    public int? RingBufferSizeMB { get; set; }
    public string? SDPassword { get; set; }
    public string? SDUserName { get; set; }
    public int? SourceBufferPreBufferPercentage { get; set; }
    public StreamingProxyTypes? StreamingProxyType { get; set; }
}

public class UpdateSettingValidator : AbstractValidator<UpdateSettingRequest>
{
}

public class UpdateSettingHandler : IRequestHandler<UpdateSettingRequest, SettingDto>
{
    private readonly ILogger<UpdateSettingRequest> _logger;
    private readonly IMapper _mapper;
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;

    public UpdateSettingHandler(ILogger<UpdateSettingRequest> logger, IMapper mapper, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
    {
        _logger = logger;
        _mapper = mapper;
        _hubContext = hubContext;
    }

    public async Task<SettingDto> Handle(UpdateSettingRequest request, CancellationToken cancellationToken)
    {
        Setting currentSetting = FileUtil.GetSetting();

        UpdateSetting(currentSetting, request);

        _logger.LogInformation("UpdateSettingRequest");
        FileUtil.UpdateSetting(currentSetting);

        SettingDto ret = _mapper.Map<SettingDto>(currentSetting);
        await _hubContext.Clients.All.SettingsUpdate(ret).ConfigureAwait(false);

        return ret;
    }

    /// <summary>
    /// Updates the current setting based on the provided request.
    /// </summary>
    /// <param name="currentSetting">The current setting.</param>
    /// <param name="request">The update setting request.</param>
    /// <returns>The updated setting as a SettingDto object.</returns>
    private static void UpdateSetting(Setting currentSetting, UpdateSettingRequest request)
    {
        if (request.CacheIcons != null && request.CacheIcons != currentSetting.CacheIcons)
        {
            currentSetting.CacheIcons = (bool)request.CacheIcons;
        }

        if (!string.IsNullOrEmpty(request.DeviceID) && request.DeviceID != currentSetting.DeviceID)
        {
            currentSetting.DeviceID = request.DeviceID;
        }

        if (!string.IsNullOrEmpty(request.FFMPegExecutable) && request.FFMPegExecutable != currentSetting.FFMPegExecutable)
        {
            currentSetting.FFMPegExecutable = request.FFMPegExecutable;
        }

        if (request.FirstFreeNumber != null && request.FirstFreeNumber >= 0 && request.FirstFreeNumber != currentSetting.FirstFreeNumber)
        {
            currentSetting.FirstFreeNumber = (long)request.FirstFreeNumber;
        }

        if (request.MaxStreamMissedErrors != null && request.MaxStreamMissedErrors >= 0 && request.MaxStreamMissedErrors != currentSetting.MaxStreamMissedErrors)
        {
            currentSetting.MaxStreamMissedErrors = (int)request.MaxStreamMissedErrors;
        }

        if (request.RingBufferSizeMB != null && request.RingBufferSizeMB >= 0 && request.RingBufferSizeMB != currentSetting.RingBufferSizeMB)
        {
            currentSetting.RingBufferSizeMB = (int)request.RingBufferSizeMB;
        }

        if (!string.IsNullOrEmpty(request.SDPassword) && request.SDPassword != currentSetting.SDPassword)
        {
            currentSetting.SDPassword = request.SDPassword;
        }

        if (!string.IsNullOrEmpty(request.SDUserName) && request.SDUserName != currentSetting.SDUserName)
        {
            currentSetting.SDUserName = request.SDUserName;
        }

        if (request.SourceBufferPreBufferPercentage != null && request.SourceBufferPreBufferPercentage >= 0 && request.SourceBufferPreBufferPercentage != currentSetting.SourceBufferPreBufferPercentage)
        {
            currentSetting.SourceBufferPreBufferPercentage = (int)request.SourceBufferPreBufferPercentage;
        }

        if (request.StreamingProxyType != null && request.StreamingProxyType != currentSetting.StreamingProxyType)
        {
            currentSetting.StreamingProxyType = (StreamingProxyTypes)request.StreamingProxyType;
        }

    }
}
