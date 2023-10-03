import { isDebug } from '@/lib/settings';
import { singletonM3UFilesListener } from '@/lib/signalr/singletonListeners';
import { isEmptyObject } from '@/lib/common/common';
import isPagedTableDto from '@/lib/common/isPagedTableDto';
import { iptvApi } from '@/lib/iptvApi';
import type * as iptv from '@/lib/iptvApi';

export const enhancedApiM3UFiles = iptvApi.enhanceEndpoints({
  endpoints: {
    m3UFilesGetM3UFile: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.M3UFileDto) => {
            updateCachedData(() => {
              if (isDebug) console.log('updateCachedData', data);
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'M3UFiles' }])) {
                if (endpointName !== 'm3UFilesGetM3UFile') continue;
                  dispatch(iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {
                    if (isDebug) console.log('updateCachedData', data, draft);
                   })
                   );
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
    },
    m3UFilesGetPagedM3UFiles: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.M3UFileDto[]) => {
            if (!data || isEmptyObject(data)) {
              if (isDebug) console.log('empty', data);
              dispatch(iptvApi.util.invalidateTags(['M3UFiles']));
              return;
            }

            updateCachedData(() => {
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'M3UFiles' }])) {
                if (endpointName !== 'm3UFilesGetPagedM3UFiles') continue;
                  dispatch(
                    iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {

                      if (isPagedTableDto(data)) {
                      data.forEach(item => {
                        const index = draft.data.findIndex(existingItem => existingItem.id === item.id);
                        if (index !== -1) {
                          draft.data[index] = item;
                        }
                        });

                        return draft;
                        }

                      data.forEach(item => {
                        const index = draft.data.findIndex(existingItem => existingItem.id === item.id);
                        if (index !== -1) {
                          draft.data[index] = item;
                        }
                        });

                      return draft;
                     })
                   )
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
    },
  }
});
