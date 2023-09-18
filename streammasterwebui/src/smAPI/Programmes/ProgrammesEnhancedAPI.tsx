import { singletonProgrammesListener } from '../../app/createSingletonListener';
import { isEmptyObject } from '../../common/common';
import isPagedTableDto from '../../components/dataSelector/isPagedTableDto';
import { iptvApi } from '../../store/iptvApi';
import type * as iptv from '../../store/iptvApi';

export const enhancedApiProgrammes = iptvApi.enhanceEndpoints({
  endpoints: {
    programmesGetPagedProgrammeNameSelections: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.ProgrammeNameDto[]) => {
            updateCachedData(() => {
              console.log('updateCachedData', data);
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'Programmes' }])) {
                if (endpointName !== 'programmesGetPagedProgrammeNameSelections') continue;
                  dispatch(
                    iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {
                      if (isEmptyObject(data)) {
                        console.log('empty', data);
                        dispatch(iptvApi.util.invalidateTags(['Programmes']));
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

         singletonProgrammesListener.addListener(updateCachedDataWithResults);

        await cacheEntryRemoved;
        singletonProgrammesListener.removeListener(updateCachedDataWithResults);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

      }
    },
    programmesGetProgrammsSimpleQuery: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.ProgrammeNameDto[]) => {
            updateCachedData(() => {
              console.log('updateCachedData', data);
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'Programmes' }])) {
                if (endpointName !== 'programmesGetProgrammsSimpleQuery') continue;
                  dispatch(
                    iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {
                      if (isEmptyObject(data)) {
                        console.log('empty', data);
                        dispatch(iptvApi.util.invalidateTags(['Programmes']));
                        return;
                      }

                      if (isPagedTableDto(data)) {
                      data.forEach(item => {
                        const index = draft.findIndex(existingItem => existingItem.id === item.id);
                        if (index !== -1) {
                          draft[index] = item;
                        }
                        });

                        return draft;
                        }

                      data.forEach(item => {
                        const index = draft.findIndex(existingItem => existingItem.id === item.id);
                        if (index !== -1) {
                          draft[index] = item;
                        }
                        });

                      return draft;
                     })
                   )
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
    },
  }
});
