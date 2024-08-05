namespace StreamMaster.API.Controllers;
//[V1ApiController("m")]
//public class MController(ILogger<MController> logger, ILogger<IByteTrackingChannel<byte[]>> byteLogger, IStreamManager streamManager, ICustomPlayListBuilder customPlayListBuilder, IM3U8Generator M3U8Generator, IMapper mapper, ICryptoService cryptoService, IRepositoryWrapper repositoryWrapper) : Controller
//{
//    //[Authorize(Policy = "SGLinks")]
//    //[HttpGet]
//    //[HttpHead]
//    //[Route("{encodedString}.m3u8")]
//    //public async Task<ActionResult> GetSMChannelM3U(string encodedString)
//    //{
//    //    (int? streamGroupId, int? smChannelId) = await cryptoService.DecodeStreamGroupIdChannelIdAsync(encodedString);
//    //    if (streamGroupId == null || smChannelId == null)
//    //    {
//    //        return NotFound();
//    //    }

//    //    SMChannel? smChannel = repositoryWrapper.SMChannel.GetSMChannel(smChannelId.Value);
//    //    if (smChannel == null)
//    //    {
//    //        logger.LogInformation("GetSMChannelM3U SG Number {id} Channel Id {Id}", smChannelId, smChannelId);
//    //        return NotFound();
//    //    }

//    //    logger.LogInformation("GetSMChannelM3U SG Number {id} Channel Name {name}", streamGroupId, smChannel.Name);

//    //    if (smChannel.SMStreams.Count == 0 || string.IsNullOrEmpty(smChannel.SMStreams.First().SMStream.Url))
//    //    {
//    //        logger.LogInformation("GetSMChannelM3U SG Number {id} hannel Name {name} no streams", streamGroupId, smChannel.Name);
//    //        return new NotFoundResult();
//    //    }

//    //    HttpContext.Response.Headers.Connection = "close";
//    //    HttpContext.Response.Headers.AccessControlAllowOrigin = "*";
//    //    HttpContext.Response.Headers.AccessControlExposeHeaders = "Content-Length";
//    //    HttpContext.Response.Headers.CacheControl = "no-cache";
//    //    List<string> Ids = [];
//    //    if (smChannel.IsCustomStream)
//    //    {
//    //        CustomPlayList? customPlayList = customPlayListBuilder.GetCustomPlayList(smChannel.Name);
//    //        if (customPlayList == null)
//    //        {
//    //            return NotFound();
//    //        }

//    //        foreach (CustomStreamNfo customStreamNfo in customPlayList.CustomStreamNfos)
//    //        {
//    //            string smStreamId = $"{customPlayList.Name}|{customStreamNfo.Movie.Title}";
//    //            SMStreamDto? smStream = repositoryWrapper.SMStream.GetSMStream(smStreamId);
//    //            if (smStream == null)
//    //            {
//    //                continue;
//    //            }
//    //            string? id = customStreamNfo.Movie.Title;
//    //            string? c = await cryptoService.EncodeStreamGroupIdStreamIdAsync(streamGroupId.Value, smStreamId);
//    //            if (c != null)
//    //            {
//    //                Ids.Add($"http://127.0.0.1:7095/m/{c}.ts");
//    //                //Ids.Add($"/m/{c}.ts");
//    //            }
//    //        }
//    //    }
//    //    else
//    //    {

//    //        foreach (string? id in smChannel.SMStreams.Select(a => a.SMStream.Id))
//    //        {
//    //            string? c = await cryptoService.EncodeStreamGroupIdStreamIdAsync(streamGroupId.Value, id);
//    //            if (c != null)
//    //            {
//    //                Ids.Add($"http://127.0.0.1:7095/m/{c}.ts");
//    //                //Ids.Add($"/m/{c}.ts");
//    //            }
//    //        }
//    //    }

//    //    string m3u8Content = M3U8Generator.CreateM3U8Content(Ids, true);

//    //    Response.Headers.Append("Content-Disposition", $"attachment; filename=index.m3u8");
//    //    return Content(m3u8Content, "application/vnd.apple.mpegurl");
//    //}

//    //[Authorize(Policy = "SGLinks")]
//    //[HttpGet]
//    //[HttpHead]
//    //[Route("{encodedString}.ts")]
//    //public async Task<IActionResult> GetSMStream(string encodedString, CancellationToken cancellationToken)
//    //{
//    //    SMStreamDto smStreamDto = new();
//    //    if (encodedString == "comedy")
//    //    {
//    //        smStreamDto = new()
//    //        {
//    //            Id = "comedy",
//    //            Name = "Comedy SPecial",
//    //            Url = "http://127.0.0.1:7095/m/kjUXihuLvcv7rwA8Z8GfgZA7pPtpZo-B4LjG1USoxF14zwI-AEGtYlwotviuIaruPkG3zAF4KFF9DXCuQZ_mpg.m3u8"
//    //        };
//    //    }
//    //    else if (encodedString == "intros")
//    //    {
//    //        smStreamDto = new()
//    //        {
//    //            Id = "Intros",
//    //            Name = "Intros",
//    //            Url = "http://10.3.10.50:7095/m/PY_BagGUAPGoCSnuCkx1K2zFVNcb4gj42nggyaOCQUKe2sia9m52j90u4jBPsQ8DkwPALKl7EmkSAH8SJOHbDA.m3u8"
//    //        };
//    //    }

//    //    else if (encodedString == "intro1")
//    //    {
//    //        smStreamDto = new()
//    //        {
//    //            Id = "Intros",
//    //            Name = "Intros",
//    //            Url = "http://127.0.0.1:7095/m/kjUXihuLvcv7rwA8Z8GfgZA7pPtpZo-B4LjG1USoxF14zwI-AEGtYlwotviuIaruPkG3zAF4KFF9DXCuQZ_mpg.m3u8"
//    //        };
//    //    }
//    //    else
//    //    {

//    //        (int? streamGroupId, string? smStreamId) = await cryptoService.DecodeStreamGroupIdStreamIdAsync(encodedString);
//    //        if (streamGroupId == null || smStreamId == null)
//    //        {
//    //            logger.LogError("Encode error");
//    //            return NotFound();
//    //        }

//    //        SMStream? smStream = await repositoryWrapper.SMStream.FirstOrDefaultAsync(a => a.Id == smStreamId).ConfigureAwait(false);
//    //        if (smStream == null)
//    //        {
//    //            return NotFound();
//    //        }
//    //        smStreamDto = mapper.Map<SMStreamDto>(smStream);
//    //    }


//    //    CancellationTokenSource cancellationTokenSource = new();
//    //    CancellationTokenSource linkedToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationTokenSource.Token, cancellationToken);

//    //    SMStreamInfo idname = new()
//    //    {
//    //        Id = smStreamDto.Id,
//    //        Name = smStreamDto.Name,
//    //        Url = smStreamDto.Url,
//    //        IsCustomStream = smStreamDto.IsCustomStream,
//    //        M3UFileId = smStreamDto.M3UFileId
//    //    };

//    //    IStreamHandler? handler = await streamManager.GetOrCreateStreamHandlerAsync(idname).ConfigureAwait(false);
//    //    if (handler == null)
//    //    {
//    //        return NotFound();
//    //    }

//    //    IByteTrackingChannel<byte[]> channel = new ByteTrackingChannel(byteLogger, 200, 10);

//    //    HttpContext.Response.Headers.Connection = "close";
//    //    HttpContext.Response.Headers.AccessControlAllowOrigin = "*";
//    //    HttpContext.Response.Headers.CacheControl = "no-cache";
//    //    HttpContext.Response.ContentType = "video/mp2t";

//    //    using CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, HttpContext.RequestAborted);

//    //    try
//    //    {
//    //        HttpContext.Response.RegisterForDispose(new UnregisterClientOnDispose(streamManager, smStreamDto));
//    //        await foreach (byte[]? buffer in channel.ReadAllAsync(linkedTokenSource.Token).ConfigureAwait(false))
//    //        {
//    //            if (linkedTokenSource.Token.IsCancellationRequested)
//    //            {
//    //                break;
//    //            }

//    //            await HttpContext.Response.Body.WriteAsync(buffer, linkedTokenSource.Token).ConfigureAwait(false);
//    //            await HttpContext.Response.Body.FlushAsync(linkedTokenSource.Token).ConfigureAwait(false);
//    //        }

//    //        await HttpContext.Response.Body.FlushAsync(linkedTokenSource.Token).ConfigureAwait(false);
//    //    }
//    //    catch (OperationCanceledException)
//    //    {
//    //        logger.LogInformation("Streaming operation cancelled.");
//    //    }
//    //    catch (Exception ex)
//    //    {
//    //        logger.LogError(ex, "An error occurred while streaming live video.");
//    //        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while streaming live video.");
//    //    }
//    //    await HttpContext.Response.Body.FlushAsync().ConfigureAwait(false);
//    //    return new EmptyResult();
//    //}


//    //private void Handler_OnStreamingStoppedEvent(object? sender, StreamHandlerStopped e)
//    //{
//    //    throw new NotImplementedException();
//    //}

//    //private class UnregisterClientOnDispose(IStreamManager streamManager, SMStreamDto smStreamDto) : IDisposable
//    //{
//    //    private readonly IStreamManager _streamManager = streamManager;
//    //    private readonly SMStreamDto _smStreamDto = smStreamDto;

//    //    public void Dispose()
//    //    {
//    //        _streamManager.RemoveClient(_smStreamDto);
//    //    }
//    //}

//    //[Authorize(Policy = "SGLinks")]
//    //[HttpGet]
//    //[HttpHead]
//    //[Route("{encodedString}.m3u8")]
//    //public async Task<ActionResult> GetM3U8(string encodedString, CancellationToken cancellationToken)
//    //{
//    //    //await HttpContext.LogRequestDetailsAsync(logger);

//    //    (int? streamGroupId, int? SMChannelId) = cryptoService.DecodeStreamGroupIdChannelIdAsync(encodedString);

//    //    SMChannel? smChannel = await repositoryWrapper.SMChannel.FirstOrDefaultAsync(a => a.Id == SMChannelId, cancellationToken: cancellationToken);
//    //    if (smChannel is null)
//    //    {
//    //        return NotFound();
//    //    }

//    //    SMChannelDto smChannelDto = mapper.Map<SMChannelDto>(smChannel);

//    //    IM3U8ChannelStatus? channelStatus = await hlsManager.TryAddAsync(smChannelDto, CancellationToken.None);
//    //    if (channelStatus == null || channelStatus.SMStreamInfo == null || string.IsNullOrEmpty(channelStatus.SMStreamInfo.Url))
//    //    {
//    //        return NotFound();
//    //    }

//    //    HLSSettings hlsSettings = intHLSSettings.CurrentValue;
//    //    StreamAccessInfo streamAccessInfo = accessTracker.UpdateAccessTime(channelStatus.SMStreamInfo.Id + ".m3u8", channelStatus.SMStreamInfo.Id, TimeSpan.FromSeconds(hlsSettings.HLSM3U8ReadTimeOutInSeconds));
//    //    if (streamAccessInfo.MillisecondsSinceLastUpdate > 0)
//    //    {
//    //        logger.LogInformation("M3U8 last update {key} {Milliseconds}ms {HLSM3U8ReadTimeOutInSeconds}", streamAccessInfo.Key, streamAccessInfo.MillisecondsSinceLastUpdate, hlsSettings.HLSM3U8ReadTimeOutInSeconds);
//    //    }

//    //    HttpContext.Response.Headers.Connection = "close";
//    //    HttpContext.Response.Headers.AccessControlAllowOrigin = "*";
//    //    HttpContext.Response.Headers.AccessControlExposeHeaders = "Content-Length";
//    //    HttpContext.Response.Headers.CacheControl = "no-cache";

//    //    string m3u8Content = await System.IO.File.ReadAllTextAsync(channelStatus.M3U8File, cancellationToken);
//    //    return Content(m3u8Content, "application/vnd.apple.mpegurl");
//    //}

//    //[Authorize(Policy = "SGLinks")]
//    //[HttpGet]
//    //[HttpHead]
//    //[Route("{encodedString}/{num}.ts")]
//    //public async Task<IActionResult> GetVideoStream(string encodedString, int num, CancellationToken cancellationToken)
//    //{
//    //    string? smStreamId = cryptoService.DecodeString(encodedString);
//    //    if (smStreamId == null)
//    //    {
//    //        logger.LogError("Encode error");
//    //        return NotFound();
//    //    }

//    //    string M3U8Directory = M3U8ChannelStatus.GetDirectory(smStreamId);

//    //    HLSSettings hlsSettings = intHLSSettings.CurrentValue;
//    //    int segment = hlsSettings.HLSSegmentCount / 2;
//    //    int timeout = hlsSettings.HLSSegmentCount / 2 * hlsSettings.HLSSegmentDurationInSeconds;
//    //    string tsFile = Path.Combine(M3U8Directory, $"{num}.ts");
//    //    if (!await FileUtil.WaitForFileAsync(tsFile, timeout, 50, cancellationToken).ConfigureAwait(false))
//    //    {
//    //        Debug.WriteLine("File not found: {0}", tsFile);
//    //        return NotFound();
//    //    }

//    //    try
//    //    {
//    //        StreamAccessInfo streamAccessInfo = accessTracker.UpdateAccessTime(smStreamId, smStreamId, TimeSpan.FromSeconds(hlsSettings.HLSTSReadTimeOutInSeconds));
//    //        if (streamAccessInfo.MillisecondsSinceLastUpdate > 0)
//    //        {
//    //            logger.LogInformation("TS last update took {key} {Milliseconds}ms {HLSM3U8ReadTimeOutInSeconds}", streamAccessInfo.Key, streamAccessInfo.MillisecondsSinceLastUpdate, hlsSettings.HLSTSReadTimeOutInSeconds);
//    //        }

//    //        HttpContext.Response.Headers.Connection = "close";
//    //        HttpContext.Response.Headers.AccessControlAllowOrigin = "*";
//    //        HttpContext.Response.Headers.CacheControl = "no-cache";

//    //        FileStream stream = new(tsFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
//    //        return new FileStreamResult(stream, "video/mp2t");
//    //    }
//    //    catch (Exception ex)
//    //    {
//    //        logger.LogError(ex, "Error streaming video file {FileName}", tsFile);
//    //        return StatusCode(500, "Error streaming video");
//    //    }
//    //}

//}
