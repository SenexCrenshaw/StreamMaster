import { singletonEPGFilesListener } from '../../app/createSingletonListener';
import { isEmptyObject } from '../../common/common';
import isPagedTableDto from '../../components/dataSelector/isPagedTableDto';
import type * as iptv from '../../store/iptvApi';
import { iptvApi } from '../../store/iptvApi';

export const enhancedApiEpgFiles = iptvApi.enhanceEndpoints({
  endpoints: {
    epgFilesGetEpgFile: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.EpgFileDto) => {
            updateCachedData(() => {
              console.log('updateCachedData', data);
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'EPGFiles' }])) {
                if (endpointName !== 'epgFilesGetEpgFile') continue;
                dispatch(iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {
                  console.log('updateCachedData', data, draft);
                })
                );
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
    },
    epgFilesGetPagedEpgFiles: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.EpgFileDto[]) => {
            updateCachedData(() => {
              console.log('updateCachedData', data);
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'EPGFiles' }])) {
                if (endpointName !== 'epgFilesGetPagedEpgFiles') continue;
                dispatch(
                  iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {
                    if (isEmptyObject(data)) {
                      console.log('empty', data);
                      dispatch(iptvApi.util.invalidateTags(['EPGFiles']));
                      return;
                    }

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

          singletonEPGFilesListener.addListener(updateCachedDataWithResults);

          await cacheEntryRemoved;
          singletonEPGFilesListener.removeListener(updateCachedDataWithResults);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

      }
    },
  }
});
