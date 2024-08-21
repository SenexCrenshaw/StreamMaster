using AutoMapper;

using Microsoft.AspNetCore.Http;

using System.Diagnostics;

namespace StreamMaster.Streams.Services;

public class VideoService(ILogger<VideoService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor, IChannelManager channelManager, IClientConfigurationService clientConfigurationService, IRepositoryWrapper repositoryWrapper, IStreamGroupService streamGroupService) : IVideoService
{
    public async Task<(Stream? stream, IClientConfiguration? clientConfiguration, string? Redirect)> GetStreamAsync(int? streamGroupId, int? streamGroupProfileId, int? smChannelId, CancellationToken cancellationToken)
    {
        Stopwatch sw = Stopwatch.StartNew();

        if (!streamGroupId.HasValue || !streamGroupProfileId.HasValue || !smChannelId.HasValue)
        {
            return (null, null, null);
        }

        int defaultSGId = await streamGroupService.GetDefaultSGIdAsync();

        SMChannel? smChannel = streamGroupId == defaultSGId
            ? repositoryWrapper.SMChannel.GetSMChannel(smChannelId.Value)
            : repositoryWrapper.SMChannel.GetSMChannelFromStreamGroup(smChannelId.Value, streamGroupId.Value);

        if (smChannel == null)
        {
            logger.LogInformation("Channel with ChannelId {smChannelId} not found, exiting", smChannelId);
            return (null, null, null);
        }

        if (smChannel.SMStreams.Count == 0 || string.IsNullOrEmpty(smChannel.SMStreams.First().SMStream.Url))
        {
            logger.LogInformation("Channel with ChannelId {smChannelId} has no streams, exiting", smChannelId);
            return (null, null, null);
        }

        StreamGroupProfile streamGroupProfile = await streamGroupService.GetStreamGroupProfileAsync(null, streamGroupProfileId);
        CommandProfileDto commandProfileDto = await streamGroupService.GetProfileFromSGIdsCommandProfileNameAsync(null, streamGroupProfile.Id, smChannel.CommandProfileName);

        if (commandProfileDto.ProfileName.Equals("Redirect", StringComparison.InvariantCultureIgnoreCase))
        {
            logger.LogInformation("Channel with ChannelId {channelId} redirecting", smChannelId);
            return (null, null, smChannel.SMStreams.First().SMStream.Url);
        }

        string? ipAddress = httpContextAccessor.HttpContext!.Connection.RemoteIpAddress?.ToString();

        SMChannelDto smChannelDto = mapper.Map<SMChannelDto>(smChannel);
        foreach (SMStreamDto sm in smChannelDto.SMStreams.OrderBy(a => a.Rank))
        {
            sm.Rank = smChannel.SMStreams.First(a => a.SMStreamId == sm.Id).Rank;
        }

        HttpRequest request = httpContextAccessor.HttpContext.Request;

        smChannelDto.StreamUrl = $"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}{request.QueryString}";

        string uniqueRequestId = request.HttpContext.TraceIdentifier;
        IClientConfiguration clientConfiguration = clientConfigurationService.NewClientConfiguration(uniqueRequestId, smChannelDto, request.Headers.UserAgent.ToString(), ipAddress ?? "unknown", httpContextAccessor.HttpContext.Response, cancellationToken);

        logger.LogInformation("Requesting channel with ChannelId {channelId}", smChannelId);
        Stream? stream = await channelManager.GetChannelStreamAsync(clientConfiguration, streamGroupProfile.Id, CancellationToken.None);
        sw.Stop();
        if (stream == null)
        {
            //logger.LogInformation("Could not str channel with ChannelId {channelId} to client {id}, took {elapsed}ms", smChannelId, uniqueRequestId, sw.ElapsedMilliseconds);
        }
        else
        {
            logger.LogInformation("Streaming channel with ChannelId {channelId} to client {id}, took {elapsed}ms", smChannelId, uniqueRequestId, sw.ElapsedMilliseconds);
        }

        return (stream, clientConfiguration, null);
    }
}
