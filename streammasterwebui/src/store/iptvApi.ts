import { emptySplitApi as api } from "./emptyApi";
export const addTagTypes = [
  "ChannelGroups",
  "EPGFiles",
  "Files",
  "Icons",
  "M3UFiles",
  "Misc",
  "Programmes",
  "Settings",
  "StreamGroups",
  "VideoStreams",
] as const;
const injectedRtkApi = api
  .enhanceEndpoints({
    addTagTypes,
  })
  .injectEndpoints({
    endpoints: (build) => ({
      channelGroupsAddChannelGroup: build.mutation<
        ChannelGroupsAddChannelGroupApiResponse,
        ChannelGroupsAddChannelGroupApiArg
      >({
        query: (queryArg) => ({
          url: `/api/channelgroups`,
          method: "POST",
          body: queryArg,
        }),
        invalidatesTags: ["ChannelGroups"],
      }),
      channelGroupsGetChannelGroups: build.query<
        ChannelGroupsGetChannelGroupsApiResponse,
        ChannelGroupsGetChannelGroupsApiArg
      >({
        query: () => ({ url: `/api/channelgroups` }),
        providesTags: ["ChannelGroups"],
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
      channelGroupsSetChannelGroupsVisible: build.mutation<
        ChannelGroupsSetChannelGroupsVisibleApiResponse,
        ChannelGroupsSetChannelGroupsVisibleApiArg
      >({
        query: (queryArg) => ({
          url: `/api/channelgroups/setchannelgroupsvisible`,
          method: "PUT",
          body: queryArg,
        }),
        invalidatesTags: ["ChannelGroups"],
      }),
      channelGroupsUpdateChannelGroup: build.mutation<
        ChannelGroupsUpdateChannelGroupApiResponse,
        ChannelGroupsUpdateChannelGroupApiArg
      >({
        query: (queryArg) => ({
          url: `/api/channelgroups/updatechannelgroup`,
          method: "PUT",
          body: queryArg,
        }),
        invalidatesTags: ["ChannelGroups"],
      }),
      channelGroupsUpdateChannelGroupOrder: build.mutation<
        ChannelGroupsUpdateChannelGroupOrderApiResponse,
        ChannelGroupsUpdateChannelGroupOrderApiArg
      >({
        query: (queryArg) => ({
          url: `/api/channelgroups/updatechannelgrouporder`,
          method: "PUT",
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
          method: "PUT",
          body: queryArg,
        }),
        invalidatesTags: ["ChannelGroups"],
      }),
      epgFilesAddEpgFile: build.mutation<
        EpgFilesAddEpgFileApiResponse,
        EpgFilesAddEpgFileApiArg
      >({
        query: (queryArg) => ({
          url: `/api/epgfiles`,
          method: "POST",
          body: queryArg,
        }),
        invalidatesTags: ["EPGFiles"],
      }),
      epgFilesGetEpgFiles: build.query<
        EpgFilesGetEpgFilesApiResponse,
        EpgFilesGetEpgFilesApiArg
      >({
        query: () => ({ url: `/api/epgfiles` }),
        providesTags: ["EPGFiles"],
      }),
      epgFilesAddEpgFileFromForm: build.mutation<
        EpgFilesAddEpgFileFromFormApiResponse,
        EpgFilesAddEpgFileFromFormApiArg
      >({
        query: (queryArg) => ({
          url: `/api/epgfiles/addepgfilefromform`,
          method: "POST",
          body: queryArg,
        }),
        invalidatesTags: ["EPGFiles"],
      }),
      epgFilesChangeEpgFileName: build.mutation<
        EpgFilesChangeEpgFileNameApiResponse,
        EpgFilesChangeEpgFileNameApiArg
      >({
        query: (queryArg) => ({
          url: `/api/epgfiles/changeepgfilename`,
          method: "PUT",
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
      epgFilesProcessEpgFile: build.mutation<
        EpgFilesProcessEpgFileApiResponse,
        EpgFilesProcessEpgFileApiArg
      >({
        query: (queryArg) => ({
          url: `/api/epgfiles/processepgfile`,
          method: "PUT",
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
          method: "PUT",
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
          method: "PUT",
        }),
        invalidatesTags: ["EPGFiles"],
      }),
      epgFilesUpdateEpgFile: build.mutation<
        EpgFilesUpdateEpgFileApiResponse,
        EpgFilesUpdateEpgFileApiArg
      >({
        query: (queryArg) => ({
          url: `/api/epgfiles/updateepgfile`,
          method: "PUT",
          body: queryArg,
        }),
        invalidatesTags: ["EPGFiles"],
      }),
      filesGetFile: build.query<FilesGetFileApiResponse, FilesGetFileApiArg>({
        query: (queryArg) => ({
          url: `/api/files/${queryArg.filetype}/${queryArg.fileName}`,
        }),
        providesTags: ["Files"],
      }),
      iconsAddIconFile: build.mutation<
        IconsAddIconFileApiResponse,
        IconsAddIconFileApiArg
      >({
        query: (queryArg) => ({
          url: `/api/icons`,
          method: "POST",
          body: queryArg,
        }),
        invalidatesTags: ["Icons"],
      }),
      iconsGetUrl: build.query<IconsGetUrlApiResponse, IconsGetUrlApiArg>({
        query: () => ({ url: `/api/icons` }),
        providesTags: ["Icons"],
      }),
      iconsAddIconFileFromForm: build.mutation<
        IconsAddIconFileFromFormApiResponse,
        IconsAddIconFileFromFormApiArg
      >({
        query: (queryArg) => ({
          url: `/api/icons/addiconfilefromform`,
          method: "POST",
          body: queryArg,
        }),
        invalidatesTags: ["Icons"],
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
      iconsCacheIconsFromVideoStreamsRequest: build.query<
        IconsCacheIconsFromVideoStreamsRequestApiResponse,
        IconsCacheIconsFromVideoStreamsRequestApiArg
      >({
        query: () => ({ url: `/api/icons/cacheiconsfromvideostreamsrequest` }),
        providesTags: ["Icons"],
      }),
      iconsGetIcon: build.query<IconsGetIconApiResponse, IconsGetIconApiArg>({
        query: (queryArg) => ({ url: `/api/icons/geticon/${queryArg}` }),
        providesTags: ["Icons"],
      }),
      iconsGetIcons: build.query<IconsGetIconsApiResponse, IconsGetIconsApiArg>(
        {
          query: () => ({ url: `/api/icons/geticons` }),
          providesTags: ["Icons"],
        }
      ),
      m3UFilesAddM3UFile: build.mutation<
        M3UFilesAddM3UFileApiResponse,
        M3UFilesAddM3UFileApiArg
      >({
        query: (queryArg) => ({
          url: `/api/m3ufiles`,
          method: "POST",
          body: queryArg,
        }),
        invalidatesTags: ["M3UFiles"],
      }),
      m3UFilesGetM3UFiles: build.query<
        M3UFilesGetM3UFilesApiResponse,
        M3UFilesGetM3UFilesApiArg
      >({
        query: () => ({ url: `/api/m3ufiles` }),
        providesTags: ["M3UFiles"],
      }),
      m3UFilesAddM3UFileFromForm: build.mutation<
        M3UFilesAddM3UFileFromFormApiResponse,
        M3UFilesAddM3UFileFromFormApiArg
      >({
        query: (queryArg) => ({
          url: `/api/m3ufiles/addm3ufilefromform`,
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
          method: "PUT",
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
      m3UFilesProcessM3UFile: build.mutation<
        M3UFilesProcessM3UFileApiResponse,
        M3UFilesProcessM3UFileApiArg
      >({
        query: (queryArg) => ({
          url: `/api/m3ufiles/processm3ufile`,
          method: "PUT",
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
          method: "PUT",
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
          method: "PUT",
        }),
        invalidatesTags: ["M3UFiles"],
      }),
      m3UFilesUpdateM3UFile: build.mutation<
        M3UFilesUpdateM3UFileApiResponse,
        M3UFilesUpdateM3UFileApiArg
      >({
        query: (queryArg) => ({
          url: `/api/m3ufiles/updatem3ufile`,
          method: "PUT",
          body: queryArg,
        }),
        invalidatesTags: ["M3UFiles"],
      }),
      miscCacheAllIcons: build.mutation<
        MiscCacheAllIconsApiResponse,
        MiscCacheAllIconsApiArg
      >({
        query: () => ({ url: `/api/misc/cacheallicons`, method: "PUT" }),
        invalidatesTags: ["Misc"],
      }),
      miscCacheIconsFromEpGs: build.mutation<
        MiscCacheIconsFromEpGsApiResponse,
        MiscCacheIconsFromEpGsApiArg
      >({
        query: () => ({ url: `/api/misc/cacheiconsfromepgs`, method: "PUT" }),
        invalidatesTags: ["Misc"],
      }),
      miscCacheIconsFromVideoStreams: build.mutation<
        MiscCacheIconsFromVideoStreamsApiResponse,
        MiscCacheIconsFromVideoStreamsApiArg
      >({
        query: () => ({
          url: `/api/misc/cacheiconsfromvideostreams`,
          method: "PUT",
        }),
        invalidatesTags: ["Misc"],
      }),
      miscReadDirectoryLogosRequest: build.mutation<
        MiscReadDirectoryLogosRequestApiResponse,
        MiscReadDirectoryLogosRequestApiArg
      >({
        query: () => ({
          url: `/api/misc/readdirectorylogosrequest`,
          method: "PUT",
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
      programmesGetProgrammeNames: build.query<
        ProgrammesGetProgrammeNamesApiResponse,
        ProgrammesGetProgrammeNamesApiArg
      >({
        query: () => ({ url: `/api/programmes/getprogrammenames` }),
        providesTags: ["Programmes"],
      }),
      programmesGetProgrammes: build.query<
        ProgrammesGetProgrammesApiResponse,
        ProgrammesGetProgrammesApiArg
      >({
        query: () => ({ url: `/api/programmes/getprogrammes` }),
        providesTags: ["Programmes"],
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
        query: () => ({ url: `/api/settings` }),
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
          method: "PUT",
          body: queryArg,
        }),
        invalidatesTags: ["Settings"],
      }),
      streamGroupsAddStreamGroup: build.mutation<
        StreamGroupsAddStreamGroupApiResponse,
        StreamGroupsAddStreamGroupApiArg
      >({
        query: (queryArg) => ({
          url: `/api/streamgroups`,
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
      streamGroupsGetAllStatisticsForAllUrls: build.query<
        StreamGroupsGetAllStatisticsForAllUrlsApiResponse,
        StreamGroupsGetAllStatisticsForAllUrlsApiArg
      >({
        query: () => ({ url: `/api/streamgroups/getallstatisticsforallurls` }),
        providesTags: ["StreamGroups"],
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
      streamGroupsGetStreamGroupByStreamNumber: build.query<
        StreamGroupsGetStreamGroupByStreamNumberApiResponse,
        StreamGroupsGetStreamGroupByStreamNumberApiArg
      >({
        query: (queryArg) => ({
          url: `/api/streamgroups/getstreamgroupbystreamnumber/${queryArg}`,
        }),
        providesTags: ["StreamGroups"],
      }),
      streamGroupsGetStreamGroupCapability: build.query<
        StreamGroupsGetStreamGroupCapabilityApiResponse,
        StreamGroupsGetStreamGroupCapabilityApiArg
      >({
        query: (queryArg) => ({
          url: `/api/streamgroups/${queryArg}/capability`,
        }),
        providesTags: ["StreamGroups"],
      }),
      streamGroupsGetStreamGroupCapability2: build.query<
        StreamGroupsGetStreamGroupCapability2ApiResponse,
        StreamGroupsGetStreamGroupCapability2ApiArg
      >({
        query: (queryArg) => ({ url: `/api/streamgroups/${queryArg}` }),
        providesTags: ["StreamGroups"],
      }),
      streamGroupsGetStreamGroupDeviceXml: build.query<
        StreamGroupsGetStreamGroupDeviceXmlApiResponse,
        StreamGroupsGetStreamGroupDeviceXmlApiArg
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
        query: (queryArg) => ({ url: `/api/streamgroups/${queryArg}/m3u` }),
        providesTags: ["StreamGroups"],
      }),
      streamGroupsGetStreamGroupM3U2: build.query<
        StreamGroupsGetStreamGroupM3U2ApiResponse,
        StreamGroupsGetStreamGroupM3U2ApiArg
      >({
        query: (queryArg) => ({
          url: `/api/streamgroups/${queryArg}/m3u/m3u.m3u`,
        }),
        providesTags: ["StreamGroups"],
      }),
      streamGroupsGetStreamGroupM3U22: build.query<
        StreamGroupsGetStreamGroupM3U22ApiResponse,
        StreamGroupsGetStreamGroupM3U22ApiArg
      >({
        query: (queryArg) => ({ url: `/api/streamgroups/${queryArg}/m3u2` }),
        providesTags: ["StreamGroups"],
      }),
      streamGroupsGetStreamGroupM3U23: build.query<
        StreamGroupsGetStreamGroupM3U23ApiResponse,
        StreamGroupsGetStreamGroupM3U23ApiArg
      >({
        query: (queryArg) => ({
          url: `/api/streamgroups/${queryArg}/m3u2/m3u.m3u`,
        }),
        providesTags: ["StreamGroups"],
      }),
      streamGroupsGetStreamGroups: build.query<
        StreamGroupsGetStreamGroupsApiResponse,
        StreamGroupsGetStreamGroupsApiArg
      >({
        query: () => ({ url: `/api/streamgroups/getstreamgroups` }),
        providesTags: ["StreamGroups"],
      }),
      streamGroupsGetStreamGroupVideoM3UStream: build.query<
        StreamGroupsGetStreamGroupVideoM3UStreamApiResponse,
        StreamGroupsGetStreamGroupVideoM3UStreamApiArg
      >({
        query: (queryArg) => ({
          url: `/api/streamgroups/${queryArg.streamGroupNumber}/stream/${queryArg.streamId}/${queryArg.clientId}.ts`,
        }),
        providesTags: ["StreamGroups"],
      }),
      streamGroupsGetStreamGroupVideoStream: build.query<
        StreamGroupsGetStreamGroupVideoStreamApiResponse,
        StreamGroupsGetStreamGroupVideoStreamApiArg
      >({
        query: (queryArg) => ({
          url: `/api/streamgroups/${queryArg.streamGroupNumber}/stream/${queryArg.streamId}`,
        }),
        providesTags: ["StreamGroups"],
      }),
      streamGroupsGetStreamGroupVideoStream2: build.query<
        StreamGroupsGetStreamGroupVideoStream2ApiResponse,
        StreamGroupsGetStreamGroupVideoStream2ApiArg
      >({
        query: (queryArg) => ({
          url: `/api/streamgroups/${queryArg.streamGroupNumber}/stream/${queryArg.streamId}.mp4`,
        }),
        providesTags: ["StreamGroups"],
      }),
      streamGroupsGetStreamM3U8: build.query<
        StreamGroupsGetStreamM3U8ApiResponse,
        StreamGroupsGetStreamM3U8ApiArg
      >({
        query: (queryArg) => ({
          url: `/api/streamgroups/${queryArg.streamGroupNumber}/stream/${queryArg.streamId}.m3u8`,
        }),
        providesTags: ["StreamGroups"],
      }),
      streamGroupsGetStreamM3U8WithClientId: build.query<
        StreamGroupsGetStreamM3U8WithClientIdApiResponse,
        StreamGroupsGetStreamM3U8WithClientIdApiArg
      >({
        query: (queryArg) => ({
          url: `/api/streamgroups/${queryArg.streamGroupNumber}/stream/${queryArg.streamId}/${queryArg.clientId}.m3u8`,
        }),
        providesTags: ["StreamGroups"],
      }),
      streamGroupsSimulateStreamFailure: build.mutation<
        StreamGroupsSimulateStreamFailureApiResponse,
        StreamGroupsSimulateStreamFailureApiArg
      >({
        query: (queryArg) => ({
          url: `/api/streamgroups/simulatestreamfailure/${queryArg}`,
          method: "POST",
        }),
        invalidatesTags: ["StreamGroups"],
      }),
      streamGroupsUpdateStreamGroup: build.mutation<
        StreamGroupsUpdateStreamGroupApiResponse,
        StreamGroupsUpdateStreamGroupApiArg
      >({
        query: (queryArg) => ({
          url: `/api/streamgroups/updatestreamgroup`,
          method: "PUT",
          body: queryArg,
        }),
        invalidatesTags: ["StreamGroups"],
      }),
      videoStreamsAddVideoStream: build.mutation<
        VideoStreamsAddVideoStreamApiResponse,
        VideoStreamsAddVideoStreamApiArg
      >({
        query: (queryArg) => ({
          url: `/api/videostreams`,
          method: "POST",
          body: queryArg,
        }),
        invalidatesTags: ["VideoStreams"],
      }),
      videoStreamsGetVideoStreams: build.query<
        VideoStreamsGetVideoStreamsApiResponse,
        VideoStreamsGetVideoStreamsApiArg
      >({
        query: () => ({ url: `/api/videostreams` }),
        providesTags: ["VideoStreams"],
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
      videoStreamsGetVideoStream: build.query<
        VideoStreamsGetVideoStreamApiResponse,
        VideoStreamsGetVideoStreamApiArg
      >({
        query: (queryArg) => ({ url: `/api/videostreams/${queryArg}` }),
        providesTags: ["VideoStreams"],
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
      videoStreamsUpdateVideoStream: build.mutation<
        VideoStreamsUpdateVideoStreamApiResponse,
        VideoStreamsUpdateVideoStreamApiArg
      >({
        query: (queryArg) => ({
          url: `/api/videostreams/updatevideostream`,
          method: "PUT",
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
          method: "PUT",
          body: queryArg,
        }),
        invalidatesTags: ["VideoStreams"],
      }),
    }),
    overrideExisting: false,
  });
export { injectedRtkApi as iptvApi };
export type ChannelGroupsAddChannelGroupApiResponse = /** status 200  */
  | undefined
  | /** status 201  */ ChannelGroupDto;
export type ChannelGroupsAddChannelGroupApiArg = AddChannelGroupRequest;
export type ChannelGroupsGetChannelGroupsApiResponse =
  /** status 200  */ ChannelGroupDto[];
export type ChannelGroupsGetChannelGroupsApiArg = void;
export type ChannelGroupsDeleteChannelGroupApiResponse =
  /** status 200  */ undefined;
export type ChannelGroupsDeleteChannelGroupApiArg = DeleteChannelGroupRequest;
export type ChannelGroupsGetChannelGroupApiResponse =
  /** status 200  */ ChannelGroupDto;
export type ChannelGroupsGetChannelGroupApiArg = number;
export type ChannelGroupsSetChannelGroupsVisibleApiResponse =
  /** status 204  */ undefined;
export type ChannelGroupsSetChannelGroupsVisibleApiArg =
  SetChannelGroupsVisibleRequest;
export type ChannelGroupsUpdateChannelGroupApiResponse =
  /** status 204  */ undefined;
export type ChannelGroupsUpdateChannelGroupApiArg = UpdateChannelGroupRequest;
export type ChannelGroupsUpdateChannelGroupOrderApiResponse =
  /** status 204  */ undefined;
export type ChannelGroupsUpdateChannelGroupOrderApiArg =
  UpdateChannelGroupOrderRequest;
export type ChannelGroupsUpdateChannelGroupsApiResponse =
  /** status 204  */ undefined;
export type ChannelGroupsUpdateChannelGroupsApiArg = UpdateChannelGroupsRequest;
export type EpgFilesAddEpgFileApiResponse = /** status 200  */
  | undefined
  | /** status 201  */ EpgFilesDto;
export type EpgFilesAddEpgFileApiArg = AddEpgFileRequest;
export type EpgFilesGetEpgFilesApiResponse = /** status 200  */ EpgFilesDto[];
export type EpgFilesGetEpgFilesApiArg = void;
export type EpgFilesAddEpgFileFromFormApiResponse = /** status 200  */
  | undefined
  | /** status 201  */ EpgFilesDto;
export type EpgFilesAddEpgFileFromFormApiArg = {
  Description?: string | null;
  EPGRank?: number;
  FormFile?: Blob | null;
  Name?: string;
  UrlSource?: string | null;
};
export type EpgFilesChangeEpgFileNameApiResponse = /** status 204  */ undefined;
export type EpgFilesChangeEpgFileNameApiArg = ChangeEpgFileNameRequest;
export type EpgFilesDeleteEpgFileApiResponse = /** status 200  */ undefined;
export type EpgFilesDeleteEpgFileApiArg = DeleteEpgFileRequest;
export type EpgFilesGetEpgFileApiResponse = /** status 200  */ EpgFilesDto;
export type EpgFilesGetEpgFileApiArg = number;
export type EpgFilesProcessEpgFileApiResponse = /** status 204  */ undefined;
export type EpgFilesProcessEpgFileApiArg = ProcessEpgFileRequest;
export type EpgFilesRefreshEpgFileApiResponse = /** status 204  */ undefined;
export type EpgFilesRefreshEpgFileApiArg = RefreshEpgFileRequest;
export type EpgFilesScanDirectoryForEpgFilesApiResponse =
  /** status 204  */ undefined;
export type EpgFilesScanDirectoryForEpgFilesApiArg = void;
export type EpgFilesUpdateEpgFileApiResponse = /** status 204  */ undefined;
export type EpgFilesUpdateEpgFileApiArg = UpdateEpgFileRequest;
export type FilesGetFileApiResponse = unknown;
export type FilesGetFileApiArg = {
  fileName: string;
  filetype: SmFileTypes;
};
export type IconsAddIconFileApiResponse = /** status 200  */
  | undefined
  | /** status 201  */ IconFileDto;
export type IconsAddIconFileApiArg = AddIconFileRequest;
export type IconsGetUrlApiResponse = /** status 200  */ string;
export type IconsGetUrlApiArg = void;
export type IconsAddIconFileFromFormApiResponse = /** status 200  */
  | undefined
  | /** status 201  */ IconFileDto;
export type IconsAddIconFileFromFormApiArg = {
  Description?: string | null;
  FormFile?: Blob | null;
  Name?: string;
  UrlSource?: string | null;
};
export type IconsAutoMatchIconToStreamsApiResponse =
  /** status 200  */ undefined;
export type IconsAutoMatchIconToStreamsApiArg = AutoMatchIconToStreamsRequest;
export type IconsCacheIconsFromVideoStreamsRequestApiResponse =
  /** status 200  */ IconFileDto[];
export type IconsCacheIconsFromVideoStreamsRequestApiArg = void;
export type IconsGetIconApiResponse = /** status 200  */ IconFileDto;
export type IconsGetIconApiArg = number;
export type IconsGetIconsApiResponse = /** status 200  */ IconFileDto[];
export type IconsGetIconsApiArg = void;
export type M3UFilesAddM3UFileApiResponse = /** status 200  */
  | undefined
  | /** status 201  */ M3UFilesDto;
export type M3UFilesAddM3UFileApiArg = AddM3UFileRequest;
export type M3UFilesGetM3UFilesApiResponse = /** status 200  */ M3UFilesDto[];
export type M3UFilesGetM3UFilesApiArg = void;
export type M3UFilesAddM3UFileFromFormApiResponse = /** status 200  */
  | undefined
  | /** status 201  */ M3UFilesDto;
export type M3UFilesAddM3UFileFromFormApiArg = {
  Description?: string | null;
  FormFile?: Blob | null;
  MaxStreamCount?: number;
  MetaData?: string | null;
  StartingChannelNumber?: number | null;
  Name?: string;
  UrlSource?: string | null;
};
export type M3UFilesChangeM3UFileNameApiResponse = /** status 204  */ undefined;
export type M3UFilesChangeM3UFileNameApiArg = ChangeM3UFileNameRequest;
export type M3UFilesDeleteM3UFileApiResponse = /** status 200  */ undefined;
export type M3UFilesDeleteM3UFileApiArg = DeleteM3UFileRequest;
export type M3UFilesGetM3UFileApiResponse = /** status 200  */ M3UFilesDto;
export type M3UFilesGetM3UFileApiArg = number;
export type M3UFilesProcessM3UFileApiResponse = /** status 204  */ undefined;
export type M3UFilesProcessM3UFileApiArg = ProcessM3UFileRequest;
export type M3UFilesRefreshM3UFileApiResponse = /** status 204  */ undefined;
export type M3UFilesRefreshM3UFileApiArg = RefreshM3UFileRequest;
export type M3UFilesScanDirectoryForM3UFilesApiResponse =
  /** status 204  */ undefined;
export type M3UFilesScanDirectoryForM3UFilesApiArg = void;
export type M3UFilesUpdateM3UFileApiResponse = /** status 204  */ undefined;
export type M3UFilesUpdateM3UFileApiArg = UpdateM3UFileRequest;
export type MiscCacheAllIconsApiResponse = /** status 204  */ undefined;
export type MiscCacheAllIconsApiArg = void;
export type MiscCacheIconsFromEpGsApiResponse = /** status 204  */ undefined;
export type MiscCacheIconsFromEpGsApiArg = void;
export type MiscCacheIconsFromVideoStreamsApiResponse =
  /** status 204  */ undefined;
export type MiscCacheIconsFromVideoStreamsApiArg = void;
export type MiscReadDirectoryLogosRequestApiResponse =
  /** status 204  */ undefined;
export type MiscReadDirectoryLogosRequestApiArg = void;
export type ProgrammesGetProgrammeApiResponse = /** status 200  */ Programme[];
export type ProgrammesGetProgrammeApiArg = string;
export type ProgrammesGetProgrammeChannelsApiResponse =
  /** status 200  */ ProgrammeChannel[];
export type ProgrammesGetProgrammeChannelsApiArg = void;
export type ProgrammesGetProgrammeNamesApiResponse =
  /** status 200  */ ProgrammeName[];
export type ProgrammesGetProgrammeNamesApiArg = void;
export type ProgrammesGetProgrammesApiResponse = /** status 200  */ Programme[];
export type ProgrammesGetProgrammesApiArg = void;
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
export type SettingsUpdateSettingApiResponse = /** status 204  */ undefined;
export type SettingsUpdateSettingApiArg = UpdateSettingRequest;
export type StreamGroupsAddStreamGroupApiResponse = /** status 200  */
  | undefined
  | /** status 201  */ StreamGroupDto;
export type StreamGroupsAddStreamGroupApiArg = AddStreamGroupRequest;
export type StreamGroupsDeleteStreamGroupApiResponse =
  /** status 200  */ undefined;
export type StreamGroupsDeleteStreamGroupApiArg = DeleteStreamGroupRequest;
export type StreamGroupsGetAllStatisticsForAllUrlsApiResponse =
  /** status 200  */ StreamStatisticsResult[];
export type StreamGroupsGetAllStatisticsForAllUrlsApiArg = void;
export type StreamGroupsGetStreamGroupApiResponse =
  /** status 200  */ StreamGroupDto;
export type StreamGroupsGetStreamGroupApiArg = number;
export type StreamGroupsGetStreamGroupByStreamNumberApiResponse =
  /** status 200  */ StreamGroupDto;
export type StreamGroupsGetStreamGroupByStreamNumberApiArg = number;
export type StreamGroupsGetStreamGroupCapabilityApiResponse =
  /** status 200  */ string;
export type StreamGroupsGetStreamGroupCapabilityApiArg = number;
export type StreamGroupsGetStreamGroupCapability2ApiResponse =
  /** status 200  */ string;
export type StreamGroupsGetStreamGroupCapability2ApiArg = number;
export type StreamGroupsGetStreamGroupDeviceXmlApiResponse =
  /** status 200  */ string;
export type StreamGroupsGetStreamGroupDeviceXmlApiArg = number;
export type StreamGroupsGetStreamGroupDiscoverApiResponse =
  /** status 200  */ string;
export type StreamGroupsGetStreamGroupDiscoverApiArg = number;
export type StreamGroupsGetStreamGroupEpgApiResponse =
  /** status 200  */ string;
export type StreamGroupsGetStreamGroupEpgApiArg = number;
export type StreamGroupsGetStreamGroupEpgForGuideApiResponse =
  /** status 200  */ EpgGuide;
export type StreamGroupsGetStreamGroupEpgForGuideApiArg = number;
export type StreamGroupsGetStreamGroupLineUpApiResponse =
  /** status 200  */ string;
export type StreamGroupsGetStreamGroupLineUpApiArg = number;
export type StreamGroupsGetStreamGroupLineUpStatusApiResponse =
  /** status 200  */ string;
export type StreamGroupsGetStreamGroupLineUpStatusApiArg = number;
export type StreamGroupsGetStreamGroupM3UApiResponse =
  /** status 200  */ string;
export type StreamGroupsGetStreamGroupM3UApiArg = number;
export type StreamGroupsGetStreamGroupM3U2ApiResponse =
  /** status 200  */ string;
export type StreamGroupsGetStreamGroupM3U2ApiArg = number;
export type StreamGroupsGetStreamGroupM3U22ApiResponse =
  /** status 200  */ string;
export type StreamGroupsGetStreamGroupM3U22ApiArg = number;
export type StreamGroupsGetStreamGroupM3U23ApiResponse =
  /** status 200  */ string;
export type StreamGroupsGetStreamGroupM3U23ApiArg = number;
export type StreamGroupsGetStreamGroupsApiResponse =
  /** status 200  */ StreamGroupDto[];
export type StreamGroupsGetStreamGroupsApiArg = void;
export type StreamGroupsGetStreamGroupVideoM3UStreamApiResponse = unknown;
export type StreamGroupsGetStreamGroupVideoM3UStreamApiArg = {
  streamGroupNumber: number;
  streamId: number;
  clientId: string;
};
export type StreamGroupsGetStreamGroupVideoStreamApiResponse = unknown;
export type StreamGroupsGetStreamGroupVideoStreamApiArg = {
  streamGroupNumber: number;
  streamId: number;
};
export type StreamGroupsGetStreamGroupVideoStream2ApiResponse = unknown;
export type StreamGroupsGetStreamGroupVideoStream2ApiArg = {
  streamGroupNumber: number;
  streamId: number;
};
export type StreamGroupsGetStreamM3U8ApiResponse = /** status 200  */ string;
export type StreamGroupsGetStreamM3U8ApiArg = {
  streamGroupNumber: number;
  streamId: number;
};
export type StreamGroupsGetStreamM3U8WithClientIdApiResponse =
  /** status 200  */ string;
export type StreamGroupsGetStreamM3U8WithClientIdApiArg = {
  streamGroupNumber: number;
  streamId: number;
  clientId: string;
};
export type StreamGroupsSimulateStreamFailureApiResponse =
  /** status 200  */ undefined;
export type StreamGroupsSimulateStreamFailureApiArg = string;
export type StreamGroupsUpdateStreamGroupApiResponse =
  /** status 204  */ undefined;
export type StreamGroupsUpdateStreamGroupApiArg = UpdateStreamGroupRequest;
export type VideoStreamsAddVideoStreamApiResponse = /** status 200  */
  | undefined
  | /** status 201  */ VideoStreamDto;
export type VideoStreamsAddVideoStreamApiArg = AddVideoStreamRequest;
export type VideoStreamsGetVideoStreamsApiResponse =
  /** status 200  */ VideoStreamDto[];
export type VideoStreamsGetVideoStreamsApiArg = void;
export type VideoStreamsDeleteVideoStreamApiResponse =
  /** status 200  */ undefined;
export type VideoStreamsDeleteVideoStreamApiArg = DeleteVideoStreamRequest;
export type VideoStreamsGetVideoStreamApiResponse =
  /** status 200  */ VideoStreamDto;
export type VideoStreamsGetVideoStreamApiArg = number;
export type VideoStreamsSetVideoStreamChannelNumbersApiResponse =
  /** status 200  */ ChannelNumberPair[] | /** status 204  */ undefined;
export type VideoStreamsSetVideoStreamChannelNumbersApiArg =
  SetVideoStreamChannelNumbersRequest;
export type VideoStreamsUpdateVideoStreamApiResponse =
  /** status 204  */ undefined;
export type VideoStreamsUpdateVideoStreamApiArg = UpdateVideoStreamRequest;
export type VideoStreamsUpdateVideoStreamsApiResponse =
  /** status 204  */ undefined;
export type VideoStreamsUpdateVideoStreamsApiArg = UpdateVideoStreamsRequest;
export type ChannelGroupArg = {
  isHidden: boolean | null;
  isReadOnly: boolean | null;
  name: string;
  rank: number;
  regexMatch: string | null;
};
export type ChannelGroupDto = ChannelGroupArg & {
  id: number;
};
export type ProblemDetails = {
  type?: string | null;
  title?: string | null;
  status?: number | null;
  detail?: string | null;
  instance?: string | null;
  [key: string]: any | null;
};
export type AddChannelGroupRequest = {
  groupName: string;
  rank: number;
};
export type DeleteChannelGroupRequest = {
  groupName: string;
};
export type SetChannelGroupsVisibleArg = {
  groupName?: string;
  isHidden?: boolean;
};
export type SetChannelGroupsVisibleRequest = {
  requests: SetChannelGroupsVisibleArg[];
};
export type UpdateChannelGroupRequest = {
  groupName: string;
  newGroupName: string | null;
  isHidden: boolean | null;
  rank: number | null;
};
export type UpdateChannelGroupOrderRequest = {
  channelGroups: ChannelGroupArg[] | null;
};
export type UpdateChannelGroupsRequest = {
  channelGroupRequests: UpdateChannelGroupRequest[];
};
export type BaseFileDto = {
  autoUpdate: boolean;
  daysToUpdate: number;
  description: string;
  downloadErrors: number;
  id: number;
  lastDownloadAttempt: string;
  lastDownloaded: string;
  metaData: string;
  name: string;
  needsUpdate: boolean;
  originalSource: string;
  url: string;
};
export type EpgFilesDto = BaseFileDto & {
  channelCount?: number;
  epgRank?: number;
  epgStartDate?: string;
  epgStopDate?: string;
  programmeCount?: number;
};
export type AddEpgFileRequest = {
  description?: string | null;
  epgRank?: number;
  formFile?: Blob | null;
  name: string;
  urlSource?: string | null;
};
export type ChangeEpgFileNameRequest = {
  id?: number;
  name?: string;
};
export type DeleteEpgFileRequest = {
  deleteFile?: boolean;
  id?: number;
};
export type ProcessEpgFileRequest = {
  epgFileId: number;
};
export type RefreshEpgFileRequest = {
  epgFileID: number;
};
export type BaseFileRequest = {
  autoUpdate?: boolean | null;
  daysToUpdate?: number | null;
  description?: string | null;
  id: number;
  metaData?: string | null;
  name?: string | null;
  url?: string | null;
};
export type UpdateEpgFileRequest = BaseFileRequest & {
  epgRank?: number | null;
};
export type SmFileTypes = 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8;
export type IconFileDto = {
  fileExists: boolean;
  id: number;
  name: string;
  originalSource: string;
  smFileType: SmFileTypes;
  source: string;
  url: string;
};
export type AddIconFileRequest = {
  description?: string | null;
  formFile?: Blob | null;
  name: string;
  urlSource?: string | null;
};
export type AutoMatchIconToStreamsRequest = {
  ids?: number[];
};
export type M3UFilesDto = BaseFileDto & {
  startingChannelNumber?: number;
  maxStreamCount?: number;
  stationCount?: number;
};
export type AddM3UFileRequest = {
  description?: string | null;
  formFile?: Blob | null;
  maxStreamCount?: number;
  metaData?: string | null;
  startingChannelNumber?: number | null;
  name: string;
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
export type ProcessM3UFileRequest = {
  m3UFileId: number;
};
export type RefreshM3UFileRequest = {
  m3UFileID: number;
};
export type UpdateM3UFileRequest = BaseFileRequest & {
  startingChannelNumber?: number | null;
  maxStreamCount?: number | null;
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
  new?: string;
  previouslyshown?: TvPreviouslyshown;
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
export type ProgrammeName = {
  channel?: string;
  channelName?: string;
  displayName?: string;
};
export type TaskQueueStatusDto = {
  command?: string;
  id?: string;
  isRunning?: boolean;
  queueTS?: string;
  startTS?: string;
  stopTS?: string;
};
export type AuthenticationType = 0 | 1 | 2;
export type StreamingProxyTypes = 0 | 1 | 2 | 3;
export type Setting = {
  adminPassword?: string;
  adminUserName?: string;
  apiKey?: string;
  apiPassword?: string;
  apiUserName?: string;
  appName?: string;
  authenticationMethod?: AuthenticationType;
  authTest?: boolean;
  baseHostURL?: string;
  cacheIcons?: boolean;
  cleanURLs?: boolean;
  databaseName?: string;
  defaultIcon?: string;
  deviceID?: string;
  ffmPegExecutable?: string;
  firstFreeNumber?: number;
  maxConnectRetry?: number;
  maxConnectRetryTimeMS?: number;
  overWriteM3UChannels?: boolean;
  ringBufferSizeMB?: number;
  sdPassword?: string;
  sdUserName?: string;
  serverKey?: string;
  sourceBufferPreBufferPercentage?: number;
  streamingProxyType?: StreamingProxyTypes;
  streamMasterIcon?: string;
  uiFolder?: string;
  urlBase?: string;
};
export type SettingDto = Setting & {
  defaultIconDto?: IconFileDto;
  requiresAuth?: boolean;
  version?: string;
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
  apiPassword?: string | null;
  apiUserName?: string | null;
  cacheIcons?: boolean | null;
  cleanURLs?: boolean | null;
  deviceID?: string | null;
  ffmPegExecutable?: string | null;
  firstFreeNumber?: number | null;
  maxConnectRetry?: number | null;
  maxConnectRetryTimeMS?: number | null;
  overWriteM3UChannels?: boolean | null;
  ringBufferSizeMB?: number | null;
  sdPassword?: string | null;
  sdUserName?: string | null;
  sourceBufferPreBufferPercentage?: number | null;
  streamingProxyType?: StreamingProxyTypes | null;
};
export type VideoStreamHandlers = 0 | 1 | 2;
export type BaseVideoStreamDto = {
  cuid: string;
  id: number;
  isActive: boolean;
  isDeleted: boolean;
  isHidden: boolean;
  isReadOnly: boolean;
  isUserCreated: boolean;
  m3UFileId: number;
  rank: number;
  streamErrorCount: number;
  streamLastFail: string;
  streamLastStream: string;
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
  rank?: number;
};
export type VideoStreamDto = BaseVideoStreamDto & {
  childVideoStreams?: ChildVideoStreamDto[];
};
export type StreamGroupDto = {
  channelGroups: ChannelGroupDto[];
  hdhrLink: string;
  id: number;
  m3ULink: string;
  name: string;
  streamGroupNumber: number;
  videoStreams: VideoStreamDto[];
  xmlLink: string;
};
export type AddStreamGroupRequest = {
  name: string;
  streamGroupNumber: number;
  videoStreamIds: number[] | null;
  channelGroupNames: string[] | null;
};
export type DeleteStreamGroupRequest = {
  id?: number;
};
export type StreamStatisticsResult = {
  inputBitsPerSecond?: number;
  inputBytesRead?: number;
  inputBytesWritten?: number;
  inputElapsedTime?: string;
  inputStartTime?: string;
  clientBitsPerSecond?: number;
  clientBytesRead?: number;
  clientBytesWritten?: number;
  clientElapsedTime?: string;
  clientStartTime?: string;
  clientId?: string;
  logo?: string | null;
  m3UStreamId?: number;
  m3UStreamName?: string;
  m3UStreamProxyType?: StreamingProxyTypes;
  streamUrl?: string | null;
};
export type EpgChannel = {
  channelNumber?: number;
  uuid?: string;
  logo?: string;
};
export type EpgProgram = {
  videoStreamId?: number;
  id?: string;
  channelUuid?: string;
  title?: string;
  description?: string;
  since?: string;
  till?: string;
  image?: string;
};
export type EpgGuide = {
  channels: EpgChannel[];
  endDate: string;
  programs: EpgProgram[];
  startDate: string;
};
export type UpdateStreamGroupRequest = {
  streamGroupId?: number;
  name?: string | null;
  streamGroupNumber?: number | null;
  videoStreamIds?: number[] | null;
  channelGroupNames?: string[] | null;
};
export type AddVideoStreamRequest = {
  tvg_name?: string;
  tvg_chno?: number | null;
  tvg_group?: string | null;
  tvg_ID?: string | null;
  tvg_logo?: string | null;
  url?: string | null;
  iptvChannelHandler?: number | null;
  createChannel?: boolean | null;
};
export type DeleteVideoStreamRequest = {
  videoStreamId?: number;
};
export type ChannelNumberPair = {
  channelNumber: number;
  id: number;
};
export type SetVideoStreamChannelNumbersRequest = {
  channelNumberPairs: ChannelNumberPair[];
};
export type VideoStreamBaseUpdate = {
  id?: number;
  tvg_chno?: number | null;
  tvg_group?: string | null;
  tvg_ID?: string | null;
  tvg_logo?: string | null;
  tvg_name?: string | null;
  url?: string | null;
};
export type VideoStreamUpdate = VideoStreamBaseUpdate & {
  childVideoStreams?: ChildVideoStreamDto[] | null;
  isActive?: boolean | null;
  isDeleted?: boolean | null;
  isHidden?: boolean | null;
  isReadOnly?: boolean | null;
  isUserCreated?: boolean | null;
  streamErrorCount?: number | null;
  streamLastFail?: string | null;
  streamLastStream?: string | null;
  streamProxyType?: StreamingProxyTypes | null;
};
export type UpdateVideoStreamRequest = VideoStreamUpdate & {
  baseHostUrl?: string;
};
export type UpdateVideoStreamsRequest = {
  videoStreamUpdates?: VideoStreamUpdate[];
};
export const {
  useChannelGroupsAddChannelGroupMutation,
  useChannelGroupsGetChannelGroupsQuery,
  useChannelGroupsDeleteChannelGroupMutation,
  useChannelGroupsGetChannelGroupQuery,
  useChannelGroupsSetChannelGroupsVisibleMutation,
  useChannelGroupsUpdateChannelGroupMutation,
  useChannelGroupsUpdateChannelGroupOrderMutation,
  useChannelGroupsUpdateChannelGroupsMutation,
  useEpgFilesAddEpgFileMutation,
  useEpgFilesGetEpgFilesQuery,
  useEpgFilesAddEpgFileFromFormMutation,
  useEpgFilesChangeEpgFileNameMutation,
  useEpgFilesDeleteEpgFileMutation,
  useEpgFilesGetEpgFileQuery,
  useEpgFilesProcessEpgFileMutation,
  useEpgFilesRefreshEpgFileMutation,
  useEpgFilesScanDirectoryForEpgFilesMutation,
  useEpgFilesUpdateEpgFileMutation,
  useFilesGetFileQuery,
  useIconsAddIconFileMutation,
  useIconsGetUrlQuery,
  useIconsAddIconFileFromFormMutation,
  useIconsAutoMatchIconToStreamsMutation,
  useIconsCacheIconsFromVideoStreamsRequestQuery,
  useIconsGetIconQuery,
  useIconsGetIconsQuery,
  useM3UFilesAddM3UFileMutation,
  useM3UFilesGetM3UFilesQuery,
  useM3UFilesAddM3UFileFromFormMutation,
  useM3UFilesChangeM3UFileNameMutation,
  useM3UFilesDeleteM3UFileMutation,
  useM3UFilesGetM3UFileQuery,
  useM3UFilesProcessM3UFileMutation,
  useM3UFilesRefreshM3UFileMutation,
  useM3UFilesScanDirectoryForM3UFilesMutation,
  useM3UFilesUpdateM3UFileMutation,
  useMiscCacheAllIconsMutation,
  useMiscCacheIconsFromEpGsMutation,
  useMiscCacheIconsFromVideoStreamsMutation,
  useMiscReadDirectoryLogosRequestMutation,
  useProgrammesGetProgrammeQuery,
  useProgrammesGetProgrammeChannelsQuery,
  useProgrammesGetProgrammeNamesQuery,
  useProgrammesGetProgrammesQuery,
  useSettingsGetIsSystemReadyQuery,
  useSettingsGetQueueStatusQuery,
  useSettingsGetSettingQuery,
  useSettingsGetSystemStatusQuery,
  useSettingsLogInQuery,
  useSettingsUpdateSettingMutation,
  useStreamGroupsAddStreamGroupMutation,
  useStreamGroupsDeleteStreamGroupMutation,
  useStreamGroupsGetAllStatisticsForAllUrlsQuery,
  useStreamGroupsGetStreamGroupQuery,
  useStreamGroupsGetStreamGroupByStreamNumberQuery,
  useStreamGroupsGetStreamGroupCapabilityQuery,
  useStreamGroupsGetStreamGroupCapability2Query,
  useStreamGroupsGetStreamGroupDeviceXmlQuery,
  useStreamGroupsGetStreamGroupDiscoverQuery,
  useStreamGroupsGetStreamGroupEpgQuery,
  useStreamGroupsGetStreamGroupEpgForGuideQuery,
  useStreamGroupsGetStreamGroupLineUpQuery,
  useStreamGroupsGetStreamGroupLineUpStatusQuery,
  useStreamGroupsGetStreamGroupM3UQuery,
  useStreamGroupsGetStreamGroupM3U2Query,
  useStreamGroupsGetStreamGroupM3U22Query,
  useStreamGroupsGetStreamGroupM3U23Query,
  useStreamGroupsGetStreamGroupsQuery,
  useStreamGroupsGetStreamGroupVideoM3UStreamQuery,
  useStreamGroupsGetStreamGroupVideoStreamQuery,
  useStreamGroupsGetStreamGroupVideoStream2Query,
  useStreamGroupsGetStreamM3U8Query,
  useStreamGroupsGetStreamM3U8WithClientIdQuery,
  useStreamGroupsSimulateStreamFailureMutation,
  useStreamGroupsUpdateStreamGroupMutation,
  useVideoStreamsAddVideoStreamMutation,
  useVideoStreamsGetVideoStreamsQuery,
  useVideoStreamsDeleteVideoStreamMutation,
  useVideoStreamsGetVideoStreamQuery,
  useVideoStreamsSetVideoStreamChannelNumbersMutation,
  useVideoStreamsUpdateVideoStreamMutation,
  useVideoStreamsUpdateVideoStreamsMutation,
} = injectedRtkApi;
