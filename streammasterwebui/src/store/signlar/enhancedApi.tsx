/* eslint-disable @typescript-eslint/no-explicit-any */
import { hubConnection } from '../../app/signalr';
import { iptvApi, type EpgGuide, type SettingDto, type StationPreview, type SystemStatus, type TaskQueueStatusDto } from '../iptvApi';

export const enhancedApi = iptvApi.enhanceEndpoints({
  endpoints: {

    schedulesDirectGetStationPreviews: {
      async onCacheEntryAdded(
        arg,
        { updateCachedData, cacheDataLoaded, cacheEntryRemoved }
      ) {
        try {
          await cacheDataLoaded;

          const applyResults = (
            data: StationPreview[]
          ) => {
            updateCachedData(
              (draft: StationPreview[]) => {
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
            'StationPreviewsUpdate',
            (data: StationPreview[]) => {
              applyResults(data);
            }
          );


          const applyResult = (
            data: StationPreview
          ) => {
            updateCachedData(
              (
                draft: StationPreview[]
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
            'StationPreviewUpdate',
            (data: StationPreview) => {
              applyResult(data);
            }
          );

          const deleteResult = (
            id: any
          ) => {
            updateCachedData(
              (
                draft: StationPreview[]
              ) => {
                return draft.filter((obj) => obj.id !== id);
              }
            );
          };

          hubConnection.on(
            'StationPreviewDelete',
            (id: number) => {
              deleteResult(id);
            }
          );


        } catch { }

        await cacheEntryRemoved;
      }
    },
    settingsGetQueueStatus: {
      async onCacheEntryAdded(
        arg,
        { updateCachedData, cacheDataLoaded, cacheEntryRemoved }
      ) {
        try {
          await cacheDataLoaded;

          const applyResults = (
            data: TaskQueueStatusDto[]
          ) => {
            updateCachedData(
              (draft: TaskQueueStatusDto[]) => {
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
            'TaskQueueStatusDtoesUpdate',
            (data: TaskQueueStatusDto[]) => {
              applyResults(data);
            }
          );


          const applyResult = (
            data: TaskQueueStatusDto
          ) => {
            updateCachedData(
              (
                draft: TaskQueueStatusDto[]
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
            'TaskQueueStatusDtoUpdate',
            (data: TaskQueueStatusDto) => {
              applyResult(data);
            }
          );

          const deleteResult = (


            id: any
          ) => {
            updateCachedData(
              (
                draft: TaskQueueStatusDto[]
              ) => {
                return draft.filter((obj) => obj.id !== id);
              }
            );
          };

          hubConnection.on(
            'TaskQueueStatusDtoDelete',
            (id: number) => {
              deleteResult(id);
            }
          );


        } catch { }

        await cacheEntryRemoved;
      }
    },
    settingsGetSetting: {
      async onCacheEntryAdded(
        arg,
        { updateCachedData, cacheDataLoaded, cacheEntryRemoved }
      ) {
        try {
          await cacheDataLoaded;

          const applyResult = (
            data: SettingDto
          ) => {
            updateCachedData(
              (
              ) => {
                return data;
              }
            );
          };

          hubConnection.on(
            'SettingDtoUpdate',
            (data: SettingDto) => {
              applyResult(data);
            }
          );


        } catch { }

        await cacheEntryRemoved;
      }
    },
    settingsGetSystemStatus: {
      async onCacheEntryAdded(
        arg,
        { updateCachedData, cacheDataLoaded, cacheEntryRemoved }
      ) {
        try {
          await cacheDataLoaded;

          const applyResult = (
            data: SystemStatus
          ) => {
            updateCachedData(
              (
              ) => {
                return data;
              }
            );
          };

          hubConnection.on(
            'SystemStatusUpdate',
            (data: SystemStatus) => {
              applyResult(data);
            }
          );


        } catch { }

        await cacheEntryRemoved;
      }
    },
    streamGroupsGetStreamGroupEpgForGuide: {
      async onCacheEntryAdded(
        arg,
        { updateCachedData, cacheDataLoaded, cacheEntryRemoved }
      ) {
        try {
          await cacheDataLoaded;

          const applyResult = (
            data: EpgGuide
          ) => {
            updateCachedData(
              (
              ) => {
                return data;
              }
            );
          };

          hubConnection.on(
            'EpgGuideUpdate',
            (data: EpgGuide) => {
              applyResult(data);
            }
          );


        } catch { }

        await cacheEntryRemoved;
      }
    },
  }
});
