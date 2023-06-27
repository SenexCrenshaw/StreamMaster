using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Hubs;

using StreamMasterDomain.Dto;

using static StreamMasterApplication.Settings.Commands.UpdateSettingHandler;

namespace StreamMasterApplication.Settings.Commands;

public class UpdateSettingRequest : IRequest<UpdateSettingResponse>
{
    public AuthenticationType? AuthenticationMethod { get; set; }
    public string? AdminPassword { get; set; }
    public string? AdminUserName { get; set; }
    public string? ApiKey { get; set; }
    public bool? EnableSSL { get; set; }
    public bool? CacheIcons { get; set; }
    public bool? CleanURLs { get; set; }
    public string? DeviceID { get; set; }
    public string? FFMPegExecutable { get; set; }
    public string? SSLCertPath { get; set; }
    public string? SSLCertPassword { get; set; }
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

public class UpdateSettingHandler : IRequestHandler<UpdateSettingRequest, UpdateSettingResponse>
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

    public class UpdateSettingResponse {
       public  SettingDto Settings { get; set; }
        public bool NeedsLogOut { get; set; }
    }

    public async Task<UpdateSettingResponse> Handle(UpdateSettingRequest request, CancellationToken cancellationToken)
    {
        Setting currentSetting = FileUtil.GetSetting();

        bool needsLogOut = UpdateSetting(currentSetting, request);

        _logger.LogInformation("UpdateSettingRequest");
        FileUtil.UpdateSetting(currentSetting);

        SettingDto ret = _mapper.Map<SettingDto>(currentSetting);
        await _hubContext.Clients.All.SettingsUpdate(ret).ConfigureAwait(false);

        return new UpdateSettingResponse{ Settings = ret, NeedsLogOut = needsLogOut };
    }

    /// <summary>
    /// Updates the current setting based on the provided request.
    /// </summary>
    /// <param name="currentSetting">The current setting.</param>
    /// <param name="request">The update setting request.</param>
    /// <returns>The updated setting as a SettingDto object.</returns>
    private static bool UpdateSetting(Setting currentSetting, UpdateSettingRequest request)
    {
        bool needsLogOut = false;
        if (request.CacheIcons != null && request.CacheIcons != currentSetting.CacheIcons)
        {
            currentSetting.CacheIcons = (bool)request.CacheIcons;
        }

        if (request.CleanURLs != null && request.CleanURLs != currentSetting.CleanURLs)
        {
            currentSetting.CleanURLs = (bool)request.CleanURLs;
        }

        if (request.EnableSSL != null && request.EnableSSL != currentSetting.EnableSSL)
        {
            currentSetting.EnableSSL = (bool)request.EnableSSL;
        }

        if (request.SSLCertPath != null && request.SSLCertPath != currentSetting.SSLCertPath)
        {
            currentSetting.SSLCertPath = request.SSLCertPath;
        }

        if (request.SSLCertPassword != null && request.SSLCertPassword != currentSetting.SSLCertPassword)
        {
            currentSetting.SSLCertPassword = request.SSLCertPassword;
        }

        if (request.OverWriteM3UChannels != null && request.OverWriteM3UChannels != currentSetting.OverWriteM3UChannels)
        {
            currentSetting.OverWriteM3UChannels = (bool)request.OverWriteM3UChannels;
        }
        if (!string.IsNullOrEmpty(request.ApiKey) && request.ApiKey != currentSetting.ApiKey)
        {
            currentSetting.ApiKey = request.ApiKey;
        }

        if (request.AdminPassword != null && request.AdminPassword != currentSetting.AdminPassword)
        {
            currentSetting.AdminPassword = request.AdminPassword;
            needsLogOut = true;
        }

        if (request.AdminUserName != null && request.AdminUserName != currentSetting.AdminUserName)
        {
            currentSetting.AdminUserName = request.AdminUserName;
            needsLogOut = true;
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

        if (request.SDPassword != null && request.SDPassword != currentSetting.SDPassword)
        {
            currentSetting.SDPassword = request.SDPassword;
        }

        if (request.SDUserName != null && request.SDUserName != currentSetting.SDUserName)
        {
            currentSetting.SDUserName = request.SDUserName;
        }

        if (request.StreamingProxyType != null && request.StreamingProxyType != currentSetting.StreamingProxyType)
        {
            currentSetting.StreamingProxyType = (StreamingProxyTypes)request.StreamingProxyType;
        }


        if (request.AuthenticationMethod != null && request.AuthenticationMethod != currentSetting.AuthenticationMethod)
        {
            needsLogOut = true;
            currentSetting.AuthenticationMethod = (AuthenticationType)request.AuthenticationMethod;
        }

        return needsLogOut;
    }
}
