import { hubConnection } from '../../app/store';
import * as StreamMasterApi from '../iptvApi';


export const enhancedApiLocal = StreamMasterApi.iptvApi.enhanceEndpoints({
  endpoints: {

    programmesGetProgrammeNames: {
      async onCacheEntryAdded(
        arg,
        { updateCachedData, cacheDataLoaded, cacheEntryRemoved }
      ) {
        try {
          await cacheDataLoaded;

          const applyResults = (
            data: StreamMasterApi.ProgrammeName[]
          ) => {
            updateCachedData(
              () => {

                return data;
              }
            );
          };

          hubConnection.on(
            'ProgrammeNamesUpdate',
            (data: StreamMasterApi.ProgrammeName[]) => {
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
    streamGroupsGetAllStatisticsForAllUrls: {
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
            'StreamStatisticsResultsUpdate',
            (data: StreamMasterApi.StreamStatisticsResult[]) => {
              applyResults(data);
            }
          );



        } catch { }

        await cacheEntryRemoved;
      }
    },
    videoStreamsGetVideoStreams: {
      async onCacheEntryAdded(
        arg,
        { updateCachedData, cacheDataLoaded, cacheEntryRemoved }
      ) {
        try {
          await cacheDataLoaded;

          const applyChannelNumbersResults = (
            channelNumbers: StreamMasterApi.ChannelNumberPair[],
          ) => {
            updateCachedData((draft: StreamMasterApi.VideoStreamDto[]) => {
              channelNumbers.forEach(function (cn) {
                const foundM3UStreamel = draft.findIndex((x) => x.id === cn.id);
                if (foundM3UStreamel !== -1) {
                  draft[foundM3UStreamel].user_Tvg_chno = cn.channelNumber;
                }
              });

              return draft;
            });

          };

          hubConnection.on(
            'VideoStreamUpdateChannelNumbers',
            (data: StreamMasterApi.ChannelNumberPair[]) => {
              applyChannelNumbersResults(data);
            },
          );

          const applyResults = (
            data: StreamMasterApi.VideoStreamDto[]
          ) => {
            updateCachedData(
              (draft: StreamMasterApi.VideoStreamDto[]) => {
                data.forEach(function (cn) {
                  const foundIndex = draft.findIndex(
                    (x) => x.id === cn.id
                  );
                  if (foundIndex !== -1) {
                    draft[foundIndex] = cn;
                  } else {
                    draft.push(cn);
                  }
                });
                return draft;
              }
            );
          };

          hubConnection.on(
            'VideoStreamDtoesUpdate',
            (data: StreamMasterApi.VideoStreamDto[]) => {
              applyResults(data);
            }
          );


          const applyResult = (
            data: StreamMasterApi.VideoStreamDto
          ) => {
            updateCachedData(
              (
                draft: StreamMasterApi.VideoStreamDto[]
              ) => {
                const foundIndex = draft.findIndex(
                  (x) => x.id === data.id
                );

                if (foundIndex === -1) {
                  draft.push(data);
                } else {
                  draft[foundIndex] = data;
                }

                return draft;
              }
            );
          };

          hubConnection.on(
            'VideoStreamDtoUpdate',
            (data: StreamMasterApi.VideoStreamDto) => {
              applyResult(data);
            }
          );

          const deleteResult = (

            // eslint-disable-next-line @typescript-eslint/no-explicit-any
            id: any
          ) => {
            updateCachedData(
              (
                draft: StreamMasterApi.VideoStreamDto[]
              ) => {
                return draft.filter((obj) => obj.id !== id);
              }
            );
          };

          hubConnection.on(
            'VideoStreamDtoDelete',
            (id: number) => {
              deleteResult(id);
            }
          );


        } catch { }

        await cacheEntryRemoved;
      }
    },
  },
});
