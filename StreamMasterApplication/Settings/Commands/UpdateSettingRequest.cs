using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Hubs;

using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;

using System.Text;

namespace StreamMasterApplication.Settings.Commands;

public class UpdateSettingRequest : IRequest<SettingDto>
{
    public bool? CacheIcons { get; set; }
    public string? APIPassword { get; set; }
    public string? APIUserName { get; set; }
    public bool? CleanURLs { get; set; }
    public string? DeviceID { get; set; }
    public string? FFMPegExecutable { get; set; }
    public long? FirstFreeNumber { get; set; }
    public int? MaxConnectRetry { get; set; }
    public int? MaxConnectRetryTimeMS { get; set; }
    public bool? OverWriteM3UChannels { get; set; }
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
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;
    private readonly ILogger<UpdateSettingRequest> _logger;
    private readonly IMapper _mapper;

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
        var initNeedsUpdating = false;

        if (request.CacheIcons != null && request.CacheIcons != currentSetting.CacheIcons)
        {
            currentSetting.CacheIcons = (bool)request.CacheIcons;
        }

        if (request.CleanURLs != null && request.CleanURLs != currentSetting.CleanURLs)
        {
            currentSetting.CleanURLs = (bool)request.CleanURLs;
        }

        if (request.OverWriteM3UChannels != null && request.OverWriteM3UChannels != currentSetting.OverWriteM3UChannels)
        {
            currentSetting.OverWriteM3UChannels = (bool)request.OverWriteM3UChannels;
        }

        if (!string.IsNullOrEmpty(request.APIPassword) && request.APIPassword != currentSetting.APIPassword)
        {
            initNeedsUpdating = true;
            currentSetting.APIPassword = request.APIPassword;
        }

        if (!string.IsNullOrEmpty(request.APIUserName) && request.APIUserName != currentSetting.APIUserName)
        {
            initNeedsUpdating = true;
            currentSetting.APIUserName = request.APIUserName;
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

        if (request.MaxConnectRetry != null && request.MaxConnectRetry >= 0 && request.MaxConnectRetry != currentSetting.MaxConnectRetry)
        {
            currentSetting.MaxConnectRetry = (int)request.MaxConnectRetry;
        }

        if (request.MaxConnectRetryTimeMS != null && request.MaxConnectRetryTimeMS >= 0 && request.MaxConnectRetryTimeMS != currentSetting.MaxConnectRetryTimeMS)
        {
            currentSetting.MaxConnectRetryTimeMS = (int)request.MaxConnectRetryTimeMS;
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

        if (initNeedsUpdating)
        {
            string indexFilePath = Path.GetFullPath("wwwroot") + Path.DirectorySeparatorChar + "initialize.js";

            Console.WriteLine($"Writing {currentSetting.BaseHostURL} information to {indexFilePath}");

            StringBuilder scriptBuilder = new();
            scriptBuilder.AppendLine("window.StreamMaster = {");
            scriptBuilder.AppendLine($"  baseHostURL: '{currentSetting.BaseHostURL}',");
            if (!string.IsNullOrEmpty(currentSetting.APIPassword) && !string.IsNullOrEmpty(currentSetting.APIUserName))
            {
                scriptBuilder.AppendLine($"  apiUserName: '{currentSetting.APIPassword}',");
                scriptBuilder.AppendLine($"  apiPassword: '{currentSetting.APIUserName}',");
            }
            scriptBuilder.AppendLine($"  baseHostURL: '{currentSetting.BaseHostURL}',");
            scriptBuilder.AppendLine($"  hubName: 'streammasterhub',");
            scriptBuilder.AppendLine($"  isDev: false,");
            scriptBuilder.AppendLine("};");
            File.WriteAllText(indexFilePath, scriptBuilder.ToString());

        }

    }
}
