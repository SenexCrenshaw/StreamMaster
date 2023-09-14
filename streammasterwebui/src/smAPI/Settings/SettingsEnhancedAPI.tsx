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
            updateCachedData((draft: iptv.SettingsGetQueueStatusApiResponse) => {
              data.forEach(item => {
                const index = draft.findIndex(existingItem => existingItem.id === item.id);
                if (index !== -1) {
                  draft[index] = item;
                }
              });

              return draft;
            });
          };

          const doSettingsGetQueueStatusUpdate = (data: iptv.TaskQueueStatusDto[]) => {
            // console.log('doSettingsGetQueueStatusUpdate')
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['Settings']));
            } else {
              updateCachedDataWithResults(data);
            }
          }

          hubConnection.on('SettingsRefresh', doSettingsGetQueueStatusUpdate);

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
            updateCachedData((draft: iptv.SettingsGetSettingApiResponse) => {
              draft=data
              return draft;
            });
          };

          const doSettingsGetSettingUpdate = (data: iptv.SettingDto) => {
            // console.log('doSettingsGetSettingUpdate')
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['Settings']));
            } else {
              updateCachedDataWithResults(data);
            }
          }

          hubConnection.on('SettingsRefresh', doSettingsGetSettingUpdate);

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
            updateCachedData((draft: iptv.SettingsGetSystemStatusApiResponse) => {
              draft=data
              return draft;
            });
          };

          const doSettingsGetSystemStatusUpdate = (data: iptv.SystemStatus) => {
            // console.log('doSettingsGetSystemStatusUpdate')
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['Settings']));
            } else {
              updateCachedDataWithResults(data);
            }
          }

          hubConnection.on('SettingsRefresh', doSettingsGetSystemStatusUpdate);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
      }
    },
  }
});
