import { emptySplitApi as api } from "./emptyApi";
export const addTagTypes = [
  "ChannelGroups",
  "EPGFiles",
  "Files",
  "Icons",
  "Logs",
  "M3UFiles",
  "Misc",
  "Programmes",
  "SchedulesDirect",
  "Settings",
  "StreamGroupChannelGroup",
  "StreamGroups",
  "StreamGroupVideoStreams",
  "VideoStreamLinks",
  "VideoStreams",
] as const;
const injectedRtkApi = api
  .enhanceEndpoints({
    addTagTypes,
  })
  .injectEndpoints({
    endpoints: (build) => ({
      channelGroupsCreateChannelGroup: build.mutation<
        ChannelGroupsCreateChannelGroupApiResponse,
        ChannelGroupsCreateChannelGroupApiArg
      >({
        query: (queryArg) => ({
          url: `/api/channelgroups`,
          method: "POST",
          body: queryArg,
        }),
        invalidatesTags: ["ChannelGroups"],
      }),
      channelGroupsDeleteAllChannelGroupsFromParameters: build.mutation<
        ChannelGroupsDeleteAllChannelGroupsFromParametersApiResponse,
        ChannelGroupsDeleteAllChannelGroupsFromParametersApiArg
      >({
        query: (queryArg) => ({
          url: `/api/channelgroups/deleteallchannelgroupsfromparameters`,
          method: "DELETE",
          body: queryArg,
        }),
        invalidatesTags: ["ChannelGroups"],
      }),
      channelGroupsDeleteChannelGroup: build.mutation<
        ChannelGroupsDeleteChannelGroupApiResponse,
        ChannelGroupsDeleteChannelGroupApiArg
      >({
        query: (queryArg) => ({
          url: `/api/channelgroups/deletechannelgroup`,
          method: "DELETE",
          body: queryArg,
        }),
        invalidatesTags: ["ChannelGroups"],
      }),
      channelGroupsGetChannelGroup: build.query<
        ChannelGroupsGetChannelGroupApiResponse,
        ChannelGroupsGetChannelGroupApiArg
      >({
        query: (queryArg) => ({ url: `/api/channelgroups/${queryArg}` }),
        providesTags: ["ChannelGroups"],
      }),
      channelGroupsGetChannelGroupIdNames: build.query<
        ChannelGroupsGetChannelGroupIdNamesApiResponse,
        ChannelGroupsGetChannelGroupIdNamesApiArg
      >({
        query: () => ({ url: `/api/channelgroups/getchannelgroupidnames` }),
        providesTags: ["ChannelGroups"],
      }),
      channelGroupsGetPagedChannelGroups: build.query<
        ChannelGroupsGetPagedChannelGroupsApiResponse,
        ChannelGroupsGetPagedChannelGroupsApiArg
      >({
        query: (queryArg) => ({
          url: `/api/channelgroups/getpagedchannelgroups`,
          params: {
            PageNumber: queryArg.pageNumber,
            PageSize: queryArg.pageSize,
            OrderBy: queryArg.orderBy,
            JSONArgumentString: queryArg.jsonArgumentString,
            JSONFiltersString: queryArg.jsonFiltersString,
          },
        }),
        providesTags: ["ChannelGroups"],
      }),
      channelGroupsUpdateChannelGroup: build.mutation<
        ChannelGroupsUpdateChannelGroupApiResponse,
        ChannelGroupsUpdateChannelGroupApiArg
      >({
        query: (queryArg) => ({
          url: `/api/channelgroups/updatechannelgroup`,
          method: "PATCH",
          body: queryArg,
        }),
        invalidatesTags: ["ChannelGroups"],
      }),
      channelGroupsUpdateChannelGroups: build.mutation<
        ChannelGroupsUpdateChannelGroupsApiResponse,
        ChannelGroupsUpdateChannelGroupsApiArg
      >({
        query: (queryArg) => ({
          url: `/api/channelgroups/updatechannelgroups`,
          method: "PATCH",
          body: queryArg,
        }),
        invalidatesTags: ["ChannelGroups"],
      }),
      channelGroupsGetChannelGroupNames: build.query<
        ChannelGroupsGetChannelGroupNamesApiResponse,
        ChannelGroupsGetChannelGroupNamesApiArg
      >({
        query: () => ({ url: `/api/channelgroups/getchannelgroupnames` }),
        providesTags: ["ChannelGroups"],
      }),
      channelGroupsGetChannelGroupsForStreamGroup: build.query<
        ChannelGroupsGetChannelGroupsForStreamGroupApiResponse,
        ChannelGroupsGetChannelGroupsForStreamGroupApiArg
      >({
        query: (queryArg) => ({
          url: `/api/channelgroups/getchannelgroupsforstreamgroup`,
          body: queryArg,
        }),
        providesTags: ["ChannelGroups"],
      }),
      epgFilesCreateEpgFile: build.mutation<
        EpgFilesCreateEpgFileApiResponse,
        EpgFilesCreateEpgFileApiArg
      >({
        query: (queryArg) => ({
          url: `/api/epgfiles/createepgfile`,
          method: "POST",
          body: queryArg,
        }),
        invalidatesTags: ["EPGFiles"],
      }),
      epgFilesCreateEpgFileFromForm: build.mutation<
        EpgFilesCreateEpgFileFromFormApiResponse,
        EpgFilesCreateEpgFileFromFormApiArg
      >({
        query: (queryArg) => ({
          url: `/api/epgfiles/createepgfilefromform`,
          method: "POST",
          body: queryArg,
        }),
        invalidatesTags: ["EPGFiles"],
      }),
      epgFilesDeleteEpgFile: build.mutation<
        EpgFilesDeleteEpgFileApiResponse,
        EpgFilesDeleteEpgFileApiArg
      >({
        query: (queryArg) => ({
          url: `/api/epgfiles/deleteepgfile`,
          method: "DELETE",
          body: queryArg,
        }),
        invalidatesTags: ["EPGFiles"],
      }),
      epgFilesGetEpgFile: build.query<
        EpgFilesGetEpgFileApiResponse,
        EpgFilesGetEpgFileApiArg
      >({
        query: (queryArg) => ({ url: `/api/epgfiles/${queryArg}` }),
        providesTags: ["EPGFiles"],
      }),
      epgFilesGetPagedEpgFiles: build.query<
        EpgFilesGetPagedEpgFilesApiResponse,
        EpgFilesGetPagedEpgFilesApiArg
      >({
        query: (queryArg) => ({
          url: `/api/epgfiles`,
          params: {
            PageNumber: queryArg.pageNumber,
            PageSize: queryArg.pageSize,
            OrderBy: queryArg.orderBy,
            JSONArgumentString: queryArg.jsonArgumentString,
            JSONFiltersString: queryArg.jsonFiltersString,
          },
        }),
        providesTags: ["EPGFiles"],
      }),
      epgFilesProcessEpgFile: build.mutation<
        EpgFilesProcessEpgFileApiResponse,
        EpgFilesProcessEpgFileApiArg
      >({
        query: (queryArg) => ({
          url: `/api/epgfiles/processepgfile`,
          method: "PATCH",
          body: queryArg,
        }),
        invalidatesTags: ["EPGFiles"],
      }),
      epgFilesRefreshEpgFile: build.mutation<
        EpgFilesRefreshEpgFileApiResponse,
        EpgFilesRefreshEpgFileApiArg
      >({
        query: (queryArg) => ({
          url: `/api/epgfiles/refreshepgfile`,
          method: "PATCH",
          body: queryArg,
        }),
        invalidatesTags: ["EPGFiles"],
      }),
      epgFilesScanDirectoryForEpgFiles: build.mutation<
        EpgFilesScanDirectoryForEpgFilesApiResponse,
        EpgFilesScanDirectoryForEpgFilesApiArg
      >({
        query: () => ({
          url: `/api/epgfiles/scandirectoryforepgfiles`,
          method: "PATCH",
        }),
        invalidatesTags: ["EPGFiles"],
      }),
      epgFilesUpdateEpgFile: build.mutation<
        EpgFilesUpdateEpgFileApiResponse,
        EpgFilesUpdateEpgFileApiArg
      >({
        query: (queryArg) => ({
          url: `/api/epgfiles/updateepgfile`,
          method: "PATCH",
          body: queryArg,
        }),
        invalidatesTags: ["EPGFiles"],
      }),
      filesGetFile: build.query<FilesGetFileApiResponse, FilesGetFileApiArg>({
        query: (queryArg) => ({
          url: `/api/files/${queryArg.filetype}/${queryArg.source}`,
        }),
        providesTags: ["Files"],
      }),
      iconsAutoMatchIconToStreams: build.mutation<
        IconsAutoMatchIconToStreamsApiResponse,
        IconsAutoMatchIconToStreamsApiArg
      >({
        query: (queryArg) => ({
          url: `/api/icons/automatchicontostreams`,
          method: "POST",
          body: queryArg,
        }),
        invalidatesTags: ["Icons"],
      }),
      iconsGetIcon: build.query<IconsGetIconApiResponse, IconsGetIconApiArg>({
        query: (queryArg) => ({ url: `/api/icons/geticon/${queryArg}` }),
        providesTags: ["Icons"],
      }),
      iconsGetIconFromSource: build.query<
        IconsGetIconFromSourceApiResponse,
        IconsGetIconFromSourceApiArg
      >({
        query: (queryArg) => ({
          url: `/api/icons/geticonfromsource`,
          params: { value: queryArg },
        }),
        providesTags: ["Icons"],
      }),
      iconsGetPagedIcons: build.query<
        IconsGetPagedIconsApiResponse,
        IconsGetPagedIconsApiArg
      >({
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
            JSONFiltersString: queryArg.jsonFiltersString,
          },
        }),
        providesTags: ["Icons"],
      }),
      iconsGetIconsSimpleQuery: build.query<
        IconsGetIconsSimpleQueryApiResponse,
        IconsGetIconsSimpleQueryApiArg
      >({
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
            JSONFiltersString: queryArg.jsonFiltersString,
          },
        }),
        providesTags: ["Icons"],
      }),
      logsGetLogRequest: build.query<
        LogsGetLogRequestApiResponse,
        LogsGetLogRequestApiArg
      >({
        query: (queryArg) => ({
          url: `/api/logs/getlogrequest`,
          params: { LastId: queryArg.lastId, MaxLines: queryArg.maxLines },
        }),
        providesTags: ["Logs"],
      }),
      m3UFilesCreateM3UFile: build.mutation<
        M3UFilesCreateM3UFileApiResponse,
        M3UFilesCreateM3UFileApiArg
      >({
        query: (queryArg) => ({
          url: `/api/m3ufiles/createm3ufile`,
          method: "POST",
          body: queryArg,
        }),
        invalidatesTags: ["M3UFiles"],
      }),
      m3UFilesCreateM3UFileFromForm: build.mutation<
        M3UFilesCreateM3UFileFromFormApiResponse,
        M3UFilesCreateM3UFileFromFormApiArg
      >({
        query: (queryArg) => ({
          url: `/api/m3ufiles/createm3ufilefromform`,
          method: "POST",
          body: queryArg,
        }),
        invalidatesTags: ["M3UFiles"],
      }),
      m3UFilesChangeM3UFileName: build.mutation<
        M3UFilesChangeM3UFileNameApiResponse,
        M3UFilesChangeM3UFileNameApiArg
      >({
        query: (queryArg) => ({
          url: `/api/m3ufiles/changem3ufilename`,
          method: "PATCH",
          body: queryArg,
        }),
        invalidatesTags: ["M3UFiles"],
      }),
      m3UFilesDeleteM3UFile: build.mutation<
        M3UFilesDeleteM3UFileApiResponse,
        M3UFilesDeleteM3UFileApiArg
      >({
        query: (queryArg) => ({
          url: `/api/m3ufiles/deletem3ufile`,
          method: "DELETE",
          body: queryArg,
        }),
        invalidatesTags: ["M3UFiles"],
      }),
      m3UFilesGetM3UFile: build.query<
        M3UFilesGetM3UFileApiResponse,
        M3UFilesGetM3UFileApiArg
      >({
        query: (queryArg) => ({ url: `/api/m3ufiles/${queryArg}` }),
        providesTags: ["M3UFiles"],
      }),
      m3UFilesGetPagedM3UFiles: build.query<
        M3UFilesGetPagedM3UFilesApiResponse,
        M3UFilesGetPagedM3UFilesApiArg
      >({
        query: (queryArg) => ({
          url: `/api/m3ufiles`,
          params: {
            PageNumber: queryArg.pageNumber,
            PageSize: queryArg.pageSize,
            OrderBy: queryArg.orderBy,
            JSONArgumentString: queryArg.jsonArgumentString,
            JSONFiltersString: queryArg.jsonFiltersString,
          },
        }),
        providesTags: ["M3UFiles"],
      }),
      m3UFilesProcessM3UFile: build.mutation<
        M3UFilesProcessM3UFileApiResponse,
        M3UFilesProcessM3UFileApiArg
      >({
        query: (queryArg) => ({
          url: `/api/m3ufiles/processm3ufile`,
          method: "PATCH",
          body: queryArg,
        }),
        invalidatesTags: ["M3UFiles"],
      }),
      m3UFilesRefreshM3UFile: build.mutation<
        M3UFilesRefreshM3UFileApiResponse,
        M3UFilesRefreshM3UFileApiArg
      >({
        query: (queryArg) => ({
          url: `/api/m3ufiles/refreshm3ufile`,
          method: "PATCH",
          body: queryArg,
        }),
        invalidatesTags: ["M3UFiles"],
      }),
      m3UFilesScanDirectoryForM3UFiles: build.mutation<
        M3UFilesScanDirectoryForM3UFilesApiResponse,
        M3UFilesScanDirectoryForM3UFilesApiArg
      >({
        query: () => ({
          url: `/api/m3ufiles/scandirectoryform3ufiles`,
          method: "PATCH",
        }),
        invalidatesTags: ["M3UFiles"],
      }),
      m3UFilesUpdateM3UFile: build.mutation<
        M3UFilesUpdateM3UFileApiResponse,
        M3UFilesUpdateM3UFileApiArg
      >({
        query: (queryArg) => ({
          url: `/api/m3ufiles/updatem3ufile`,
          method: "PATCH",
          body: queryArg,
        }),
        invalidatesTags: ["M3UFiles"],
      }),
      m3UFilesGetM3UFileNames: build.query<
        M3UFilesGetM3UFileNamesApiResponse,
        M3UFilesGetM3UFileNamesApiArg
      >({
        query: () => ({ url: `/api/m3ufiles/getm3ufilenames` }),
        providesTags: ["M3UFiles"],
      }),
      miscBuildIconsCacheFromVideoStreams: build.mutation<
        MiscBuildIconsCacheFromVideoStreamsApiResponse,
        MiscBuildIconsCacheFromVideoStreamsApiArg
      >({
        query: () => ({
          url: `/api/misc/buildiconscachefromvideostreams`,
          method: "PATCH",
        }),
        invalidatesTags: ["Misc"],
      }),
      miscBuildProgIconsCacheFromEpGsRequest: build.mutation<
        MiscBuildProgIconsCacheFromEpGsRequestApiResponse,
        MiscBuildProgIconsCacheFromEpGsRequestApiArg
      >({
        query: () => ({
          url: `/api/misc/buildprogiconscachefromepgsrequest`,
          method: "PATCH",
        }),
        invalidatesTags: ["Misc"],
      }),
      programmesGetProgramme: build.query<
        ProgrammesGetProgrammeApiResponse,
        ProgrammesGetProgrammeApiArg
      >({
        query: (queryArg) => ({
          url: `/api/programmes/getprogramme/${queryArg}`,
        }),
        providesTags: ["Programmes"],
      }),
      programmesGetProgrammeChannels: build.query<
        ProgrammesGetProgrammeChannelsApiResponse,
        ProgrammesGetProgrammeChannelsApiArg
      >({
        query: () => ({ url: `/api/programmes/getprogrammechannels` }),
        providesTags: ["Programmes"],
      }),
      programmesGetPagedProgrammeNameSelections: build.query<
        ProgrammesGetPagedProgrammeNameSelectionsApiResponse,
        ProgrammesGetPagedProgrammeNameSelectionsApiArg
      >({
        query: (queryArg) => ({
          url: `/api/programmes/getpagedprogrammenameselections`,
          params: {
            Count: queryArg.count,
            First: queryArg.first,
            Last: queryArg.last,
            PageNumber: queryArg.pageNumber,
            PageSize: queryArg.pageSize,
            OrderBy: queryArg.orderBy,
            JSONArgumentString: queryArg.jsonArgumentString,
            JSONFiltersString: queryArg.jsonFiltersString,
          },
        }),
        providesTags: ["Programmes"],
      }),
      programmesGetProgrammes: build.query<
        ProgrammesGetProgrammesApiResponse,
        ProgrammesGetProgrammesApiArg
      >({
        query: () => ({ url: `/api/programmes/getprogrammes` }),
        providesTags: ["Programmes"],
      }),
      programmesGetProgrammeNames: build.query<
        ProgrammesGetProgrammeNamesApiResponse,
        ProgrammesGetProgrammeNamesApiArg
      >({
        query: () => ({ url: `/api/programmes/getprogrammenames` }),
        providesTags: ["Programmes"],
      }),
      programmesGetProgrammsSimpleQuery: build.query<
        ProgrammesGetProgrammsSimpleQueryApiResponse,
        ProgrammesGetProgrammsSimpleQueryApiArg
      >({
        query: (queryArg) => ({
          url: `/api/programmes/getprogrammssimplequery`,
          params: {
            Count: queryArg.count,
            First: queryArg.first,
            Last: queryArg.last,
            PageNumber: queryArg.pageNumber,
            PageSize: queryArg.pageSize,
            OrderBy: queryArg.orderBy,
            JSONArgumentString: queryArg.jsonArgumentString,
            JSONFiltersString: queryArg.jsonFiltersString,
          },
        }),
        providesTags: ["Programmes"],
      }),
      programmesGetProgrammeFromDisplayName: build.query<
        ProgrammesGetProgrammeFromDisplayNameApiResponse,
        ProgrammesGetProgrammeFromDisplayNameApiArg
      >({
        query: (queryArg) => ({
          url: `/api/programmes/getprogrammefromdisplayname`,
          body: queryArg,
        }),
        providesTags: ["Programmes"],
      }),
      schedulesDirectGetCountries: build.query<
        SchedulesDirectGetCountriesApiResponse,
        SchedulesDirectGetCountriesApiArg
      >({
        query: () => ({ url: `/api/schedulesdirect/getcountries` }),
        providesTags: ["SchedulesDirect"],
      }),
      schedulesDirectGetHeadends: build.query<
        SchedulesDirectGetHeadendsApiResponse,
        SchedulesDirectGetHeadendsApiArg
      >({
        query: (queryArg) => ({
          url: `/api/schedulesdirect/getheadends`,
          params: {
            country: queryArg.country,
            postalCode: queryArg.postalCode,
          },
        }),
        providesTags: ["SchedulesDirect"],
      }),
      schedulesDirectGetLineup: build.query<
        SchedulesDirectGetLineupApiResponse,
        SchedulesDirectGetLineupApiArg
      >({
        query: (queryArg) => ({
          url: `/api/schedulesdirect/getlineup`,
          params: { lineup: queryArg },
        }),
        providesTags: ["SchedulesDirect"],
      }),
      schedulesDirectGetLineupPreviews: build.query<
        SchedulesDirectGetLineupPreviewsApiResponse,
        SchedulesDirectGetLineupPreviewsApiArg
      >({
        query: () => ({ url: `/api/schedulesdirect/getlineuppreviews` }),
        providesTags: ["SchedulesDirect"],
      }),
      schedulesDirectGetLineups: build.query<
        SchedulesDirectGetLineupsApiResponse,
        SchedulesDirectGetLineupsApiArg
      >({
        query: () => ({ url: `/api/schedulesdirect/getlineups` }),
        providesTags: ["SchedulesDirect"],
      }),
      schedulesDirectGetSchedules: build.query<
        SchedulesDirectGetSchedulesApiResponse,
        SchedulesDirectGetSchedulesApiArg
      >({
        query: () => ({ url: `/api/schedulesdirect/getschedules` }),
        providesTags: ["SchedulesDirect"],
      }),
      schedulesDirectGetStationPreviews: build.query<
        SchedulesDirectGetStationPreviewsApiResponse,
        SchedulesDirectGetStationPreviewsApiArg
      >({
        query: () => ({ url: `/api/schedulesdirect/getstationpreviews` }),
        providesTags: ["SchedulesDirect"],
      }),
      schedulesDirectGetStations: build.query<
        SchedulesDirectGetStationsApiResponse,
        SchedulesDirectGetStationsApiArg
      >({
        query: () => ({ url: `/api/schedulesdirect/getstations` }),
        providesTags: ["SchedulesDirect"],
      }),
      schedulesDirectGetStatus: build.query<
        SchedulesDirectGetStatusApiResponse,
        SchedulesDirectGetStatusApiArg
      >({
        query: () => ({ url: `/api/schedulesdirect/getstatus` }),
        providesTags: ["SchedulesDirect"],
      }),
      settingsGetIsSystemReady: build.query<
        SettingsGetIsSystemReadyApiResponse,
        SettingsGetIsSystemReadyApiArg
      >({
        query: () => ({ url: `/api/settings/getissystemready` }),
        providesTags: ["Settings"],
      }),
      settingsGetQueueStatus: build.query<
        SettingsGetQueueStatusApiResponse,
        SettingsGetQueueStatusApiArg
      >({
        query: () => ({ url: `/api/settings/getqueuestatus` }),
        providesTags: ["Settings"],
      }),
      settingsGetSetting: build.query<
        SettingsGetSettingApiResponse,
        SettingsGetSettingApiArg
      >({
        query: () => ({ url: `/api/settings/getsetting` }),
        providesTags: ["Settings"],
      }),
      settingsGetSystemStatus: build.query<
        SettingsGetSystemStatusApiResponse,
        SettingsGetSystemStatusApiArg
      >({
        query: () => ({ url: `/api/settings/getsystemstatus` }),
        providesTags: ["Settings"],
      }),
      settingsLogIn: build.query<SettingsLogInApiResponse, SettingsLogInApiArg>(
        {
          query: (queryArg) => ({ url: `/api/settings/login`, body: queryArg }),
          providesTags: ["Settings"],
        }
      ),
      settingsUpdateSetting: build.mutation<
        SettingsUpdateSettingApiResponse,
        SettingsUpdateSettingApiArg
      >({
        query: (queryArg) => ({
          url: `/api/settings/updatesetting`,
          method: "PATCH",
          body: queryArg,
        }),
        invalidatesTags: ["Settings"],
      }),
      streamGroupChannelGroupSyncStreamGroupChannelGroups: build.mutation<
        StreamGroupChannelGroupSyncStreamGroupChannelGroupsApiResponse,
        StreamGroupChannelGroupSyncStreamGroupChannelGroupsApiArg
      >({
        query: (queryArg) => ({
          url: `/api/streamgroupchannelgroup/syncstreamgroupchannelgroups`,
          method: "PATCH",
          body: queryArg,
        }),
        invalidatesTags: ["StreamGroupChannelGroup"],
      }),
      streamGroupChannelGroupGetChannelGroupsFromStreamGroup: build.query<
        StreamGroupChannelGroupGetChannelGroupsFromStreamGroupApiResponse,
        StreamGroupChannelGroupGetChannelGroupsFromStreamGroupApiArg
      >({
        query: (queryArg) => ({
          url: `/api/streamgroupchannelgroup/getchannelgroupsfromstreamgroup`,
          params: { StreamGroupId: queryArg },
        }),
        providesTags: ["StreamGroupChannelGroup"],
      }),
      streamGroupsCreateStreamGroup: build.mutation<
        StreamGroupsCreateStreamGroupApiResponse,
        StreamGroupsCreateStreamGroupApiArg
      >({
        query: (queryArg) => ({
          url: `/api/streamgroups/createstreamgroup`,
          method: "POST",
          body: queryArg,
        }),
        invalidatesTags: ["StreamGroups"],
      }),
      streamGroupsDeleteStreamGroup: build.mutation<
        StreamGroupsDeleteStreamGroupApiResponse,
        StreamGroupsDeleteStreamGroupApiArg
      >({
        query: (queryArg) => ({
          url: `/api/streamgroups/deletestreamgroup`,
          method: "DELETE",
          body: queryArg,
        }),
        invalidatesTags: ["StreamGroups"],
      }),
      streamGroupsGetStreamGroup: build.query<
        StreamGroupsGetStreamGroupApiResponse,
        StreamGroupsGetStreamGroupApiArg
      >({
        query: (queryArg) => ({
          url: `/api/streamgroups/getstreamgroup/${queryArg}`,
        }),
        providesTags: ["StreamGroups"],
      }),
      streamGroupsGetStreamGroupCapability: build.query<
        StreamGroupsGetStreamGroupCapabilityApiResponse,
        StreamGroupsGetStreamGroupCapabilityApiArg
      >({
        query: (queryArg) => ({ url: `/api/streamgroups/${queryArg}` }),
        providesTags: ["StreamGroups"],
      }),
      streamGroupsGetStreamGroupCapability2: build.query<
        StreamGroupsGetStreamGroupCapability2ApiResponse,
        StreamGroupsGetStreamGroupCapability2ApiArg
      >({
        query: (queryArg) => ({
          url: `/api/streamgroups/${queryArg}/capability`,
        }),
        providesTags: ["StreamGroups"],
      }),
      streamGroupsGetStreamGroupCapability3: build.query<
        StreamGroupsGetStreamGroupCapability3ApiResponse,
        StreamGroupsGetStreamGroupCapability3ApiArg
      >({
        query: (queryArg) => ({
          url: `/api/streamgroups/${queryArg}/device.xml`,
        }),
        providesTags: ["StreamGroups"],
      }),
      streamGroupsGetStreamGroupDiscover: build.query<
        StreamGroupsGetStreamGroupDiscoverApiResponse,
        StreamGroupsGetStreamGroupDiscoverApiArg
      >({
        query: (queryArg) => ({
          url: `/api/streamgroups/${queryArg}/discover.json`,
        }),
        providesTags: ["StreamGroups"],
      }),
      streamGroupsGetStreamGroupEpg: build.query<
        StreamGroupsGetStreamGroupEpgApiResponse,
        StreamGroupsGetStreamGroupEpgApiArg
      >({
        query: (queryArg) => ({ url: `/api/streamgroups/${queryArg}/epg.xml` }),
        providesTags: ["StreamGroups"],
      }),
      streamGroupsGetStreamGroupEpgForGuide: build.query<
        StreamGroupsGetStreamGroupEpgForGuideApiResponse,
        StreamGroupsGetStreamGroupEpgForGuideApiArg
      >({
        query: (queryArg) => ({
          url: `/api/streamgroups/${queryArg}/epgguide.json`,
        }),
        providesTags: ["StreamGroups"],
      }),
      streamGroupsGetStreamGroupLineUp: build.query<
        StreamGroupsGetStreamGroupLineUpApiResponse,
        StreamGroupsGetStreamGroupLineUpApiArg
      >({
        query: (queryArg) => ({
          url: `/api/streamgroups/${queryArg}/lineup.json`,
        }),
        providesTags: ["StreamGroups"],
      }),
      streamGroupsGetStreamGroupLineUpStatus: build.query<
        StreamGroupsGetStreamGroupLineUpStatusApiResponse,
        StreamGroupsGetStreamGroupLineUpStatusApiArg
      >({
        query: (queryArg) => ({
          url: `/api/streamgroups/${queryArg}/lineup_status.json`,
        }),
        providesTags: ["StreamGroups"],
      }),
      streamGroupsGetStreamGroupM3U: build.query<
        StreamGroupsGetStreamGroupM3UApiResponse,
        StreamGroupsGetStreamGroupM3UApiArg
      >({
        query: (queryArg) => ({ url: `/api/streamgroups/${queryArg}/m3u.m3u` }),
        providesTags: ["StreamGroups"],
      }),
      streamGroupsGetPagedStreamGroups: build.query<
        StreamGroupsGetPagedStreamGroupsApiResponse,
        StreamGroupsGetPagedStreamGroupsApiArg
      >({
        query: (queryArg) => ({
          url: `/api/streamgroups`,
          params: {
            PageNumber: queryArg.pageNumber,
            PageSize: queryArg.pageSize,
            OrderBy: queryArg.orderBy,
            JSONArgumentString: queryArg.jsonArgumentString,
            JSONFiltersString: queryArg.jsonFiltersString,
          },
        }),
        providesTags: ["StreamGroups"],
      }),
      streamGroupsUpdateStreamGroup: build.mutation<
        StreamGroupsUpdateStreamGroupApiResponse,
        StreamGroupsUpdateStreamGroupApiArg
      >({
        query: (queryArg) => ({
          url: `/api/streamgroups/updatestreamgroup`,
          method: "PATCH",
          body: queryArg,
        }),
        invalidatesTags: ["StreamGroups"],
      }),
      streamGroupVideoStreamsGetStreamGroupVideoStreamIds: build.query<
        StreamGroupVideoStreamsGetStreamGroupVideoStreamIdsApiResponse,
        StreamGroupVideoStreamsGetStreamGroupVideoStreamIdsApiArg
      >({
        query: (queryArg) => ({
          url: `/api/streamgroupvideostreams/getstreamgroupvideostreamids`,
          params: { StreamGroupId: queryArg },
        }),
        providesTags: ["StreamGroupVideoStreams"],
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
            JSONFiltersString: queryArg.jsonFiltersString,
          },
        }),
        providesTags: ["StreamGroupVideoStreams"],
      }),
      streamGroupVideoStreamsSetVideoStreamRanks: build.mutation<
        StreamGroupVideoStreamsSetVideoStreamRanksApiResponse,
        StreamGroupVideoStreamsSetVideoStreamRanksApiArg
      >({
        query: (queryArg) => ({
          url: `/api/streamgroupvideostreams/setvideostreamranks`,
          method: "PATCH",
          body: queryArg,
        }),
        invalidatesTags: ["StreamGroupVideoStreams"],
      }),
      streamGroupVideoStreamsSyncVideoStreamToStreamGroupPost: build.mutation<
        StreamGroupVideoStreamsSyncVideoStreamToStreamGroupPostApiResponse,
        StreamGroupVideoStreamsSyncVideoStreamToStreamGroupPostApiArg
      >({
        query: (queryArg) => ({
          url: `/api/streamgroupvideostreams/syncvideostreamtostreamgroup`,
          method: "POST",
          body: queryArg,
        }),
        invalidatesTags: ["StreamGroupVideoStreams"],
      }),
      streamGroupVideoStreamsSyncVideoStreamToStreamGroupDelete: build.mutation<
        StreamGroupVideoStreamsSyncVideoStreamToStreamGroupDeleteApiResponse,
        StreamGroupVideoStreamsSyncVideoStreamToStreamGroupDeleteApiArg
      >({
        query: (queryArg) => ({
          url: `/api/streamgroupvideostreams/syncvideostreamtostreamgroup`,
          method: "DELETE",
          body: queryArg,
        }),
        invalidatesTags: ["StreamGroupVideoStreams"],
      }),
      videoStreamLinksAddVideoStreamToVideoStream: build.mutation<
        VideoStreamLinksAddVideoStreamToVideoStreamApiResponse,
        VideoStreamLinksAddVideoStreamToVideoStreamApiArg
      >({
        query: (queryArg) => ({
          url: `/api/videostreamlinks/addvideostreamtovideostream`,
          method: "PATCH",
          body: queryArg,
        }),
        invalidatesTags: ["VideoStreamLinks"],
      }),
      videoStreamLinksGetVideoStreamVideoStreamIds: build.query<
        VideoStreamLinksGetVideoStreamVideoStreamIdsApiResponse,
        VideoStreamLinksGetVideoStreamVideoStreamIdsApiArg
      >({
        query: (queryArg) => ({
          url: `/api/videostreamlinks/getvideostreamvideostreamids`,
          params: { videoStreamId: queryArg },
        }),
        providesTags: ["VideoStreamLinks"],
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
            JSONFiltersString: queryArg.jsonFiltersString,
          },
        }),
        providesTags: ["VideoStreamLinks"],
      }),
      videoStreamLinksRemoveVideoStreamFromVideoStream: build.mutation<
        VideoStreamLinksRemoveVideoStreamFromVideoStreamApiResponse,
        VideoStreamLinksRemoveVideoStreamFromVideoStreamApiArg
      >({
        query: (queryArg) => ({
          url: `/api/videostreamlinks/removevideostreamfromvideostream`,
          method: "PATCH",
          body: queryArg,
        }),
        invalidatesTags: ["VideoStreamLinks"],
      }),
      videoStreamsCreateVideoStream: build.mutation<
        VideoStreamsCreateVideoStreamApiResponse,
        VideoStreamsCreateVideoStreamApiArg
      >({
        query: (queryArg) => ({
          url: `/api/videostreams/createvideostream`,
          method: "POST",
          body: queryArg,
        }),
        invalidatesTags: ["VideoStreams"],
      }),
      videoStreamsChangeVideoStreamChannel: build.mutation<
        VideoStreamsChangeVideoStreamChannelApiResponse,
        VideoStreamsChangeVideoStreamChannelApiArg
      >({
        query: (queryArg) => ({
          url: `/api/videostreams/changevideostreamchannel`,
          method: "POST",
          body: queryArg,
        }),
        invalidatesTags: ["VideoStreams"],
      }),
      videoStreamsDeleteVideoStream: build.mutation<
        VideoStreamsDeleteVideoStreamApiResponse,
        VideoStreamsDeleteVideoStreamApiArg
      >({
        query: (queryArg) => ({
          url: `/api/videostreams/deletevideostream`,
          method: "DELETE",
          body: queryArg,
        }),
        invalidatesTags: ["VideoStreams"],
      }),
      videoStreamsFailClient: build.mutation<
        VideoStreamsFailClientApiResponse,
        VideoStreamsFailClientApiArg
      >({
        query: (queryArg) => ({
          url: `/api/videostreams/failclient`,
          method: "POST",
          body: queryArg,
        }),
        invalidatesTags: ["VideoStreams"],
      }),
      videoStreamsGetAllStatisticsForAllUrls: build.query<
        VideoStreamsGetAllStatisticsForAllUrlsApiResponse,
        VideoStreamsGetAllStatisticsForAllUrlsApiArg
      >({
        query: () => ({ url: `/api/videostreams/getallstatisticsforallurls` }),
        providesTags: ["VideoStreams"],
      }),
      videoStreamsGetChannelLogoDtos: build.query<
        VideoStreamsGetChannelLogoDtosApiResponse,
        VideoStreamsGetChannelLogoDtosApiArg
      >({
        query: () => ({ url: `/api/videostreams/getchannellogodtos` }),
        providesTags: ["VideoStreams"],
      }),
      videoStreamsGetVideoStream: build.query<
        VideoStreamsGetVideoStreamApiResponse,
        VideoStreamsGetVideoStreamApiArg
      >({
        query: (queryArg) => ({ url: `/api/videostreams/${queryArg}` }),
        providesTags: ["VideoStreams"],
      }),
      videoStreamsGetPagedVideoStreams: build.query<
        VideoStreamsGetPagedVideoStreamsApiResponse,
        VideoStreamsGetPagedVideoStreamsApiArg
      >({
        query: (queryArg) => ({
          url: `/api/videostreams`,
          params: {
            PageNumber: queryArg.pageNumber,
            PageSize: queryArg.pageSize,
            OrderBy: queryArg.orderBy,
            JSONArgumentString: queryArg.jsonArgumentString,
            JSONFiltersString: queryArg.jsonFiltersString,
          },
        }),
        providesTags: ["VideoStreams"],
      }),
      videoStreamsGetVideoStreamStream: build.query<
        VideoStreamsGetVideoStreamStreamApiResponse,
        VideoStreamsGetVideoStreamStreamApiArg
      >({
        query: (queryArg) => ({ url: `/api/videostreams/stream/${queryArg}` }),
        providesTags: ["VideoStreams"],
      }),
      videoStreamsGetVideoStreamStream2: build.query<
        VideoStreamsGetVideoStreamStream2ApiResponse,
        VideoStreamsGetVideoStreamStream2ApiArg
      >({
        query: (queryArg) => ({
          url: `/api/videostreams/stream/${queryArg}.mp4`,
        }),
        providesTags: ["VideoStreams"],
      }),
      videoStreamsGetVideoStreamStream3: build.query<
        VideoStreamsGetVideoStreamStream3ApiResponse,
        VideoStreamsGetVideoStreamStream3ApiArg
      >({
        query: (queryArg) => ({
          url: `/api/videostreams/stream/${queryArg.encodedIds}/${queryArg.name}`,
        }),
        providesTags: ["VideoStreams"],
      }),
      videoStreamsReSetVideoStreamsLogo: build.mutation<
        VideoStreamsReSetVideoStreamsLogoApiResponse,
        VideoStreamsReSetVideoStreamsLogoApiArg
      >({
        query: (queryArg) => ({
          url: `/api/videostreams/resetvideostreamslogo`,
          method: "PATCH",
          body: queryArg,
        }),
        invalidatesTags: ["VideoStreams"],
      }),
      videoStreamsSetVideoStreamChannelNumbers: build.mutation<
        VideoStreamsSetVideoStreamChannelNumbersApiResponse,
        VideoStreamsSetVideoStreamChannelNumbersApiArg
      >({
        query: (queryArg) => ({
          url: `/api/videostreams/setvideostreamchannelnumbers`,
          method: "PATCH",
          body: queryArg,
        }),
        invalidatesTags: ["VideoStreams"],
      }),
      videoStreamsSetVideoStreamsLogoFromEpg: build.mutation<
        VideoStreamsSetVideoStreamsLogoFromEpgApiResponse,
        VideoStreamsSetVideoStreamsLogoFromEpgApiArg
      >({
        query: (queryArg) => ({
          url: `/api/videostreams/setvideostreamslogofromepg`,
          method: "PATCH",
          body: queryArg,
        }),
        invalidatesTags: ["VideoStreams"],
      }),
      videoStreamsUpdateVideoStream: build.mutation<
        VideoStreamsUpdateVideoStreamApiResponse,
        VideoStreamsUpdateVideoStreamApiArg
      >({
        query: (queryArg) => ({
          url: `/api/videostreams/updatevideostream`,
          method: "PATCH",
          body: queryArg,
        }),
        invalidatesTags: ["VideoStreams"],
      }),
      videoStreamsUpdateVideoStreams: build.mutation<
        VideoStreamsUpdateVideoStreamsApiResponse,
        VideoStreamsUpdateVideoStreamsApiArg
      >({
        query: (queryArg) => ({
          url: `/api/videostreams/updatevideostreams`,
          method: "PATCH",
          body: queryArg,
        }),
        invalidatesTags: ["VideoStreams"],
      }),
      videoStreamsUpdateAllVideoStreamsFromParameters: build.mutation<
        VideoStreamsUpdateAllVideoStreamsFromParametersApiResponse,
        VideoStreamsUpdateAllVideoStreamsFromParametersApiArg
      >({
        query: (queryArg) => ({
          url: `/api/videostreams/updateallvideostreamsfromparameters`,
          method: "PATCH",
          body: queryArg,
        }),
        invalidatesTags: ["VideoStreams"],
      }),
      videoStreamsDeleteAllVideoStreamsFromParameters: build.mutation<
        VideoStreamsDeleteAllVideoStreamsFromParametersApiResponse,
        VideoStreamsDeleteAllVideoStreamsFromParametersApiArg
      >({
        query: (queryArg) => ({
          url: `/api/videostreams/deleteallvideostreamsfromparameters`,
          method: "DELETE",
          body: queryArg,
        }),
        invalidatesTags: ["VideoStreams"],
      }),
      videoStreamsSetVideoStreamChannelNumbersFromParameters: build.mutation<
        VideoStreamsSetVideoStreamChannelNumbersFromParametersApiResponse,
        VideoStreamsSetVideoStreamChannelNumbersFromParametersApiArg
      >({
        query: (queryArg) => ({
          url: `/api/videostreams/setvideostreamchannelnumbersfromparameters`,
          method: "PATCH",
          body: queryArg,
        }),
        invalidatesTags: ["VideoStreams"],
      }),
      videoStreamsSetVideoStreamsLogoFromEpgFromParameters: build.mutation<
        VideoStreamsSetVideoStreamsLogoFromEpgFromParametersApiResponse,
        VideoStreamsSetVideoStreamsLogoFromEpgFromParametersApiArg
      >({
        query: (queryArg) => ({
          url: `/api/videostreams/setvideostreamslogofromepgfromparameters`,
          method: "PATCH",
          body: queryArg,
        }),
        invalidatesTags: ["VideoStreams"],
      }),
      videoStreamsReSetVideoStreamsLogoFromParameters: build.mutation<
        VideoStreamsReSetVideoStreamsLogoFromParametersApiResponse,
        VideoStreamsReSetVideoStreamsLogoFromParametersApiArg
      >({
        query: (queryArg) => ({
          url: `/api/videostreams/resetvideostreamslogofromparameters`,
          method: "PATCH",
          body: queryArg,
        }),
        invalidatesTags: ["VideoStreams"],
      }),
      videoStreamsSimulateStreamFailureForAll: build.mutation<
        VideoStreamsSimulateStreamFailureForAllApiResponse,
        VideoStreamsSimulateStreamFailureForAllApiArg
      >({
        query: () => ({
          url: `/api/videostreams/simulatestreamfailureforall`,
          method: "POST",
        }),
        invalidatesTags: ["VideoStreams"],
      }),
      videoStreamsSimulateStreamFailure: build.mutation<
        VideoStreamsSimulateStreamFailureApiResponse,
        VideoStreamsSimulateStreamFailureApiArg
      >({
        query: (queryArg) => ({
          url: `/api/videostreams/simulatestreamfailure`,
          method: "POST",
          body: queryArg,
        }),
        invalidatesTags: ["VideoStreams"],
      }),
    }),
    overrideExisting: false,
  });
export { injectedRtkApi as iptvApi };
export type ChannelGroupsCreateChannelGroupApiResponse = unknown;
export type ChannelGroupsCreateChannelGroupApiArg = CreateChannelGroupRequest;
export type ChannelGroupsDeleteAllChannelGroupsFromParametersApiResponse =
  unknown;
export type ChannelGroupsDeleteAllChannelGroupsFromParametersApiArg =
  DeleteAllChannelGroupsFromParametersRequest;
export type ChannelGroupsDeleteChannelGroupApiResponse = unknown;
export type ChannelGroupsDeleteChannelGroupApiArg = DeleteChannelGroupRequest;
export type ChannelGroupsGetChannelGroupApiResponse =
  /** status 200  */ ChannelGroupDto;
export type ChannelGroupsGetChannelGroupApiArg = number;
export type ChannelGroupsGetChannelGroupIdNamesApiResponse =
  /** status 200  */ ChannelGroupIdName[];
export type ChannelGroupsGetChannelGroupIdNamesApiArg = void;
export type ChannelGroupsGetPagedChannelGroupsApiResponse =
  /** status 200  */ PagedResponseOfChannelGroupDto;
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
export type ChannelGroupsGetChannelGroupNamesApiResponse =
  /** status 200  */ string[];
export type ChannelGroupsGetChannelGroupNamesApiArg = void;
export type ChannelGroupsGetChannelGroupsForStreamGroupApiResponse =
  /** status 200  */ ChannelGroupDto[];
export type ChannelGroupsGetChannelGroupsForStreamGroupApiArg =
  GetChannelGroupsForStreamGroupRequest;
export type EpgFilesCreateEpgFileApiResponse = unknown;
export type EpgFilesCreateEpgFileApiArg = CreateEpgFileRequest;
export type EpgFilesCreateEpgFileFromFormApiResponse = unknown;
export type EpgFilesCreateEpgFileFromFormApiArg = {
  Description?: string | null;
  EPGRank?: number;
  FormFile?: Blob | null;
  Name?: string;
  UrlSource?: string | null;
};
export type EpgFilesDeleteEpgFileApiResponse = unknown;
export type EpgFilesDeleteEpgFileApiArg = DeleteEpgFileRequest;
export type EpgFilesGetEpgFileApiResponse = /** status 200  */ EpgFileDto;
export type EpgFilesGetEpgFileApiArg = number;
export type EpgFilesGetPagedEpgFilesApiResponse =
  /** status 200  */ PagedResponseOfEpgFileDto;
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
export type FilesGetFileApiResponse = unknown;
export type FilesGetFileApiArg = {
  source: string;
  filetype: SmFileTypes;
};
export type IconsAutoMatchIconToStreamsApiResponse = unknown;
export type IconsAutoMatchIconToStreamsApiArg = AutoMatchIconToStreamsRequest;
export type IconsGetIconApiResponse = /** status 200  */ IconFileDto;
export type IconsGetIconApiArg = number;
export type IconsGetIconFromSourceApiResponse = /** status 200  */ IconFileDto;
export type IconsGetIconFromSourceApiArg = string;
export type IconsGetPagedIconsApiResponse =
  /** status 200  */ PagedResponseOfIconFileDto;
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
export type IconsGetIconsSimpleQueryApiResponse =
  /** status 200  */ IconFileDto[];
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
export type LogsGetLogRequestApiResponse = /** status 200  */ LogEntryDto[];
export type LogsGetLogRequestApiArg = {
  lastId?: number;
  maxLines?: number;
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
export type M3UFilesGetPagedM3UFilesApiResponse =
  /** status 200  */ PagedResponseOfM3UFileDto;
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
export type MiscBuildIconsCacheFromVideoStreamsApiResponse = unknown;
export type MiscBuildIconsCacheFromVideoStreamsApiArg = void;
export type MiscBuildProgIconsCacheFromEpGsRequestApiResponse = unknown;
export type MiscBuildProgIconsCacheFromEpGsRequestApiArg = void;
export type ProgrammesGetProgrammeApiResponse = /** status 200  */ Programme[];
export type ProgrammesGetProgrammeApiArg = string;
export type ProgrammesGetProgrammeChannelsApiResponse =
  /** status 200  */ ProgrammeChannel[];
export type ProgrammesGetProgrammeChannelsApiArg = void;
export type ProgrammesGetPagedProgrammeNameSelectionsApiResponse =
  /** status 200  */ PagedResponseOfProgrammeNameDto;
export type ProgrammesGetPagedProgrammeNameSelectionsApiArg = {
  count?: number;
  first?: number;
  last?: number;
  pageNumber?: number;
  pageSize?: number;
  orderBy?: string;
  jsonArgumentString?: string | null;
  jsonFiltersString?: string | null;
};
export type ProgrammesGetProgrammesApiResponse = /** status 200  */ Programme[];
export type ProgrammesGetProgrammesApiArg = void;
export type ProgrammesGetProgrammeNamesApiResponse =
  /** status 200  */ string[];
export type ProgrammesGetProgrammeNamesApiArg = void;
export type ProgrammesGetProgrammsSimpleQueryApiResponse =
  /** status 200  */ ProgrammeNameDto[];
export type ProgrammesGetProgrammsSimpleQueryApiArg = {
  count?: number;
  first?: number;
  last?: number;
  pageNumber?: number;
  pageSize?: number;
  orderBy?: string;
  jsonArgumentString?: string | null;
  jsonFiltersString?: string | null;
};
export type ProgrammesGetProgrammeFromDisplayNameApiResponse =
  /** status 200  */ ProgrammeNameDto;
export type ProgrammesGetProgrammeFromDisplayNameApiArg =
  GetProgrammeFromDisplayNameRequest;
export type SchedulesDirectGetCountriesApiResponse =
  /** status 200  */ Countries;
export type SchedulesDirectGetCountriesApiArg = void;
export type SchedulesDirectGetHeadendsApiResponse =
  /** status 200  */ HeadendDto[];
export type SchedulesDirectGetHeadendsApiArg = {
  country?: string;
  postalCode?: string;
};
export type SchedulesDirectGetLineupApiResponse =
  /** status 200  */ LineUpResult;
export type SchedulesDirectGetLineupApiArg = string;
export type SchedulesDirectGetLineupPreviewsApiResponse =
  /** status 200  */ LineUpPreview[];
export type SchedulesDirectGetLineupPreviewsApiArg = void;
export type SchedulesDirectGetLineupsApiResponse =
  /** status 200  */ LineUpsResult;
export type SchedulesDirectGetLineupsApiArg = void;
export type SchedulesDirectGetSchedulesApiResponse =
  /** status 200  */ Schedule[];
export type SchedulesDirectGetSchedulesApiArg = void;
export type SchedulesDirectGetStationPreviewsApiResponse =
  /** status 200  */ StationPreview[];
export type SchedulesDirectGetStationPreviewsApiArg = void;
export type SchedulesDirectGetStationsApiResponse =
  /** status 200  */ Station[];
export type SchedulesDirectGetStationsApiArg = void;
export type SchedulesDirectGetStatusApiResponse = /** status 200  */ SdStatus;
export type SchedulesDirectGetStatusApiArg = void;
export type SettingsGetIsSystemReadyApiResponse = /** status 200  */ boolean;
export type SettingsGetIsSystemReadyApiArg = void;
export type SettingsGetQueueStatusApiResponse =
  /** status 200  */ TaskQueueStatusDto[];
export type SettingsGetQueueStatusApiArg = void;
export type SettingsGetSettingApiResponse = /** status 200  */ SettingDto;
export type SettingsGetSettingApiArg = void;
export type SettingsGetSystemStatusApiResponse =
  /** status 200  */ SystemStatus;
export type SettingsGetSystemStatusApiArg = void;
export type SettingsLogInApiResponse = /** status 200  */ boolean;
export type SettingsLogInApiArg = LogInRequest;
export type SettingsUpdateSettingApiResponse = unknown;
export type SettingsUpdateSettingApiArg = UpdateSettingRequest;
export type StreamGroupChannelGroupSyncStreamGroupChannelGroupsApiResponse =
  /** status 200  */ StreamGroupDto;
export type StreamGroupChannelGroupSyncStreamGroupChannelGroupsApiArg =
  SyncStreamGroupChannelGroupsRequest;
export type StreamGroupChannelGroupGetChannelGroupsFromStreamGroupApiResponse =
  /** status 200  */ ChannelGroupDto[];
export type StreamGroupChannelGroupGetChannelGroupsFromStreamGroupApiArg =
  number;
export type StreamGroupsCreateStreamGroupApiResponse = unknown;
export type StreamGroupsCreateStreamGroupApiArg = CreateStreamGroupRequest;
export type StreamGroupsDeleteStreamGroupApiResponse = unknown;
export type StreamGroupsDeleteStreamGroupApiArg = DeleteStreamGroupRequest;
export type StreamGroupsGetStreamGroupApiResponse =
  /** status 200  */ StreamGroupDto;
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
export type StreamGroupsGetStreamGroupEpgForGuideApiResponse =
  /** status 200  */ EpgGuide;
export type StreamGroupsGetStreamGroupEpgForGuideApiArg = number;
export type StreamGroupsGetStreamGroupLineUpApiResponse = unknown;
export type StreamGroupsGetStreamGroupLineUpApiArg = string;
export type StreamGroupsGetStreamGroupLineUpStatusApiResponse = unknown;
export type StreamGroupsGetStreamGroupLineUpStatusApiArg = string;
export type StreamGroupsGetStreamGroupM3UApiResponse = unknown;
export type StreamGroupsGetStreamGroupM3UApiArg = string;
export type StreamGroupsGetPagedStreamGroupsApiResponse =
  /** status 200  */ PagedResponseOfStreamGroupDto;
export type StreamGroupsGetPagedStreamGroupsApiArg = {
  pageNumber?: number;
  pageSize?: number;
  orderBy?: string;
  jsonArgumentString?: string | null;
  jsonFiltersString?: string | null;
};
export type StreamGroupsUpdateStreamGroupApiResponse = unknown;
export type StreamGroupsUpdateStreamGroupApiArg = UpdateStreamGroupRequest;
export type StreamGroupVideoStreamsGetStreamGroupVideoStreamIdsApiResponse =
  /** status 200  */ VideoStreamIsReadOnly[];
export type StreamGroupVideoStreamsGetStreamGroupVideoStreamIdsApiArg = number;
export type StreamGroupVideoStreamsGetPagedStreamGroupVideoStreamsApiResponse =
  /** status 200  */ PagedResponseOfVideoStreamDto;
export type StreamGroupVideoStreamsGetPagedStreamGroupVideoStreamsApiArg = {
  streamGroupId?: number;
  pageNumber?: number;
  pageSize?: number;
  orderBy?: string;
  jsonArgumentString?: string | null;
  jsonFiltersString?: string | null;
};
export type StreamGroupVideoStreamsSetVideoStreamRanksApiResponse = unknown;
export type StreamGroupVideoStreamsSetVideoStreamRanksApiArg =
  SetVideoStreamRanksRequest;
export type StreamGroupVideoStreamsSyncVideoStreamToStreamGroupPostApiResponse =
  unknown;
export type StreamGroupVideoStreamsSyncVideoStreamToStreamGroupPostApiArg =
  SyncVideoStreamToStreamGroupRequest;
export type StreamGroupVideoStreamsSyncVideoStreamToStreamGroupDeleteApiResponse =
  unknown;
export type StreamGroupVideoStreamsSyncVideoStreamToStreamGroupDeleteApiArg =
  SyncVideoStreamToStreamGroupRequest;
export type VideoStreamLinksAddVideoStreamToVideoStreamApiResponse = unknown;
export type VideoStreamLinksAddVideoStreamToVideoStreamApiArg =
  AddVideoStreamToVideoStreamRequest;
export type VideoStreamLinksGetVideoStreamVideoStreamIdsApiResponse =
  /** status 200  */ string[];
export type VideoStreamLinksGetVideoStreamVideoStreamIdsApiArg = string;
export type VideoStreamLinksGetPagedVideoStreamVideoStreamsApiResponse =
  /** status 200  */ PagedResponseOfChildVideoStreamDto;
export type VideoStreamLinksGetPagedVideoStreamVideoStreamsApiArg = {
  pageNumber?: number;
  pageSize?: number;
  orderBy?: string;
  jsonArgumentString?: string | null;
  jsonFiltersString?: string | null;
};
export type VideoStreamLinksRemoveVideoStreamFromVideoStreamApiResponse =
  unknown;
export type VideoStreamLinksRemoveVideoStreamFromVideoStreamApiArg =
  RemoveVideoStreamFromVideoStreamRequest;
export type VideoStreamsCreateVideoStreamApiResponse = unknown;
export type VideoStreamsCreateVideoStreamApiArg = CreateVideoStreamRequest;
export type VideoStreamsChangeVideoStreamChannelApiResponse = unknown;
export type VideoStreamsChangeVideoStreamChannelApiArg =
  ChangeVideoStreamChannelRequest;
export type VideoStreamsDeleteVideoStreamApiResponse = unknown;
export type VideoStreamsDeleteVideoStreamApiArg = DeleteVideoStreamRequest;
export type VideoStreamsFailClientApiResponse = unknown;
export type VideoStreamsFailClientApiArg = FailClientRequest;
export type VideoStreamsGetAllStatisticsForAllUrlsApiResponse =
  /** status 200  */ StreamStatisticsResult[];
export type VideoStreamsGetAllStatisticsForAllUrlsApiArg = void;
export type VideoStreamsGetChannelLogoDtosApiResponse =
  /** status 200  */ ChannelLogoDto[];
export type VideoStreamsGetChannelLogoDtosApiArg = void;
export type VideoStreamsGetVideoStreamApiResponse =
  /** status 200  */ VideoStreamDto;
export type VideoStreamsGetVideoStreamApiArg = string;
export type VideoStreamsGetPagedVideoStreamsApiResponse =
  /** status 200  */ PagedResponseOfVideoStreamDto;
export type VideoStreamsGetPagedVideoStreamsApiArg = {
  pageNumber?: number;
  pageSize?: number;
  orderBy?: string;
  jsonArgumentString?: string | null;
  jsonFiltersString?: string | null;
};
export type VideoStreamsGetVideoStreamStreamApiResponse = unknown;
export type VideoStreamsGetVideoStreamStreamApiArg = string;
export type VideoStreamsGetVideoStreamStream2ApiResponse = unknown;
export type VideoStreamsGetVideoStreamStream2ApiArg = string;
export type VideoStreamsGetVideoStreamStream3ApiResponse = unknown;
export type VideoStreamsGetVideoStreamStream3ApiArg = {
  encodedIds: string;
  name: string;
};
export type VideoStreamsReSetVideoStreamsLogoApiResponse = unknown;
export type VideoStreamsReSetVideoStreamsLogoApiArg =
  ReSetVideoStreamsLogoRequest;
export type VideoStreamsSetVideoStreamChannelNumbersApiResponse = unknown;
export type VideoStreamsSetVideoStreamChannelNumbersApiArg =
  SetVideoStreamChannelNumbersRequest;
export type VideoStreamsSetVideoStreamsLogoFromEpgApiResponse = unknown;
export type VideoStreamsSetVideoStreamsLogoFromEpgApiArg =
  SetVideoStreamsLogoFromEpgRequest;
export type VideoStreamsUpdateVideoStreamApiResponse = unknown;
export type VideoStreamsUpdateVideoStreamApiArg = UpdateVideoStreamRequest;
export type VideoStreamsUpdateVideoStreamsApiResponse = unknown;
export type VideoStreamsUpdateVideoStreamsApiArg = UpdateVideoStreamsRequest;
export type VideoStreamsUpdateAllVideoStreamsFromParametersApiResponse =
  unknown;
export type VideoStreamsUpdateAllVideoStreamsFromParametersApiArg =
  UpdateAllVideoStreamsFromParametersRequest;
export type VideoStreamsDeleteAllVideoStreamsFromParametersApiResponse =
  unknown;
export type VideoStreamsDeleteAllVideoStreamsFromParametersApiArg =
  DeleteAllVideoStreamsFromParametersRequest;
export type VideoStreamsSetVideoStreamChannelNumbersFromParametersApiResponse =
  unknown;
export type VideoStreamsSetVideoStreamChannelNumbersFromParametersApiArg =
  SetVideoStreamChannelNumbersFromParametersRequest;
export type VideoStreamsSetVideoStreamsLogoFromEpgFromParametersApiResponse =
  unknown;
export type VideoStreamsSetVideoStreamsLogoFromEpgFromParametersApiArg =
  SetVideoStreamsLogoFromEpgFromParametersRequest;
export type VideoStreamsReSetVideoStreamsLogoFromParametersApiResponse =
  unknown;
export type VideoStreamsReSetVideoStreamsLogoFromParametersApiArg =
  ReSetVideoStreamsLogoFromParametersRequest;
export type VideoStreamsSimulateStreamFailureForAllApiResponse = unknown;
export type VideoStreamsSimulateStreamFailureForAllApiArg = void;
export type VideoStreamsSimulateStreamFailureApiResponse = unknown;
export type VideoStreamsSimulateStreamFailureApiArg =
  SimulateStreamFailureRequest;
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
  isHidden?: boolean;
  isReadOnly?: boolean;
  name: string;
};
export type ChannelGroupDto = ChannelGroupArg & {
  id?: number;
};
export type ChannelGroupIdName = {
  id?: number;
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
  epgRank?: number;
  formFile?: Blob | null;
  name?: string;
  urlSource?: string | null;
};
export type DeleteEpgFileRequest = {
  deleteFile?: boolean;
  id?: number;
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
  channelCount?: number;
  epgStartDate?: string;
  epgStopDate?: string;
  programmeCount?: number;
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
  epgRank?: number | null;
};
export type SmFileTypes = 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9;
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
  id?: number;
  logLevel?: LogLevel;
  logLevelName?: string;
  message?: string;
  timeStamp?: string;
};
export type LogEntryDto = LogEntry & object;
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
  startingChannelNumber?: number;
  maxStreamCount?: number;
  stationCount?: number;
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
export type TvAudio = {
  stereo?: string | null;
};
export type TvCategory = {
  lang?: string | null;
  text?: string | null;
};
export type TvActor = {
  role?: string | null;
  text?: string | null;
};
export type TvCredits = {
  actor?: TvActor[] | null;
  director?: string[] | null;
  producer?: string[] | null;
  writer?: string[] | null;
};
export type TvDesc = {
  lang?: string | null;
  text?: string | null;
};
export type TvEpisodenum = {
  system?: string | null;
  text?: string | null;
};
export type TvIcon = {
  height?: string | null;
  src?: string | null;
  width?: string | null;
};
export type TvPreviouslyshown = {
  start?: string | null;
};
export type TvRating = {
  system?: string | null;
  value?: string | null;
};
export type TvSubtitle = {
  lang?: string | null;
  text?: string | null;
};
export type TvTitle = {
  lang?: string | null;
  text?: string | null;
};
export type TvVideo = {
  quality?: string | null;
};
export type Programme = {
  audio?: TvAudio;
  category?: TvCategory[];
  channel?: string;
  channelName?: string;
  displayName?: string;
  credits?: TvCredits;
  desc?: TvDesc;
  epgFileId?: number;
  episodenum?: TvEpisodenum[];
  icon?: TvIcon[];
  language?: string;
  new?: string | null;
  live?: string | null;
  premiere?: string | null;
  previouslyshown?: TvPreviouslyshown | null;
  rating?: TvRating;
  start?: string;
  startDateTime?: string;
  stop?: string;
  stopDateTime?: string;
  subtitle?: TvSubtitle;
  title?: TvTitle;
  video?: TvVideo;
};
export type ProgrammeChannel = {
  channel?: string;
  endDateTime?: string;
  epgFileId?: number;
  programmeCount?: number;
  startDateTime?: string;
};
export type ProgrammeNameDto = {
  id: string;
  channel: string;
  channelName: string;
  displayName: string;
};
export type PagedResponseOfProgrammeNameDto = {
  data: ProgrammeNameDto[];
  pageNumber: number;
  pageSize: number;
  totalPageCount: number;
  totalItemCount: number;
  first: number;
};
export type GetProgrammeFromDisplayNameRequest = {
  value?: string;
};
export type NorthAmerica = {
  fullName?: string;
  shortName?: string;
  postalCodeExample?: string;
  postalCode?: string;
};
export type Europe = {
  fullName?: string;
  shortName?: string;
  postalCodeExample?: string;
  postalCode?: string;
  onePostalCode?: boolean | null;
};
export type LatinAmerica = {
  fullName?: string;
  shortName?: string;
  postalCodeExample?: string;
  postalCode?: string;
  onePostalCode?: boolean | null;
};
export type Caribbean = {
  fullName?: string;
  shortName?: string;
  postalCodeExample?: string;
  postalCode?: string;
  onePostalCode?: boolean;
};
export type Oceanium = {
  fullName?: string;
  shortName?: string;
  postalCodeExample?: string;
  postalCode?: string;
};
export type Countries = {
  "North America"?: NorthAmerica[];
  Europe?: Europe[];
  "Latin America"?: LatinAmerica[];
  Caribbean?: Caribbean[];
  Oceania?: Oceanium[];
};
export type HeadendDto = {
  headend?: string;
  lineup?: string;
  location?: string;
  name?: string;
  transport?: string;
};
export type Map = {
  stationID?: string;
  uhfVhf?: number;
  atscMajor?: number;
  atscMinor?: number;
};
export type Broadcaster = {
  city?: string;
  state?: string;
  postalcode?: string;
  country?: string;
};
export type Logo = {
  URL?: string;
  height?: number;
  width?: number;
  md5?: string;
};
export type StationLogo = Logo & {
  source?: string;
  category?: string;
};
export type Station = {
  affiliate?: string;
  broadcaster?: Broadcaster;
  broadcastLanguage?: string[];
  callsign?: string;
  descriptionLanguage?: string[];
  isCommercialFree?: boolean | null;
  lineUp?: string;
  logo?: Logo;
  name?: string;
  stationID?: string;
  stationLogo?: StationLogo[];
};
export type Metadata = {
  lineup?: string;
  modified?: string;
  transport?: string;
};
export type LineUpResult = {
  map?: Map[];
  stations?: Station[];
  metadata?: Metadata;
};
export type LineUpPreview = {
  id?: number;
  affiliate?: string;
  callsign?: string;
  channel?: string;
  lineUp?: string;
  name?: string;
};
export type Lineup = {
  id?: string;
  lineup?: string;
  name?: string;
  transport?: string;
  location?: string;
  uri?: string;
  isDeleted?: boolean;
};
export type LineUpsResult = {
  code?: number;
  serverID?: string;
  datetime?: string;
  lineups?: Lineup[];
};
export type Program = {
  programID?: string;
  airDateTime?: string;
  duration?: number;
  md5?: string;
  audioProperties?: string[];
  videoProperties?: string[];
  new?: boolean | null;
  liveTapeDelay?: string;
  educational?: boolean | null;
  isPremiereOrFinale?: string;
  premiere?: boolean | null;
};
export type ScheduleMetadata = {
  modified?: string;
  md5?: string;
  startDate?: string;
};
export type Schedule = {
  stationID?: string;
  programs?: Program[];
  metadata?: ScheduleMetadata;
};
export type StationPreview = {
  affiliate?: string;
  callsign?: string;
  id?: number;
  lineUp?: string;
  name?: string;
  stationId?: string;
};
export type Account = {
  expires?: string;
  maxLineups?: number;
  messages?: any[];
};
export type SdSystemstatus = {
  date?: string;
  message?: string;
  status?: string;
};
export type SdStatus = {
  account?: Account;
  code?: number;
  datetime?: string;
  lastDataUpdate?: string;
  lineups?: Lineup[];
  notifications?: any[];
  serverID?: string;
  systemStatus?: SdSystemstatus[];
};
export type TaskQueueStatusDto = {
  command?: string;
  id?: string;
  isRunning?: boolean;
  queueTS?: string;
  startTS?: string;
  stopTS?: string;
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
export type AuthenticationType = 0 | 2;
export type StreamingProxyTypes = 0 | 1 | 2 | 3;
export type BaseSettings = M3USettings & {
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
  epgAlwaysUseVideoStreamName?: boolean;
  ffmPegExecutable?: string;
  globalStreamLimit?: number;
  maxConnectRetry?: number;
  maxConnectRetryTimeMS?: number;
  overWriteM3UChannels?: boolean;
  preloadPercentage?: number;
  ringBufferSizeMB?: number;
  sdCountry?: string;
  sdPassword?: string;
  sdPostalCode?: string;
  sdStationIds?: string[];
  nameRegex?: string[];
  sdUserName?: string;
  sslCertPassword?: string;
  sslCertPath?: string;
  streamingClientUserAgent?: string;
  streamingProxyType?: StreamingProxyTypes;
  videoStreamAlwaysUseEPGLogo?: boolean;
};
export type SettingDto = BaseSettings & {
  release?: string;
  version?: string;
  ffmpegDefaultOptions?: string;
};
export type SystemStatus = {
  isSystemReady?: boolean;
};
export type LogInRequest = {
  password?: string;
  userName?: string;
};
export type UpdateSettingRequest = {
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
  epgAlwaysUseVideoStreamName?: boolean | null;
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
  sdCountry?: string | null;
  sdPassword?: string | null;
  sdPostalCode?: string | null;
  sdStationIds?: string[] | null;
  sdUserName?: string | null;
  sourceBufferPreBufferPercentage?: number | null;
  sslCertPassword?: string | null;
  sslCertPath?: string | null;
  streamingClientUserAgent?: string | null;
  streamingProxyType?: StreamingProxyTypes | null;
  videoStreamAlwaysUseEPGLogo?: boolean | null;
  nameRegex?: string[] | null;
};
export type StreamGroupDto = {
  isLoading: boolean;
  hdhrLink: string;
  isReadOnly: boolean;
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
export type CreateStreamGroupRequest = {
  name: string;
};
export type DeleteStreamGroupRequest = {
  id?: number;
};
export type EpgChannel = {
  channelNumber?: number;
  logo?: string;
  uuid?: string;
};
export type EpgProgram = {
  channelUuid?: string;
  description?: string;
  id?: string;
  image?: string;
  since?: string;
  till?: string;
  title?: string;
  videoStreamId?: string;
};
export type EpgGuide = {
  channels: EpgChannel[];
  endDate: string;
  programs: EpgProgram[];
  startDate: string;
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
};
export type VideoStreamIsReadOnly = {
  rank?: number;
  isReadOnly?: boolean;
  videoStreamId?: string;
};
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
  streamProxyType: StreamingProxyTypes;
  tvg_chno: number;
  tvg_group: string;
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
export type ChildVideoStreamDto = BaseVideoStreamDto & {
  maxStreams: number;
  rank: number;
};
export type VideoStreamDto = BaseVideoStreamDto & {
  isLoading?: boolean;
  channelGroupId?: number;
  rank?: number;
  childVideoStreams?: ChildVideoStreamDto[];
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
export type AddVideoStreamToVideoStreamRequest = {
  parentVideoStreamId: string;
  childVideoStreamId: string;
  rank: number | null;
};
export type PagedResponseOfChildVideoStreamDto = {
  data: ChildVideoStreamDto[];
  pageNumber: number;
  pageSize: number;
  totalPageCount: number;
  totalItemCount: number;
  first: number;
};
export type RemoveVideoStreamFromVideoStreamRequest = {
  parentVideoStreamId: string;
  childVideoStreamId: string;
};
export type VideoStreamBaseRequest = {
  toggleVisibility?: boolean | null;
  isHidden?: boolean | null;
  tvg_chno?: number | null;
  tvg_group?: string | null;
  tvg_ID?: string | null;
  tvg_logo?: string | null;
  tvg_name?: string | null;
  url?: string | null;
  childVideoStreams?: ChildVideoStreamDto[] | null;
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
  m3UStreamId?: string;
  m3UStreamName?: string;
  m3UStreamProxyType?: StreamingProxyTypes;
  rank?: number;
  streamUrl?: string | null;
  videoStreamId?: string;
  videoStreamName?: string;
};
export type ChannelLogoDto = {
  id?: number;
  epgId?: string;
  epgFileId?: number;
  logoUrl?: string;
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
  streamProxyType?: StreamingProxyTypes | null;
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
  useEpgFilesGetEpgFileQuery,
  useEpgFilesGetPagedEpgFilesQuery,
  useEpgFilesProcessEpgFileMutation,
  useEpgFilesRefreshEpgFileMutation,
  useEpgFilesScanDirectoryForEpgFilesMutation,
  useEpgFilesUpdateEpgFileMutation,
  useFilesGetFileQuery,
  useIconsAutoMatchIconToStreamsMutation,
  useIconsGetIconQuery,
  useIconsGetIconFromSourceQuery,
  useIconsGetPagedIconsQuery,
  useIconsGetIconsSimpleQueryQuery,
  useLogsGetLogRequestQuery,
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
  useMiscBuildIconsCacheFromVideoStreamsMutation,
  useMiscBuildProgIconsCacheFromEpGsRequestMutation,
  useProgrammesGetProgrammeQuery,
  useProgrammesGetProgrammeChannelsQuery,
  useProgrammesGetPagedProgrammeNameSelectionsQuery,
  useProgrammesGetProgrammesQuery,
  useProgrammesGetProgrammeNamesQuery,
  useProgrammesGetProgrammsSimpleQueryQuery,
  useProgrammesGetProgrammeFromDisplayNameQuery,
  useSchedulesDirectGetCountriesQuery,
  useSchedulesDirectGetHeadendsQuery,
  useSchedulesDirectGetLineupQuery,
  useSchedulesDirectGetLineupPreviewsQuery,
  useSchedulesDirectGetLineupsQuery,
  useSchedulesDirectGetSchedulesQuery,
  useSchedulesDirectGetStationPreviewsQuery,
  useSchedulesDirectGetStationsQuery,
  useSchedulesDirectGetStatusQuery,
  useSettingsGetIsSystemReadyQuery,
  useSettingsGetQueueStatusQuery,
  useSettingsGetSettingQuery,
  useSettingsGetSystemStatusQuery,
  useSettingsLogInQuery,
  useSettingsUpdateSettingMutation,
  useStreamGroupChannelGroupSyncStreamGroupChannelGroupsMutation,
  useStreamGroupChannelGroupGetChannelGroupsFromStreamGroupQuery,
  useStreamGroupsCreateStreamGroupMutation,
  useStreamGroupsDeleteStreamGroupMutation,
  useStreamGroupsGetStreamGroupQuery,
  useStreamGroupsGetStreamGroupCapabilityQuery,
  useStreamGroupsGetStreamGroupCapability2Query,
  useStreamGroupsGetStreamGroupCapability3Query,
  useStreamGroupsGetStreamGroupDiscoverQuery,
  useStreamGroupsGetStreamGroupEpgQuery,
  useStreamGroupsGetStreamGroupEpgForGuideQuery,
  useStreamGroupsGetStreamGroupLineUpQuery,
  useStreamGroupsGetStreamGroupLineUpStatusQuery,
  useStreamGroupsGetStreamGroupM3UQuery,
  useStreamGroupsGetPagedStreamGroupsQuery,
  useStreamGroupsUpdateStreamGroupMutation,
  useStreamGroupVideoStreamsGetStreamGroupVideoStreamIdsQuery,
  useStreamGroupVideoStreamsGetPagedStreamGroupVideoStreamsQuery,
  useStreamGroupVideoStreamsSetVideoStreamRanksMutation,
  useStreamGroupVideoStreamsSyncVideoStreamToStreamGroupPostMutation,
  useStreamGroupVideoStreamsSyncVideoStreamToStreamGroupDeleteMutation,
  useVideoStreamLinksAddVideoStreamToVideoStreamMutation,
  useVideoStreamLinksGetVideoStreamVideoStreamIdsQuery,
  useVideoStreamLinksGetPagedVideoStreamVideoStreamsQuery,
  useVideoStreamLinksRemoveVideoStreamFromVideoStreamMutation,
  useVideoStreamsCreateVideoStreamMutation,
  useVideoStreamsChangeVideoStreamChannelMutation,
  useVideoStreamsDeleteVideoStreamMutation,
  useVideoStreamsFailClientMutation,
  useVideoStreamsGetAllStatisticsForAllUrlsQuery,
  useVideoStreamsGetChannelLogoDtosQuery,
  useVideoStreamsGetVideoStreamQuery,
  useVideoStreamsGetPagedVideoStreamsQuery,
  useVideoStreamsGetVideoStreamStreamQuery,
  useVideoStreamsGetVideoStreamStream2Query,
  useVideoStreamsGetVideoStreamStream3Query,
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
} = injectedRtkApi;
