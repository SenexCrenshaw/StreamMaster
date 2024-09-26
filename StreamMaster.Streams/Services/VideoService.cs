﻿using AutoMapper;

using Microsoft.AspNetCore.Http;

using System.Diagnostics;

namespace StreamMaster.Streams.Services;

public class VideoService(
    ILogger<VideoService> logger,
    IMapper mapper,
    IHttpContextAccessor httpContextAccessor,
    IChannelManager channelManager,
    IClientConfigurationService clientConfigurationService,
    IRepositoryWrapper repositoryWrapper,
    IStreamGroupService streamGroupService) : IVideoService
{
    public async Task<StreamResult> GetStreamAsync(int? streamGroupId, int? streamGroupProfileId, int? smChannelId, CancellationToken cancellationToken)
    {
        Stopwatch sw = Stopwatch.StartNew();

        // Validate parameters
        if (!ValidateStreamParameters(streamGroupId, streamGroupProfileId, smChannelId))
        {
            return new StreamResult();
        }

        // Fetch channel
        SMChannel? smChannel = await FetchSMChannelAsync(streamGroupId!.Value, smChannelId!.Value);
        if (smChannel == null)
        {
            logger.LogInformation("Channel with ChannelId {smChannelId} not found, exiting", smChannelId);
            return new StreamResult();
        }

        // Ensure channel has streams
        if (!ChannelHasStreams(smChannel))
        {
            logger.LogInformation("Channel with ChannelId {smChannelId} has no streams, exiting", smChannelId);
            return new StreamResult();
        }

        // Fetch stream profile
        StreamGroupProfile streamGroupProfile = await streamGroupService.GetStreamGroupProfileAsync(null, streamGroupProfileId);
        CommandProfileDto commandProfileDto = await streamGroupService.GetProfileFromSGIdsCommandProfileNameAsync(null, streamGroupProfile.Id, smChannel.CommandProfileName);

        // Handle redirects
        if (commandProfileDto.ProfileName.Equals("Redirect", StringComparison.InvariantCultureIgnoreCase))
        {
            logger.LogInformation("Channel with ChannelId {channelId} redirecting", smChannelId);
            return new StreamResult { RedirectUrl = smChannel.SMStreams.First().SMStream.Url };
        }

        // Create client configuration
        IClientConfiguration clientConfiguration = CreateClientConfiguration(smChannel, cancellationToken);

        // Get stream
        Stream? stream = await channelManager.GetChannelStreamAsync(clientConfiguration, streamGroupProfile.Id, CancellationToken.None);

        // Log results
        LogStreamResult(smChannelId, stream, sw);

        return new StreamResult { Stream = stream, ClientConfiguration = clientConfiguration };
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
            : repositoryWrapper.SMChannel.GetSMChannelFromStreamGroup(smChannelId, streamGroupId);
    }

    private static bool ChannelHasStreams(SMChannel smChannel)
    {
        return smChannel.SMStreams.Count > 0 && !string.IsNullOrEmpty(smChannel.SMStreams.First().SMStream.Url);
    }

    private IClientConfiguration CreateClientConfiguration(SMChannel smChannel, CancellationToken cancellationToken)
    {
        string? ipAddress = httpContextAccessor.HttpContext!.Connection.RemoteIpAddress?.ToString();
        SMChannelDto smChannelDto = mapper.Map<SMChannelDto>(smChannel);

        HttpRequest request = httpContextAccessor.HttpContext.Request;
        smChannelDto.StreamUrl = $"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}{request.QueryString}";

        string uniqueRequestId = request.HttpContext.TraceIdentifier;

        return clientConfigurationService.NewClientConfiguration(
            uniqueRequestId,
            smChannelDto,
            request.Headers.UserAgent.ToString(),
            ipAddress ?? "unknown",
            httpContextAccessor.HttpContext.Response,
            cancellationToken
        );
    }

    private void LogStreamResult(int? smChannelId, Stream? stream, Stopwatch sw)
    {
        if (stream == null)
        {
            logger.LogInformation("Channel with ChannelId {channelId} failed", smChannelId);
        }
        else
        {
            logger.LogInformation("Streaming channel with ChannelId {channelId} took {elapsed}ms", smChannelId, sw.ElapsedMilliseconds);
        }
    }
}