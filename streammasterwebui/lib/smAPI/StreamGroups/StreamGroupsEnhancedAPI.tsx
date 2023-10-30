import { isDev } from '@/lib/settings';
import { singletonStreamGroupsListener } from '@/lib/signalr/singletonListeners';
import { isEmptyObject } from '@/lib/common/common';
import isPagedTableDto from '@/lib/common/isPagedTableDto';
import { iptvApi } from '@/lib/iptvApi';
import type * as iptv from '@/lib/iptvApi';

export const enhancedApiStreamGroups = iptvApi.enhanceEndpoints({
  endpoints: {
    streamGroupsGetPagedStreamGroups: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.StreamGroupDto[]) => {
            if (!data || isEmptyObject(data)) {
              if (isDev) console.log('StreamGroups Full Refresh');
              dispatch(iptvApi.util.invalidateTags(['StreamGroups']));
              return;
            }

            updateCachedData(() => {
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'StreamGroups' }])) {
                if (endpointName === 'streamGroupsGetPagedStreamGroups') {
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

          singletonStreamGroupsListener.addListener(updateCachedDataWithResults);

          await cacheEntryRemoved;
          singletonStreamGroupsListener.removeListener(updateCachedDataWithResults);
        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }
      }
    // eslint-disable-next-line comma-dangle
    },
    streamGroupsGetStreamGroup: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.StreamGroupDto) => {
            updateCachedData(() => {
              {
                if (isDev) console.log('updateCachedData', data);
                for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'StreamGroups' }])) {
                  if (endpointName !== 'streamGroupsGetStreamGroup') continue;
                  dispatch(iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {
                    {
                      if (isDev) console.log('updateCachedData', data, draft);
                    }
                  }));
                } }
            });
          };

          singletonStreamGroupsListener.addListener(updateCachedDataWithResults);

          await cacheEntryRemoved;
          singletonStreamGroupsListener.removeListener(updateCachedDataWithResults);
        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }
      }
    // eslint-disable-next-line comma-dangle
    },
    streamGroupsGetStreamGroupEpgForGuide: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.EpgGuide) => {
            updateCachedData(() => {
              {
                if (isDev) console.log('updateCachedData', data);
                for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'StreamGroups' }])) {
                  if (endpointName !== 'streamGroupsGetStreamGroupEpgForGuide') continue;
                  dispatch(iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {
                    {
                      if (isDev) console.log('updateCachedData', data, draft);
                    }
                  }));
                } }
            });
          };

          singletonStreamGroupsListener.addListener(updateCachedDataWithResults);

          await cacheEntryRemoved;
          singletonStreamGroupsListener.removeListener(updateCachedDataWithResults);
        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }
      }
    // eslint-disable-next-line comma-dangle
    },
  }
});
