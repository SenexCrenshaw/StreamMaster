
import { hubConnection } from '../../app/signalr';
import * as StreamMasterApi from '../iptvApi';

export type SetVideoStreamVisibleRet = {
  isHidden?: boolean;
  videoStreamId?: string;
};

export const enhancedApiLocal = StreamMasterApi.iptvApi.enhanceEndpoints({
  // addTagTypes: ["GetStreamGroupVideoStreams"],
  endpoints: {
    channelGroupsGetChannelGroupNames: {
      async onCacheEntryAdded(api, { dispatch, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;
          hubConnection.on(
            'ChannelGroupsRefresh',
            () => {
              dispatch(StreamMasterApi.iptvApi.util.invalidateTags(["ChannelGroups"]));
            }
          );
        } catch { }

        await cacheEntryRemoved;
      }
    },
    channelGroupsGetChannelGroups: {
      async onCacheEntryAdded(api, { dispatch, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;
          hubConnection.on(
            'ChannelGroupsRefresh',
            () => {
              dispatch(StreamMasterApi.iptvApi.util.invalidateTags(["ChannelGroups"]));
            }
          );
        } catch { }

        await cacheEntryRemoved;
      }
    },
    epgFilesGetEpgFiles: {
      async onCacheEntryAdded(api, { dispatch, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;
          hubConnection.on(
            'EPGFilesRefresh',
            () => {
              dispatch(StreamMasterApi.iptvApi.util.invalidateTags(["EPGFiles"]));
            }
          );
        } catch { }

        await cacheEntryRemoved;
      }
    },
    iconsGetIcons: {
      async onCacheEntryAdded(api, { dispatch, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;
          hubConnection.on(
            'IconsRefresh',
            () => {
              dispatch(StreamMasterApi.iptvApi.util.invalidateTags(["Icons"]));
            }
          );
        } catch { }

        await cacheEntryRemoved;
      }
    },
    m3UFilesGetM3UFiles: {
      async onCacheEntryAdded(api, { dispatch, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;
          hubConnection.on(
            'M3UFilesRefresh',
            () => {
              dispatch(StreamMasterApi.iptvApi.util.invalidateTags(["M3UFiles"]));
            }
          );
        } catch { }

        await cacheEntryRemoved;
      }
    },
    programmesGetProgrammeNameSelections: {
      async onCacheEntryAdded(api, { dispatch, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;
          hubConnection.on(
            'ProgrammesRefresh',
            () => {
              dispatch(StreamMasterApi.iptvApi.util.invalidateTags(["Programmes"]));
            }
          );
        } catch { }

        await cacheEntryRemoved;
      }
    },
    programmesGetProgrammsSimpleQuery: {
      async onCacheEntryAdded(api, { dispatch, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;
          hubConnection.on(
            'ProgrammesRefresh',
            () => {
              dispatch(StreamMasterApi.iptvApi.util.invalidateTags(["Programmes"]));
            }
          );
        } catch { }

        await cacheEntryRemoved;
      }
    },
    settingsGetSetting: {
      async onCacheEntryAdded(
        arg,
        { updateCachedData, cacheDataLoaded, cacheEntryRemoved },
      ) {
        try {
          await cacheDataLoaded;

          const applyResult = (data: StreamMasterApi.SettingDto) => {
            updateCachedData((draft: StreamMasterApi.SettingDto) => {
              draft = data;

              return draft;
            });
          };

          hubConnection.on(
            'SettingsUpdate',
            (data: StreamMasterApi.SettingDto) => {
              applyResult(data);
            },
          );
        } catch { }

        await cacheEntryRemoved;
      },
    },
    streamGroupsGetStreamGroups: {
      async onCacheEntryAdded(api, { dispatch, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;
          hubConnection.on(
            'StreamGroupsRefresh',
            () => {
              dispatch(StreamMasterApi.iptvApi.util.invalidateTags(["StreamGroups"]));
            }
          );
        } catch { }

        await cacheEntryRemoved;
      }
    },

    videoStreamsGetAllStatisticsForAllUrls: {
      async onCacheEntryAdded(arg, { updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const applyResults = (
            data: StreamMasterApi.StreamStatisticsResult[]
          ) => {
            updateCachedData(
              () => {

                return data;
              }
            );
          };

          hubConnection.on(
            'streamstatisticsresultsupdate',
            (data: StreamMasterApi.StreamStatisticsResult[]) => {
              applyResults(data);
            }
          );



        } catch { }

        await cacheEntryRemoved;
      }
    },
    videoStreamsGetVideoStreams: {
      async onCacheEntryAdded(api, { dispatch, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;
          hubConnection.on(
            'VideoStreamsRefresh',
            () => {
              dispatch(StreamMasterApi.iptvApi.util.invalidateTags(["VideoStreams"]));
            }
          );
        } catch { }

        await cacheEntryRemoved;
      }
    },
  }
});
