import { hubConnection } from '../../app/signalr';
import { isEmptyObject } from '../../common/common';
import { iptvApi } from '../../store/iptvApi';
import type * as iptv from '../../store/iptvApi';

export const enhancedApiSettings = iptvApi.enhanceEndpoints({
  endpoints: {
    settingsGetQueueStatus: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.TaskQueueStatusDto[]) => {
            updateCachedData((draft: iptv.TaskQueueStatusDto[]) => {
              draft=data
              return draft;
            });
          };

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
        hubConnection.off('SettingsRefresh');
      }
    },
    settingsGetSetting: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.SettingDto) => {
            updateCachedData((draft: iptv.SettingDto) => {
              draft=data
              return draft;
            });
          };

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
        hubConnection.off('SettingsRefresh');
      }
    },
    settingsGetSystemStatus: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.SystemStatus) => {
            updateCachedData((draft: iptv.SystemStatus) => {
              draft=data
              return draft;
            });
          };

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
        hubConnection.off('SettingsRefresh');
      }
    },
  }
});
