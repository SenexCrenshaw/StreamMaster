import { isDev } from '@/lib/settings';
import { singletonStreamGroupVideoStreamsListener } from '@/lib/signalr/singletonListeners';
import { isEmptyObject } from '@/lib/common/common';
import isPagedTableDto from '@/lib/common/isPagedTableDto';
import { iptvApi } from '@/lib/iptvApi';
import type * as iptv from '@/lib/iptvApi';

export const enhancedApiStreamGroupVideoStreams = iptvApi.enhanceEndpoints({
  endpoints: {
    streamGroupVideoStreamsGetPagedStreamGroupVideoStreams: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.VideoStreamDto[]) => {
            if (!data || isEmptyObject(data)) {
              if (isDev) console.log('StreamGroupVideoStreams Full Refresh');
              dispatch(iptvApi.util.invalidateTags(['StreamGroupVideoStreams']));
              return;
            }

            updateCachedData(() => {
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'StreamGroupVideoStreams' }])) {
                if (endpointName === 'streamGroupVideoStreamsGetPagedStreamGroupVideoStreams') {
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

          singletonStreamGroupVideoStreamsListener.addListener(updateCachedDataWithResults);

          await cacheEntryRemoved;
          singletonStreamGroupVideoStreamsListener.removeListener(updateCachedDataWithResults);
        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }
      }
    // eslint-disable-next-line comma-dangle
    },
  }
});
