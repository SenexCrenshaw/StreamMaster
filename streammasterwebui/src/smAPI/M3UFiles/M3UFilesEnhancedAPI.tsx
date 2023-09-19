import { singletonM3UFilesListener } from '../../app/createSingletonListener';
import { isEmptyObject } from '../../common/common';
import isPagedTableDto from '../../components/dataSelector/isPagedTableDto';
import { iptvApi } from '../../store/iptvApi';
import type * as iptv from '../../store/iptvApi';

export const enhancedApiM3UFiles = iptvApi.enhanceEndpoints({
  endpoints: {
    m3UFilesGetM3UFile: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.M3UFileDto) => {
            updateCachedData(() => {
              console.log('updateCachedData', data);
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'M3UFiles' }])) {
                if (endpointName !== 'm3UFilesGetM3UFile') continue;
                  dispatch(iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {
                    console.log('updateCachedData', data, draft);
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
              console.log('empty', data);
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
