import { isDev } from '@lib/settings';
import { singletonEPGFilesListener } from '@lib/signalr/singletonListeners';
import { isEmptyObject } from '@lib/common/common';
import isPagedTableDto from '@lib/common/isPagedTableDto';
import { iptvApi } from '@lib/iptvApi';
import type * as iptv from '@lib/iptvApi';

export const enhancedApiEpgFiles = iptvApi.enhanceEndpoints({
  endpoints: {
    epgFilesGetEpgFile: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.EpgFileDto) => {
            updateCachedData(() => {
              {
                if (isDev) console.log('updateCachedData', data);
                for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'EPGFiles' }])) {
                  if (endpointName !== 'epgFilesGetEpgFile') continue;
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

          singletonEPGFilesListener.addListener(updateCachedDataWithResults);

          await cacheEntryRemoved;
          singletonEPGFilesListener.removeListener(updateCachedDataWithResults);
        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }
      }
      // eslint-disable-next-line comma-dangle
    },
    epgFilesGetPagedEpgFiles: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.EpgFileDto[]) => {
            if (!data || isEmptyObject(data)) {
              if (isDev) console.log('EPGFiles Full Refresh');
              dispatch(iptvApi.util.invalidateTags(['EPGFiles']));
              return;
            }

            updateCachedData(() => {
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'EPGFiles' }])) {
                if (endpointName === 'epgFilesGetPagedEpgFiles') {
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

          singletonEPGFilesListener.addListener(updateCachedDataWithResults);

          await cacheEntryRemoved;
          singletonEPGFilesListener.removeListener(updateCachedDataWithResults);
        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }
      }
      // eslint-disable-next-line comma-dangle
    }
  }
});
