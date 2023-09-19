import { singletonStreamGroupsListener } from '../../app/createSingletonListener';
import { isEmptyObject } from '../../common/common';
import isPagedTableDto from '../../components/dataSelector/isPagedTableDto';
import type * as iptv from '../../store/iptvApi';
import { iptvApi } from '../../store/iptvApi';

export const enhancedApiStreamGroups = iptvApi.enhanceEndpoints({
  endpoints: {
    streamGroupsGetPagedStreamGroups: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.StreamGroupDto[]) => {
            if (!data || isEmptyObject(data)) {
              console.log('empty', data);
              dispatch(iptvApi.util.invalidateTags(['StreamGroups']));
              return;
            }

            updateCachedData(() => {
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'StreamGroups' }])) {
                if (endpointName !== 'streamGroupsGetPagedStreamGroups') continue;
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

          singletonStreamGroupsListener.addListener(updateCachedDataWithResults);

          await cacheEntryRemoved;
          singletonStreamGroupsListener.removeListener(updateCachedDataWithResults);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

      }
    },
    streamGroupsGetStreamGroup: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.StreamGroupDto) => {
            updateCachedData(() => {
              console.log('updateCachedData', data);
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'StreamGroups' }])) {
                if (endpointName !== 'streamGroupsGetStreamGroup') continue;
                dispatch(iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {
                  console.log('updateCachedData', data, draft);
                })
                );
              }


            });
          };

          singletonStreamGroupsListener.addListener(updateCachedDataWithResults);

          await cacheEntryRemoved;
          singletonStreamGroupsListener.removeListener(updateCachedDataWithResults);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

      }
    },
    streamGroupsGetStreamGroupEpgForGuide: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.EpgGuide) => {
            updateCachedData(() => {
              console.log('updateCachedData', data);
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'StreamGroups' }])) {
                if (endpointName !== 'streamGroupsGetStreamGroupEpgForGuide') continue;
                dispatch(iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {
                  console.log('updateCachedData', data, draft);
                })
                );
              }


            });
          };

          singletonStreamGroupsListener.addListener(updateCachedDataWithResults);

          await cacheEntryRemoved;
          singletonStreamGroupsListener.removeListener(updateCachedDataWithResults);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

      }
    },
  }
});
