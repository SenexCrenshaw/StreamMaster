using System.Diagnostics;

using AutoMapper;

using Microsoft.AspNetCore.Http;

namespace StreamMaster.Streams.Services;

/// <summary>
/// Service for managing video-related operations, including adding clients to channels.
/// </summary>
public sealed class VideoService(
    ILogger<VideoService> logger,
    IMapper mapper,
    IChannelService channelService,
    IClientConfigurationService clientConfigurationService,
    IRepositoryWrapper repositoryWrapper,
    IStreamGroupService streamGroupService) : IVideoService, IDisposable
{
    /// <inheritdoc/>
    public async Task<StreamResult> AddClientToChannelAsync(
        HttpContext httpContext,
        int? streamGroupId,
        int? streamGroupProfileId,
        int? smChannelId,
        CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            if (!ValidateStreamParameters(streamGroupId, streamGroupProfileId, smChannelId))
            {
                return new StreamResult();
            }

            if (streamGroupId is null || smChannelId is null)
            {
                return new StreamResult();
            }

            SMChannel? smChannel = await FetchSMChannelAsync(streamGroupId.Value, smChannelId.Value);
            if (smChannel == null)
            {
                logger.LogInformation("Channel with ChannelId {ChannelId} not found.", smChannelId);
                return new StreamResult();
            }

            if (!ChannelHasStreamsOrChannels(smChannel))
            {
                logger.LogInformation("Channel with ChannelId {ChannelId} has no streams or channels.", smChannelId);
                return new StreamResult();
            }

            SMChannelDto smChannelDto = mapper.Map<SMChannelDto>(smChannel);
            await UpdateChannelRanksAsync(smChannelDto);

            StreamGroupProfile streamGroupProfile = await streamGroupService.GetStreamGroupProfileAsync(null, streamGroupProfileId);
            CommandProfileDto commandProfileDto = await streamGroupService.GetProfileFromSGIdsCommandProfileNameAsync(
                null, streamGroupProfile.Id, smChannelDto.CommandProfileName);

            if (commandProfileDto.ProfileName.Equals("Redirect", StringComparison.InvariantCultureIgnoreCase))
            {
                logger.LogInformation("Channel with ChannelId {ChannelId} is redirecting.", smChannelId);
                return new StreamResult { RedirectUrl = smChannelDto.SMStreamDtos[0].Url };
            }

            IClientConfiguration clientConfiguration = CreateClientConfiguration(httpContext, smChannelDto, cancellationToken);

            bool result = await channelService.AddClientToChannelAsync(clientConfiguration, streamGroupProfile.Id, CancellationToken.None);

            stopwatch.Stop();
            logger.LogInformation(
                "Channel with ChannelId {ChannelId} {Status} in {ElapsedMilliseconds}ms.",
                smChannelId, result ? "streaming" : "failed", stopwatch.ElapsedMilliseconds);

            return new StreamResult { ClientConfiguration = clientConfiguration };
        }
        finally
        {
            stopwatch.Stop();
        }
    }

    private static bool ValidateStreamParameters(int? streamGroupId, int? streamGroupProfileId, int? smChannelId)
    {
        return streamGroupId.HasValue && streamGroupProfileId.HasValue && smChannelId.HasValue;
    }

    private async Task<SMChannel?> FetchSMChannelAsync(int streamGroupId, int smChannelId)
    {
        int defaultSGId = await streamGroupService.GetDefaultSGIdAsync();
        return streamGroupId == defaultSGId
            ? repositoryWrapper.SMChannel.GetSMChannel(smChannelId)
            : await repositoryWrapper.SMChannel.GetSMChannelFromStreamGroupAsync(smChannelId, streamGroupId);
    }

    private async Task UpdateChannelRanksAsync(SMChannelDto smChannelDto)
    {
        await repositoryWrapper.SMChannelStreamLink.UpdateSMChannelDtoRanks(smChannelDto);
        await repositoryWrapper.SMChannelChannelLink.UpdateSMChannelDtoRanks(smChannelDto);
    }

    private static bool ChannelHasStreamsOrChannels(SMChannel smChannel)
    {
        return smChannel.SMChannelType == StreamMaster.Domain.Enums.SMChannelTypeEnum.MultiView
            ? smChannel.SMChannels.Count != 0
            : smChannel.SMStreams.Any(s => !string.IsNullOrEmpty(s.SMStream?.Url));
    }

    private IClientConfiguration CreateClientConfiguration(
        HttpContext httpContext,
        SMChannelDto smChannelDto,
        CancellationToken cancellationToken)
    {
        string ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        HttpRequest request = httpContext.Request;
        smChannelDto.StreamUrl = $"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}{request.QueryString}";

        string uniqueRequestId = request.HttpContext.TraceIdentifier;

        return clientConfigurationService.NewClientConfiguration(
            uniqueRequestId,
            smChannelDto,
            request.Headers.UserAgent.ToString(),
            ipAddress,
            httpContext.Response,
            cancellationToken);
    }

    /// <summary>
    /// Releases unmanaged and managed resources used by the service.
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
