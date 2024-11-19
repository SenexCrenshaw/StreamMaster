namespace StreamMaster.Application.SchedulesDirect.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetStationChannelNamesRequest : IRequest<DataResponse<List<StationChannelName>>>;

internal class GetStationChannelNamesHandler(ICacheManager cacheManager, IEPGFileService ePGFileService, IFileUtilService fileUtilService)
    : IRequestHandler<GetStationChannelNamesRequest, DataResponse<List<StationChannelName>>>
{
    public async Task<DataResponse<List<StationChannelName>>> Handle(GetStationChannelNamesRequest request, CancellationToken cancellationToken)
    {

        List<StationChannelName> sdChannelNames = [];// schedulesDirectDataService.GetStationChannelNames().ToList();

        if (File.Exists(BuildInfo.SDXMLFile))
        {
            if (cacheManager.StationChannelNames.TryGetValue(EPGHelper.SchedulesDirectId, out List<StationChannelName>? values))
            {
                sdChannelNames = values;
            }
            else
            {
                List<StationChannelName>? newNames = await fileUtilService.ProcessStationChannelNamesAsync(BuildInfo.SDXMLFile, EPGHelper.SchedulesDirectId);
                if (newNames is not null)
                {
                    sdChannelNames = newNames;
                }
            }
        }

        List<EPGFile> epgFiles = await ePGFileService.GetEPGFilesAsync();
        List<StationChannelName> channelNames = [];// schedulesDirectDataService.GetStationChannelNames().ToList();

        foreach (EPGFile epgFile in epgFiles)
        {
            if (cacheManager.StationChannelNames.TryGetValue(epgFile.EPGNumber, out List<StationChannelName>? values))
            {
                channelNames.AddRange(values);
                continue;
            }

            string epgPath = Path.Combine(FileDefinitions.EPG.DirectoryLocation, epgFile.Source);

            List<StationChannelName>? newNames = await fileUtilService.ProcessStationChannelNamesAsync(epgPath, epgFile.EPGNumber);
            if (newNames is not null)
            {
                channelNames.AddRange(newNames);
            }
        }

        List<StationChannelName> ret = [.. sdChannelNames, .. channelNames];

        return DataResponse<List<StationChannelName>>.Success([.. ret.OrderBy(a => a.ChannelName)]);
    }
}
