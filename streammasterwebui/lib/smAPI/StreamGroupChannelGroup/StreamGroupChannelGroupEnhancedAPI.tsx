import { isDev } from '@/lib/settings';
import { singletonStreamGroupChannelGroupListener } from '@/lib/signalr/singletonListeners';
import { isEmptyObject } from '@/lib/common/common';
import isPagedTableDto from '@/lib/common/isPagedTableDto';
import { iptvApi } from '@/lib/iptvApi';
import type * as iptv from '@/lib/iptvApi';

export const enhancedApiStreamGroupChannelGroup = iptvApi.enhanceEndpoints({
  endpoints: {
    streamGroupChannelGroupGetChannelGroupsFromStreamGroup: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.ChannelGroupDto[]) => {
            if (!data || isEmptyObject(data)) {
              if (isDev) console.log('StreamGroupChannelGroup Full Refresh');
              dispatch(iptvApi.util.invalidateTags(['StreamGroupChannelGroup']));
              return;
            }

            updateCachedData(() => {
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'StreamGroupChannelGroup' }])) {
                if (endpointName === 'streamGroupChannelGroupGetChannelGroupsFromStreamGroup') {
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

          singletonStreamGroupChannelGroupListener.addListener(updateCachedDataWithResults);

          await cacheEntryRemoved;
          singletonStreamGroupChannelGroupListener.removeListener(updateCachedDataWithResults);
        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }
      }
    // eslint-disable-next-line comma-dangle
    },
  }
});
