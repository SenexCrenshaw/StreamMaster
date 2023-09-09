/* eslint-disable @typescript-eslint/no-unused-vars */
import { hubConnection } from '../../app/signalr';
import { isEmptyObject } from '../../common/common';
import { iptvApi, type PagedResponseOfStreamGroupDto, type PagedResponseOfVideoStreamDto, type SettingDto, type StreamGroupDto, type StreamStatisticsResult, type VideoStreamDto } from '../iptvApi';

export type SetVideoStreamVisibleRet = {
  isHidden?: boolean;
  videoStreamId?: string;
};


// eslint-disable-next-line @typescript-eslint/no-unnecessary-type-constraint
const applyPagedResults = <T extends unknown>(
  data: T[],
  draft: T[],
  idGetter: (item: T) => unknown
) => {
  data.forEach((cn) => {
    const foundIndex = draft.findIndex(
      (x: T) => idGetter(x) === idGetter(cn)
    );
    if (foundIndex !== -1) {
      draft[foundIndex] = cn;
    }
  });
  return draft;
};


export const enhancedApiLocal = iptvApi.enhanceEndpoints({
  endpoints: {
    channelGroupsGetChannelGroupNames: {
      async onCacheEntryAdded(api, { dispatch, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;
          hubConnection.off('ChannelGroupsRefresh');
          hubConnection.on(
            'ChannelGroupsRefresh',
            () => {
              dispatch(iptvApi.util.invalidateTags(["ChannelGroups"]));
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
          hubConnection.off('ChannelGroupsRefresh');
          hubConnection.on(
            'ChannelGroupsRefresh',
            () => {
              dispatch(iptvApi.util.invalidateTags(["ChannelGroups"]));
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
          hubConnection.off('EPGFilesRefresh');
          hubConnection.on(
            'EPGFilesRefresh',
            () => {
              dispatch(iptvApi.util.invalidateTags(["EPGFiles"]));
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
          hubConnection.off('IconsRefresh');
          hubConnection.on(
            'IconsRefresh',
            () => {
              dispatch(iptvApi.util.invalidateTags(["Icons"]));
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
          hubConnection.off('M3UFilesRefresh');
          hubConnection.on(
            'M3UFilesRefresh',
            () => {
              dispatch(iptvApi.util.invalidateTags(["M3UFiles"]));
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
          hubConnection.off('ProgrammesRefresh');
          hubConnection.on(
            'ProgrammesRefresh',
            () => {
              dispatch(iptvApi.util.invalidateTags(["Programmes"]));
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
          hubConnection.off('ProgrammesRefresh');
          hubConnection.on(
            'ProgrammesRefresh',
            () => {
              dispatch(iptvApi.util.invalidateTags(["Programmes"]));
              hubConnection.off('ProgrammesRefresh');
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

          const applyResult = (data: SettingDto) => {
            updateCachedData((draft: SettingDto) => {
              draft = data;

              return draft;
            });
          };

          hubConnection.off('SettingsUpdate');
          hubConnection.on(
            'SettingsUpdate',
            (data: SettingDto) => {
              applyResult(data);
            },
          );

        } catch { }

        await cacheEntryRemoved;
      },
    },
    streamGroupChannelGroupGetChannelGroupsFromStreamGroup: {
      async onCacheEntryAdded(api, { dispatch, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          hubConnection.off('StreamGroupChannelGroupsRefresh');
          hubConnection.on(
            'StreamGroupChannelGroupsRefresh',
            () => {
              dispatch(iptvApi.util.invalidateTags(["StreamGroupChannelGroup"]));
            }
          );

        } catch { }

        await cacheEntryRemoved;
      }
    },
    streamGroupsGetStreamGroups: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const applyResults = (
            data: StreamGroupDto[]
          ) => {
            updateCachedData((draft: PagedResponseOfStreamGroupDto) => {
              data.forEach(function (cn) {
                const foundIndex = draft.data.findIndex(
                  (x) => x.id === cn.id
                );

                if (foundIndex !== -1) {
                  draft.data[foundIndex] = cn;
                }
              });

              return draft;
            }
            );
          };

          hubConnection.off('StreamGroupsRefresh');
          hubConnection.on(
            'StreamGroupsRefresh',
            (data: StreamGroupDto[]) => {
              if (isEmptyObject(data)) {
                dispatch(iptvApi.util.invalidateTags(["StreamGroups"]));
              } else {
                applyResults(data);
              }
            }
          );
        } catch { }

        await cacheEntryRemoved;
      }
    },
    streamGroupVideoStreamsGetStreamGroupVideoStreams: {
      async onCacheEntryAdded(api, { dispatch, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          hubConnection.off('StreamGroupVideoStreamsRefresh');
          hubConnection.on(
            'StreamGroupVideoStreamsRefresh',
            () => {
              dispatch(iptvApi.util.invalidateTags(["StreamGroupVideoStreams"]));
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
            data: StreamStatisticsResult[]
          ) => {
            updateCachedData(
              () => {

                return data;
              }
            );
          };

          hubConnection.off('streamstatisticsresultsupdate');
          hubConnection.on(
            'streamstatisticsresultsupdate',
            (data: StreamStatisticsResult[]) => {
              applyResults(data);
            }
          );



        } catch { }

        await cacheEntryRemoved;
      }
    },
    videoStreamsGetVideoStreams: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const applyResults = (
            data: VideoStreamDto[]
          ) => {
            updateCachedData((draft: PagedResponseOfVideoStreamDto) => {

              const updatedDraft = applyPagedResults(
                data,
                draft.data,
                (item: VideoStreamDto) => item.id
              );
              draft.data = updatedDraft;
              return draft;

            });
          };

          hubConnection.off('VideoStreamsRefresh');
          hubConnection.on(
            'VideoStreamsRefresh',
            (data: VideoStreamDto[]) => {
              // dispatch(iptvApi.util.invalidateTags(["VideoStreams"]));
              // if (isEmptyObject(data)) {
              //   dispatch(iptvApi.util.invalidateTags(["VideoStreams"]));
              // } else {
              //   applyResults(data);
              // }

            }
          );

        } catch { }

        await cacheEntryRemoved;
      }
    },
  }
});
