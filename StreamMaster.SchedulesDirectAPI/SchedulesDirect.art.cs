using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI.Domain.Enums;
using StreamMaster.SchedulesDirectAPI.Helpers;

using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Enums;

using System.Collections.Concurrent;
using System.Net;
using System.Text.Json;

namespace StreamMaster.SchedulesDirectAPI;
public partial class SchedulesDirect
{   
    private static readonly SemaphoreSlim downloadSemaphore = new(MaxParallelDownloads);


    public async Task<List<ProgramMetadata>?> GetArtworkAsync(string[] request)
    {
        DateTime dtStart = DateTime.Now;
        List<ProgramMetadata>? ret = await schedulesDirectAPI.GetApiResponse<List<ProgramMetadata>>(APIMethod.POST, "metadata/programs/", request);
        if (ret != null)
        {
            logger.LogDebug($"Successfully retrieved artwork info for {ret.Count}/{request.Length} programs. ({DateTime.Now - dtStart:G})");
        }
        else
        {
            logger.LogDebug($"Did not receive a response from Schedules Direct for artwork info of {request.Length} programs. ({DateTime.Now - dtStart:G})");
        }

        return ret;
    }

    public async Task<List<string>?> GetCustomLogosFromServerAsync(string server)
    {
        return await schedulesDirectAPI.GetApiResponse<List<string>>(APIMethod.GET, server);
    }


    private void DownloadImageResponses(List<string> imageQueue, ConcurrentBag<ProgramMetadata> programMetadata,  int start = 0)
    {
        // reject 0 requests
        if (imageQueue.Count - start < 1) return;

        // build the array of series to request images for
        var series = new string[Math.Min(imageQueue.Count - start, MaxImgQueries)];
        for (var i = 0; i < series.Length; ++i)
        {
            series[i] = imageQueue[start + i];
        }

        // request images from Schedules Direct
        var responses =  GetArtworkAsync(series).Result;
        if (responses != null)
        {
            Parallel.ForEach(responses, programMetadata.Add);
        }
        else
        {
            logger.LogInformation("Did not receive a response from Schedules Direct for artwork info of {count} programs, first entry {entry}.", series.Length, series.Any()? series[0]:"");
            var AA = 1;
        }
    }

    private void UpdateMovieIcons(List<MxfProgram> mxfPrograms)
    {
        foreach (var prog in mxfPrograms.Where(a => a.extras.ContainsKey("artwork")))
        {
            List<ProgramArtwork> artwork = prog.extras["artwork"];
            //artwork = SDHelpers.GetTieredImages(artwork, ["episode"]).Where(arg => arg.Aspect.Equals("2x3")).ToList();
            UpdateIcons(artwork.Select(a => a.Uri), prog.Title);
        }
    }
      

          private void UpdateIcons(List<MxfService> Services)
    {
        foreach (var service in Services.Where(a => a.extras.ContainsKey("logo")))
        {
           StationImage artwork = service.extras["logo"];
            //artwork = SDHelpers.GetTieredImages(artwork);

            UpdateIcon(artwork.Url, service.CallSign);
        }
    }
    private void UpdateIcons(List<MxfProgram> mxfPrograms)
    {
        foreach (var prog in mxfPrograms.Where(a => a.extras.ContainsKey("artwork")))
        {
            List<ProgramArtwork> artwork = prog.extras["artwork"];
            //artwork = SDHelpers.GetTieredImages(artwork);

            UpdateIcons(artwork.Select(a => a.Uri), prog.Title);
        }
    }

    private void UpdateSeasonIcons(List<MxfSeason> mxfSeasons)
    {
        foreach (var prog in mxfSeasons.Where(a => a.extras.ContainsKey("artwork")))
        {
            List<ProgramArtwork> artwork = prog.extras["artwork"];
            //artwork = SDHelpers.GetTieredImages(artwork, new List<string> { "season" })
            UpdateIcons(artwork.Select(a => a.Uri), prog.Title);
        }
    }
    private void UpdateIcons(List<MxfSeriesInfo> mxfSeriesInfos)
    {
        foreach (var prog in mxfSeriesInfos.Where(a => a.extras.ContainsKey("artwork")))
        {
            List<ProgramArtwork> artwork = prog.extras["artwork"];
            UpdateIcons(artwork.Select(a=>a.Uri), prog.Title);
        }
    }
    private void UpdateIcon(string artworkUri, string title)
    {
        if (string.IsNullOrEmpty(artworkUri)) return;

        List<IconFileDto> icons = memoryCache.Icons();
        
            if (icons.Any(a => a.SMFileType == SMFileTypes.SDImage && a.Source == artworkUri)) return;

            icons.Add(new IconFileDto { Id = icons.Count, Source = artworkUri, SMFileType = SMFileTypes.SDImage, Name = title });
        
        memoryCache.SetCache(icons);
    }
    private void UpdateIcons(IEnumerable<string> artworkUris, string title)
    {
        if (!artworkUris.Any()) return;
        List<IconFileDto> icons = memoryCache.Icons();
        //var logos = art.ToList();
        foreach (var artworkUri in artworkUris)        
        {
            UpdateIcon(artworkUri, title);

        }

        //for (int i = 0; i < icons.Count; i++)
        //{
        //    icons[i].Id = i;
        //}
        memoryCache.SetCache(icons);
    }

    
    private async Task<bool> DownloadSdLogo(string uri, string filePath, CancellationToken cancellationToken)
    {

        filePath = FileUtil.CleanUpFileName(filePath);
        try
        {
            if (!await EnsureToken(cancellationToken).ConfigureAwait(false))
            {
                return false;
            }

                Setting setting = await settingsService.GetSettingsAsync(cancellationToken);

            HttpClient httpClient = SDHelpers.CreateHttpClient(setting.ClientUserAgent);
            using HttpResponseMessage response = await httpClient.GetAsync(uri, cancellationToken).ConfigureAwait(false);

            if (response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.NotFound)
            {
                return false;
            }

            if (response.IsSuccessStatusCode)
            {
                Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                if (stream != null)
                {
                    //var cropImg = await CropAndResizeImageAsync(stream);

                    //// Crop and save image
                    //using (var outputStream = File.Create(filePath))
                    //{
                    //    cropImg.Save(outputStream, new JpegEncoder());
                    //}


                    using var outputStream = File.Create(filePath);
                    await stream.CopyToAsync(outputStream, cancellationToken);
                    stream.Close();
                    stream.Dispose();
                    logger.LogDebug($"Downloaded image from {uri} to {filePath}: {response.StatusCode}");
                    return true;
                }


            }
            else
            {
                logger.LogError($"Failed to download image from {uri} to {filePath}: {response.StatusCode}");
            }

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to download image from {Url} to {FileName}.", uri, filePath);
        }
        return false;
    }



    ////public async Task ProcessProgramsImages(List<Programme> sDPrograms, CancellationToken cancellationToken)
    ////{
    ////    List<string> programIds = sDPrograms
    ////        .Where(a => a.HasImageArtwork == true || a.HasSportsArtwork == true || a.HasSeriesArtwork == true || a.HasSeasonArtwork == true || a.HasMovieArtwork == true || a.HasEpisodeArtwork == true)
    ////        .Select(a => a.ProgramID).Distinct().ToList();
    ////    List<string> distinctProgramIds = programIds
    ////                                         .Distinct()
    ////                                         .Select(a => a.Length >= 10 ? a[..10] : a) // Select the leftmost 10 characters
    ////                                         .ToList();

    ////    if (programIds.Any())
    ////    {
    ////        List<ProgramMetadata>? fetchedResults = await schedulesDirectAPI.PostData<List<ProgramMetadata>>("metadata/programs/", programIds, cancellationToken).ConfigureAwait(false);
    ////        if (fetchedResults == null)
    ////        {
    ////            return;
    ////        }

    ////        int count = 0;

    ////        foreach (ProgramMetadata m in fetchedResults)
    ////        {
    ////            ++count;
    ////            logger.LogInformation("Caching program icons for {count}/{totalCount} programs", count, fetchedResults.Count);
    ////            Programme? sdProg = sDPrograms.Find(a => a.ProgramID == m.ProgramID);
    ////            string cats = string.Join(',', m.ProgramArtwork.Select(a => a.Category).Distinct());
    ////            string tiers = string.Join(',', m.ProgramArtwork.Select(a => a.Tier).Distinct());

    ////            if (sdProg is null)
    ////            {
    ////                continue;
    ////            }

    ////            if (sdProg.HasEpisodeArtwork == true)
    ////            {
    ////                //List<ProgramArtwork> catEpisode = m.ProgramArtwork.Where(item => item.Tier == "Episode").ToList();
    ////                //await DownloadImages(m.ProgramID, catEpisode, cancellationToken);
    ////            }

    ////            if (sdProg.HasMovieArtwork == true)
    ////            {
    ////                List<ProgramArtwork> iconsSports = m.ProgramArtwork.Where(item => item.Category.StartsWith("Poster")).ToList();
    ////                await DownloadImages(m.ProgramID, iconsSports, cancellationToken);
    ////                continue;
    ////            }

    ////            if (sdProg.HasSeasonArtwork == true)
    ////            {
    ////                List<ProgramArtwork> catSeason = m.ProgramArtwork.Where(item => item.Tier == "Season").ToList();
    ////                await DownloadImages(m.ProgramID, catSeason, cancellationToken);
    ////            }

    ////            if (sdProg.HasSeriesArtwork == true)
    ////            {
    ////                List<ProgramArtwork> catSeries = m.ProgramArtwork.Where(item => item.Tier == "Series").ToList();
    ////                await DownloadImages(m.ProgramID, catSeries, cancellationToken);
    ////            }

    ////            if (sdProg.HasSportsArtwork == true)
    ////            {
    ////                List<ProgramArtwork> iconsSports = [.. m.ProgramArtwork];
    ////                await DownloadImages(m.ProgramID, iconsSports, cancellationToken);
    ////            }

    ////            if (sdProg.HasImageArtwork == true)
    ////            {
    ////                await DownloadImages(m.ProgramID, m.ProgramArtwork, cancellationToken);
    ////            }
    ////        }
    ////    }
    ////}

    //private async Task DownloadImages(string programId, List<ProgramArtwork> iconsList, CancellationToken cancellationToken)
    //{
    //    List<ProgramArtwork> icons = iconsList.Where(a => a.Category == "Banner-L1").ToList();
    //    if (!icons.Any())
    //    {
    //        icons = iconsList.Where(a => a.Category == "Iconic").ToList();
    //        if (!icons.Any())
    //        {
    //            icons = iconsList;
    //        }
    //    }

    //    icons = icons.Where(a => !string.IsNullOrEmpty(a.Uri) && a.Width <= 600 && a.Height <= 600).ToList();
    //    if (!icons.Any())
    //    {
    //        return;
    //    }

    //    logger.LogInformation("Downloading {count} icons for {ProgramID} program", icons.Count, programId);

    //    // Create a SemaphoreSlim for limiting concurrent downloads
    //    using SemaphoreSlim semaphore = new(4);
    //    List<Task<bool>> tasks = icons.ConvertAll(async icon =>
    //    {
    //        // Wait to enter the semaphore (limits the concurrency level)
    //        await semaphore.WaitAsync(cancellationToken);

    //        try
    //        {
    //            // Perform the download task
    //            return await GetImageUrl(programId, icon, cancellationToken);
    //        }
    //        finally
    //        {
    //            // Release the semaphore
    //            semaphore.Release();
    //        }
    //    });

    //    // Wait for all tasks to complete
    //    await Task.WhenAll(tasks);
    //}

    

    //public async Task<bool> GetImageUrl(string programId, ProgramArtwork icon, CancellationToken cancellationToken)
    

    //public async Task<bool> DownloadSdLogo(string uri, string filepath)
    //{
    //    //List<ImageInfo> imageInfos = memoryCache.ImageInfos();

    //    //if (File.Exists(ImageInfoFilePath) && !imageInfos.Any())
    //    //{
    //    //    imageInfos = JsonSerializer.Deserialize<List<ImageInfo>>(File.ReadAllText(ImageInfoFilePath)) ?? [];
    //    //    memoryCache.SetCache(imageInfos);
    //    //}

    //    //if (imageInfos.Find(a => a.IconUri == icon.Uri) != null)
    //    //{
    //    //    return true;
    //    //}

    //    //string fullName = Path.Combine(BuildInfo.SDImagesFolder, $"{programId}_{icon.Category}_{icon.Tier}_{icon.Width}x{icon.Height}.png");
    //    filepath = CleanUpFileName(filepath);

    //    try
    //    {
    //        string url = "";
    //        if (icon.Uri.StartsWith("http"))
    //        {
    //            url = icon.Uri;
    //        }
    //        else
    //        {
    //            url = await SdToken.GetAPIUrl($"image/{icon.Uri}", cancellationToken);
    //        }

    //        Setting setting = await settingsService.GetSettingsAsync(cancellationToken);
    //        _ = await EnsureToken(cancellationToken);

    //        HttpClient httpClient = SDHelpers.CreateHttpClient(setting.ClientUserAgent);
    //        using HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);

    //        if (response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.NotFound)
    //        {
    //            return false;
    //        }

    //        _ = response.EnsureSuccessStatusCode();

    //        Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

    //        if (stream != null)
    //        {
    //            using FileStream fileStream = new(fullName, FileMode.Create);
    //            await stream.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);
    //            WriteImageInfoToJsonFile(programId, icon, url, fullName);
    //        }

    //        return true;
    //    }
    //    catch (Exception ex)
    //    {
    //        logger.LogError(ex, "Failed to download image from {Url} to {FileName}.", icon.Uri, fullName);
    //    }
    //    return false;
    //}
    //private void WriteImageInfoToJsonFile(string programId, ProgramArtwork icon, string realUrl, string fullName)
    //{
    //    lock (fileLock)
    //    {
    //        try
    //        {
    //            List<ImageInfo> imageInfos = memoryCache.ImageInfos();

    //            if (imageInfos.Find(a => a.IconUri == icon.Uri) != null)
    //            {
    //                return;
    //            }

    //            UriBuilder uriBuilder = new(realUrl)
    //            {
    //                // Remove all query parameters by setting the Query property to an empty string
    //                Query = ""
    //            };

    //            // Get the modified URL
    //            string modifiedUrl = uriBuilder.Uri.ToString();

    //            // Add the new imageInfo
    //            imageInfos.Add(new ImageInfo(programId, icon.Uri, modifiedUrl, fullName, icon.Width, icon.Height));

    //            // Serialize the updated imageInfos to JSON and write it to the file using System.Text.Json
    //            string json = JsonSerializer.Serialize(imageInfos, new JsonSerializerOptions { WriteIndented = true });
    //            File.WriteAllText(ImageInfoFilePath, json);
    //        }
    //        catch (Exception ex)
    //        {
    //            logger.LogError(ex, "Failed to update image information in JSON file.");
    //        }
    //    }
    //}

}