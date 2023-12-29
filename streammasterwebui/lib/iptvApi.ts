import { emptySplitApi as api } from './redux/emptyApi';
export const addTagTypes = [
  'ChannelGroups',
  'EPGFiles',
  'Icons',
  'Logs',
  'Programmes',
  'SchedulesDirect',
  'StreamGroupChannelGroup',
  'StreamGroupVideoStreams',
  'Test',
  'VideoStreamLinks',
  'VideoStreams',
  'Files',
  'M3UFiles',
  'Misc',
  'Queue',
  'Settings',
  'StreamGroups'
] as const;
const injectedRtkApi = api
  .enhanceEndpoints({
    addTagTypes
  })
  .injectEndpoints({
    endpoints: (build) => ({
      channelGroupsCreateChannelGroup: build.mutation<ChannelGroupsCreateChannelGroupApiResponse, ChannelGroupsCreateChannelGroupApiArg>({
        query: (queryArg) => ({ url: `/api/channelgroups`, method: 'POST', body: queryArg }),
        invalidatesTags: ['ChannelGroups']
      }),
      channelGroupsDeleteAllChannelGroupsFromParameters: build.mutation<
        ChannelGroupsDeleteAllChannelGroupsFromParametersApiResponse,
        ChannelGroupsDeleteAllChannelGroupsFromParametersApiArg
      >({
        query: (queryArg) => ({ url: `/api/channelgroups/deleteallchannelgroupsfromparameters`, method: 'DELETE', body: queryArg }),
        invalidatesTags: ['ChannelGroups']
      }),
      channelGroupsDeleteChannelGroup: build.mutation<ChannelGroupsDeleteChannelGroupApiResponse, ChannelGroupsDeleteChannelGroupApiArg>({
        query: (queryArg) => ({ url: `/api/channelgroups/deletechannelgroup`, method: 'DELETE', body: queryArg }),
        invalidatesTags: ['ChannelGroups']
      }),
      channelGroupsGetChannelGroup: build.query<ChannelGroupsGetChannelGroupApiResponse, ChannelGroupsGetChannelGroupApiArg>({
        query: (queryArg) => ({ url: `/api/channelgroups/${queryArg}` }),
        providesTags: ['ChannelGroups']
      }),
      channelGroupsGetChannelGroupIdNames: build.query<ChannelGroupsGetChannelGroupIdNamesApiResponse, ChannelGroupsGetChannelGroupIdNamesApiArg>({
        query: () => ({ url: `/api/channelgroups/getchannelgroupidnames` }),
        providesTags: ['ChannelGroups']
      }),
      channelGroupsGetPagedChannelGroups: build.query<ChannelGroupsGetPagedChannelGroupsApiResponse, ChannelGroupsGetPagedChannelGroupsApiArg>({
        query: (queryArg) => ({
          url: `/api/channelgroups/getpagedchannelgroups`,
          params: {
            PageNumber: queryArg.pageNumber,
            PageSize: queryArg.pageSize,
            OrderBy: queryArg.orderBy,
            JSONArgumentString: queryArg.jsonArgumentString,
            JSONFiltersString: queryArg.jsonFiltersString
          }
        }),
        providesTags: ['ChannelGroups']
      }),
      channelGroupsUpdateChannelGroup: build.mutation<ChannelGroupsUpdateChannelGroupApiResponse, ChannelGroupsUpdateChannelGroupApiArg>({
        query: (queryArg) => ({ url: `/api/channelgroups/updatechannelgroup`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['ChannelGroups']
      }),
      channelGroupsUpdateChannelGroups: build.mutation<ChannelGroupsUpdateChannelGroupsApiResponse, ChannelGroupsUpdateChannelGroupsApiArg>({
        query: (queryArg) => ({ url: `/api/channelgroups/updatechannelgroups`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['ChannelGroups']
      }),
      channelGroupsGetChannelGroupNames: build.query<ChannelGroupsGetChannelGroupNamesApiResponse, ChannelGroupsGetChannelGroupNamesApiArg>({
        query: () => ({ url: `/api/channelgroups/getchannelgroupnames` }),
        providesTags: ['ChannelGroups']
      }),
      channelGroupsGetChannelGroupsForStreamGroup: build.query<
        ChannelGroupsGetChannelGroupsForStreamGroupApiResponse,
        ChannelGroupsGetChannelGroupsForStreamGroupApiArg
      >({
        query: (queryArg) => ({ url: `/api/channelgroups/getchannelgroupsforstreamgroup`, body: queryArg }),
        providesTags: ['ChannelGroups']
      }),
      epgFilesCreateEpgFile: build.mutation<EpgFilesCreateEpgFileApiResponse, EpgFilesCreateEpgFileApiArg>({
        query: (queryArg) => ({ url: `/api/epgfiles/createepgfile`, method: 'POST', body: queryArg }),
        invalidatesTags: ['EPGFiles']
      }),
      epgFilesCreateEpgFileFromForm: build.mutation<EpgFilesCreateEpgFileFromFormApiResponse, EpgFilesCreateEpgFileFromFormApiArg>({
        query: (queryArg) => ({ url: `/api/epgfiles/createepgfilefromform`, method: 'POST', body: queryArg }),
        invalidatesTags: ['EPGFiles']
      }),
      epgFilesDeleteEpgFile: build.mutation<EpgFilesDeleteEpgFileApiResponse, EpgFilesDeleteEpgFileApiArg>({
        query: (queryArg) => ({ url: `/api/epgfiles/deleteepgfile`, method: 'DELETE', body: queryArg }),
        invalidatesTags: ['EPGFiles']
      }),
      epgFilesGetEpgColors: build.query<EpgFilesGetEpgColorsApiResponse, EpgFilesGetEpgColorsApiArg>({
        query: () => ({ url: `/api/epgfiles/getepgcolors` }),
        providesTags: ['EPGFiles']
      }),
      epgFilesGetEpgFile: build.query<EpgFilesGetEpgFileApiResponse, EpgFilesGetEpgFileApiArg>({
        query: (queryArg) => ({ url: `/api/epgfiles/${queryArg}` }),
        providesTags: ['EPGFiles']
      }),
      epgFilesGetEpgFilePreviewById: build.query<EpgFilesGetEpgFilePreviewByIdApiResponse, EpgFilesGetEpgFilePreviewByIdApiArg>({
        query: (queryArg) => ({ url: `/api/epgfiles/getepgfilepreviewbyid`, params: { id: queryArg } }),
        providesTags: ['EPGFiles']
      }),
      epgFilesGetEpgNextEpgNumber: build.query<EpgFilesGetEpgNextEpgNumberApiResponse, EpgFilesGetEpgNextEpgNumberApiArg>({
        query: () => ({ url: `/api/epgfiles/getepgnextepgnumber` }),
        providesTags: ['EPGFiles']
      }),
      epgFilesGetPagedEpgFiles: build.query<EpgFilesGetPagedEpgFilesApiResponse, EpgFilesGetPagedEpgFilesApiArg>({
        query: (queryArg) => ({
          url: `/api/epgfiles`,
          params: {
            PageNumber: queryArg.pageNumber,
            PageSize: queryArg.pageSize,
            OrderBy: queryArg.orderBy,
            JSONArgumentString: queryArg.jsonArgumentString,
            JSONFiltersString: queryArg.jsonFiltersString
          }
        }),
        providesTags: ['EPGFiles']
      }),
      epgFilesProcessEpgFile: build.mutation<EpgFilesProcessEpgFileApiResponse, EpgFilesProcessEpgFileApiArg>({
        query: (queryArg) => ({ url: `/api/epgfiles/processepgfile`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['EPGFiles']
      }),
      epgFilesRefreshEpgFile: build.mutation<EpgFilesRefreshEpgFileApiResponse, EpgFilesRefreshEpgFileApiArg>({
        query: (queryArg) => ({ url: `/api/epgfiles/refreshepgfile`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['EPGFiles']
      }),
      epgFilesScanDirectoryForEpgFiles: build.mutation<EpgFilesScanDirectoryForEpgFilesApiResponse, EpgFilesScanDirectoryForEpgFilesApiArg>({
        query: () => ({ url: `/api/epgfiles/scandirectoryforepgfiles`, method: 'PATCH' }),
        invalidatesTags: ['EPGFiles']
      }),
      epgFilesUpdateEpgFile: build.mutation<EpgFilesUpdateEpgFileApiResponse, EpgFilesUpdateEpgFileApiArg>({
        query: (queryArg) => ({ url: `/api/epgfiles/updateepgfile`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['EPGFiles']
      }),
      iconsAutoMatchIconToStreams: build.mutation<IconsAutoMatchIconToStreamsApiResponse, IconsAutoMatchIconToStreamsApiArg>({
        query: (queryArg) => ({ url: `/api/icons/automatchicontostreams`, method: 'POST', body: queryArg }),
        invalidatesTags: ['Icons']
      }),
      iconsGetIcon: build.query<IconsGetIconApiResponse, IconsGetIconApiArg>({
        query: (queryArg) => ({ url: `/api/icons/geticon/${queryArg}` }),
        providesTags: ['Icons']
      }),
      iconsGetIconFromSource: build.query<IconsGetIconFromSourceApiResponse, IconsGetIconFromSourceApiArg>({
        query: (queryArg) => ({ url: `/api/icons/geticonfromsource`, params: { value: queryArg } }),
        providesTags: ['Icons']
      }),
      iconsGetPagedIcons: build.query<IconsGetPagedIconsApiResponse, IconsGetPagedIconsApiArg>({
        query: (queryArg) => ({
          url: `/api/icons`,
          params: {
            Count: queryArg.count,
            First: queryArg.first,
            Last: queryArg.last,
            PageNumber: queryArg.pageNumber,
            PageSize: queryArg.pageSize,
            OrderBy: queryArg.orderBy,
            JSONArgumentString: queryArg.jsonArgumentString,
            JSONFiltersString: queryArg.jsonFiltersString
          }
        }),
        providesTags: ['Icons']
      }),
      iconsGetIconsSimpleQuery: build.query<IconsGetIconsSimpleQueryApiResponse, IconsGetIconsSimpleQueryApiArg>({
        query: (queryArg) => ({
          url: `/api/icons/geticonssimplequery`,
          params: {
            Count: queryArg.count,
            First: queryArg.first,
            Last: queryArg.last,
            PageNumber: queryArg.pageNumber,
            PageSize: queryArg.pageSize,
            OrderBy: queryArg.orderBy,
            JSONArgumentString: queryArg.jsonArgumentString,
            JSONFiltersString: queryArg.jsonFiltersString
          }
        }),
        providesTags: ['Icons']
      }),
      logsGetLog: build.query<LogsGetLogApiResponse, LogsGetLogApiArg>({
        query: (queryArg) => ({ url: `/api/logs/getlog`, params: { LastId: queryArg.lastId, MaxLines: queryArg.maxLines } }),
        providesTags: ['Logs']
      }),
      programmesGetProgramme: build.query<ProgrammesGetProgrammeApiResponse, ProgrammesGetProgrammeApiArg>({
        query: (queryArg) => ({ url: `/api/programmes/getprogramme/${queryArg}` }),
        providesTags: ['Programmes']
      }),
      programmesGetProgrammes: build.query<ProgrammesGetProgrammesApiResponse, ProgrammesGetProgrammesApiArg>({
        query: () => ({ url: `/api/programmes/getprogrammes` }),
        providesTags: ['Programmes']
      }),
      schedulesDirectAddLineup: build.mutation<SchedulesDirectAddLineupApiResponse, SchedulesDirectAddLineupApiArg>({
        query: (queryArg) => ({ url: `/api/schedulesdirect/addlineup`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['SchedulesDirect']
      }),
      schedulesDirectAddStation: build.mutation<SchedulesDirectAddStationApiResponse, SchedulesDirectAddStationApiArg>({
        query: (queryArg) => ({ url: `/api/schedulesdirect/addstation`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['SchedulesDirect']
      }),
      schedulesDirectGetAvailableCountries: build.query<SchedulesDirectGetAvailableCountriesApiResponse, SchedulesDirectGetAvailableCountriesApiArg>({
        query: () => ({ url: `/api/schedulesdirect/getavailablecountries` }),
        providesTags: ['SchedulesDirect']
      }),
      schedulesDirectGetChannelNames: build.query<SchedulesDirectGetChannelNamesApiResponse, SchedulesDirectGetChannelNamesApiArg>({
        query: () => ({ url: `/api/schedulesdirect/getchannelnames` }),
        providesTags: ['SchedulesDirect']
      }),
      schedulesDirectGetHeadends: build.query<SchedulesDirectGetHeadendsApiResponse, SchedulesDirectGetHeadendsApiArg>({
        query: (queryArg) => ({ url: `/api/schedulesdirect/getheadends`, body: queryArg }),
        providesTags: ['SchedulesDirect']
      }),
      schedulesDirectGetLineupPreviewChannel: build.query<SchedulesDirectGetLineupPreviewChannelApiResponse, SchedulesDirectGetLineupPreviewChannelApiArg>({
        query: (queryArg) => ({ url: `/api/schedulesdirect/getlineuppreviewchannel`, body: queryArg }),
        providesTags: ['SchedulesDirect']
      }),
      schedulesDirectGetLineups: build.query<SchedulesDirectGetLineupsApiResponse, SchedulesDirectGetLineupsApiArg>({
        query: () => ({ url: `/api/schedulesdirect/getlineups` }),
        providesTags: ['SchedulesDirect']
      }),
      schedulesDirectGetPagedStationChannelNameSelections: build.query<
        SchedulesDirectGetPagedStationChannelNameSelectionsApiResponse,
        SchedulesDirectGetPagedStationChannelNameSelectionsApiArg
      >({
        query: (queryArg) => ({
          url: `/api/schedulesdirect/getpagedstationchannelnameselections`,
          params: {
            Count: queryArg.count,
            First: queryArg.first,
            Last: queryArg.last,
            PageNumber: queryArg.pageNumber,
            PageSize: queryArg.pageSize,
            OrderBy: queryArg.orderBy,
            JSONArgumentString: queryArg.jsonArgumentString,
            JSONFiltersString: queryArg.jsonFiltersString
          }
        }),
        providesTags: ['SchedulesDirect']
      }),
      schedulesDirectGetSelectedStationIds: build.query<SchedulesDirectGetSelectedStationIdsApiResponse, SchedulesDirectGetSelectedStationIdsApiArg>({
        query: () => ({ url: `/api/schedulesdirect/getselectedstationids` }),
        providesTags: ['SchedulesDirect']
      }),
      schedulesDirectGetStationChannelMaps: build.query<SchedulesDirectGetStationChannelMapsApiResponse, SchedulesDirectGetStationChannelMapsApiArg>({
        query: () => ({ url: `/api/schedulesdirect/getstationchannelmaps` }),
        providesTags: ['SchedulesDirect']
      }),
      schedulesDirectGetStationChannelNameFromDisplayName: build.query<
        SchedulesDirectGetStationChannelNameFromDisplayNameApiResponse,
        SchedulesDirectGetStationChannelNameFromDisplayNameApiArg
      >({
        query: (queryArg) => ({ url: `/api/schedulesdirect/getstationchannelnamefromdisplayname`, body: queryArg }),
        providesTags: ['SchedulesDirect']
      }),
      schedulesDirectGetStationChannelNamesSimpleQuery: build.query<
        SchedulesDirectGetStationChannelNamesSimpleQueryApiResponse,
        SchedulesDirectGetStationChannelNamesSimpleQueryApiArg
      >({
        query: (queryArg) => ({
          url: `/api/schedulesdirect/getstationchannelnamessimplequery`,
          params: {
            Count: queryArg.count,
            First: queryArg.first,
            Last: queryArg.last,
            PageNumber: queryArg.pageNumber,
            PageSize: queryArg.pageSize,
            OrderBy: queryArg.orderBy,
            JSONArgumentString: queryArg.jsonArgumentString,
            JSONFiltersString: queryArg.jsonFiltersString
          }
        }),
        providesTags: ['SchedulesDirect']
      }),
      schedulesDirectGetStationPreviews: build.query<SchedulesDirectGetStationPreviewsApiResponse, SchedulesDirectGetStationPreviewsApiArg>({
        query: () => ({ url: `/api/schedulesdirect/getstationpreviews` }),
        providesTags: ['SchedulesDirect']
      }),
      schedulesDirectGetUserStatus: build.query<SchedulesDirectGetUserStatusApiResponse, SchedulesDirectGetUserStatusApiArg>({
        query: () => ({ url: `/api/schedulesdirect/getuserstatus` }),
        providesTags: ['SchedulesDirect']
      }),
      schedulesDirectRemoveLineup: build.mutation<SchedulesDirectRemoveLineupApiResponse, SchedulesDirectRemoveLineupApiArg>({
        query: (queryArg) => ({ url: `/api/schedulesdirect/removelineup`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['SchedulesDirect']
      }),
      schedulesDirectRemoveStation: build.mutation<SchedulesDirectRemoveStationApiResponse, SchedulesDirectRemoveStationApiArg>({
        query: (queryArg) => ({ url: `/api/schedulesdirect/removestation`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['SchedulesDirect']
      }),
      streamGroupChannelGroupSyncStreamGroupChannelGroups: build.mutation<
        StreamGroupChannelGroupSyncStreamGroupChannelGroupsApiResponse,
        StreamGroupChannelGroupSyncStreamGroupChannelGroupsApiArg
      >({
        query: (queryArg) => ({ url: `/api/streamgroupchannelgroup/syncstreamgroupchannelgroups`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['StreamGroupChannelGroup']
      }),
      streamGroupChannelGroupGetChannelGroupsFromStreamGroup: build.query<
        StreamGroupChannelGroupGetChannelGroupsFromStreamGroupApiResponse,
        StreamGroupChannelGroupGetChannelGroupsFromStreamGroupApiArg
      >({
        query: (queryArg) => ({ url: `/api/streamgroupchannelgroup/getchannelgroupsfromstreamgroup`, params: { StreamGroupId: queryArg } }),
        providesTags: ['StreamGroupChannelGroup']
      }),
      streamGroupVideoStreamsGetStreamGroupVideoStreamIds: build.query<
        StreamGroupVideoStreamsGetStreamGroupVideoStreamIdsApiResponse,
        StreamGroupVideoStreamsGetStreamGroupVideoStreamIdsApiArg
      >({
        query: (queryArg) => ({ url: `/api/streamgroupvideostreams/getstreamgroupvideostreamids`, params: { StreamGroupId: queryArg } }),
        providesTags: ['StreamGroupVideoStreams']
      }),
      streamGroupVideoStreamsGetPagedStreamGroupVideoStreams: build.query<
        StreamGroupVideoStreamsGetPagedStreamGroupVideoStreamsApiResponse,
        StreamGroupVideoStreamsGetPagedStreamGroupVideoStreamsApiArg
      >({
        query: (queryArg) => ({
          url: `/api/streamgroupvideostreams/getpagedstreamgroupvideostreams`,
          params: {
            StreamGroupId: queryArg.streamGroupId,
            PageNumber: queryArg.pageNumber,
            PageSize: queryArg.pageSize,
            OrderBy: queryArg.orderBy,
            JSONArgumentString: queryArg.jsonArgumentString,
            JSONFiltersString: queryArg.jsonFiltersString
          }
        }),
        providesTags: ['StreamGroupVideoStreams']
      }),
      streamGroupVideoStreamsSetVideoStreamRanks: build.mutation<
        StreamGroupVideoStreamsSetVideoStreamRanksApiResponse,
        StreamGroupVideoStreamsSetVideoStreamRanksApiArg
      >({
        query: (queryArg) => ({ url: `/api/streamgroupvideostreams/setvideostreamranks`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['StreamGroupVideoStreams']
      }),
      streamGroupVideoStreamsSyncVideoStreamToStreamGroupPost: build.mutation<
        StreamGroupVideoStreamsSyncVideoStreamToStreamGroupPostApiResponse,
        StreamGroupVideoStreamsSyncVideoStreamToStreamGroupPostApiArg
      >({
        query: (queryArg) => ({ url: `/api/streamgroupvideostreams/syncvideostreamtostreamgroup`, method: 'POST', body: queryArg }),
        invalidatesTags: ['StreamGroupVideoStreams']
      }),
      streamGroupVideoStreamsSyncVideoStreamToStreamGroupDelete: build.mutation<
        StreamGroupVideoStreamsSyncVideoStreamToStreamGroupDeleteApiResponse,
        StreamGroupVideoStreamsSyncVideoStreamToStreamGroupDeleteApiArg
      >({
        query: (queryArg) => ({ url: `/api/streamgroupvideostreams/syncvideostreamtostreamgroup`, method: 'DELETE', body: queryArg }),
        invalidatesTags: ['StreamGroupVideoStreams']
      }),
      streamGroupVideoStreamsSetStreamGroupVideoStreamChannelNumbers: build.mutation<
        StreamGroupVideoStreamsSetStreamGroupVideoStreamChannelNumbersApiResponse,
        StreamGroupVideoStreamsSetStreamGroupVideoStreamChannelNumbersApiArg
      >({
        query: (queryArg) => ({ url: `/api/streamgroupvideostreams/setstreamgroupvideostreamchannelnumbers`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['StreamGroupVideoStreams']
      }),
      testEpgSync: build.mutation<TestEpgSyncApiResponse, TestEpgSyncApiArg>({
        query: () => ({ url: `/api/test/epgsync`, method: 'PUT' }),
        invalidatesTags: ['Test']
      }),
      videoStreamLinksAddVideoStreamToVideoStream: build.mutation<
        VideoStreamLinksAddVideoStreamToVideoStreamApiResponse,
        VideoStreamLinksAddVideoStreamToVideoStreamApiArg
      >({
        query: (queryArg) => ({ url: `/api/videostreamlinks/addvideostreamtovideostream`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['VideoStreamLinks']
      }),
      videoStreamLinksGetVideoStreamVideoStreamIds: build.query<
        VideoStreamLinksGetVideoStreamVideoStreamIdsApiResponse,
        VideoStreamLinksGetVideoStreamVideoStreamIdsApiArg
      >({
        query: (queryArg) => ({ url: `/api/videostreamlinks/getvideostreamvideostreamids`, params: { videoStreamId: queryArg } }),
        providesTags: ['VideoStreamLinks']
      }),
      videoStreamLinksGetPagedVideoStreamVideoStreams: build.query<
        VideoStreamLinksGetPagedVideoStreamVideoStreamsApiResponse,
        VideoStreamLinksGetPagedVideoStreamVideoStreamsApiArg
      >({
        query: (queryArg) => ({
          url: `/api/videostreamlinks/getpagedvideostreamvideostreams`,
          params: {
            PageNumber: queryArg.pageNumber,
            PageSize: queryArg.pageSize,
            OrderBy: queryArg.orderBy,
            JSONArgumentString: queryArg.jsonArgumentString,
            JSONFiltersString: queryArg.jsonFiltersString
          }
        }),
        providesTags: ['VideoStreamLinks']
      }),
      videoStreamLinksRemoveVideoStreamFromVideoStream: build.mutation<
        VideoStreamLinksRemoveVideoStreamFromVideoStreamApiResponse,
        VideoStreamLinksRemoveVideoStreamFromVideoStreamApiArg
      >({
        query: (queryArg) => ({ url: `/api/videostreamlinks/removevideostreamfromvideostream`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['VideoStreamLinks']
      }),
      videoStreamsCreateVideoStream: build.mutation<VideoStreamsCreateVideoStreamApiResponse, VideoStreamsCreateVideoStreamApiArg>({
        query: (queryArg) => ({ url: `/api/videostreams/createvideostream`, method: 'POST', body: queryArg }),
        invalidatesTags: ['VideoStreams']
      }),
      videoStreamsChangeVideoStreamChannel: build.mutation<VideoStreamsChangeVideoStreamChannelApiResponse, VideoStreamsChangeVideoStreamChannelApiArg>({
        query: (queryArg) => ({ url: `/api/videostreams/changevideostreamchannel`, method: 'POST', body: queryArg }),
        invalidatesTags: ['VideoStreams']
      }),
      videoStreamsDeleteVideoStream: build.mutation<VideoStreamsDeleteVideoStreamApiResponse, VideoStreamsDeleteVideoStreamApiArg>({
        query: (queryArg) => ({ url: `/api/videostreams/deletevideostream`, method: 'DELETE', body: queryArg }),
        invalidatesTags: ['VideoStreams']
      }),
      videoStreamsFailClient: build.mutation<VideoStreamsFailClientApiResponse, VideoStreamsFailClientApiArg>({
        query: (queryArg) => ({ url: `/api/videostreams/failclient`, method: 'POST', body: queryArg }),
        invalidatesTags: ['VideoStreams']
      }),
      videoStreamsGetAllStatisticsForAllUrls: build.query<VideoStreamsGetAllStatisticsForAllUrlsApiResponse, VideoStreamsGetAllStatisticsForAllUrlsApiArg>({
        query: () => ({ url: `/api/videostreams/getallstatisticsforallurls` }),
        providesTags: ['VideoStreams']
      }),
      videoStreamsGetVideoStream: build.query<VideoStreamsGetVideoStreamApiResponse, VideoStreamsGetVideoStreamApiArg>({
        query: (queryArg) => ({ url: `/api/videostreams/${queryArg}` }),
        providesTags: ['VideoStreams']
      }),
      videoStreamsGetVideoStreamNames: build.query<VideoStreamsGetVideoStreamNamesApiResponse, VideoStreamsGetVideoStreamNamesApiArg>({
        query: () => ({ url: `/api/videostreams/getvideostreamnames` }),
        providesTags: ['VideoStreams']
      }),
      videoStreamsGetPagedVideoStreams: build.query<VideoStreamsGetPagedVideoStreamsApiResponse, VideoStreamsGetPagedVideoStreamsApiArg>({
        query: (queryArg) => ({
          url: `/api/videostreams`,
          params: {
            PageNumber: queryArg.pageNumber,
            PageSize: queryArg.pageSize,
            OrderBy: queryArg.orderBy,
            JSONArgumentString: queryArg.jsonArgumentString,
            JSONFiltersString: queryArg.jsonFiltersString
          }
        }),
        providesTags: ['VideoStreams']
      }),
      videoStreamsReadAndWrite: build.mutation<VideoStreamsReadAndWriteApiResponse, VideoStreamsReadAndWriteApiArg>({
        query: (queryArg) => ({ url: `/api/videostreams`, method: 'POST', body: queryArg.body, params: { filePath: queryArg.filePath } }),
        invalidatesTags: ['VideoStreams']
      }),
      videoStreamsGetVideoStreamStreamGet: build.query<VideoStreamsGetVideoStreamStreamGetApiResponse, VideoStreamsGetVideoStreamStreamGetApiArg>({
        query: (queryArg) => ({ url: `/api/videostreams/stream/${queryArg}` }),
        providesTags: ['VideoStreams']
      }),
      videoStreamsGetVideoStreamStreamHead: build.mutation<VideoStreamsGetVideoStreamStreamHeadApiResponse, VideoStreamsGetVideoStreamStreamHeadApiArg>({
        query: (queryArg) => ({ url: `/api/videostreams/stream/${queryArg}`, method: 'HEAD' }),
        invalidatesTags: ['VideoStreams']
      }),
      videoStreamsGetVideoStreamStreamGet2: build.query<VideoStreamsGetVideoStreamStreamGet2ApiResponse, VideoStreamsGetVideoStreamStreamGet2ApiArg>({
        query: (queryArg) => ({ url: `/api/videostreams/stream/${queryArg}.mp4` }),
        providesTags: ['VideoStreams']
      }),
      videoStreamsGetVideoStreamStreamHead2: build.mutation<VideoStreamsGetVideoStreamStreamHead2ApiResponse, VideoStreamsGetVideoStreamStreamHead2ApiArg>({
        query: (queryArg) => ({ url: `/api/videostreams/stream/${queryArg}.mp4`, method: 'HEAD' }),
        invalidatesTags: ['VideoStreams']
      }),
      videoStreamsGetVideoStreamStreamGet3: build.query<VideoStreamsGetVideoStreamStreamGet3ApiResponse, VideoStreamsGetVideoStreamStreamGet3ApiArg>({
        query: (queryArg) => ({ url: `/api/videostreams/stream/${queryArg.encodedIds}/${queryArg.name}` }),
        providesTags: ['VideoStreams']
      }),
      videoStreamsGetVideoStreamStreamHead3: build.mutation<VideoStreamsGetVideoStreamStreamHead3ApiResponse, VideoStreamsGetVideoStreamStreamHead3ApiArg>({
        query: (queryArg) => ({ url: `/api/videostreams/stream/${queryArg.encodedIds}/${queryArg.name}`, method: 'HEAD' }),
        invalidatesTags: ['VideoStreams']
      }),
      videoStreamsReSetVideoStreamsLogo: build.mutation<VideoStreamsReSetVideoStreamsLogoApiResponse, VideoStreamsReSetVideoStreamsLogoApiArg>({
        query: (queryArg) => ({ url: `/api/videostreams/resetvideostreamslogo`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['VideoStreams']
      }),
      videoStreamsSetVideoStreamChannelNumbers: build.mutation<
        VideoStreamsSetVideoStreamChannelNumbersApiResponse,
        VideoStreamsSetVideoStreamChannelNumbersApiArg
      >({
        query: (queryArg) => ({ url: `/api/videostreams/setvideostreamchannelnumbers`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['VideoStreams']
      }),
      videoStreamsSetVideoStreamsLogoFromEpg: build.mutation<VideoStreamsSetVideoStreamsLogoFromEpgApiResponse, VideoStreamsSetVideoStreamsLogoFromEpgApiArg>({
        query: (queryArg) => ({ url: `/api/videostreams/setvideostreamslogofromepg`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['VideoStreams']
      }),
      videoStreamsUpdateVideoStream: build.mutation<VideoStreamsUpdateVideoStreamApiResponse, VideoStreamsUpdateVideoStreamApiArg>({
        query: (queryArg) => ({ url: `/api/videostreams/updatevideostream`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['VideoStreams']
      }),
      videoStreamsUpdateVideoStreams: build.mutation<VideoStreamsUpdateVideoStreamsApiResponse, VideoStreamsUpdateVideoStreamsApiArg>({
        query: (queryArg) => ({ url: `/api/videostreams/updatevideostreams`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['VideoStreams']
      }),
      videoStreamsUpdateAllVideoStreamsFromParameters: build.mutation<
        VideoStreamsUpdateAllVideoStreamsFromParametersApiResponse,
        VideoStreamsUpdateAllVideoStreamsFromParametersApiArg
      >({
        query: (queryArg) => ({ url: `/api/videostreams/updateallvideostreamsfromparameters`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['VideoStreams']
      }),
      videoStreamsDeleteAllVideoStreamsFromParameters: build.mutation<
        VideoStreamsDeleteAllVideoStreamsFromParametersApiResponse,
        VideoStreamsDeleteAllVideoStreamsFromParametersApiArg
      >({
        query: (queryArg) => ({ url: `/api/videostreams/deleteallvideostreamsfromparameters`, method: 'DELETE', body: queryArg }),
        invalidatesTags: ['VideoStreams']
      }),
      videoStreamsSetVideoStreamChannelNumbersFromParameters: build.mutation<
        VideoStreamsSetVideoStreamChannelNumbersFromParametersApiResponse,
        VideoStreamsSetVideoStreamChannelNumbersFromParametersApiArg
      >({
        query: (queryArg) => ({ url: `/api/videostreams/setvideostreamchannelnumbersfromparameters`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['VideoStreams']
      }),
      videoStreamsSetVideoStreamsLogoFromEpgFromParameters: build.mutation<
        VideoStreamsSetVideoStreamsLogoFromEpgFromParametersApiResponse,
        VideoStreamsSetVideoStreamsLogoFromEpgFromParametersApiArg
      >({
        query: (queryArg) => ({ url: `/api/videostreams/setvideostreamslogofromepgfromparameters`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['VideoStreams']
      }),
      videoStreamsReSetVideoStreamsLogoFromParameters: build.mutation<
        VideoStreamsReSetVideoStreamsLogoFromParametersApiResponse,
        VideoStreamsReSetVideoStreamsLogoFromParametersApiArg
      >({
        query: (queryArg) => ({ url: `/api/videostreams/resetvideostreamslogofromparameters`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['VideoStreams']
      }),
      videoStreamsSimulateStreamFailureForAll: build.mutation<
        VideoStreamsSimulateStreamFailureForAllApiResponse,
        VideoStreamsSimulateStreamFailureForAllApiArg
      >({
        query: () => ({ url: `/api/videostreams/simulatestreamfailureforall`, method: 'POST' }),
        invalidatesTags: ['VideoStreams']
      }),
      videoStreamsSimulateStreamFailure: build.mutation<VideoStreamsSimulateStreamFailureApiResponse, VideoStreamsSimulateStreamFailureApiArg>({
        query: (queryArg) => ({ url: `/api/videostreams/simulatestreamfailure`, method: 'POST', body: queryArg }),
        invalidatesTags: ['VideoStreams']
      }),
      videoStreamsAutoSetEpg: build.mutation<VideoStreamsAutoSetEpgApiResponse, VideoStreamsAutoSetEpgApiArg>({
        query: (queryArg) => ({ url: `/api/videostreams/autosetepg`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['VideoStreams']
      }),
      videoStreamsAutoSetEpgFromParameters: build.mutation<VideoStreamsAutoSetEpgFromParametersApiResponse, VideoStreamsAutoSetEpgFromParametersApiArg>({
        query: (queryArg) => ({ url: `/api/videostreams/autosetepgfromparameters`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['VideoStreams']
      }),
      videoStreamsSetVideoStreamTimeShifts: build.mutation<VideoStreamsSetVideoStreamTimeShiftsApiResponse, VideoStreamsSetVideoStreamTimeShiftsApiArg>({
        query: (queryArg) => ({ url: `/api/videostreams/setvideostreamtimeshifts`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['VideoStreams']
      }),
      videoStreamsSetVideoStreamTimeShiftFromParameters: build.mutation<
        VideoStreamsSetVideoStreamTimeShiftFromParametersApiResponse,
        VideoStreamsSetVideoStreamTimeShiftFromParametersApiArg
      >({
        query: (queryArg) => ({ url: `/api/videostreams/setvideostreamtimeshiftfromparameters`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['VideoStreams']
      }),
      videoStreamsGetVideoStreamInfoFromId: build.query<VideoStreamsGetVideoStreamInfoFromIdApiResponse, VideoStreamsGetVideoStreamInfoFromIdApiArg>({
        query: (queryArg) => ({ url: `/api/videostreams/getvideostreaminfofromid`, params: { channelVideoStreamId: queryArg } }),
        providesTags: ['VideoStreams']
      }),
      videoStreamsGetVideoStreamInfoFromUrl: build.query<VideoStreamsGetVideoStreamInfoFromUrlApiResponse, VideoStreamsGetVideoStreamInfoFromUrlApiArg>({
        query: (queryArg) => ({ url: `/api/videostreams/getvideostreaminfofromurl`, params: { streamUrl: queryArg } }),
        providesTags: ['VideoStreams']
      }),
      filesGetFile: build.query<FilesGetFileApiResponse, FilesGetFileApiArg>({
        query: (queryArg) => ({ url: `/api/files/${queryArg.filetype}/${queryArg.source}` }),
        providesTags: ['Files']
      }),
      m3UFilesCreateM3UFile: build.mutation<M3UFilesCreateM3UFileApiResponse, M3UFilesCreateM3UFileApiArg>({
        query: (queryArg) => ({ url: `/api/m3ufiles/createm3ufile`, method: 'POST', body: queryArg }),
        invalidatesTags: ['M3UFiles']
      }),
      m3UFilesCreateM3UFileFromForm: build.mutation<M3UFilesCreateM3UFileFromFormApiResponse, M3UFilesCreateM3UFileFromFormApiArg>({
        query: (queryArg) => ({ url: `/api/m3ufiles/createm3ufilefromform`, method: 'POST', body: queryArg }),
        invalidatesTags: ['M3UFiles']
      }),
      m3UFilesChangeM3UFileName: build.mutation<M3UFilesChangeM3UFileNameApiResponse, M3UFilesChangeM3UFileNameApiArg>({
        query: (queryArg) => ({ url: `/api/m3ufiles/changem3ufilename`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['M3UFiles']
      }),
      m3UFilesDeleteM3UFile: build.mutation<M3UFilesDeleteM3UFileApiResponse, M3UFilesDeleteM3UFileApiArg>({
        query: (queryArg) => ({ url: `/api/m3ufiles/deletem3ufile`, method: 'DELETE', body: queryArg }),
        invalidatesTags: ['M3UFiles']
      }),
      m3UFilesGetM3UFile: build.query<M3UFilesGetM3UFileApiResponse, M3UFilesGetM3UFileApiArg>({
        query: (queryArg) => ({ url: `/api/m3ufiles/${queryArg}` }),
        providesTags: ['M3UFiles']
      }),
      m3UFilesGetPagedM3UFiles: build.query<M3UFilesGetPagedM3UFilesApiResponse, M3UFilesGetPagedM3UFilesApiArg>({
        query: (queryArg) => ({
          url: `/api/m3ufiles`,
          params: {
            PageNumber: queryArg.pageNumber,
            PageSize: queryArg.pageSize,
            OrderBy: queryArg.orderBy,
            JSONArgumentString: queryArg.jsonArgumentString,
            JSONFiltersString: queryArg.jsonFiltersString
          }
        }),
        providesTags: ['M3UFiles']
      }),
      m3UFilesProcessM3UFile: build.mutation<M3UFilesProcessM3UFileApiResponse, M3UFilesProcessM3UFileApiArg>({
        query: (queryArg) => ({ url: `/api/m3ufiles/processm3ufile`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['M3UFiles']
      }),
      m3UFilesRefreshM3UFile: build.mutation<M3UFilesRefreshM3UFileApiResponse, M3UFilesRefreshM3UFileApiArg>({
        query: (queryArg) => ({ url: `/api/m3ufiles/refreshm3ufile`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['M3UFiles']
      }),
      m3UFilesScanDirectoryForM3UFiles: build.mutation<M3UFilesScanDirectoryForM3UFilesApiResponse, M3UFilesScanDirectoryForM3UFilesApiArg>({
        query: () => ({ url: `/api/m3ufiles/scandirectoryform3ufiles`, method: 'PATCH' }),
        invalidatesTags: ['M3UFiles']
      }),
      m3UFilesUpdateM3UFile: build.mutation<M3UFilesUpdateM3UFileApiResponse, M3UFilesUpdateM3UFileApiArg>({
        query: (queryArg) => ({ url: `/api/m3ufiles/updatem3ufile`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['M3UFiles']
      }),
      m3UFilesGetM3UFileNames: build.query<M3UFilesGetM3UFileNamesApiResponse, M3UFilesGetM3UFileNamesApiArg>({
        query: () => ({ url: `/api/m3ufiles/getm3ufilenames` }),
        providesTags: ['M3UFiles']
      }),
      miscGetDownloadServiceStatus: build.query<MiscGetDownloadServiceStatusApiResponse, MiscGetDownloadServiceStatusApiArg>({
        query: () => ({ url: `/api/misc/getdownloadservicestatus` }),
        providesTags: ['Misc']
      }),
      miscGetTestM3U: build.query<MiscGetTestM3UApiResponse, MiscGetTestM3UApiArg>({
        query: (queryArg) => ({ url: `/api/misc/gettestm3u`, params: { numberOfStreams: queryArg } }),
        providesTags: ['Misc']
      }),
      queueGetQueueStatus: build.query<QueueGetQueueStatusApiResponse, QueueGetQueueStatusApiArg>({
        query: () => ({ url: `/api/queue/getqueuestatus` }),
        providesTags: ['Queue']
      }),
      settingsGetIsSystemReady: build.query<SettingsGetIsSystemReadyApiResponse, SettingsGetIsSystemReadyApiArg>({
        query: () => ({ url: `/api/settings/getissystemready` }),
        providesTags: ['Settings']
      }),
      settingsGetSetting: build.query<SettingsGetSettingApiResponse, SettingsGetSettingApiArg>({
        query: () => ({ url: `/api/settings/getsetting` }),
        providesTags: ['Settings']
      }),
      settingsGetSystemStatus: build.query<SettingsGetSystemStatusApiResponse, SettingsGetSystemStatusApiArg>({
        query: () => ({ url: `/api/settings/getsystemstatus` }),
        providesTags: ['Settings']
      }),
      settingsLogIn: build.query<SettingsLogInApiResponse, SettingsLogInApiArg>({
        query: (queryArg) => ({ url: `/api/settings/login`, body: queryArg }),
        providesTags: ['Settings']
      }),
      settingsUpdateSetting: build.mutation<SettingsUpdateSettingApiResponse, SettingsUpdateSettingApiArg>({
        query: (queryArg) => ({ url: `/api/settings/updatesetting`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['Settings']
      }),
      streamGroupsCreateStreamGroup: build.mutation<StreamGroupsCreateStreamGroupApiResponse, StreamGroupsCreateStreamGroupApiArg>({
        query: (queryArg) => ({ url: `/api/streamgroups/createstreamgroup`, method: 'POST', body: queryArg }),
        invalidatesTags: ['StreamGroups']
      }),
      streamGroupsDeleteStreamGroup: build.mutation<StreamGroupsDeleteStreamGroupApiResponse, StreamGroupsDeleteStreamGroupApiArg>({
        query: (queryArg) => ({ url: `/api/streamgroups/deletestreamgroup`, method: 'DELETE', body: queryArg }),
        invalidatesTags: ['StreamGroups']
      }),
      streamGroupsGetStreamGroup: build.query<StreamGroupsGetStreamGroupApiResponse, StreamGroupsGetStreamGroupApiArg>({
        query: (queryArg) => ({ url: `/api/streamgroups/getstreamgroup/${queryArg}` }),
        providesTags: ['StreamGroups']
      }),
      streamGroupsGetStreamGroupCapability: build.query<StreamGroupsGetStreamGroupCapabilityApiResponse, StreamGroupsGetStreamGroupCapabilityApiArg>({
        query: (queryArg) => ({ url: `/api/streamgroups/${queryArg}` }),
        providesTags: ['StreamGroups']
      }),
      streamGroupsGetStreamGroupCapability2: build.query<StreamGroupsGetStreamGroupCapability2ApiResponse, StreamGroupsGetStreamGroupCapability2ApiArg>({
        query: (queryArg) => ({ url: `/api/streamgroups/${queryArg}/capability` }),
        providesTags: ['StreamGroups']
      }),
      streamGroupsGetStreamGroupCapability3: build.query<StreamGroupsGetStreamGroupCapability3ApiResponse, StreamGroupsGetStreamGroupCapability3ApiArg>({
        query: (queryArg) => ({ url: `/api/streamgroups/${queryArg}/device.xml` }),
        providesTags: ['StreamGroups']
      }),
      streamGroupsGetStreamGroupDiscover: build.query<StreamGroupsGetStreamGroupDiscoverApiResponse, StreamGroupsGetStreamGroupDiscoverApiArg>({
        query: (queryArg) => ({ url: `/api/streamgroups/${queryArg}/discover.json` }),
        providesTags: ['StreamGroups']
      }),
      streamGroupsGetStreamGroupEpg: build.query<StreamGroupsGetStreamGroupEpgApiResponse, StreamGroupsGetStreamGroupEpgApiArg>({
        query: (queryArg) => ({ url: `/api/streamgroups/${queryArg}/epg.xml` }),
        providesTags: ['StreamGroups']
      }),
      streamGroupsGetStreamGroupLineup: build.query<StreamGroupsGetStreamGroupLineupApiResponse, StreamGroupsGetStreamGroupLineupApiArg>({
        query: (queryArg) => ({ url: `/api/streamgroups/${queryArg}/lineup.json` }),
        providesTags: ['StreamGroups']
      }),
      streamGroupsGetStreamGroupLineupStatus: build.query<StreamGroupsGetStreamGroupLineupStatusApiResponse, StreamGroupsGetStreamGroupLineupStatusApiArg>({
        query: (queryArg) => ({ url: `/api/streamgroups/${queryArg}/lineup_status.json` }),
        providesTags: ['StreamGroups']
      }),
      streamGroupsGetStreamGroupM3U: build.query<StreamGroupsGetStreamGroupM3UApiResponse, StreamGroupsGetStreamGroupM3UApiArg>({
        query: (queryArg) => ({ url: `/api/streamgroups/${queryArg}/m3u.m3u` }),
        providesTags: ['StreamGroups']
      }),
      streamGroupsGetPagedStreamGroups: build.query<StreamGroupsGetPagedStreamGroupsApiResponse, StreamGroupsGetPagedStreamGroupsApiArg>({
        query: (queryArg) => ({
          url: `/api/streamgroups`,
          params: {
            PageNumber: queryArg.pageNumber,
            PageSize: queryArg.pageSize,
            OrderBy: queryArg.orderBy,
            JSONArgumentString: queryArg.jsonArgumentString,
            JSONFiltersString: queryArg.jsonFiltersString
          }
        }),
        providesTags: ['StreamGroups']
      }),
      streamGroupsUpdateStreamGroup: build.mutation<StreamGroupsUpdateStreamGroupApiResponse, StreamGroupsUpdateStreamGroupApiArg>({
        query: (queryArg) => ({ url: `/api/streamgroups/updatestreamgroup`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['StreamGroups']
      }),
      streamGroupsGetStreamGroupVideoStreamUrl: build.query<
        StreamGroupsGetStreamGroupVideoStreamUrlApiResponse,
        StreamGroupsGetStreamGroupVideoStreamUrlApiArg
      >({
        query: (queryArg) => ({ url: `/api/streamgroups/getstreamgroupvideostreamurl`, params: { VideoStreamId: queryArg } }),
        providesTags: ['StreamGroups']
      })
    }),
    overrideExisting: false
  });
export { injectedRtkApi as iptvApi };
export type ChannelGroupsCreateChannelGroupApiResponse = unknown;
export type ChannelGroupsCreateChannelGroupApiArg = CreateChannelGroupRequest;
export type ChannelGroupsDeleteAllChannelGroupsFromParametersApiResponse = unknown;
export type ChannelGroupsDeleteAllChannelGroupsFromParametersApiArg = DeleteAllChannelGroupsFromParametersRequest;
export type ChannelGroupsDeleteChannelGroupApiResponse = unknown;
export type ChannelGroupsDeleteChannelGroupApiArg = DeleteChannelGroupRequest;
export type ChannelGroupsGetChannelGroupApiResponse = /** status 200  */ ChannelGroupDto;
export type ChannelGroupsGetChannelGroupApiArg = number;
export type ChannelGroupsGetChannelGroupIdNamesApiResponse = /** status 200  */ ChannelGroupIdName[];
export type ChannelGroupsGetChannelGroupIdNamesApiArg = void;
export type ChannelGroupsGetPagedChannelGroupsApiResponse = /** status 200  */ PagedResponseOfChannelGroupDto;
export type ChannelGroupsGetPagedChannelGroupsApiArg = {
  pageNumber?: number;
  pageSize?: number;
  orderBy?: string;
  jsonArgumentString?: string | null;
  jsonFiltersString?: string | null;
};
export type ChannelGroupsUpdateChannelGroupApiResponse = unknown;
export type ChannelGroupsUpdateChannelGroupApiArg = UpdateChannelGroupRequest;
export type ChannelGroupsUpdateChannelGroupsApiResponse = unknown;
export type ChannelGroupsUpdateChannelGroupsApiArg = UpdateChannelGroupsRequest;
export type ChannelGroupsGetChannelGroupNamesApiResponse = /** status 200  */ string[];
export type ChannelGroupsGetChannelGroupNamesApiArg = void;
export type ChannelGroupsGetChannelGroupsForStreamGroupApiResponse = /** status 200  */ ChannelGroupDto[];
export type ChannelGroupsGetChannelGroupsForStreamGroupApiArg = GetChannelGroupsForStreamGroupRequest;
export type EpgFilesCreateEpgFileApiResponse = unknown;
export type EpgFilesCreateEpgFileApiArg = CreateEpgFileRequest;
export type EpgFilesCreateEpgFileFromFormApiResponse = unknown;
export type EpgFilesCreateEpgFileFromFormApiArg = {
  Description?: string | null;
  FormFile?: Blob | null;
  Name?: string;
  EPGNumber?: number;
  UrlSource?: string | null;
  Color?: string | null;
};
export type EpgFilesDeleteEpgFileApiResponse = unknown;
export type EpgFilesDeleteEpgFileApiArg = DeleteEpgFileRequest;
export type EpgFilesGetEpgColorsApiResponse = /** status 200  */ EpgColorDto[];
export type EpgFilesGetEpgColorsApiArg = void;
export type EpgFilesGetEpgFileApiResponse = /** status 200  */ EpgFileDto;
export type EpgFilesGetEpgFileApiArg = number;
export type EpgFilesGetEpgFilePreviewByIdApiResponse = /** status 200  */ EpgFilePreviewDto[];
export type EpgFilesGetEpgFilePreviewByIdApiArg = number;
export type EpgFilesGetEpgNextEpgNumberApiResponse = /** status 200  */ number;
export type EpgFilesGetEpgNextEpgNumberApiArg = void;
export type EpgFilesGetPagedEpgFilesApiResponse = /** status 200  */ PagedResponseOfEpgFileDto;
export type EpgFilesGetPagedEpgFilesApiArg = {
  pageNumber?: number;
  pageSize?: number;
  orderBy?: string;
  jsonArgumentString?: string | null;
  jsonFiltersString?: string | null;
};
export type EpgFilesProcessEpgFileApiResponse = unknown;
export type EpgFilesProcessEpgFileApiArg = ProcessEpgFileRequest;
export type EpgFilesRefreshEpgFileApiResponse = unknown;
export type EpgFilesRefreshEpgFileApiArg = RefreshEpgFileRequest;
export type EpgFilesScanDirectoryForEpgFilesApiResponse = unknown;
export type EpgFilesScanDirectoryForEpgFilesApiArg = void;
export type EpgFilesUpdateEpgFileApiResponse = unknown;
export type EpgFilesUpdateEpgFileApiArg = UpdateEpgFileRequest;
export type IconsAutoMatchIconToStreamsApiResponse = unknown;
export type IconsAutoMatchIconToStreamsApiArg = AutoMatchIconToStreamsRequest;
export type IconsGetIconApiResponse = /** status 200  */ IconFileDto;
export type IconsGetIconApiArg = number;
export type IconsGetIconFromSourceApiResponse = /** status 200  */ IconFileDto;
export type IconsGetIconFromSourceApiArg = string;
export type IconsGetPagedIconsApiResponse = /** status 200  */ PagedResponseOfIconFileDto;
export type IconsGetPagedIconsApiArg = {
  count?: number;
  first?: number;
  last?: number;
  pageNumber?: number;
  pageSize?: number;
  orderBy?: string;
  jsonArgumentString?: string | null;
  jsonFiltersString?: string | null;
};
export type IconsGetIconsSimpleQueryApiResponse = /** status 200  */ IconFileDto[];
export type IconsGetIconsSimpleQueryApiArg = {
  count?: number;
  first?: number;
  last?: number;
  pageNumber?: number;
  pageSize?: number;
  orderBy?: string;
  jsonArgumentString?: string | null;
  jsonFiltersString?: string | null;
};
export type LogsGetLogApiResponse = /** status 200  */ LogEntryDto[];
export type LogsGetLogApiArg = {
  lastId?: number;
  maxLines?: number;
};
export type ProgrammesGetProgrammeApiResponse = /** status 200  */ XmltvProgramme[];
export type ProgrammesGetProgrammeApiArg = string;
export type ProgrammesGetProgrammesApiResponse = /** status 200  */ XmltvProgramme[];
export type ProgrammesGetProgrammesApiArg = void;
export type SchedulesDirectAddLineupApiResponse = /** status 200  */ boolean;
export type SchedulesDirectAddLineupApiArg = AddLineup;
export type SchedulesDirectAddStationApiResponse = /** status 200  */ boolean;
export type SchedulesDirectAddStationApiArg = AddStation;
export type SchedulesDirectGetAvailableCountriesApiResponse = /** status 200  */ CountryData[];
export type SchedulesDirectGetAvailableCountriesApiArg = void;
export type SchedulesDirectGetChannelNamesApiResponse = /** status 200  */ string[];
export type SchedulesDirectGetChannelNamesApiArg = void;
export type SchedulesDirectGetHeadendsApiResponse = /** status 200  */ HeadendDto[];
export type SchedulesDirectGetHeadendsApiArg = GetHeadends;
export type SchedulesDirectGetLineupPreviewChannelApiResponse = /** status 200  */ LineupPreviewChannel[];
export type SchedulesDirectGetLineupPreviewChannelApiArg = GetLineupPreviewChannel;
export type SchedulesDirectGetLineupsApiResponse = /** status 200  */ SubscribedLineup[];
export type SchedulesDirectGetLineupsApiArg = void;
export type SchedulesDirectGetPagedStationChannelNameSelectionsApiResponse = /** status 200  */ PagedResponseOfStationChannelName;
export type SchedulesDirectGetPagedStationChannelNameSelectionsApiArg = {
  count?: number;
  first?: number;
  last?: number;
  pageNumber?: number;
  pageSize?: number;
  orderBy?: string;
  jsonArgumentString?: string | null;
  jsonFiltersString?: string | null;
};
export type SchedulesDirectGetSelectedStationIdsApiResponse = /** status 200  */ StationIdLineup[];
export type SchedulesDirectGetSelectedStationIdsApiArg = void;
export type SchedulesDirectGetStationChannelMapsApiResponse = /** status 200  */ StationChannelMap[];
export type SchedulesDirectGetStationChannelMapsApiArg = void;
export type SchedulesDirectGetStationChannelNameFromDisplayNameApiResponse = /** status 200  */ StationChannelName;
export type SchedulesDirectGetStationChannelNameFromDisplayNameApiArg = GetStationChannelNameFromDisplayName;
export type SchedulesDirectGetStationChannelNamesSimpleQueryApiResponse = /** status 200  */ StationChannelName[];
export type SchedulesDirectGetStationChannelNamesSimpleQueryApiArg = {
  count?: number;
  first?: number;
  last?: number;
  pageNumber?: number;
  pageSize?: number;
  orderBy?: string;
  jsonArgumentString?: string | null;
  jsonFiltersString?: string | null;
};
export type SchedulesDirectGetStationPreviewsApiResponse = /** status 200  */ StationPreview[];
export type SchedulesDirectGetStationPreviewsApiArg = void;
export type SchedulesDirectGetUserStatusApiResponse = /** status 200  */ UserStatus;
export type SchedulesDirectGetUserStatusApiArg = void;
export type SchedulesDirectRemoveLineupApiResponse = /** status 200  */ boolean;
export type SchedulesDirectRemoveLineupApiArg = RemoveLineup;
export type SchedulesDirectRemoveStationApiResponse = /** status 200  */ boolean;
export type SchedulesDirectRemoveStationApiArg = RemoveStation;
export type StreamGroupChannelGroupSyncStreamGroupChannelGroupsApiResponse = /** status 200  */ StreamGroupDto;
export type StreamGroupChannelGroupSyncStreamGroupChannelGroupsApiArg = SyncStreamGroupChannelGroupsRequest;
export type StreamGroupChannelGroupGetChannelGroupsFromStreamGroupApiResponse = /** status 200  */ ChannelGroupDto[];
export type StreamGroupChannelGroupGetChannelGroupsFromStreamGroupApiArg = number;
export type StreamGroupVideoStreamsGetStreamGroupVideoStreamIdsApiResponse = /** status 200  */ VideoStreamIsReadOnly[];
export type StreamGroupVideoStreamsGetStreamGroupVideoStreamIdsApiArg = number;
export type StreamGroupVideoStreamsGetPagedStreamGroupVideoStreamsApiResponse = /** status 200  */ PagedResponseOfVideoStreamDto;
export type StreamGroupVideoStreamsGetPagedStreamGroupVideoStreamsApiArg = {
  streamGroupId?: number;
  pageNumber?: number;
  pageSize?: number;
  orderBy?: string;
  jsonArgumentString?: string | null;
  jsonFiltersString?: string | null;
};
export type StreamGroupVideoStreamsSetVideoStreamRanksApiResponse = unknown;
export type StreamGroupVideoStreamsSetVideoStreamRanksApiArg = SetVideoStreamRanksRequest;
export type StreamGroupVideoStreamsSyncVideoStreamToStreamGroupPostApiResponse = unknown;
export type StreamGroupVideoStreamsSyncVideoStreamToStreamGroupPostApiArg = SyncVideoStreamToStreamGroupRequest;
export type StreamGroupVideoStreamsSyncVideoStreamToStreamGroupDeleteApiResponse = unknown;
export type StreamGroupVideoStreamsSyncVideoStreamToStreamGroupDeleteApiArg = SyncVideoStreamToStreamGroupRequest;
export type StreamGroupVideoStreamsSetStreamGroupVideoStreamChannelNumbersApiResponse = unknown;
export type StreamGroupVideoStreamsSetStreamGroupVideoStreamChannelNumbersApiArg = SetStreamGroupVideoStreamChannelNumbersRequest;
export type TestEpgSyncApiResponse = /** status 200  */ boolean;
export type TestEpgSyncApiArg = void;
export type VideoStreamLinksAddVideoStreamToVideoStreamApiResponse = unknown;
export type VideoStreamLinksAddVideoStreamToVideoStreamApiArg = AddVideoStreamToVideoStreamRequest;
export type VideoStreamLinksGetVideoStreamVideoStreamIdsApiResponse = /** status 200  */ string[];
export type VideoStreamLinksGetVideoStreamVideoStreamIdsApiArg = string;
export type VideoStreamLinksGetPagedVideoStreamVideoStreamsApiResponse = /** status 200  */ PagedResponseOfVideoStreamDto;
export type VideoStreamLinksGetPagedVideoStreamVideoStreamsApiArg = {
  pageNumber?: number;
  pageSize?: number;
  orderBy?: string;
  jsonArgumentString?: string | null;
  jsonFiltersString?: string | null;
};
export type VideoStreamLinksRemoveVideoStreamFromVideoStreamApiResponse = unknown;
export type VideoStreamLinksRemoveVideoStreamFromVideoStreamApiArg = RemoveVideoStreamFromVideoStreamRequest;
export type VideoStreamsCreateVideoStreamApiResponse = unknown;
export type VideoStreamsCreateVideoStreamApiArg = CreateVideoStreamRequest;
export type VideoStreamsChangeVideoStreamChannelApiResponse = unknown;
export type VideoStreamsChangeVideoStreamChannelApiArg = ChangeVideoStreamChannelRequest;
export type VideoStreamsDeleteVideoStreamApiResponse = unknown;
export type VideoStreamsDeleteVideoStreamApiArg = DeleteVideoStreamRequest;
export type VideoStreamsFailClientApiResponse = unknown;
export type VideoStreamsFailClientApiArg = FailClientRequest;
export type VideoStreamsGetAllStatisticsForAllUrlsApiResponse = /** status 200  */ StreamStatisticsResult[];
export type VideoStreamsGetAllStatisticsForAllUrlsApiArg = void;
export type VideoStreamsGetVideoStreamApiResponse = /** status 200  */ VideoStreamDto;
export type VideoStreamsGetVideoStreamApiArg = string;
export type VideoStreamsGetVideoStreamNamesApiResponse = /** status 200  */ IdName[];
export type VideoStreamsGetVideoStreamNamesApiArg = void;
export type VideoStreamsGetPagedVideoStreamsApiResponse = /** status 200  */ PagedResponseOfVideoStreamDto;
export type VideoStreamsGetPagedVideoStreamsApiArg = {
  pageNumber?: number;
  pageSize?: number;
  orderBy?: string;
  jsonArgumentString?: string | null;
  jsonFiltersString?: string | null;
};
export type VideoStreamsReadAndWriteApiResponse = unknown;
export type VideoStreamsReadAndWriteApiArg = {
  filePath?: string;
  body: Blob;
};
export type VideoStreamsGetVideoStreamStreamGetApiResponse = unknown;
export type VideoStreamsGetVideoStreamStreamGetApiArg = string;
export type VideoStreamsGetVideoStreamStreamHeadApiResponse = unknown;
export type VideoStreamsGetVideoStreamStreamHeadApiArg = string;
export type VideoStreamsGetVideoStreamStreamGet2ApiResponse = unknown;
export type VideoStreamsGetVideoStreamStreamGet2ApiArg = string;
export type VideoStreamsGetVideoStreamStreamHead2ApiResponse = unknown;
export type VideoStreamsGetVideoStreamStreamHead2ApiArg = string;
export type VideoStreamsGetVideoStreamStreamGet3ApiResponse = unknown;
export type VideoStreamsGetVideoStreamStreamGet3ApiArg = {
  encodedIds: string;
  name: string;
};
export type VideoStreamsGetVideoStreamStreamHead3ApiResponse = unknown;
export type VideoStreamsGetVideoStreamStreamHead3ApiArg = {
  encodedIds: string;
  name: string;
};
export type VideoStreamsReSetVideoStreamsLogoApiResponse = unknown;
export type VideoStreamsReSetVideoStreamsLogoApiArg = ReSetVideoStreamsLogoRequest;
export type VideoStreamsSetVideoStreamChannelNumbersApiResponse = unknown;
export type VideoStreamsSetVideoStreamChannelNumbersApiArg = SetVideoStreamChannelNumbersRequest;
export type VideoStreamsSetVideoStreamsLogoFromEpgApiResponse = unknown;
export type VideoStreamsSetVideoStreamsLogoFromEpgApiArg = SetVideoStreamsLogoFromEpgRequest;
export type VideoStreamsUpdateVideoStreamApiResponse = unknown;
export type VideoStreamsUpdateVideoStreamApiArg = UpdateVideoStreamRequest;
export type VideoStreamsUpdateVideoStreamsApiResponse = unknown;
export type VideoStreamsUpdateVideoStreamsApiArg = UpdateVideoStreamsRequest;
export type VideoStreamsUpdateAllVideoStreamsFromParametersApiResponse = unknown;
export type VideoStreamsUpdateAllVideoStreamsFromParametersApiArg = UpdateAllVideoStreamsFromParametersRequest;
export type VideoStreamsDeleteAllVideoStreamsFromParametersApiResponse = unknown;
export type VideoStreamsDeleteAllVideoStreamsFromParametersApiArg = DeleteAllVideoStreamsFromParametersRequest;
export type VideoStreamsSetVideoStreamChannelNumbersFromParametersApiResponse = unknown;
export type VideoStreamsSetVideoStreamChannelNumbersFromParametersApiArg = SetVideoStreamChannelNumbersFromParametersRequest;
export type VideoStreamsSetVideoStreamsLogoFromEpgFromParametersApiResponse = unknown;
export type VideoStreamsSetVideoStreamsLogoFromEpgFromParametersApiArg = SetVideoStreamsLogoFromEpgFromParametersRequest;
export type VideoStreamsReSetVideoStreamsLogoFromParametersApiResponse = unknown;
export type VideoStreamsReSetVideoStreamsLogoFromParametersApiArg = ReSetVideoStreamsLogoFromParametersRequest;
export type VideoStreamsSimulateStreamFailureForAllApiResponse = unknown;
export type VideoStreamsSimulateStreamFailureForAllApiArg = void;
export type VideoStreamsSimulateStreamFailureApiResponse = unknown;
export type VideoStreamsSimulateStreamFailureApiArg = SimulateStreamFailureRequest;
export type VideoStreamsAutoSetEpgApiResponse = unknown;
export type VideoStreamsAutoSetEpgApiArg = AutoSetEpgRequest;
export type VideoStreamsAutoSetEpgFromParametersApiResponse = unknown;
export type VideoStreamsAutoSetEpgFromParametersApiArg = AutoSetEpgFromParametersRequest;
export type VideoStreamsSetVideoStreamTimeShiftsApiResponse = unknown;
export type VideoStreamsSetVideoStreamTimeShiftsApiArg = SetVideoStreamTimeShiftsRequest;
export type VideoStreamsSetVideoStreamTimeShiftFromParametersApiResponse = unknown;
export type VideoStreamsSetVideoStreamTimeShiftFromParametersApiArg = SetVideoStreamTimeShiftFromParametersRequest;
export type VideoStreamsGetVideoStreamInfoFromIdApiResponse = /** status 200  */ VideoInfo;
export type VideoStreamsGetVideoStreamInfoFromIdApiArg = string;
export type VideoStreamsGetVideoStreamInfoFromUrlApiResponse = /** status 200  */ VideoInfo;
export type VideoStreamsGetVideoStreamInfoFromUrlApiArg = string;
export type FilesGetFileApiResponse = unknown;
export type FilesGetFileApiArg = {
  source: string;
  filetype: SmFileTypes;
};
export type M3UFilesCreateM3UFileApiResponse = unknown;
export type M3UFilesCreateM3UFileApiArg = CreateM3UFileRequest;
export type M3UFilesCreateM3UFileFromFormApiResponse = unknown;
export type M3UFilesCreateM3UFileFromFormApiArg = {
  Description?: string | null;
  MaxStreamCount?: number;
  StartingChannelNumber?: number | null;
  FormFile?: Blob | null;
  Name?: string;
  UrlSource?: string | null;
};
export type M3UFilesChangeM3UFileNameApiResponse = unknown;
export type M3UFilesChangeM3UFileNameApiArg = ChangeM3UFileNameRequest;
export type M3UFilesDeleteM3UFileApiResponse = unknown;
export type M3UFilesDeleteM3UFileApiArg = DeleteM3UFileRequest;
export type M3UFilesGetM3UFileApiResponse = /** status 200  */ M3UFileDto;
export type M3UFilesGetM3UFileApiArg = number;
export type M3UFilesGetPagedM3UFilesApiResponse = /** status 200  */ PagedResponseOfM3UFileDto;
export type M3UFilesGetPagedM3UFilesApiArg = {
  pageNumber?: number;
  pageSize?: number;
  orderBy?: string;
  jsonArgumentString?: string | null;
  jsonFiltersString?: string | null;
};
export type M3UFilesProcessM3UFileApiResponse = unknown;
export type M3UFilesProcessM3UFileApiArg = ProcessM3UFileRequest;
export type M3UFilesRefreshM3UFileApiResponse = unknown;
export type M3UFilesRefreshM3UFileApiArg = RefreshM3UFileRequest;
export type M3UFilesScanDirectoryForM3UFilesApiResponse = unknown;
export type M3UFilesScanDirectoryForM3UFilesApiArg = void;
export type M3UFilesUpdateM3UFileApiResponse = unknown;
export type M3UFilesUpdateM3UFileApiArg = UpdateM3UFileRequest;
export type M3UFilesGetM3UFileNamesApiResponse = /** status 200  */ string[];
export type M3UFilesGetM3UFileNamesApiArg = void;
export type MiscGetDownloadServiceStatusApiResponse = /** status 200  */ ImageDownloadServiceStatus;
export type MiscGetDownloadServiceStatusApiArg = void;
export type MiscGetTestM3UApiResponse = unknown;
export type MiscGetTestM3UApiArg = number;
export type QueueGetQueueStatusApiResponse = /** status 200  */ TaskQueueStatus[];
export type QueueGetQueueStatusApiArg = void;
export type SettingsGetIsSystemReadyApiResponse = /** status 200  */ boolean;
export type SettingsGetIsSystemReadyApiArg = void;
export type SettingsGetSettingApiResponse = /** status 200  */ SettingDto;
export type SettingsGetSettingApiArg = void;
export type SettingsGetSystemStatusApiResponse = /** status 200  */ SdSystemStatus;
export type SettingsGetSystemStatusApiArg = void;
export type SettingsLogInApiResponse = /** status 200  */ boolean;
export type SettingsLogInApiArg = LogInRequest;
export type SettingsUpdateSettingApiResponse = unknown;
export type SettingsUpdateSettingApiArg = UpdateSettingRequest;
export type StreamGroupsCreateStreamGroupApiResponse = unknown;
export type StreamGroupsCreateStreamGroupApiArg = CreateStreamGroupRequest;
export type StreamGroupsDeleteStreamGroupApiResponse = unknown;
export type StreamGroupsDeleteStreamGroupApiArg = DeleteStreamGroupRequest;
export type StreamGroupsGetStreamGroupApiResponse = /** status 200  */ StreamGroupDto;
export type StreamGroupsGetStreamGroupApiArg = number;
export type StreamGroupsGetStreamGroupCapabilityApiResponse = unknown;
export type StreamGroupsGetStreamGroupCapabilityApiArg = string;
export type StreamGroupsGetStreamGroupCapability2ApiResponse = unknown;
export type StreamGroupsGetStreamGroupCapability2ApiArg = string;
export type StreamGroupsGetStreamGroupCapability3ApiResponse = unknown;
export type StreamGroupsGetStreamGroupCapability3ApiArg = string;
export type StreamGroupsGetStreamGroupDiscoverApiResponse = unknown;
export type StreamGroupsGetStreamGroupDiscoverApiArg = string;
export type StreamGroupsGetStreamGroupEpgApiResponse = unknown;
export type StreamGroupsGetStreamGroupEpgApiArg = string;
export type StreamGroupsGetStreamGroupLineupApiResponse = unknown;
export type StreamGroupsGetStreamGroupLineupApiArg = string;
export type StreamGroupsGetStreamGroupLineupStatusApiResponse = unknown;
export type StreamGroupsGetStreamGroupLineupStatusApiArg = string;
export type StreamGroupsGetStreamGroupM3UApiResponse = unknown;
export type StreamGroupsGetStreamGroupM3UApiArg = string;
export type StreamGroupsGetPagedStreamGroupsApiResponse = /** status 200  */ PagedResponseOfStreamGroupDto;
export type StreamGroupsGetPagedStreamGroupsApiArg = {
  pageNumber?: number;
  pageSize?: number;
  orderBy?: string;
  jsonArgumentString?: string | null;
  jsonFiltersString?: string | null;
};
export type StreamGroupsUpdateStreamGroupApiResponse = unknown;
export type StreamGroupsUpdateStreamGroupApiArg = UpdateStreamGroupRequest;
export type StreamGroupsGetStreamGroupVideoStreamUrlApiResponse = /** status 200  */ string;
export type StreamGroupsGetStreamGroupVideoStreamUrlApiArg = string;
export type CreateChannelGroupRequest = {
  groupName: string;
  isReadOnly: boolean;
};
export type QueryStringParameters = {
  pageNumber?: number;
  pageSize?: number;
  orderBy?: string;
  jsonArgumentString?: string | null;
  jsonFiltersString?: string | null;
};
export type ChannelGroupParameters = QueryStringParameters & object;
export type DeleteAllChannelGroupsFromParametersRequest = {
  parameters?: ChannelGroupParameters;
};
export type DeleteChannelGroupRequest = {
  channelGroupId: number;
};
export type ChannelGroupStreamCount = {
  channelGroupId?: number;
  activeCount?: number;
  totalCount?: number;
  hiddenCount?: number;
};
export type ChannelGroupArg = ChannelGroupStreamCount & {
  isHidden: boolean;
  isReadOnly: boolean;
  name: string;
};
export type ChannelGroupDto = ChannelGroupArg & {
  id: number;
};
export type ChannelGroupIdName = {
  id?: number;
  totalCount?: number;
  name?: string;
};
export type PagedResponseOfChannelGroupDto = {
  data: ChannelGroupDto[];
  pageNumber: number;
  pageSize: number;
  totalPageCount: number;
  totalItemCount: number;
  first: number;
};
export type UpdateChannelGroupRequest = {
  channelGroupId?: number;
  newGroupName?: string | null;
  isHidden?: boolean | null;
  toggleVisibility?: boolean | null;
};
export type UpdateChannelGroupsRequest = {
  channelGroupRequests: UpdateChannelGroupRequest[];
};
export type GetChannelGroupsForStreamGroupRequest = {
  streamGroupId?: number;
};
export type CreateEpgFileRequest = {
  description?: string | null;
  formFile?: Blob | null;
  name?: string;
  epgNumber?: number;
  urlSource?: string | null;
  color?: string | null;
};
export type DeleteEpgFileRequest = {
  deleteFile?: boolean;
  id?: number;
};
export type EpgColorDto = {
  id?: number;
  stationId?: string;
  color?: string;
};
export type BaseFileDto = {
  source: string;
  autoUpdate: boolean;
  description: string;
  downloadErrors: number;
  hoursToUpdate: number;
  id: number;
  lastDownloadAttempt: string;
  lastDownloaded: string;
  name: string;
  needsUpdate: boolean;
  url: string;
};
export type EpgFileDto = BaseFileDto & {
  epgNumber: number;
  color: string;
  channelCount: number;
  epgStartDate: string;
  epgStopDate: string;
  programmeCount: number;
};
export type EpgFilePreviewDto = {
  id?: number;
  channelLogo?: string;
  channelNumber?: string;
  channelName?: string;
};
export type PagedResponseOfEpgFileDto = {
  data: EpgFileDto[];
  pageNumber: number;
  pageSize: number;
  totalPageCount: number;
  totalItemCount: number;
  first: number;
};
export type ProcessEpgFileRequest = {
  id?: number;
};
export type RefreshEpgFileRequest = {
  id: number;
};
export type BaseFileRequest = {
  autoUpdate?: boolean | null;
  hoursToUpdate?: number | null;
  description?: string | null;
  id: number;
  metaData?: string | null;
  name?: string | null;
  url?: string | null;
};
export type UpdateEpgFileRequest = BaseFileRequest & {
  epgNumber?: number | null;
  color?: string | null;
};
export type AutoMatchIconToStreamsRequest = {
  ids?: string[];
};
export type IconFileDto = {
  id: number;
  name: string;
  source: string;
};
export type PagedResponseOfIconFileDto = {
  data: IconFileDto[];
  pageNumber: number;
  pageSize: number;
  totalPageCount: number;
  totalItemCount: number;
  first: number;
};
export type LogLevel = 0 | 1 | 2 | 3 | 4 | 5 | 6;
export type LogEntry = {
  id: number;
  logLevel: LogLevel;
  logLevelName: string;
  message: string;
  timeStamp: string;
};
export type LogEntryDto = LogEntry & object;
export type XmltvText = {
  language?: string | null;
  text?: string | null;
};
export type XmltvSubtitles = {
  language?: string;
  type?: string;
};
export type XmltvActor = {
  role?: string;
  actor?: string;
};
export type XmltvCredit = {
  directors?: string[] | null;
  actors?: XmltvActor[] | null;
  writers?: string[] | null;
  adapters?: string[];
  producers?: string[] | null;
  composers?: string[];
  editors?: string[];
  presenters?: string[] | null;
  commentators?: string[];
  guests?: string[] | null;
};
export type XmltvLength = {
  units?: string;
  text?: string;
};
export type XmltvIcon = {
  src?: string;
  width?: number;
  height?: number;
};
export type XmltvEpisodeNum = {
  system?: string;
  text?: string;
};
export type XmltvVideo = {
  present?: string;
  colour?: string;
  aspect?: string;
  quality?: string;
};
export type XmltvAudio = {
  present?: string;
  stereo?: string;
};
export type XmltvPreviouslyShown = {
  start?: string;
  channel?: string;
  text?: string;
};
export type XmltvRating = {
  value?: string;
  elementValue?: string;
  icons?: XmltvIcon[];
  system?: string;
};
export type XmltvReview = {
  type?: string;
  source?: string;
  reviewer?: string;
  lang?: string;
  text?: string;
};
export type XmltvProgramme = {
  start?: string;
  startDateTime?: string;
  epgFileId?: number;
  stop?: string;
  stopDateTime?: string;
  pdcStart?: string;
  vpsStart?: string;
  showView?: string;
  videoPlus?: string;
  channel?: string;
  clumpIdx?: string;
  titles?: XmltvText[] | null;
  subTitles2?: XmltvSubtitles[] | null;
  subTitles?: XmltvText[] | null;
  descriptions?: XmltvText[] | null;
  credits?: XmltvCredit | null;
  date?: string | null;
  categories?: XmltvText[] | null;
  keywords?: XmltvText[] | null;
  language?: XmltvText | null;
  origLanguage?: XmltvText;
  length?: XmltvLength;
  icons?: XmltvIcon[] | null;
  urls?: string[];
  countries?: XmltvText[];
  sport?: XmltvText | null;
  teams?: XmltvText[] | null;
  episodeNums?: XmltvEpisodeNum[];
  video?: XmltvVideo | null;
  audio?: XmltvAudio | null;
  previouslyShown?: XmltvPreviouslyShown | null;
  premiere?: XmltvText | null;
  lastChance?: XmltvText;
  new?: string | null;
  live?: string | null;
  rating?: XmltvRating[] | null;
  starRating?: XmltvRating[] | null;
  review?: XmltvReview[] | null;
};
export type AddLineup = {
  lineup?: string;
};
export type StationRequest = {
  stationId?: string;
  lineUp?: string;
};
export type AddStation = {
  requests?: StationRequest[];
};
export type Country = {
  fullName?: string;
  shortName?: string;
  postalCodeExample?: string;
  postalCode?: string;
  onePostalCode?: boolean;
};
export type CountryData = {
  key?: string;
  countries?: Country[];
};
export type HeadendDto = {
  id?: string;
  headendId?: string;
  lineup?: string;
  location?: string;
  name?: string;
  transport?: string;
};
export type GetHeadends = {
  country?: string;
  postalCode?: string;
};
export type LineupPreviewChannel = {
  id?: number;
  channel?: string;
  name?: string;
  callsign?: string;
  affiliate?: string;
};
export type GetLineupPreviewChannel = {
  lineup?: string;
};
export type SubscribedLineup = {
  id?: string;
  lineup?: string;
  name?: string;
  transport?: string;
  location?: string;
  uri?: string;
  isDeleted?: boolean;
};
export type StationChannelName = {
  id: string;
  channel: string;
  channelName: string;
  displayName: string;
};
export type PagedResponseOfStationChannelName = {
  data: StationChannelName[];
  pageNumber: number;
  pageSize: number;
  totalPageCount: number;
  totalItemCount: number;
  first: number;
};
export type StationIdLineup = {
  lineup?: string;
  stationId?: string;
  id?: string;
};
export type LineupChannel = {
  channelNumber?: string;
  myChannelNumber?: number;
  myChannelSubnumber?: number;
  stationID?: string;
  uhfVhf?: number | null;
  atscMajor?: number | null;
  atscMinor?: number | null;
  atscType?: string;
  frequencyHz?: number | null;
  polarization?: string;
  deliverySystem?: string;
  modulationSystem?: string;
  symbolrate?: number | null;
  fec?: string;
  serviceID?: number | null;
  networkID?: number | null;
  transportID?: number | null;
  channel?: string;
  virtualChannel?: string;
  channelMajor?: number | null;
  channelMinor?: number | null;
  providerChannel?: string;
  providerCallsign?: string;
  logicalChannelNumber?: string;
  matchType?: string;
};
export type StationBroadcaster = {
  city?: string;
  state?: string;
  postalcode?: string;
  country?: string;
};
export type StationImage = {
  URL?: string;
  height?: number;
  width?: number;
  md5?: string;
  source?: string;
  category?: string;
};
export type LineupStation = {
  stationID?: string;
  name?: string;
  callsign?: string;
  affiliate?: string;
  broadcastLanguage?: string[];
  descriptionLanguage?: string[];
  broadcaster?: StationBroadcaster;
  isCommercialFree?: boolean | null;
  stationLogo?: StationImage[];
  logo?: StationImage;
};
export type LineupMetadata = {
  lineup?: string;
  modified?: string;
  transport?: string;
  modulation?: string;
};
export type StationChannelMap = {
  map?: LineupChannel[];
  stations?: LineupStation[];
  metadata?: LineupMetadata | null;
};
export type GetStationChannelNameFromDisplayName = {
  value?: string;
};
export type Logo = {
  URL?: string;
  height?: number;
  width?: number;
  md5?: string;
};
export type StationPreview = {
  logo?: Logo;
  affiliate?: string;
  callsign?: string;
  id?: string;
  lineup?: string;
  name?: string;
  stationId?: string;
};
export type BaseResponse = {
  response?: string;
  code?: number;
  serverID?: string;
  message?: string;
  datetime?: string;
  uuid?: string;
};
export type StatusAccount = {
  expires?: string;
  messages?: string[];
  maxLineups?: number;
};
export type StatusLineup = {
  lineup?: string;
  modified?: string;
  uri?: string;
  isDeleted?: boolean;
};
export type SystemStatus = {
  date?: string;
  status?: string;
  message?: string;
};
export type UserStatus = BaseResponse & {
  account?: StatusAccount;
  lineups?: StatusLineup[];
  lastDataUpdate?: string;
  notifications?: string[];
  systemStatus?: SystemStatus[];
};
export type RemoveLineup = {
  lineup?: string;
};
export type RemoveStation = {
  requests?: StationRequest[];
};
export type StreamGroupDto = {
  isLoading: boolean;
  hdhrLink: string;
  isReadOnly: boolean;
  autoSetChannelNumbers: boolean;
  streamCount: number;
  id: number;
  m3ULink: string;
  name: string;
  xmlLink: string;
};
export type SyncStreamGroupChannelGroupsRequest = {
  streamGroupId?: number;
  channelGroupIds?: number[];
};
export type VideoStreamIsReadOnly = {
  rank?: number;
  isReadOnly?: boolean;
  videoStreamId?: string;
};
export type StreamingProxyTypes = 0 | 1 | 2 | 3;
export type VideoStreamHandlers = 0 | 1 | 2;
export type BaseVideoStreamDto = {
  id: string;
  isActive: boolean;
  isDeleted: boolean;
  isHidden: boolean;
  isReadOnly: boolean;
  isUserCreated: boolean;
  m3UFileId: number;
  m3UFileName: string;
  streamingProxyType: StreamingProxyTypes;
  groupTitle: string;
  tvg_chno: number;
  tvg_group: string;
  timeShift: string;
  tvg_ID: string;
  tvg_logo: string;
  tvg_name: string;
  url: string;
  user_Tvg_chno: number;
  user_Tvg_group: string;
  user_Tvg_ID: string;
  user_Tvg_ID_DisplayName: string;
  user_Tvg_logo: string;
  user_Tvg_name: string;
  user_Url: string;
  videoStreamHandler: VideoStreamHandlers;
};
export type VideoStreamDto = BaseVideoStreamDto & {
  maxStreams: number;
  isLoading: boolean;
  channelGroupId: number;
  rank: number;
  childVideoStreams: VideoStreamDto[];
};
export type PagedResponseOfVideoStreamDto = {
  data: VideoStreamDto[];
  pageNumber: number;
  pageSize: number;
  totalPageCount: number;
  totalItemCount: number;
  first: number;
};
export type VideoStreamIdRank = {
  videoStreamId?: string;
  rank?: number;
};
export type SetVideoStreamRanksRequest = {
  streamGroupId: number;
  videoStreamIDRanks: VideoStreamIdRank[];
};
export type SyncVideoStreamToStreamGroupRequest = {
  streamGroupId: number;
  videoStreamId: string;
};
export type SetStreamGroupVideoStreamChannelNumbersRequest = {
  streamGroupId?: number;
  startingNumber?: number;
  orderBy?: string;
};
export type AddVideoStreamToVideoStreamRequest = {
  parentVideoStreamId: string;
  childVideoStreamId: string;
  rank: number | null;
};
export type RemoveVideoStreamFromVideoStreamRequest = {
  parentVideoStreamId: string;
  childVideoStreamId: string;
};
export type VideoStreamBaseRequest = {
  streamingProxyType?: StreamingProxyTypes | null;
  toggleVisibility?: boolean | null;
  groupTitle?: string | null;
  tvg_chno?: number | null;
  tvg_group?: string | null;
  timeShift?: string | null;
  tvg_ID?: string | null;
  tvg_logo?: string | null;
  tvg_name?: string | null;
  url?: string | null;
  videoStreams?: VideoStreamDto[] | null;
};
export type CreateVideoStreamRequest = VideoStreamBaseRequest & object;
export type ChangeVideoStreamChannelRequest = {
  playingVideoStreamId?: string;
  newVideoStreamId?: string;
};
export type DeleteVideoStreamRequest = {
  id?: string;
};
export type FailClientRequest = {
  clientId: string;
};
export type StreamStatisticsResult = {
  id?: string;
  circularBufferId?: string;
  clientAgent?: string;
  clientBitsPerSecond?: number;
  clientBytesRead?: number;
  clientBytesWritten?: number;
  clientElapsedTime?: string;
  clientId?: string;
  clientStartTime?: string;
  inputBitsPerSecond?: number;
  inputBytesRead?: number;
  inputBytesWritten?: number;
  inputElapsedTime?: string;
  inputStartTime?: string;
  logo?: string | null;
  m3UStreamingProxyType?: StreamingProxyTypes;
  rank?: number;
  streamUrl?: string | null;
  videoStreamId?: string;
  channelName?: string;
  videoStreamName?: string;
  clientIPAddress?: string;
  channelId?: string;
};
export type IdName = {
  id: string;
  name: string;
};
export type ReSetVideoStreamsLogoRequest = {
  ids?: string[];
};
export type SetVideoStreamChannelNumbersRequest = {
  ids: string[];
  overWriteExisting: boolean;
  startNumber: number;
  orderBy: string;
};
export type SetVideoStreamsLogoFromEpgRequest = {
  ids?: string[];
  orderBy?: string | null;
};
export type UpdateVideoStreamRequest = VideoStreamBaseRequest & {
  id?: string;
};
export type UpdateVideoStreamsRequest = {
  videoStreamUpdates?: UpdateVideoStreamRequest[];
};
export type VideoStreamParameters = QueryStringParameters & object;
export type UpdateAllVideoStreamsFromParametersRequest = {
  parameters?: VideoStreamParameters;
  request?: UpdateVideoStreamRequest;
  channelGroupIds?: number[] | null;
};
export type DeleteAllVideoStreamsFromParametersRequest = {
  parameters?: VideoStreamParameters;
};
export type SetVideoStreamChannelNumbersFromParametersRequest = {
  parameters?: VideoStreamParameters;
  overWriteExisting?: boolean;
  startNumber?: number;
};
export type SetVideoStreamsLogoFromEpgFromParametersRequest = {
  parameters?: VideoStreamParameters;
};
export type ReSetVideoStreamsLogoFromParametersRequest = {
  parameters?: VideoStreamParameters;
};
export type SimulateStreamFailureRequest = {
  streamUrl: string;
};
export type AutoSetEpgRequest = {
  ids?: string[];
};
export type AutoSetEpgFromParametersRequest = {
  parameters?: VideoStreamParameters;
  ids?: string[];
};
export type SetVideoStreamTimeShiftsRequest = {
  ids: string[];
  timeShift: string;
};
export type SetVideoStreamTimeShiftFromParametersRequest = {
  parameters?: VideoStreamParameters;
  timeShift?: string;
};
export type Disposition = {
  default?: number;
  dub?: number;
  original?: number;
  comment?: number;
  lyrics?: number;
  karaoke?: number;
  forced?: number;
  hearing_impaired?: number;
  visual_impaired?: number;
  clean_effects?: number;
  attached_pic?: number;
  timed_thumbnails?: number;
};
export type VideoStreamInfo = {
  index?: number;
  codec_name?: string;
  codec_long_name?: string;
  profile?: string;
  codec_type?: string;
  codec_tag_string?: string;
  codec_tag?: string;
  width?: number;
  height?: number;
  coded_width?: number;
  coded_height?: number;
  closed_captions?: number;
  has_b_frames?: number;
  sample_aspect_ratio?: string;
  display_aspect_ratio?: string;
  pix_fmt?: string;
  level?: number;
  color_range?: string;
  color_space?: string;
  color_transfer?: string;
  color_primaries?: string;
  chroma_location?: string;
  field_order?: string;
  refs?: number;
  is_avc?: string;
  nal_length_size?: string;
  id?: string;
  r_frame_rate?: string;
  avg_frame_rate?: string;
  time_base?: string;
  start_pts?: any;
  start_time?: string;
  bits_per_raw_sample?: string;
  disposition?: Disposition;
  sample_fmt?: string;
  sample_rate?: string;
  channels?: number | null;
  channel_layout?: string;
  bits_per_sample?: number | null;
  bit_rate?: string;
};
export type Format = {
  filename?: string;
  nb_streams?: number;
  nb_programs?: number;
  format_name?: string;
  format_long_name?: string;
  start_time?: string;
  probe_score?: number;
};
export type VideoInfo = {
  streams?: VideoStreamInfo[];
  format?: Format;
};
export type SmFileTypes = 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10 | 11;
export type CreateM3UFileRequest = {
  description?: string | null;
  maxStreamCount?: number;
  startingChannelNumber?: number | null;
  formFile?: Blob | null;
  name?: string;
  urlSource?: string | null;
};
export type ChangeM3UFileNameRequest = {
  id?: number;
  name?: string;
};
export type DeleteM3UFileRequest = {
  deleteFile?: boolean;
  id?: number;
};
export type M3UFileDto = BaseFileDto & {
  startingChannelNumber: number;
  maxStreamCount: number;
  stationCount: number;
};
export type PagedResponseOfM3UFileDto = {
  data: M3UFileDto[];
  pageNumber: number;
  pageSize: number;
  totalPageCount: number;
  totalItemCount: number;
  first: number;
};
export type ProcessM3UFileRequest = {
  id?: number;
};
export type RefreshM3UFileRequest = {
  id?: number;
};
export type UpdateM3UFileRequest = BaseFileRequest & {
  maxStreamCount?: number | null;
  startingChannelNumber?: number | null;
};
export type ImageDownloadServiceStatus = {
  id?: number;
  totalDownloadAttempts?: number;
  totalInQueue?: number;
  totalSuccessful?: number;
  totalAlreadyExists?: number;
  totalNoArt?: number;
  totalErrors?: number;
};
export type TaskQueueStatus = {
  command?: string;
  id?: number;
  isRunning?: boolean;
  queueTS?: string;
  startTS?: string;
  stopTS?: string;
  elapsedTS?: string;
};
export type M3USettings = {
  m3UFieldChannelId?: boolean;
  m3UFieldChannelNumber?: boolean;
  m3UFieldCUID?: boolean;
  m3UFieldGroupTitle?: boolean;
  m3UFieldTvgChno?: boolean;
  m3UFieldTvgId?: boolean;
  m3UFieldTvgLogo?: boolean;
  m3UFieldTvgName?: boolean;
  m3UIgnoreEmptyEPGID?: boolean;
};
export type SdSettings = {
  seriesPosterArt?: boolean;
  seriesWsArt?: boolean;
  seriesPosterAspect?: string;
  artworkSize?: string;
  excludeCastAndCrew?: boolean;
  alternateSEFormat?: boolean;
  prefixEpisodeDescription?: boolean;
  prefixEpisodeTitle?: boolean;
  appendEpisodeDesc?: boolean;
  sdepgDays?: number;
  sdEnabled?: boolean;
  sdUserName?: string;
  sdCountry?: string;
  sdPassword?: string;
  sdPostalCode?: string;
  preferredLogoStyle?: string;
  alternateLogoStyle?: string;
  sdStationIds?: StationIdLineup[];
  seasonEventImages?: boolean;
  xmltvAddFillerData?: boolean;
  xmltvFillerProgramDescription?: string;
  xmltvFillerProgramLength?: number;
  xmltvIncludeChannelNumbers?: boolean;
  xmltvExtendedInfoInTitleDescriptions?: boolean;
  xmltvSingleImage?: boolean;
};
export type AuthenticationType = 0 | 2;
export type BaseSettings = M3USettings & {
  maxConcurrentDownloads?: number;
  sdSettings?: SdSettings;
  expectedServiceCount?: number;
  adminPassword?: string;
  adminUserName?: string;
  defaultIcon?: string;
  uiFolder?: string;
  urlBase?: string;
  logPerformance?: string[];
  apiKey?: string;
  authenticationMethod?: AuthenticationType;
  cacheIcons?: boolean;
  cleanURLs?: boolean;
  clientUserAgent?: string;
  deviceID?: string;
  dummyRegex?: string;
  ffMpegOptions?: string;
  enableSSL?: boolean;
  ffmPegExecutable?: string;
  ffProbeExecutable?: string;
  globalStreamLimit?: number;
  maxConnectRetry?: number;
  maxConnectRetryTimeMS?: number;
  overWriteM3UChannels?: boolean;
  preloadPercentage?: number;
  ringBufferSizeMB?: number;
  nameRegex?: string[];
  sslCertPassword?: string;
  sslCertPath?: string;
  streamingClientUserAgent?: string;
  streamingProxyType?: StreamingProxyTypes;
  videoStreamAlwaysUseEPGLogo?: boolean;
  showClientHostNames?: boolean;
};
export type SettingDto = BaseSettings & {
  release?: string;
  version?: string;
  ffmpegDefaultOptions?: string;
  isDebug?: boolean;
};
export type SdSystemStatus = {
  isSystemReady?: boolean;
};
export type LogInRequest = {
  password?: string;
  userName?: string;
};
export type SdSettingsRequest = {
  preferredLogoStyle?: string | null;
  alternateLogoStyle?: string | null;
  seriesPosterArt?: boolean | null;
  seriesWsArt?: boolean | null;
  seriesPosterAspect?: string | null;
  artworkSize?: string | null;
  excludeCastAndCrew?: boolean | null;
  alternateSEFormat?: boolean | null;
  prefixEpisodeDescription?: boolean | null;
  prefixEpisodeTitle?: boolean | null;
  appendEpisodeDesc?: boolean | null;
  sdepgDays?: number | null;
  sdEnabled?: boolean | null;
  sdUserName?: string | null;
  sdCountry?: string | null;
  sdPassword?: string | null;
  sdPostalCode?: string | null;
  sdStationIds?: StationIdLineup[] | null;
  seasonEventImages?: boolean | null;
  xmltvAddFillerData?: boolean | null;
  xmltvFillerProgramDescription?: string | null;
  xmltvFillerProgramLength?: number | null;
  xmltvIncludeChannelNumbers?: boolean | null;
  xmltvExtendedInfoInTitleDescriptions?: boolean | null;
  xmltvSingleImage?: boolean | null;
};
export type UpdateSettingRequest = {
  sdSettings?: SdSettingsRequest | null;
  showClientHostNames?: boolean | null;
  adminPassword?: string | null;
  adminUserName?: string | null;
  apiKey?: string | null;
  authenticationMethod?: AuthenticationType | null;
  cacheIcons?: boolean | null;
  cleanURLs?: boolean | null;
  clientUserAgent?: string | null;
  deviceID?: string | null;
  dummyRegex?: string | null;
  enableSSL?: boolean | null;
  ffmPegExecutable?: string | null;
  globalStreamLimit?: number | null;
  m3UFieldChannelId?: boolean | null;
  m3UFieldChannelNumber?: boolean | null;
  m3UFieldCUID?: boolean | null;
  m3UFieldGroupTitle?: boolean | null;
  m3UFieldTvgChno?: boolean | null;
  m3UFieldTvgId?: boolean | null;
  m3UFieldTvgLogo?: boolean | null;
  m3UFieldTvgName?: boolean | null;
  m3UIgnoreEmptyEPGID?: boolean | null;
  maxConnectRetry?: number | null;
  maxConnectRetryTimeMS?: number | null;
  overWriteM3UChannels?: boolean | null;
  preloadPercentage?: number | null;
  ringBufferSizeMB?: number | null;
  sourceBufferPreBufferPercentage?: number | null;
  sslCertPassword?: string | null;
  sslCertPath?: string | null;
  streamingClientUserAgent?: string | null;
  streamingProxyType?: StreamingProxyTypes | null;
  videoStreamAlwaysUseEPGLogo?: boolean | null;
  nameRegex?: string[] | null;
};
export type CreateStreamGroupRequest = {
  name: string;
};
export type DeleteStreamGroupRequest = {
  id?: number;
};
export type PagedResponseOfStreamGroupDto = {
  data: StreamGroupDto[];
  pageNumber: number;
  pageSize: number;
  totalPageCount: number;
  totalItemCount: number;
  first: number;
};
export type UpdateStreamGroupRequest = {
  streamGroupId?: number;
  name?: string | null;
  autoSetChannelNumbers?: boolean | null;
};
export const {
  useChannelGroupsCreateChannelGroupMutation,
  useChannelGroupsDeleteAllChannelGroupsFromParametersMutation,
  useChannelGroupsDeleteChannelGroupMutation,
  useChannelGroupsGetChannelGroupQuery,
  useChannelGroupsGetChannelGroupIdNamesQuery,
  useChannelGroupsGetPagedChannelGroupsQuery,
  useChannelGroupsUpdateChannelGroupMutation,
  useChannelGroupsUpdateChannelGroupsMutation,
  useChannelGroupsGetChannelGroupNamesQuery,
  useChannelGroupsGetChannelGroupsForStreamGroupQuery,
  useEpgFilesCreateEpgFileMutation,
  useEpgFilesCreateEpgFileFromFormMutation,
  useEpgFilesDeleteEpgFileMutation,
  useEpgFilesGetEpgColorsQuery,
  useEpgFilesGetEpgFileQuery,
  useEpgFilesGetEpgFilePreviewByIdQuery,
  useEpgFilesGetEpgNextEpgNumberQuery,
  useEpgFilesGetPagedEpgFilesQuery,
  useEpgFilesProcessEpgFileMutation,
  useEpgFilesRefreshEpgFileMutation,
  useEpgFilesScanDirectoryForEpgFilesMutation,
  useEpgFilesUpdateEpgFileMutation,
  useIconsAutoMatchIconToStreamsMutation,
  useIconsGetIconQuery,
  useIconsGetIconFromSourceQuery,
  useIconsGetPagedIconsQuery,
  useIconsGetIconsSimpleQueryQuery,
  useLogsGetLogQuery,
  useProgrammesGetProgrammeQuery,
  useProgrammesGetProgrammesQuery,
  useSchedulesDirectAddLineupMutation,
  useSchedulesDirectAddStationMutation,
  useSchedulesDirectGetAvailableCountriesQuery,
  useSchedulesDirectGetChannelNamesQuery,
  useSchedulesDirectGetHeadendsQuery,
  useSchedulesDirectGetLineupPreviewChannelQuery,
  useSchedulesDirectGetLineupsQuery,
  useSchedulesDirectGetPagedStationChannelNameSelectionsQuery,
  useSchedulesDirectGetSelectedStationIdsQuery,
  useSchedulesDirectGetStationChannelMapsQuery,
  useSchedulesDirectGetStationChannelNameFromDisplayNameQuery,
  useSchedulesDirectGetStationChannelNamesSimpleQueryQuery,
  useSchedulesDirectGetStationPreviewsQuery,
  useSchedulesDirectGetUserStatusQuery,
  useSchedulesDirectRemoveLineupMutation,
  useSchedulesDirectRemoveStationMutation,
  useStreamGroupChannelGroupSyncStreamGroupChannelGroupsMutation,
  useStreamGroupChannelGroupGetChannelGroupsFromStreamGroupQuery,
  useStreamGroupVideoStreamsGetStreamGroupVideoStreamIdsQuery,
  useStreamGroupVideoStreamsGetPagedStreamGroupVideoStreamsQuery,
  useStreamGroupVideoStreamsSetVideoStreamRanksMutation,
  useStreamGroupVideoStreamsSyncVideoStreamToStreamGroupPostMutation,
  useStreamGroupVideoStreamsSyncVideoStreamToStreamGroupDeleteMutation,
  useStreamGroupVideoStreamsSetStreamGroupVideoStreamChannelNumbersMutation,
  useTestEpgSyncMutation,
  useVideoStreamLinksAddVideoStreamToVideoStreamMutation,
  useVideoStreamLinksGetVideoStreamVideoStreamIdsQuery,
  useVideoStreamLinksGetPagedVideoStreamVideoStreamsQuery,
  useVideoStreamLinksRemoveVideoStreamFromVideoStreamMutation,
  useVideoStreamsCreateVideoStreamMutation,
  useVideoStreamsChangeVideoStreamChannelMutation,
  useVideoStreamsDeleteVideoStreamMutation,
  useVideoStreamsFailClientMutation,
  useVideoStreamsGetAllStatisticsForAllUrlsQuery,
  useVideoStreamsGetVideoStreamQuery,
  useVideoStreamsGetVideoStreamNamesQuery,
  useVideoStreamsGetPagedVideoStreamsQuery,
  useVideoStreamsReadAndWriteMutation,
  useVideoStreamsGetVideoStreamStreamGetQuery,
  useVideoStreamsGetVideoStreamStreamHeadMutation,
  useVideoStreamsGetVideoStreamStreamGet2Query,
  useVideoStreamsGetVideoStreamStreamHead2Mutation,
  useVideoStreamsGetVideoStreamStreamGet3Query,
  useVideoStreamsGetVideoStreamStreamHead3Mutation,
  useVideoStreamsReSetVideoStreamsLogoMutation,
  useVideoStreamsSetVideoStreamChannelNumbersMutation,
  useVideoStreamsSetVideoStreamsLogoFromEpgMutation,
  useVideoStreamsUpdateVideoStreamMutation,
  useVideoStreamsUpdateVideoStreamsMutation,
  useVideoStreamsUpdateAllVideoStreamsFromParametersMutation,
  useVideoStreamsDeleteAllVideoStreamsFromParametersMutation,
  useVideoStreamsSetVideoStreamChannelNumbersFromParametersMutation,
  useVideoStreamsSetVideoStreamsLogoFromEpgFromParametersMutation,
  useVideoStreamsReSetVideoStreamsLogoFromParametersMutation,
  useVideoStreamsSimulateStreamFailureForAllMutation,
  useVideoStreamsSimulateStreamFailureMutation,
  useVideoStreamsAutoSetEpgMutation,
  useVideoStreamsAutoSetEpgFromParametersMutation,
  useVideoStreamsSetVideoStreamTimeShiftsMutation,
  useVideoStreamsSetVideoStreamTimeShiftFromParametersMutation,
  useVideoStreamsGetVideoStreamInfoFromIdQuery,
  useVideoStreamsGetVideoStreamInfoFromUrlQuery,
  useFilesGetFileQuery,
  useM3UFilesCreateM3UFileMutation,
  useM3UFilesCreateM3UFileFromFormMutation,
  useM3UFilesChangeM3UFileNameMutation,
  useM3UFilesDeleteM3UFileMutation,
  useM3UFilesGetM3UFileQuery,
  useM3UFilesGetPagedM3UFilesQuery,
  useM3UFilesProcessM3UFileMutation,
  useM3UFilesRefreshM3UFileMutation,
  useM3UFilesScanDirectoryForM3UFilesMutation,
  useM3UFilesUpdateM3UFileMutation,
  useM3UFilesGetM3UFileNamesQuery,
  useMiscGetDownloadServiceStatusQuery,
  useMiscGetTestM3UQuery,
  useQueueGetQueueStatusQuery,
  useSettingsGetIsSystemReadyQuery,
  useSettingsGetSettingQuery,
  useSettingsGetSystemStatusQuery,
  useSettingsLogInQuery,
  useSettingsUpdateSettingMutation,
  useStreamGroupsCreateStreamGroupMutation,
  useStreamGroupsDeleteStreamGroupMutation,
  useStreamGroupsGetStreamGroupQuery,
  useStreamGroupsGetStreamGroupCapabilityQuery,
  useStreamGroupsGetStreamGroupCapability2Query,
  useStreamGroupsGetStreamGroupCapability3Query,
  useStreamGroupsGetStreamGroupDiscoverQuery,
  useStreamGroupsGetStreamGroupEpgQuery,
  useStreamGroupsGetStreamGroupLineupQuery,
  useStreamGroupsGetStreamGroupLineupStatusQuery,
  useStreamGroupsGetStreamGroupM3UQuery,
  useStreamGroupsGetPagedStreamGroupsQuery,
  useStreamGroupsUpdateStreamGroupMutation,
  useStreamGroupsGetStreamGroupVideoStreamUrlQuery
} = injectedRtkApi;
