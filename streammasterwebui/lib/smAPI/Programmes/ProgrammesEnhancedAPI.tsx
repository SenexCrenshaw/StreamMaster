import { isDev } from '@/lib/settings';
import { singletonProgrammesListener } from '@/lib/signalr/singletonListeners';
import { isEmptyObject } from '@/lib/common/common';
import isPagedTableDto from '@/lib/common/isPagedTableDto';
import { iptvApi } from '@/lib/iptvApi';
import type * as iptv from '@/lib/iptvApi';

export const enhancedApiProgrammes = iptvApi.enhanceEndpoints({
  endpoints: {
    programmesGetPagedProgrammeNameSelections: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.ProgrammeNameDto[]) => {
            if (!data || isEmptyObject(data)) {
              if (isDev) console.log('Programmes Full Refresh');
              dispatch(iptvApi.util.invalidateTags(['Programmes']));
              return;
            }

            updateCachedData(() => {
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'Programmes' }])) {
                if (endpointName === 'programmesGetPagedProgrammeNameSelections') {
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

          singletonProgrammesListener.addListener(updateCachedDataWithResults);

          await cacheEntryRemoved;
          singletonProgrammesListener.removeListener(updateCachedDataWithResults);
        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }
      }
    // eslint-disable-next-line comma-dangle
    },
    programmesGetProgrammsSimpleQuery: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.ProgrammeNameDto[]) => {
            if (!data || isEmptyObject(data)) {
              if (isDev) console.log('Programmes Full Refresh');
              dispatch(iptvApi.util.invalidateTags(['Programmes']));
              return;
            }

            updateCachedData(() => {
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'Programmes' }])) {
                if (endpointName === 'programmesGetProgrammsSimpleQuery') {
                  dispatch(
                    iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {
                      if (isPagedTableDto(data)) {
                        for (const item of data) {
                          const index = draft.findIndex((existingItem) => existingItem.id === item.id);
                          if (index !== -1) {
                            draft[index] = item;
                          }
                        }
                        return draft;
                      }
                      for (const item of data) {
                        const index = draft.findIndex((existingItem) => existingItem.id === item.id);
                        if (index !== -1) {
                          draft[index] = item;
                        }
                      }
                      return draft;
                    })
                  );
                }
              }
            });
          };

          singletonProgrammesListener.addListener(updateCachedDataWithResults);

          await cacheEntryRemoved;
          singletonProgrammesListener.removeListener(updateCachedDataWithResults);
        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }
      }
    // eslint-disable-next-line comma-dangle
    },
  }
});
