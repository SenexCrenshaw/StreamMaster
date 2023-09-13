import { hubConnection } from '../../app/signalr';
import { isEmptyObject } from '../../common/common';
import type * as iptv from '../../store/iptvApi';
import { iptvApi } from '../../store/iptvApi';

export const enhancedApiSettings = iptvApi.enhanceEndpoints({
  endpoints: {
    settingsGetQueueStatus: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.TaskQueueStatusDto[]) => {
            updateCachedData((draft: iptv.TaskQueueStatusDto[]) => {
              data.forEach(item => {
                const index = draft.findIndex(existingItem => existingItem.id === item.id);
                if (index !== -1) {
                  draft[index] = item;
                }
              });

              return draft;
            });
          };

          hubConnection.off('SettingsRefresh');
          hubConnection.on('SettingsRefresh', (data: iptv.TaskQueueStatusDto[]) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['Settings']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
      }
    },
    settingsGetSetting: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.SettingDto) => {
            updateCachedData((draft: iptv.SettingDto) => {
              draft = data
              return draft;
            });
          };

          hubConnection.off('SettingsRefresh');
          hubConnection.on('SettingsRefresh', (data: iptv.SettingDto) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['Settings']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
      }
    },
    settingsGetSystemStatus: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.SystemStatus) => {
            updateCachedData((draft: iptv.SystemStatus) => {
              draft = data
              return draft;
            });
          };

          hubConnection.off('SettingsRefresh');
          hubConnection.on('SettingsRefresh', (data: iptv.SystemStatus) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['Settings']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
      }
    },
  }
});
