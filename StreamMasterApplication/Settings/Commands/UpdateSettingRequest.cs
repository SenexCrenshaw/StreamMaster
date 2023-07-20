using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI;

using StreamMasterApplication.Hubs;

using StreamMasterDomain.Dto;

using static StreamMasterApplication.Settings.Commands.UpdateSettingHandler;

namespace StreamMasterApplication.Settings.Commands;

public class UpdateSettingRequest : IRequest<UpdateSettingResponse>
{
    public string? AdminPassword { get; set; }
    public string? AdminUserName { get; set; }
    public string? ApiKey { get; set; }
    public AuthenticationType? AuthenticationMethod { get; set; }
    public bool? CacheIcons { get; set; }
    public bool? CleanURLs { get; set; }
    public string? ClientUserAgent { get; set; }
    public string? DeviceID { get; set; }
    public string? DummyRegex { get; set; }
    public bool? EnableSSL { get; set; }
    public string? FFMPegExecutable { get; set; }
    public long? FirstFreeNumber { get; set; }
    public bool? M3UFieldChannelId { get; set; }
    public bool? M3UFieldChannelNumber { get; set; }
    public bool? M3UFieldCUID { get; set; }
    public bool? M3UFieldGroupTitle { get; set; }
    public bool? M3UFieldTvgChno { get; set; }
    public bool? M3UFieldTvgId { get; set; }
    public bool? M3UFieldTvgLogo { get; set; }
    public bool? M3UFieldTvgName { get; set; }
    public int? MaxConnectRetry { get; set; }
    public int? MaxConnectRetryTimeMS { get; set; }
    public bool? OverWriteM3UChannels { get; set; }
    public int? PreloadPercentage { get; set; }
    public int? RingBufferSizeMB { get; set; }
    public string? SDPassword { get; set; }
    public string? SDUserName { get; set; }
    public int? SourceBufferPreBufferPercentage { get; set; }
    public string? SSLCertPassword { get; set; }
    public string? SSLCertPath { get; set; }
    public string? StreamingClientUserAgent { get; set; }
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

    public async Task<UpdateSettingResponse> Handle(UpdateSettingRequest request, CancellationToken cancellationToken)
    {
        Setting currentSetting = FileUtil.GetSetting();

        bool needsLogOut = UpdateSetting(currentSetting, request);

        _logger.LogInformation("UpdateSettingRequest");
        FileUtil.UpdateSetting(currentSetting);

        SettingDto ret = _mapper.Map<SettingDto>(currentSetting);
        await _hubContext.Clients.All.SettingsUpdate(ret).ConfigureAwait(false);

        return new UpdateSettingResponse { Settings = ret, NeedsLogOut = needsLogOut };
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

        if (request.M3UFieldCUID != null)
        {
            currentSetting.M3UFieldCUID = (bool)request.M3UFieldCUID;
        }

        if (request.M3UFieldChannelId != null)
        {
            currentSetting.M3UFieldChannelId = (bool)request.M3UFieldChannelId;
        }


        if (request.M3UFieldChannelNumber != null)
        {
            currentSetting.M3UFieldChannelNumber = (bool)request.M3UFieldChannelNumber;
        }

        if (request.M3UFieldTvgName != null)
        {
            currentSetting.M3UFieldTvgName = (bool)request.M3UFieldTvgName;
        }

        if (request.DummyRegex != null)
        {
            currentSetting.DummyRegex = request.DummyRegex;
        }

        if (request.M3UFieldTvgChno != null)
        {
            currentSetting.M3UFieldTvgChno = (bool)request.M3UFieldTvgChno;
        }

        if (request.M3UFieldTvgId != null)
        {
            currentSetting.M3UFieldTvgId = (bool)request.M3UFieldTvgId;
        }

        if (request.M3UFieldTvgLogo != null)
        {
            currentSetting.M3UFieldTvgLogo = (bool)request.M3UFieldTvgLogo;
        }

        if (request.M3UFieldGroupTitle != null)
        {
            currentSetting.M3UFieldGroupTitle = (bool)request.M3UFieldGroupTitle;
        }

        if (request.SSLCertPath != null && request.SSLCertPath != currentSetting.SSLCertPath)
        {
            currentSetting.SSLCertPath = request.SSLCertPath;
        }

        if (request.SSLCertPassword != null && request.SSLCertPassword != currentSetting.SSLCertPassword)
        {
            currentSetting.SSLCertPassword = request.SSLCertPassword;
        }

        if (request.ClientUserAgent != null && request.ClientUserAgent != currentSetting.ClientUserAgent)
        {
            currentSetting.ClientUserAgent = request.ClientUserAgent;
        }

        if (request.StreamingClientUserAgent != null && request.StreamingClientUserAgent != currentSetting.StreamingClientUserAgent)
        {
            currentSetting.StreamingClientUserAgent = request.StreamingClientUserAgent;
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

        if (request.PreloadPercentage != null && request.PreloadPercentage >= 0 && request.PreloadPercentage <= 100 && request.PreloadPercentage != currentSetting.PreloadPercentage)
        {
            currentSetting.PreloadPercentage = (int)request.PreloadPercentage;
        }

        if (request.RingBufferSizeMB != null && request.RingBufferSizeMB >= 0 && request.RingBufferSizeMB != currentSetting.RingBufferSizeMB)
        {
            currentSetting.RingBufferSizeMB = (int)request.RingBufferSizeMB;
        }

        if (request.SDPassword != null)
        {
            currentSetting.SDPassword = HashHelper.GetSHA1Hash(request.SDPassword);
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

    public class UpdateSettingResponse
    {
        public bool NeedsLogOut { get; set; }
        public SettingDto Settings { get; set; }
    }
}
