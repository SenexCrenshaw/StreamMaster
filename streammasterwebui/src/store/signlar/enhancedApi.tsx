import { hubConnection } from '../../app/signalr';
import * as StreamMasterApi from '../iptvApi';

export const enhancedApi = StreamMasterApi.iptvApi.enhanceEndpoints({
  endpoints: {

    schedulesDirectGetStationPreviews: {
      async onCacheEntryAdded(
        arg,
        { updateCachedData, cacheDataLoaded, cacheEntryRemoved }
      ) {
        try {
          await cacheDataLoaded;

          const applyResults = (
            data: StreamMasterApi.StationPreview[]
          ) => {
            updateCachedData(
              (draft: StreamMasterApi.StationPreview[]) => {
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
            (data: StreamMasterApi.StationPreview[]) => {
              applyResults(data);
            }
          );


          const applyResult = (
            data: StreamMasterApi.StationPreview
          ) => {
            updateCachedData(
              (
                draft: StreamMasterApi.StationPreview[]
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
            (data: StreamMasterApi.StationPreview) => {
              applyResult(data);
            }
          );

          const deleteResult = (


            id: any
          ) => {
            updateCachedData(
              (
                draft: StreamMasterApi.StationPreview[]
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
            data: StreamMasterApi.TaskQueueStatusDto[]
          ) => {
            updateCachedData(
              (draft: StreamMasterApi.TaskQueueStatusDto[]) => {
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
            (data: StreamMasterApi.TaskQueueStatusDto[]) => {
              applyResults(data);
            }
          );


          const applyResult = (
            data: StreamMasterApi.TaskQueueStatusDto
          ) => {
            updateCachedData(
              (
                draft: StreamMasterApi.TaskQueueStatusDto[]
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
            (data: StreamMasterApi.TaskQueueStatusDto) => {
              applyResult(data);
            }
          );

          const deleteResult = (


            id: any
          ) => {
            updateCachedData(
              (
                draft: StreamMasterApi.TaskQueueStatusDto[]
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
            data: StreamMasterApi.SettingDto
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
            (data: StreamMasterApi.SettingDto) => {
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
            data: StreamMasterApi.SystemStatus
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
            (data: StreamMasterApi.SystemStatus) => {
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
            data: StreamMasterApi.EpgGuide
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
            (data: StreamMasterApi.EpgGuide) => {
              applyResult(data);
            }
          );


        } catch { }

        await cacheEntryRemoved;
      }
    },
  }
});
