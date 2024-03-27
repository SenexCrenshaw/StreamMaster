import { emptySplitApi as api } from './redux/emptyApi';
export const addTagTypes = [
  'ChannelGroups',
  'EPGFiles',
  'Logs',
  'Programmes',
  'StreamGroupChannelGroup',
  'StreamGroupVideoStreams',
  'Test',
  'Files',
  'Icons',
  'M3UFiles',
  'Misc',
  'Profiles',
  'Queue',
  'SchedulesDirect',
  'Settings',
  'SMStreams',
  'Statistics',
  'Stream',
  'StreamGroups',
  'V',
  'VideoStreamLinks',
  'VideoStreams'
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
      filesGetFile: build.query<FilesGetFileApiResponse, FilesGetFileApiArg>({
        query: (queryArg) => ({ url: `/api/files/${queryArg.filetype}/${queryArg.source}` }),
        providesTags: ['Files']
      }),
      iconsAutoMatchIconToStreams: build.mutation<IconsAutoMatchIconToStreamsApiResponse, IconsAutoMatchIconToStreamsApiArg>({
        query: (queryArg) => ({ url: `/api/icons/automatchicontostreams`, method: 'POST', body: queryArg }),
        invalidatesTags: ['Icons']
      }),
      iconsGetIconFromSource: build.query<IconsGetIconFromSourceApiResponse, IconsGetIconFromSourceApiArg>({
        query: (queryArg) => ({ url: `/api/icons/geticonfromsource`, params: { value: queryArg } }),
        providesTags: ['Icons']
      }),
      iconsReadDirectoryLogos: build.query<IconsReadDirectoryLogosApiResponse, IconsReadDirectoryLogosApiArg>({
        query: () => ({ url: `/api/icons/readdirectorylogos` }),
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
      iconsGetIcons: build.query<IconsGetIconsApiResponse, IconsGetIconsApiArg>({
        query: () => ({ url: `/api/icons/geticons` }),
        providesTags: ['Icons']
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
      miscBackup: build.mutation<MiscBackupApiResponse, MiscBackupApiArg>({
        query: () => ({ url: `/api/misc/backup`, method: 'PUT' }),
        invalidatesTags: ['Misc']
      }),
      profilesGetFfmpegProfiles: build.query<ProfilesGetFfmpegProfilesApiResponse, ProfilesGetFfmpegProfilesApiArg>({
        query: () => ({ url: `/api/profiles/getffmpegprofiles` }),
        providesTags: ['Profiles']
      }),
      profilesAddFfmpegProfile: build.mutation<ProfilesAddFfmpegProfileApiResponse, ProfilesAddFfmpegProfileApiArg>({
        query: (queryArg) => ({ url: `/api/profiles/addffmpegprofile`, method: 'PUT', body: queryArg }),
        invalidatesTags: ['Profiles']
      }),
      profilesRemoveFfmpegProfile: build.mutation<ProfilesRemoveFfmpegProfileApiResponse, ProfilesRemoveFfmpegProfileApiArg>({
        query: (queryArg) => ({ url: `/api/profiles/removeffmpegprofile`, method: 'DELETE', body: queryArg }),
        invalidatesTags: ['Profiles']
      }),
      profilesUpdateFfmpegProfile: build.mutation<ProfilesUpdateFfmpegProfileApiResponse, ProfilesUpdateFfmpegProfileApiArg>({
        query: (queryArg) => ({ url: `/api/profiles/updateffmpegprofile`, method: 'PATCH', body: queryArg }),
        invalidatesTags: ['Profiles']
      }),
      queueGetQueueStatus: build.query<QueueGetQueueStatusApiResponse, QueueGetQueueStatusApiArg>({
        query: () => ({ url: `/api/queue/getqueuestatus` }),
        providesTags: ['Queue']
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
      schedulesDirectGetStationChannelNames: build.query<SchedulesDirectGetStationChannelNamesApiResponse, SchedulesDirectGetStationChannelNamesApiArg>({
        query: () => ({ url: `/api/schedulesdirect/getstationchannelnames` }),
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
      smStreamsGetPagedSmStreams: build.query<SmStreamsGetPagedSmStreamsApiResponse, SmStreamsGetPagedSmStreamsApiArg>({
        query: (queryArg) => ({
          url: `/api/smstreams/getpagedsmstreams`,
          params: {
            PageNumber: queryArg.pageNumber,
            PageSize: queryArg.pageSize,
            OrderBy: queryArg.orderBy,
            JSONArgumentString: queryArg.jsonArgumentString,
            JSONFiltersString: queryArg.jsonFiltersString
          }
        }),
        providesTags: ['SMStreams']
      }),
      smStreamsToggleSmStreamVisible: build.query<SmStreamsToggleSmStreamVisibleApiResponse, SmStreamsToggleSmStreamVisibleApiArg>({
        query: (queryArg) => ({ url: `/api/smstreams/togglesmstreamvisible`, body: queryArg }),
        providesTags: ['SMStreams']
      }),
      statisticsGetClientStatistics: build.query<StatisticsGetClientStatisticsApiResponse, StatisticsGetClientStatisticsApiArg>({
        query: () => ({ url: `/api/statistics/getclientstatistics` }),
        providesTags: ['Statistics']
      }),
      statisticsGetInputStatistics: build.query<StatisticsGetInputStatisticsApiResponse, StatisticsGetInputStatisticsApiArg>({
        query: () => ({ url: `/api/statistics/getinputstatistics` }),
        providesTags: ['Statistics']
      }),
      streamGetM3U8Get: build.query<StreamGetM3U8GetApiResponse, StreamGetM3U8GetApiArg>({
        query: (queryArg) => ({ url: `/api/stream/${queryArg}.m3u8` }),
        providesTags: ['Stream']
      }),
      streamGetM3U8Head: build.mutation<StreamGetM3U8HeadApiResponse, StreamGetM3U8HeadApiArg>({
        query: (queryArg) => ({ url: `/api/stream/${queryArg}.m3u8`, method: 'HEAD' }),
        invalidatesTags: ['Stream']
      }),
      streamGetVideoStreamGet: build.query<StreamGetVideoStreamGetApiResponse, StreamGetVideoStreamGetApiArg>({
        query: (queryArg) => ({ url: `/api/stream/${queryArg.videoStreamId}/${queryArg.num}.ts` }),
        providesTags: ['Stream']
      }),
      streamGetVideoStreamHead: build.mutation<StreamGetVideoStreamHeadApiResponse, StreamGetVideoStreamHeadApiArg>({
        query: (queryArg) => ({ url: `/api/stream/${queryArg.videoStreamId}/${queryArg.num}.ts`, method: 'HEAD' }),
        invalidatesTags: ['Stream']
      }),
      streamGetVideoStreamMp4Get: build.query<StreamGetVideoStreamMp4GetApiResponse, StreamGetVideoStreamMp4GetApiArg>({
        query: (queryArg) => ({ url: `/api/stream/${queryArg}.mp4` }),
        providesTags: ['Stream']
      }),
      streamGetVideoStreamMp4Head: build.mutation<StreamGetVideoStreamMp4HeadApiResponse, StreamGetVideoStreamMp4HeadApiArg>({
        query: (queryArg) => ({ url: `/api/stream/${queryArg}.mp4`, method: 'HEAD' }),
        invalidatesTags: ['Stream']
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
      streamGroupsGetVideoStreamStreamFromAutoGet: build.query<
        StreamGroupsGetVideoStreamStreamFromAutoGetApiResponse,
        StreamGroupsGetVideoStreamStreamFromAutoGetApiArg
      >({
        query: (queryArg) => ({ url: `/api/streamgroups/${queryArg.encodedId}/auto/v${queryArg.channelId}` }),
        providesTags: ['StreamGroups']
      }),
      streamGroupsGetVideoStreamStreamFromAutoHead: build.mutation<
        StreamGroupsGetVideoStreamStreamFromAutoHeadApiResponse,
        StreamGroupsGetVideoStreamStreamFromAutoHeadApiArg
      >({
        query: (queryArg) => ({ url: `/api/streamgroups/${queryArg.encodedId}/auto/v${queryArg.channelId}`, method: 'HEAD' }),
        invalidatesTags: ['StreamGroups']
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
      }),
      vGetVideoStreamStreamGet: build.query<VGetVideoStreamStreamGetApiResponse, VGetVideoStreamStreamGetApiArg>({
        query: (queryArg) => ({ url: `/v/v/${queryArg}` }),
        providesTags: ['V']
      }),
      vGetVideoStreamStreamHead: build.mutation<VGetVideoStreamStreamHeadApiResponse, VGetVideoStreamStreamHeadApiArg>({
        query: (queryArg) => ({ url: `/v/v/${queryArg}`, method: 'HEAD' }),
        invalidatesTags: ['V']
      }),
      vGetVideoStreamStreamGet2: build.query<VGetVideoStreamStreamGet2ApiResponse, VGetVideoStreamStreamGet2ApiArg>({
        query: (queryArg) => ({ url: `/v/v/${queryArg}.ts` }),
        providesTags: ['V']
      }),
      vGetVideoStreamStreamHead2: build.mutation<VGetVideoStreamStreamHead2ApiResponse, VGetVideoStreamStreamHead2ApiArg>({
        query: (queryArg) => ({ url: `/v/v/${queryArg}.ts`, method: 'HEAD' }),
        invalidatesTags: ['V']
      }),
      vGetStreamGroupM3U: build.query<VGetStreamGroupM3UApiResponse, VGetStreamGroupM3UApiArg>({
        query: (queryArg) => ({ url: `/v/s/${queryArg}.m3u` }),
        providesTags: ['V']
      }),
      vGetStreamGroupEpg: build.query<VGetStreamGroupEpgApiResponse, VGetStreamGroupEpgApiArg>({
        query: (queryArg) => ({ url: `/v/s/${queryArg}.xml` }),
        providesTags: ['V']
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
        query: (queryArg) => ({ url: `/api/videostreams/stream/${queryArg}.ts` }),
        providesTags: ['VideoStreams']
      }),
      videoStreamsGetVideoStreamStreamHead3: build.mutation<VideoStreamsGetVideoStreamStreamHead3ApiResponse, VideoStreamsGetVideoStreamStreamHead3ApiArg>({
        query: (queryArg) => ({ url: `/api/videostreams/stream/${queryArg}.ts`, method: 'HEAD' }),
        invalidatesTags: ['VideoStreams']
      }),
      videoStreamsGetVideoStreamStreamGet4: build.query<VideoStreamsGetVideoStreamStreamGet4ApiResponse, VideoStreamsGetVideoStreamStreamGet4ApiArg>({
        query: (queryArg) => ({ url: `/api/videostreams/stream/${queryArg.encodedIds}/${queryArg.name}` }),
        providesTags: ['VideoStreams']
      }),
      videoStreamsGetVideoStreamStreamHead4: build.mutation<VideoStreamsGetVideoStreamStreamHead4ApiResponse, VideoStreamsGetVideoStreamStreamHead4ApiArg>({
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
      videoStreamsGetVideoStreamNamesAndUrls: build.query<VideoStreamsGetVideoStreamNamesAndUrlsApiResponse, VideoStreamsGetVideoStreamNamesAndUrlsApiArg>({
        query: () => ({ url: `/api/videostreams/getvideostreamnamesandurls` }),
        providesTags: ['VideoStreams']
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
  FormFile?: Blob | null;
  Name?: string;
  FileName?: string;
  EPGNumber?: number;
  TimeShift?: number | null;
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
export type LogsGetLogApiResponse = /** status 200  */ LogEntryDto[];
export type LogsGetLogApiArg = {
  lastId?: number;
  maxLines?: number;
};
export type ProgrammesGetProgrammeApiResponse = /** status 200  */ XmltvProgramme[];
export type ProgrammesGetProgrammeApiArg = string;
export type ProgrammesGetProgrammesApiResponse = /** status 200  */ XmltvProgramme[];
export type ProgrammesGetProgrammesApiArg = void;
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
export type FilesGetFileApiResponse = unknown;
export type FilesGetFileApiArg = {
  source: string;
  filetype: SmFileTypes;
};
export type IconsAutoMatchIconToStreamsApiResponse = unknown;
export type IconsAutoMatchIconToStreamsApiArg = AutoMatchIconToStreamsRequest;
export type IconsGetIconFromSourceApiResponse = /** status 200  */ IconFileDto;
export type IconsGetIconFromSourceApiArg = string;
export type IconsReadDirectoryLogosApiResponse = unknown;
export type IconsReadDirectoryLogosApiArg = void;
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
export type IconsGetIconsApiResponse = /** status 200  */ IconFileDto[];
export type IconsGetIconsApiArg = void;
export type M3UFilesCreateM3UFileApiResponse = unknown;
export type M3UFilesCreateM3UFileApiArg = CreateM3UFileRequest;
export type M3UFilesCreateM3UFileFromFormApiResponse = unknown;
export type M3UFilesCreateM3UFileFromFormApiArg = {
  Description?: string | null;
  MaxStreamCount?: number;
  OverWriteChannels?: boolean | null;
  StartingChannelNumber?: number | null;
  FormFile?: Blob | null;
  Name?: string;
  UrlSource?: string | null;
  VODTags?: string[] | null;
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
export type MiscBackupApiResponse = unknown;
export type MiscBackupApiArg = void;
export type ProfilesGetFfmpegProfilesApiResponse = /** status 200  */ FfmpegProfileDtos;
export type ProfilesGetFfmpegProfilesApiArg = void;
export type ProfilesAddFfmpegProfileApiResponse = /** status 200  */ UpdateSettingResponse;
export type ProfilesAddFfmpegProfileApiArg = AddFfmpegProfileRequest;
export type ProfilesRemoveFfmpegProfileApiResponse = /** status 200  */ UpdateSettingResponse;
export type ProfilesRemoveFfmpegProfileApiArg = RemoveFfmpegProfileRequest;
export type ProfilesUpdateFfmpegProfileApiResponse = /** status 200  */ UpdateSettingResponse;
export type ProfilesUpdateFfmpegProfileApiArg = UpdateFfmpegProfileRequest;
export type QueueGetQueueStatusApiResponse = /** status 200  */ TaskQueueStatus[];
export type QueueGetQueueStatusApiArg = void;
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
export type SchedulesDirectGetStationChannelNamesApiResponse = /** status 200  */ StationChannelName[];
export type SchedulesDirectGetStationChannelNamesApiArg = void;
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
export type SmStreamsGetPagedSmStreamsApiResponse = /** status 200  */ PagedResponseOfSmStreamDto;
export type SmStreamsGetPagedSmStreamsApiArg = {
  pageNumber?: number;
  pageSize?: number;
  orderBy?: string;
  jsonArgumentString?: string | null;
  jsonFiltersString?: string | null;
};
export type SmStreamsToggleSmStreamVisibleApiResponse = /** status 200  */ boolean;
export type SmStreamsToggleSmStreamVisibleApiArg = ToggleSmStreamVisibleRequest;
export type StatisticsGetClientStatisticsApiResponse = /** status 200  */ ClientStreamingStatistics[];
export type StatisticsGetClientStatisticsApiArg = void;
export type StatisticsGetInputStatisticsApiResponse = /** status 200  */ InputStreamingStatistics[];
export type StatisticsGetInputStatisticsApiArg = void;
export type StreamGetM3U8GetApiResponse = unknown;
export type StreamGetM3U8GetApiArg = string;
export type StreamGetM3U8HeadApiResponse = unknown;
export type StreamGetM3U8HeadApiArg = string;
export type StreamGetVideoStreamGetApiResponse = unknown;
export type StreamGetVideoStreamGetApiArg = {
  videoStreamId: string;
  num: number;
};
export type StreamGetVideoStreamHeadApiResponse = unknown;
export type StreamGetVideoStreamHeadApiArg = {
  videoStreamId: string;
  num: number;
};
export type StreamGetVideoStreamMp4GetApiResponse = unknown;
export type StreamGetVideoStreamMp4GetApiArg = string;
export type StreamGetVideoStreamMp4HeadApiResponse = unknown;
export type StreamGetVideoStreamMp4HeadApiArg = string;
export type StreamGroupsCreateStreamGroupApiResponse = unknown;
export type StreamGroupsCreateStreamGroupApiArg = CreateStreamGroupRequest;
export type StreamGroupsDeleteStreamGroupApiResponse = unknown;
export type StreamGroupsDeleteStreamGroupApiArg = DeleteStreamGroupRequest;
export type StreamGroupsGetStreamGroupApiResponse = /** status 200  */ StreamGroupDto;
export type StreamGroupsGetStreamGroupApiArg = number;
export type StreamGroupsGetVideoStreamStreamFromAutoGetApiResponse = unknown;
export type StreamGroupsGetVideoStreamStreamFromAutoGetApiArg = {
  encodedId: string;
  channelId: string;
};
export type StreamGroupsGetVideoStreamStreamFromAutoHeadApiResponse = unknown;
export type StreamGroupsGetVideoStreamStreamFromAutoHeadApiArg = {
  encodedId: string;
  channelId: string;
};
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
export type VGetVideoStreamStreamGetApiResponse = unknown;
export type VGetVideoStreamStreamGetApiArg = string;
export type VGetVideoStreamStreamHeadApiResponse = unknown;
export type VGetVideoStreamStreamHeadApiArg = string;
export type VGetVideoStreamStreamGet2ApiResponse = unknown;
export type VGetVideoStreamStreamGet2ApiArg = string;
export type VGetVideoStreamStreamHead2ApiResponse = unknown;
export type VGetVideoStreamStreamHead2ApiArg = string;
export type VGetStreamGroupM3UApiResponse = unknown;
export type VGetStreamGroupM3UApiArg = string;
export type VGetStreamGroupEpgApiResponse = unknown;
export type VGetStreamGroupEpgApiArg = string;
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
export type VideoStreamsGetVideoStreamStreamGet3ApiArg = string;
export type VideoStreamsGetVideoStreamStreamHead3ApiResponse = unknown;
export type VideoStreamsGetVideoStreamStreamHead3ApiArg = string;
export type VideoStreamsGetVideoStreamStreamGet4ApiResponse = unknown;
export type VideoStreamsGetVideoStreamStreamGet4ApiArg = {
  encodedIds: string;
  name: string;
};
export type VideoStreamsGetVideoStreamStreamHead4ApiResponse = unknown;
export type VideoStreamsGetVideoStreamStreamHead4ApiArg = {
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
export type VideoStreamsGetVideoStreamNamesAndUrlsApiResponse = /** status 200  */ IdNameUrl[];
export type VideoStreamsGetVideoStreamNamesAndUrlsApiArg = void;
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
  formFile?: Blob | null;
  name?: string;
  fileName?: string;
  epgNumber?: number;
  timeShift?: number | null;
  urlSource?: string | null;
  color?: string | null;
};
export type DeleteEpgFileRequest = {
  deleteFile?: boolean;
  id?: number;
};
export type EpgColorDto = {
  id?: number;
  epgNumber?: number;
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
  timeShift: number;
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
  timeShift?: number | null;
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
  episodeNums?: XmltvEpisodeNum[] | null;
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
export type StreamGroupDto = {
  ffmpegProfileId: string;
  isLoading: boolean;
  hdhrLink: string;
  isReadOnly: boolean;
  autoSetChannelNumbers: boolean;
  streamCount: number;
  id: number;
  m3ULink: string;
  name: string;
  xmlLink: string;
  shortM3ULink: string;
  shortEPGLink: string;
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
  shortId: string;
  isHidden: boolean;
  isUserCreated: boolean;
  m3UFileId: number;
  m3UFileName: string;
  streamingProxyType: StreamingProxyTypes;
  groupTitle: string;
  tvg_chno: number;
  tvg_group: string;
  timeShift: number;
  tvg_ID: string;
  stationId: string;
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
  isReadOnly: boolean;
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
export type SmFileTypes = 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10 | 11;
export type AutoMatchIconToStreamsRequest = {
  ids?: string[];
};
export type IconFileDto = {
  id: string;
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
export type CreateM3UFileRequest = {
  description?: string | null;
  maxStreamCount?: number;
  overWriteChannels?: boolean | null;
  startingChannelNumber?: number | null;
  formFile?: Blob | null;
  name?: string;
  urlSource?: string | null;
  vodTags?: string[] | null;
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
  vodTags: string[];
  overwriteChannelNumbers: boolean;
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
  overWriteChannels?: boolean | null;
  forceRun?: boolean;
};
export type RefreshM3UFileRequest = {
  id?: number;
  forceRun?: boolean;
};
export type UpdateM3UFileRequest = BaseFileRequest & {
  maxStreamCount?: number | null;
  startingChannelNumber?: number | null;
  overWriteChannels?: boolean | null;
  vodTags?: string[] | null;
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
export type FfmpegProfile = {
  parameters: string;
  timeout: number;
  isM3U8: boolean;
};
export type FfmpegProfileDto = FfmpegProfile & {
  name: string;
};
export type FfmpegProfileDtos = FfmpegProfileDto[];
export type AuthenticationType = 0 | 2;
export type BaseSettings = {
  m3UFieldGroupTitle?: boolean;
  m3UIgnoreEmptyEPGID?: boolean;
  m3UUseChnoForId?: boolean;
  m3UUseCUIDForChannelID?: boolean;
  m3UStationId?: boolean;
  backupEnabled?: boolean;
  backupVersionsToKeep?: number;
  backupInterval?: number;
  prettyEPG?: boolean;
  maxLogFiles?: number;
  maxLogFileSizeMB?: number;
  enablePrometheus?: boolean;
  maxStreamReStart?: number;
  maxConcurrentDownloads?: number;
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
  nameRegex?: string[];
  sslCertPassword?: string;
  sslCertPath?: string;
  streamingClientUserAgent?: string;
  streamingProxyType?: StreamingProxyTypes;
  videoStreamAlwaysUseEPGLogo?: boolean;
  showClientHostNames?: boolean;
};
export type StationIdLineup = {
  lineup?: string;
  stationId?: string;
  id?: string;
};
export type SdSettings = {
  alternateSEFormat?: boolean;
  alternateLogoStyle?: string;
  appendEpisodeDesc?: boolean;
  artworkSize?: string;
  excludeCastAndCrew?: boolean;
  preferredLogoStyle?: string;
  prefixEpisodeDescription?: boolean;
  prefixEpisodeTitle?: boolean;
  sdEnabled?: boolean;
  sdepgDays?: number;
  sdCountry?: string;
  sdPassword?: string;
  sdPostalCode?: string;
  sdStationIds?: StationIdLineup[];
  sdUserName?: string;
  seasonEventImages?: boolean;
  seriesPosterArt?: boolean;
  seriesPosterAspect?: string;
  seriesWsArt?: boolean;
  xmltvAddFillerData?: boolean;
  xmltvFillerProgramLength?: number;
  xmltvExtendedInfoInTitleDescriptions?: boolean;
  xmltvIncludeChannelNumbers?: boolean;
  xmltvSingleImage?: boolean;
};
export type HlsSettings = {
  hlsM3U8Enable?: boolean;
  hlsffmpegOptions?: string;
  hlsReconnectDurationInSeconds?: number;
  hlsSegmentDurationInSeconds?: number;
  hlsSegmentCount?: number;
  hlsM3U8CreationTimeOutInSeconds?: number;
  hlsM3U8ReadTimeOutInSeconds?: number;
  hlstsReadTimeOutInSeconds?: number;
};
export type SettingDto = BaseSettings & {
  sdSettings?: SdSettings;
  hls?: HlsSettings;
  release?: string;
  version?: string;
  ffmpegDefaultOptions?: string;
  isDebug?: boolean;
};
export type UpdateSettingResponse = {
  needsLogOut?: boolean;
  settings?: SettingDto;
};
export type AddFfmpegProfileRequest = {
  name?: string;
  parameters?: string;
  timeOut?: number;
  isM3U8?: boolean;
};
export type RemoveFfmpegProfileRequest = {
  name?: string;
};
export type UpdateFfmpegProfileRequest = {
  name?: string;
  newName?: string | null;
  parameters?: string | null;
  timeOut?: number | null;
  isM3U8?: boolean | null;
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
  id?: string;
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
  xmltvFillerProgramLength?: number | null;
  xmltvIncludeChannelNumbers?: boolean | null;
  xmltvExtendedInfoInTitleDescriptions?: boolean | null;
  xmltvSingleImage?: boolean | null;
};
export type UpdateSettingRequest = {
  backupEnabled?: boolean | null;
  backupVersionsToKeep?: number | null;
  backupInterval?: number | null;
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
  ffMpegOptions?: string | null;
  globalStreamLimit?: number | null;
  m3UFieldGroupTitle?: boolean | null;
  m3UStationId?: boolean | null;
  m3UUseChnoForId?: boolean | null;
  m3UIgnoreEmptyEPGID?: boolean | null;
  m3UUseCUIDForChannelID?: boolean | null;
  prettyEPG?: boolean | null;
  maxConnectRetry?: number | null;
  maxConnectRetryTimeMS?: number | null;
  sslCertPassword?: string | null;
  sslCertPath?: string | null;
  streamingClientUserAgent?: string | null;
  streamingProxyType?: StreamingProxyTypes | null;
  videoStreamAlwaysUseEPGLogo?: boolean | null;
  enablePrometheus?: boolean | null;
  maxLogFiles?: number | null;
  maxLogFileSizeMB?: number | null;
  nameRegex?: string[] | null;
};
export type SmStream = {
  id?: string;
  filePosition?: number;
  isHidden?: boolean;
  isUserCreated?: boolean;
  m3UFileId?: number;
  tvg_chno?: number;
  m3UFileName?: string;
  shortId?: string;
  group?: string;
  epgid?: string;
  logo?: string;
  name?: string;
  url?: string;
  stationId?: string;
};
export type SMStreamDto = SmStream & {
  realUrl?: string;
};
export type PagedResponseOfSmStreamDto = {
  data: SMStreamDto[];
  pageNumber: number;
  pageSize: number;
  totalPageCount: number;
  totalItemCount: number;
  first: number;
};
export type ToggleSmStreamVisibleRequest = {
  id?: string;
};
export type ClientStreamingStatistics = {
  readBitsPerSecond?: number;
  bytesRead?: number;
  channelName?: string;
  videoStreamName?: string;
  clientId?: string;
  clientAgent?: string;
  clientIPAddress?: string;
  elapsedTime?: string;
  startTime?: string;
};
export type InputStreamingStatistics = {
  bitsPerSecond?: number;
  rank?: number;
  streamUrl?: string | null;
  bytesRead?: number;
  bytesWritten?: number;
  elapsedTime?: string;
  startTime?: string;
  id?: string;
  channelName?: string;
  channelId?: string;
  logo?: string | null;
  clients?: number;
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
  ffmpegProfileId?: string | null;
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
  stationId?: string | null;
  streamingProxyType?: StreamingProxyTypes | null;
  toggleVisibility?: boolean | null;
  groupTitle?: string | null;
  tvg_chno?: number | null;
  tvg_group?: string | null;
  timeShift?: number | null;
  tvg_ID?: string | null;
  tvg_logo?: string | null;
  tvg_name?: string | null;
  url?: string | null;
  childVideoStreams?: VideoStreamDto[] | null;
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
  timeShift: number;
};
export type SetVideoStreamTimeShiftFromParametersRequest = {
  parameters?: VideoStreamParameters;
  timeShift?: number;
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
export type IdNameUrl = {
  id: string;
  name: string;
  url: string;
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
  useLogsGetLogQuery,
  useProgrammesGetProgrammeQuery,
  useProgrammesGetProgrammesQuery,
  useStreamGroupChannelGroupSyncStreamGroupChannelGroupsMutation,
  useStreamGroupChannelGroupGetChannelGroupsFromStreamGroupQuery,
  useStreamGroupVideoStreamsGetStreamGroupVideoStreamIdsQuery,
  useStreamGroupVideoStreamsGetPagedStreamGroupVideoStreamsQuery,
  useStreamGroupVideoStreamsSetVideoStreamRanksMutation,
  useStreamGroupVideoStreamsSyncVideoStreamToStreamGroupPostMutation,
  useStreamGroupVideoStreamsSyncVideoStreamToStreamGroupDeleteMutation,
  useStreamGroupVideoStreamsSetStreamGroupVideoStreamChannelNumbersMutation,
  useTestEpgSyncMutation,
  useFilesGetFileQuery,
  useIconsAutoMatchIconToStreamsMutation,
  useIconsGetIconFromSourceQuery,
  useIconsReadDirectoryLogosQuery,
  useIconsGetPagedIconsQuery,
  useIconsGetIconsSimpleQueryQuery,
  useIconsGetIconsQuery,
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
  useMiscBackupMutation,
  useProfilesGetFfmpegProfilesQuery,
  useProfilesAddFfmpegProfileMutation,
  useProfilesRemoveFfmpegProfileMutation,
  useProfilesUpdateFfmpegProfileMutation,
  useQueueGetQueueStatusQuery,
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
  useSchedulesDirectGetStationChannelNamesQuery,
  useSchedulesDirectGetStationChannelNamesSimpleQueryQuery,
  useSchedulesDirectGetStationPreviewsQuery,
  useSchedulesDirectGetUserStatusQuery,
  useSchedulesDirectRemoveLineupMutation,
  useSchedulesDirectRemoveStationMutation,
  useSettingsGetIsSystemReadyQuery,
  useSettingsGetSettingQuery,
  useSettingsGetSystemStatusQuery,
  useSettingsLogInQuery,
  useSettingsUpdateSettingMutation,
  useSmStreamsGetPagedSmStreamsQuery,
  useSmStreamsToggleSmStreamVisibleQuery,
  useStatisticsGetClientStatisticsQuery,
  useStatisticsGetInputStatisticsQuery,
  useStreamGetM3U8GetQuery,
  useStreamGetM3U8HeadMutation,
  useStreamGetVideoStreamGetQuery,
  useStreamGetVideoStreamHeadMutation,
  useStreamGetVideoStreamMp4GetQuery,
  useStreamGetVideoStreamMp4HeadMutation,
  useStreamGroupsCreateStreamGroupMutation,
  useStreamGroupsDeleteStreamGroupMutation,
  useStreamGroupsGetStreamGroupQuery,
  useStreamGroupsGetVideoStreamStreamFromAutoGetQuery,
  useStreamGroupsGetVideoStreamStreamFromAutoHeadMutation,
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
  useStreamGroupsGetStreamGroupVideoStreamUrlQuery,
  useVGetVideoStreamStreamGetQuery,
  useVGetVideoStreamStreamHeadMutation,
  useVGetVideoStreamStreamGet2Query,
  useVGetVideoStreamStreamHead2Mutation,
  useVGetStreamGroupM3UQuery,
  useVGetStreamGroupEpgQuery,
  useVideoStreamLinksAddVideoStreamToVideoStreamMutation,
  useVideoStreamLinksGetVideoStreamVideoStreamIdsQuery,
  useVideoStreamLinksGetPagedVideoStreamVideoStreamsQuery,
  useVideoStreamLinksRemoveVideoStreamFromVideoStreamMutation,
  useVideoStreamsCreateVideoStreamMutation,
  useVideoStreamsChangeVideoStreamChannelMutation,
  useVideoStreamsDeleteVideoStreamMutation,
  useVideoStreamsFailClientMutation,
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
  useVideoStreamsGetVideoStreamStreamGet4Query,
  useVideoStreamsGetVideoStreamStreamHead4Mutation,
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
  useVideoStreamsGetVideoStreamNamesAndUrlsQuery
} = injectedRtkApi;
