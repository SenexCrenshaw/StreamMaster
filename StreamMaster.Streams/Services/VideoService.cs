using System.Diagnostics;

using AutoMapper;

using Microsoft.AspNetCore.Http;

namespace StreamMaster.Streams.Services;

public class VideoService(
    ILogger<VideoService> logger,
    IMapper mapper,
    //IHttpContextAccessor httpContextAccessor,
    IChannelService channelService,
    IClientConfigurationService clientConfigurationService,
    IRepositoryWrapper repositoryWrapper,
    IStreamGroupService streamGroupService) : IVideoService
{
    public async Task<StreamResult> AddClientToChannelAsync(HttpContext HttpContext, int? streamGroupId, int? streamGroupProfileId, int? smChannelId, CancellationToken cancellationToken)
    {
        Stopwatch sw = Stopwatch.StartNew();
        try
        {
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
            if (!ChannelHasStreamsOrChannels(smChannel))
            {
                logger.LogInformation("Channel with ChannelId {smChannelId} has no streams or channels, exiting", smChannelId);
                return new StreamResult();
            }

            SMChannelDto smChannelDto = mapper.Map<SMChannelDto>(smChannel);

            await repositoryWrapper.SMChannelStreamLink.UpdateSMChannelDtoRanks(smChannelDto);
            await repositoryWrapper.SMChannelChannelLink.UpdateSMChannelDtoRanks(smChannelDto);

            // Fetch stream profile
            StreamGroupProfile streamGroupProfile = await streamGroupService.GetStreamGroupProfileAsync(null, streamGroupProfileId);
            CommandProfileDto commandProfileDto = await streamGroupService.GetProfileFromSGIdsCommandProfileNameAsync(null, streamGroupProfile.Id, smChannelDto.CommandProfileName);

            // Handle redirects
            if (commandProfileDto.ProfileName.Equals("Redirect", StringComparison.InvariantCultureIgnoreCase))
            {
                logger.LogInformation("Channel with ChannelId {channelId} redirecting", smChannelId);
                return new StreamResult { RedirectUrl = smChannelDto.SMStreamDtos[0].Url };
            }

            // Create client configuration
            IClientConfiguration clientConfiguration = CreateClientConfiguration(HttpContext, smChannelDto, cancellationToken);

            // Get stream
            bool result = await channelService.AddClientToChannelAsync(clientConfiguration, streamGroupProfile.Id, CancellationToken.None);
            sw.Stop();
            if (result)
            {
                logger.LogInformation("Streaming channel with ChannelId {channelId} took {elapsed}ms", smChannelId, sw.ElapsedMilliseconds);
            }
            else
            {
                logger.LogInformation("Channel with ChannelId {channelId} failed in {elapsed}ms", smChannelId, sw.ElapsedMilliseconds);
            }

            return new StreamResult { ClientConfiguration = clientConfiguration };
        }
        finally
        {
            sw.Stop();
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

    private static bool ChannelHasStreamsOrChannels(SMChannel smChannel)
    {
        return smChannel.SMChannelType == StreamMaster.Domain.Enums.SMChannelTypeEnum.MultiView
            ? smChannel.SMChannels.Count > 0
            : smChannel.SMStreams.Count > 0 && !string.IsNullOrEmpty(smChannel.SMStreams.First().SMStream!.Url);
    }

    private IClientConfiguration CreateClientConfiguration(HttpContext HttpContext, SMChannelDto smChannelDto, CancellationToken cancellationToken)
    {
        string? ipAddress = HttpContext!.Connection.RemoteIpAddress?.ToString();

        HttpRequest request = HttpContext.Request;
        smChannelDto.StreamUrl = $"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}{request.QueryString}";

        string uniqueRequestId = request.HttpContext.TraceIdentifier;

        return clientConfigurationService.NewClientConfiguration(
            uniqueRequestId,
            smChannelDto,
            request.Headers.UserAgent.ToString(),
            ipAddress ?? "unknown",
            HttpContext.Response,
            cancellationToken
        );
    }
}