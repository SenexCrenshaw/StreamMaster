
import { hubConnection } from '../../app/signalr';
import * as StreamMasterApi from '../iptvApi';

export type SetVideoStreamVisibleRet = {
  isHidden?: boolean;
  videoStreamId?: string;
};

export const enhancedApiLocal = StreamMasterApi.iptvApi.enhanceEndpoints({
  endpoints: {
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
    programmesGetProgrammeNames: {
      async onCacheEntryAdded(
        arg,
        { updateCachedData, cacheDataLoaded, cacheEntryRemoved }
      ) {
        try {
          await cacheDataLoaded;

          const applyResults = (
            data: StreamMasterApi.ProgrammeNameDto[]
          ) => {
            updateCachedData(
              () => {

                return data;
              }
            );
          };

          hubConnection.on(
            'ProgrammeNamesUpdate',
            (data: StreamMasterApi.ProgrammeNameDto[]) => {
              applyResults(data);
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
    videoStreamsGetAllStatisticsForAllUrls: {
      async onCacheEntryAdded(
        arg,
        { updateCachedData, cacheDataLoaded, cacheEntryRemoved }
      ) {
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
