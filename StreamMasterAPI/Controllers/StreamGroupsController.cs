using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;
using StreamMasterApplication.StreamGroups;
using StreamMasterApplication.StreamGroups.Commands;
using StreamMasterApplication.StreamGroups.Queries;
using StreamMasterApplication.VideoStreams.Queries;

using StreamMasterDomain.Authentication;
using StreamMasterDomain.Common;
using StreamMasterDomain.Configuration;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Enums;

using System.Collections.Concurrent;
using System.Text;
using System.Web;

namespace StreamMasterAPI.Controllers;

public class StreamGroupsController : ApiControllerBase, IStreamGroupController
{
    private static readonly ConcurrentDictionary<string, ClientTracker> clientTrackers = new();
    private readonly IConfigFileProvider _configFileProvider;
    private readonly ILogger<StreamGroupsController> _logger;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;
    private readonly IRingBufferManager _ringBufferManager;

    public StreamGroupsController(IRingBufferManager ringBufferManager, IConfigFileProvider configFileProvider, IMapper mapper, IMemoryCache memoryCache, ILogger<StreamGroupsController> logger)
    {
        _configFileProvider = configFileProvider;
        _mapper = mapper;
        _memoryCache = memoryCache;
        _ringBufferManager = ringBufferManager;
        _logger = logger;
    }

    public static int GenerateMediaSequence()
    {
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan elapsedTime = DateTime.UtcNow - epochStart;
        int mediaSequence = (int)(elapsedTime.TotalSeconds / 10);

        return mediaSequence;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(StreamGroupDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AddStreamGroup(AddStreamGroupRequest request)
    {
        StreamGroupDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity != null ? CreatedAtAction(nameof(GetStreamGroup), new { id = entity.Id }, entity) : Ok();
    }

    [HttpDelete]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteStreamGroup(DeleteStreamGroupRequest request)
    {
        int? data = await Mediator.Send(request).ConfigureAwait(false);
        return data == null ? NotFound() : NoContent();
    }

    [Route("[action]")]
    [ProducesResponseType(typeof(List<StreamStatisticsResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllStatisticsForAllUrls()
    {
        List<StreamStatisticsResult> data = await Mediator.Send(new GetAllStatisticsForAllUrls()).ConfigureAwait(false);
        return Ok(data);
    }

    [HttpGet]
    [Route("[action]/{id}")]
    [ProducesResponseType(typeof(StreamGroupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StreamGroupDto>> GetStreamGroup(int StreamGroupNumber)
    {
        StreamGroupDto? data = await Mediator.Send(new GetStreamGroup(StreamGroupNumber)).ConfigureAwait(false);

        return data != null ? (ActionResult<StreamGroupDto>)data : (ActionResult<StreamGroupDto>)NotFound();
    }

    [Authorize(Policy = "SGLinks")]
    [HttpGet]
    [Route("[action]/{StreamGroupNumber}")]
    [ProducesResponseType(typeof(StreamGroupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StreamGroupDto>> GetStreamGroupByStreamNumber(int StreamGroupNumber)
    {
        StreamGroupDto? data = await Mediator.Send(new GetStreamGroupByStreamNumber(StreamGroupNumber)).ConfigureAwait(false);

        return data != null ? (ActionResult<StreamGroupDto>)data : (ActionResult<StreamGroupDto>)NotFound();
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("{encodedId}")]
    [Route("{encodedId}/capability")]
    [Route("{encodedId}/device.xml")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetStreamGroupCapability(string encodedId)
    {
        int? streamGroupNumber = encodedId.DecodeValue128(_configFileProvider.Setting.ServerKey);
        if (streamGroupNumber == null)
        {
            return new NotFoundResult();
        }

        string xml = await Mediator.Send(new GetStreamGroupCapability((int)streamGroupNumber)).ConfigureAwait(false);
        return new ContentResult
        {
            Content = xml,
            ContentType = "application/xml",
            StatusCode = 200
        };
    }

    //[HttpGet]
    //[Route("{encodedId}/device.xml")]
    //[ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    //public async Task<IActionResult> GetStreamGroupDeviceXML(string encodedId)
    //{
    //    int? streamGroupNumber = encodedId.DecodeValue128(_configFileProvider.Setting.ServerKey);
    //    if (streamGroupNumber == null)
    //    {
    //        return new NotFoundResult();
    //    }

    //    string xml = await Mediator.Send(new GetStreamGroupCapability((int)streamGroupNumber)).ConfigureAwait(false);
    //    return new ContentResult
    //    {
    //        Content = xml,
    //        ContentType = "application/xml",
    //        StatusCode = 200
    //    };
    //}

    [HttpGet]
    [Route("{encodedId}/discover.json")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetStreamGroupDiscover(string encodedId)
    {
        int? streamGroupNumber = encodedId.DecodeValue128(_configFileProvider.Setting.ServerKey);
        if (streamGroupNumber == null)
        {
            return new NotFoundResult();
        }

        string json = await Mediator.Send(new GetStreamGroupDiscover((int)streamGroupNumber)).ConfigureAwait(false);
        return new ContentResult
        {
            Content = json,
            ContentType = "text/json",
            StatusCode = 200
        };
    }

    //[HttpGet]
    //[Route("{encodedId}/capability")]
    //[ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    //public async Task<IActionResult> GetStreamGroupEncodedCapability(string encodedId)
    //{
    //    int? streamGroupNumber = encodedId.DecodeValue128(_configFileProvider.Setting.ServerKey);
    //    if (streamGroupNumber == null)
    //    {
    //        return new NotFoundResult();
    //    }

    //    string xml = await Mediator.Send(new GetStreamGroupCapability((int)streamGroupNumber)).ConfigureAwait(false);
    //    return new ContentResult
    //    {
    //        Content = xml,
    //        ContentType = "application/xml",
    //        StatusCode = 200
    //    };
    //}

    [Authorize(Policy = "SGLinks")]
    [HttpGet]
    [Route("epg/{encodedId}.xml")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetStreamGroupEPG(string encodedId)
    {
        int? streamGroupNumber = encodedId.DecodeValue128(_configFileProvider.Setting.ServerKey);
        if (streamGroupNumber == null)
        {
            return new NotFoundResult();
        }

        string xml = await Mediator.Send(new GetStreamGroupEPG((int)streamGroupNumber)).ConfigureAwait(false);
        return new FileContentResult(Encoding.UTF8.GetBytes(xml), "application/xml")
        {
            FileDownloadName = $"epg-{streamGroupNumber}.xml"
        };
    }

    [HttpGet]
    [Route("{StreamGroupNumber}/epgGuide.json")]
    [ProducesResponseType(typeof(EPGGuide), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EPGGuide>> GetStreamGroupEPGForGuide(int StreamGroupNumber)
    {
        var data = await Mediator.Send(new GetStreamGroupEPGForGuide(StreamGroupNumber)).ConfigureAwait(false);
        return data != null ? (ActionResult<EPGGuide>)data : (ActionResult<EPGGuide>)NotFound();
    }

    [HttpGet]
    [Route("{encodedId}/lineup.json")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetStreamGroupLineUp(string encodedId)
    {
        int? streamGroupNumber = encodedId.DecodeValue128(_configFileProvider.Setting.ServerKey);
        if (streamGroupNumber == null)
        {
            return new NotFoundResult();
        }

        string json = await Mediator.Send(new GetStreamGroupLineUp((int)streamGroupNumber)).ConfigureAwait(false);
        return new ContentResult
        {
            Content = json,
            ContentType = "application/json",
            StatusCode = 200
        };
    }

    [HttpGet]
    [Route("{encodedId}/lineup_status.json")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetStreamGroupLineUpStatus(string encodedId)
    {
        int? streamGroupNumber = encodedId.DecodeValue128(_configFileProvider.Setting.ServerKey);
        if (streamGroupNumber == null)
        {
            return new NotFoundResult();
        }
        string json = await Mediator.Send(new GetStreamGroupLineUpStatus((int)streamGroupNumber)).ConfigureAwait(false);
        return new ContentResult
        {
            Content = json,
            ContentType = "text/json",
            StatusCode = 200
        };
    }

    [Authorize(Policy = "SGLinks")]
    [HttpGet]
    [Route("m3u/{encodedId}.m3u")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetStreamGroupM3U(string encodedId)
    {
        int? streamGroupNumber = encodedId.DecodeValue128(_configFileProvider.Setting.ServerKey);
        if (streamGroupNumber == null)
        {
            return new NotFoundResult();
        }

        string data = await Mediator.Send(new GetStreamGroupM3U((int)streamGroupNumber)).ConfigureAwait(false);

        return new FileContentResult(Encoding.UTF8.GetBytes(data), "application/x-mpegURL")
        {
            FileDownloadName = $"m3u-{streamGroupNumber}.m3u"
        };
    }

    [HttpGet]
    [Route("{StreamGroupNumber}/m3u2")]
    [Route("{StreamGroupNumber}/m3u2/m3u.m3u")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ContentResult> GetStreamGroupM3U2(int StreamGroupNumber)
    {
        string data = await Mediator.Send(new GetStreamGroupM3U2(StreamGroupNumber)).ConfigureAwait(false);

        return new ContentResult
        {
            Content = data,
            ContentType = "application/x-mpegURL"
        };
    }

    [HttpGet]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StreamGroupDto>))]
    public async Task<ActionResult<IEnumerable<StreamGroupDto>>> GetStreamGroups()
    {
        IEnumerable<StreamGroupDto> data = await Mediator.Send(new GetStreamGroups()).ConfigureAwait(false);
        return data.ToList();
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("{StreamGroupNumber}/stream/{StreamId}/{ClientID}.ts")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStreamGroupVideoM3UStream(int StreamGroupNumber, int StreamId, string ClientID, CancellationToken cancellationToken)
    {
        string clientIdString = HttpContext.Session.GetString("ClientId");
        if (string.IsNullOrEmpty(clientIdString))
        {
            if (string.IsNullOrEmpty(ClientID))
            {
                _logger.LogCritical("ClientID null or mismatch for stream {streamId} as proxy is set to none", StreamId);
                return NotFound();
            }

            clientIdString = ClientID;
        }

        ClientTracker clientTracker;
        clientTrackers.TryGetValue(clientIdString, out clientTracker);
        if (clientTracker == null)
        {
            _logger.LogInformation("GetStreamGroupVideoStream request. SG Number {id} ChannelId {channelId} not found exiting", StreamGroupNumber, StreamId);
            return NotFound();
        }

        _logger.LogInformation("GetStreamM3U8 for stream {clientIdString} ", clientIdString);

        Guid clientId;
        if (string.IsNullOrEmpty(clientIdString) || clientIdString != ClientID || !Guid.TryParse(clientIdString, out clientId))
        {
            _logger.LogCritical("ClientID null or mismatch for stream {streamId} as proxy is set to none", StreamId);
            return NotFound();
        }

        var settings = FileUtil.GetSetting();

        if (settings.StreamingProxyType == StreamingProxyTypes.None)
        {
            _logger.LogCritical("Cannot get M3U8 for stream {streamId} as proxy is set to none", StreamId);
            return NotFound();
        }

        var videoStream = await Mediator.Send(new GetVideoStream(StreamId), cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("GetStreamGroupVideoStream request. SG Number {id} ChannelId {channelId}", StreamGroupNumber, StreamId);

        if (videoStream == null)
        {
            _logger.LogInformation("GetStreamGroupVideoStream request. SG Number {id} ChannelId {channelId} not found exiting", StreamGroupNumber, StreamId);
            return NotFound();
        }

        int streamBufferSize = 128 * 1024;

        List<VideoStreamDto> videoStreams = new List<VideoStreamDto> { videoStream };
        StreamerConfiguration config = new()
        {
            VideoStreams = videoStreams,
            BufferSize = settings.RingBufferSizeMB * 1024 * 1000,
            CancellationToken = cancellationToken,
            MaxConnectRetry = settings.MaxConnectRetry,
            MaxConnectRetryTimeMS = settings.MaxConnectRetryTimeMS,
            M3UStream = true
        };
        config.ClientId = clientId;

        // Get the read stream for the client
        (Stream? stream, Guid clientIdNew, ProxyStreamError? error) = await _ringBufferManager.GetStream(config);

        if (stream != null)
        {
            int maxbytes = (int)(3.7 * 1000 * 1024);

            // Send the response in chunks
            var outputStream = Response.Body;

            var buffTimeOut = 6;

            if (clientTracker.SegmentCount < 1)
            {
                ++clientTracker.SegmentCount;
                maxbytes = 1 * 1000 * 1024;
                buffTimeOut = 1;
            }

            byte[] buffer = new byte[streamBufferSize];

            FileStreamResult fileStreamResult = new(stream, "video/mp4");
            var fileStream = fileStreamResult.FileStream;

            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(buffTimeOut));
            _logger.LogDebug("clientTracker offset: {OffSet}", clientTracker.OffSet);
            var sent = 0;

            while (!cts.Token.IsCancellationRequested)
            {
                int bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead == 0)
                {
                    // No more data to read
                    break;
                }

                await outputStream.WriteAsync(buffer, 0, bytesRead);
                await outputStream.FlushAsync();
                //clientTracker.OffSet += bytesRead;

                if (cts.Token.IsCancellationRequested)
                {
                    _logger.LogInformation("GetStreamGroupVideoM3UStream Token Cancelled {sent}", sent);
                    // Stop sending data after 10 seconds
                    break;
                }

                sent += bytesRead;
                if (maxbytes != 0 && sent >= maxbytes)
                {
                    _logger.LogInformation("GetStreamGroupVideoM3UStream Max Bytes Cancelled {sent}", sent);
                    break;
                }
                //if (maxbytes == 0)
                //{
                //    // Check if cancellation has been requested
                //    if (cts.Token.IsCancellationRequested)
                //    {
                //        // Stop sending data after 10 seconds
                //        break;
                //    }
                //}
                //else
                //{
                //    sent += bytesRead;
                //    if (maxbytes != 0 && sent + bytesRead >= maxbytes)
                //    {
                //        break;
                //    }
                //}
            }

            return new EmptyResult();
        }
        else if (error != null)
        {
            // Log the error using the built-in logging framework
            _logger.LogError("Error getting FFmpeg stream: {error.Message}", error.Message);

            return GetStatus(error.ErrorCode);
            // Return an appropriate error response to the client
        }
        else
        {
            // Unknown error occurred
            return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred");
        }
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("stream/{encodedIds}")]
    [Route("stream/{encodedIds}.mp4")]
    [Route("stream/{encodedIds}/{name}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStreamGroupVideoStream(string encodedIds,string name, CancellationToken cancellationToken)
    {
        (int? StreamGroupNumberNull, int? StreamIdNull) = encodedIds.DecodeValues128(_configFileProvider.Setting.ServerKey);
        if (StreamGroupNumberNull == null || StreamIdNull == null)
        {
            return new NotFoundResult();
        }

        int streamGroupNumber = (int)StreamGroupNumberNull;
        int videoStreamId = (int)StreamIdNull;

        var videoStream = await Mediator.Send(new GetVideoStream(videoStreamId), cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("GetStreamGroupVideoStream request. SG Number {id} ChannelId {channelId}", streamGroupNumber, videoStreamId);

        if (videoStream == null)
        {
            _logger.LogInformation("GetStreamGroupVideoStream request. SG Number {id} ChannelId {channelId} not found exiting", streamGroupNumber, videoStreamId);
            return NotFound();
        }

        HttpContext.Session.Remove("ClientId");

        var settings = FileUtil.GetSetting();

        if (settings.StreamingProxyType == StreamingProxyTypes.None)
        {
            _logger.LogInformation("GetStreamGroupVideoStream request SG Number {id} ChannelId {channelId} proxy is none, sending redirect", streamGroupNumber, videoStreamId);

            return Redirect(videoStream.User_Url);
        }

        List<VideoStreamDto> videoStreams = new List<VideoStreamDto> { videoStream };
        StreamerConfiguration config = new()
        {
            VideoStreams = videoStreams,
            BufferSize = settings.RingBufferSizeMB * 1024 * 1000,
            CancellationToken = cancellationToken,
            MaxConnectRetry = settings.MaxConnectRetry,
            MaxConnectRetryTimeMS = settings.MaxConnectRetryTimeMS,
        };

        // Get the read stream for the client
        (Stream? stream, Guid clientId, ProxyStreamError? error) = await _ringBufferManager.GetStream(config);

        HttpContext.Response.RegisterForDispose(new UnregisterClientOnDispose(_ringBufferManager, config));
        if (stream != null)
        {
            return new FileStreamResult(stream, "video/mp4");
        }
        else if (error != null)
        {
            // Log the error using the built-in logging framework
            _logger.LogError("Error getting FFmpeg stream: {error.Message}", error.Message);

            return GetStatus(error.ErrorCode);
            // Return an appropriate error response to the client
        }
        else
        {
            // Unknown error occurred
            return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred");
        }
    }

    [HttpGet]
    [Route("{streamGroupNumber}/stream/{streamId}.m3u8")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetStreamM3U8(int streamGroupNumber, int streamId, CancellationToken cancellationToken)
    {
        var settings = FileUtil.GetSetting();

        if (settings.StreamingProxyType == StreamingProxyTypes.None)
        {
            _logger.LogCritical("Cannot get M3U8 Playlist for stream {streamId} as proxy is set to none", streamId);
            return NotFound();
        }

        var clientIdString = CheckClientID(streamGroupNumber, streamId);

        _logger.LogInformation("GetStreamM3U8 for stream {clientIdString} ", clientIdString);

        string url = $"/api/streamgroups/{streamGroupNumber}/stream/{streamId}/{clientIdString}.ts";

        int mediaSequence = GenerateMediaSequence();

        StringBuilder playlistBuilder = new StringBuilder();

        playlistBuilder.AppendLine("#EXTM3U");
        playlistBuilder.AppendLine("#EXT-X-VERSION:3");
        playlistBuilder.AppendLine("#EXT-X-ALLOW-CACHE:YES");
        playlistBuilder.AppendLine("#EXT-X-TARGETDURATION:10");
        playlistBuilder.AppendLine("#EXT-X-MEDIA-SEQUENCE:" + mediaSequence);
        playlistBuilder.AppendLine();

        for (int i = 0; i < 48; i++)
        {
            playlistBuilder.AppendLine("#EXTINF:10.010000,");
            playlistBuilder.AppendLine(url);
        }

        var m3u8Playlist = playlistBuilder.ToString();

        //        string m3u8Playlist = @"#EXTM3U
        //#EXT-X-VERSION:3
        //#EXT-X-ALLOW-CACHE:YES
        //#EXT-X-TARGETDURATION:10
        //#EXT-X-MEDIA-SEQUENCE:" + mediaSequence + @"

        //#EXTINF:1,
        //" + url + @"
        //#EXTINF:10.010000,
        //" + url + @"
        //#EXTINF:86400,
        //" + url + @"
        //#EXTINF:10.010000,
        //" + url + @"
        //#EXTINF:10.010000,
        //" + url + @"
        //#EXTINF:10.010000,
        //" + url;

        // _logger.LogInformation("GetStreamM3U8 for stream {clientIdString}
        // returning playlist:\n{m3u8Playlist}", clientIdString, m3u8Playlist);

        return new ContentResult
        {
            Content = m3u8Playlist,
            ContentType = "application/x-mpegURL",
            StatusCode = 200
        };
    }

    [HttpGet]
    [Route("{StreamGroupNumber}/stream/{StreamId}/{clientId}.m3u8")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetStreamM3U8WithClientId(int StreamGroupNumber, int StreamId, string clientId, CancellationToken cancellationToken)
    {
        var settings = FileUtil.GetSetting();

        if (settings.StreamingProxyType == StreamingProxyTypes.None)
        {
            _logger.LogCritical("Cannot get M3U8 Playlist for stream {streamId} as proxy is set to none", StreamId);
            return NotFound();
        }

        var clientIdString = CheckClientID(StreamGroupNumber, StreamId, clientId);

        ClientTracker clientTracker;
        clientTrackers.TryGetValue(clientIdString, out clientTracker);
        if (clientTracker == null)
        {
            _logger.LogInformation("GetStreamM3U8WithClientId request. SG Number {id} ChannelId {channelId} not found exiting", StreamGroupNumber, StreamId);
            return NotFound();
        }

        _logger.LogInformation("GetStreamM3U8 for stream {clientIdString} ", clientIdString);

        string url = $"/api/streamgroups/{StreamGroupNumber}/stream/{StreamId}/{clientIdString}.ts";

        int mediaSequence = GenerateMediaSequence();

        const string timeout = "10.0";
        StringBuilder playlistBuilder = new StringBuilder();

        playlistBuilder.AppendLine("#EXTM3U");
        playlistBuilder.AppendLine("#EXT-X-VERSION:3");
        playlistBuilder.AppendLine("#EXT-X-ALLOW-CACHE:YES");
        playlistBuilder.AppendLine("#EXT-X-TARGETDURATION:" + timeout);
        playlistBuilder.AppendLine("#EXT-X-MEDIA-SEQUENCE:" + mediaSequence);
        playlistBuilder.AppendLine();

        ++clientTracker.M3UGet;
        if (clientTracker.M3UGet == 1 || clientTracker.SegmentCount < 1)
        {
            playlistBuilder.AppendLine($"#EXTINF:1,\n{url}");
        }

        for (int i = 0; i < 5; i++)
        {
            playlistBuilder.AppendLine($"#EXTINF:{timeout},\n{url}");
        }

        var m3u8Playlist = playlistBuilder.ToString();

        //_logger.LogInformation("GetStreamM3U8 for stream {clientIdString} returning playlist:\n{m3u8Playlist}", clientIdString, m3u8Playlist);

        return new ContentResult
        {
            Content = m3u8Playlist,
            ContentType = "application/x-mpegURL",
            StatusCode = 200
        };
    }

    [HttpPost]
    [Route("[action]/{streamUrl}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult SimulateStreamFailure(string streamUrl)
    {
        if (string.IsNullOrEmpty(streamUrl))
        {
            return BadRequest("streamUrl is required.");
        }

        _ringBufferManager.SimulateStreamFailure(HttpUtility.UrlDecode(streamUrl));
        return Ok();
    }

    [HttpPut]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateStreamGroup(UpdateStreamGroupRequest request)
    {
        StreamGroupDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity == null ? NotFound() : NoContent();
    }

    private string CheckClientID(int streamGroupNumber, int streamId, string clientId = "")
    {
        if (!string.IsNullOrEmpty(clientId))
        {
            if (!clientTrackers.TryGetValue(clientId, out _))
            {
                clientTrackers.TryAdd(clientId, new ClientTracker { StreamGroupNumber = streamGroupNumber, StreamId = streamId });
            }
            HttpContext.Session.SetString("ClientId", clientId);
            return clientId;
        }

        string clientIdString = HttpContext.Session.GetString("ClientId");

        if (string.IsNullOrEmpty(clientIdString) ||
            !clientTrackers.TryGetValue(clientIdString, out ClientTracker cl)
            || cl.StreamGroupNumber != streamGroupNumber || cl.StreamId != streamId)
        {
            clientIdString = Guid.NewGuid().ToString();
            clientTrackers.TryAdd(clientIdString, new ClientTracker { StreamGroupNumber = streamGroupNumber, StreamId = streamId });
        }

        HttpContext.Session.SetString("ClientId", clientIdString);
        return clientIdString;
    }

    private ObjectResult GetStatus(ProxyStreamErrorCode proxyStreamErrorCode)
    {
        return proxyStreamErrorCode switch
        {
            ProxyStreamErrorCode.FileNotFound => StatusCode(StatusCodes.Status500InternalServerError, "FFmpeg executable not found"),
            ProxyStreamErrorCode.IoError => StatusCode(StatusCodes.Status502BadGateway, "Error connecting to upstream server"),
            ProxyStreamErrorCode.UnknownError => StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred"),
            ProxyStreamErrorCode.HttpRequestError => StatusCode(StatusCodes.Status500InternalServerError, "An Http Request Error occurred"),
            ProxyStreamErrorCode.ChannelManagerFinished => StatusCode(200, "Channel Manager Exited"),
            ProxyStreamErrorCode.HttpError => StatusCode(StatusCodes.Status500InternalServerError, "An Http Request Error occurred"),
            ProxyStreamErrorCode.DownloadError => StatusCode(StatusCodes.Status500InternalServerError, "Could not parse stream"),
            ProxyStreamErrorCode.MasterPlayListNotSupported => StatusCode(StatusCodes.Status500InternalServerError, "M3U8 Master playlist not supported"),
            _ => StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred"),
        };
    }

    private string GetUrl()
    {
        var request = HttpContext.Request;
        var scheme = request.Scheme;
        var host = request.Host;
        var path = request.Path;
        var queryString = request.QueryString;

        var url = $"{scheme}://{host}{path}{queryString}";

        return url;
    }

    private class UnregisterClientOnDispose : IDisposable
    {
        private readonly StreamerConfiguration _config;
        private readonly IRingBufferManager _ringBufferManager;

        public UnregisterClientOnDispose(IRingBufferManager ringBufferManager, StreamerConfiguration config)
        {
            _ringBufferManager = ringBufferManager;
            _config = config;
        }

        public void Dispose()
        {
            _ringBufferManager.RemoveClient(_config);
        }
    }
}

public class ClientTracker
{
    public Guid ClientId { get; set; }
    public int M3UGet { get; set; } = 0;
    public int OffSet { get; set; }
    public int SegmentCount { get; set; } = 0;
    public int StreamGroupNumber { get; set; }
    public int StreamId { get; set; }
}
