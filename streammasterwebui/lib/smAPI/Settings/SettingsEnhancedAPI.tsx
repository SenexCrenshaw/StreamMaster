import { isDev } from '@lib/settings';
import { singletonSettingsListener } from '@lib/signalr/singletonListeners';
import { isEmptyObject } from '@lib/common/common';
import isPagedTableDto from '@lib/common/isPagedTableDto';
import { iptvApi } from '@lib/iptvApi';
import type * as iptv from '@lib/iptvApi';

export const enhancedApiSettings = iptvApi.enhanceEndpoints({
  endpoints: {
    settingsGetSetting: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.SettingDto) => {
            updateCachedData(() => {{
              if (isDev) console.log('updateCachedData', data);
              if (!data) {
                dispatch(iptvApi.util.invalidateTags(['Settings']));
                return;
              }
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'Settings' }])) {
                if (endpointName !== 'settingsGetSetting') continue;
                  dispatch(iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {{
                    if (isDev) console.log('updateCachedData', data, draft);
                   }})
                   );
                 }}
            });
          };

          singletonSettingsListener.addListener(updateCachedDataWithResults);

          await cacheEntryRemoved;
          singletonSettingsListener.removeListener(updateCachedDataWithResults);
        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }
      }
    // eslint-disable-next-line comma-dangle
    },
    settingsGetSystemStatus: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.SDSystemStatus) => {
            updateCachedData(() => {{
              if (isDev) console.log('updateCachedData', data);
              if (!data) {
                dispatch(iptvApi.util.invalidateTags(['Settings']));
                return;
              }
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'Settings' }])) {
                if (endpointName !== 'settingsGetSystemStatus') continue;
                  dispatch(iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {{
                    if (isDev) console.log('updateCachedData', data, draft);
                   }})
                   );
                 }}
            });
          };

          singletonSettingsListener.addListener(updateCachedDataWithResults);

          await cacheEntryRemoved;
          singletonSettingsListener.removeListener(updateCachedDataWithResults);
        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }
      }
    // eslint-disable-next-line comma-dangle
    },
  }
});
