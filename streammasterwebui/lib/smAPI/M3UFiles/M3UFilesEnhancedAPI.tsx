import { isEmptyObject } from '@lib/common/common';
import isPagedTableDto from '@lib/common/isPagedTableDto';
import type * as iptv from '@lib/iptvApi';
import { iptvApi } from '@lib/iptvApi';
import { isDev } from '@lib/settings';
import { singletonM3UFilesListener } from '@lib/signalr/singletonListeners';

export const enhancedApiM3UFiles = iptvApi.enhanceEndpoints({
  endpoints: {
    m3UFilesGetM3UFile: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.M3UFileDto) => {
            updateCachedData(() => {
              {
                if (isDev) console.log('updateCachedData', data);
                for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'M3UFiles' }])) {
                  if (endpointName !== 'm3UFilesGetM3UFile') continue;
                  dispatch(
                    iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {
                      {
                        if (isDev) console.log('updateCachedData', data, draft);
                      }
                    })
                  );
                }
              }
            });
          };

          singletonM3UFilesListener.addListener(updateCachedDataWithResults);

          await cacheEntryRemoved;
          singletonM3UFilesListener.removeListener(updateCachedDataWithResults);
        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }
      }
      // eslint-disable-next-line comma-dangle
    },
    m3UFilesGetPagedM3UFiles: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.M3UFileDto[]) => {
            if (!data || isEmptyObject(data)) {
              if (isDev) console.log('M3UFiles Full Refresh');
              dispatch(iptvApi.util.invalidateTags(['M3UFiles']));
              return;
            }

            updateCachedData(() => {
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'M3UFiles' }])) {
                if (endpointName === 'm3UFilesGetPagedM3UFiles') {
                  dispatch(
                    iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {
                      if (isPagedTableDto(data)) {
                        for (const item of data) {
                          const index = draft.data.findIndex((existingItem) => existingItem.id === item.id);
                          if (index !== -1) {
                            draft.data[index] = item;
                          }
                        }
                        return draft;
                      }
                      for (const item of data) {
                        const index = draft.data.findIndex((existingItem) => existingItem.id === item.id);
                        if (index !== -1) {
                          draft.data[index] = item;
                        }
                      }
                      return draft;
                    })
                  );
                }
              }
            });
          };

          singletonM3UFilesListener.addListener(updateCachedDataWithResults);

          await cacheEntryRemoved;
          singletonM3UFilesListener.removeListener(updateCachedDataWithResults);
        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }
      }
      // eslint-disable-next-line comma-dangle
    }
  }
});
