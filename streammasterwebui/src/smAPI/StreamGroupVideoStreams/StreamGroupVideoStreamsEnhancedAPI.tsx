import { hubConnection } from '../../app/signalr';
import { isEmptyObject } from '../../common/common';
import { iptvApi } from '../../store/iptvApi';
import type * as iptv from '../../store/iptvApi';

export const enhancedApiStreamGroupVideoStreams = iptvApi.enhanceEndpoints({
  endpoints: {
    streamGroupVideoStreamsGetPagedStreamGroupVideoStreams: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.VideoStreamDto[]) => {
            updateCachedData((draft: iptv.StreamGroupVideoStreamsGetPagedStreamGroupVideoStreamsApiResponse) => {
              data.forEach(item => {
                const index = draft.data.findIndex(existingItem => existingItem.id === item.id);
                if (index !== -1) {
                  draft.data[index] = item;
                }
              });

              return draft;
            });
          };

          const doStreamGroupVideoStreamsGetPagedStreamGroupVideoStreamsUpdate = (data: iptv.VideoStreamDto[]) => {
            // console.log('doStreamGroupVideoStreamsGetPagedStreamGroupVideoStreamsUpdate')
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['StreamGroupVideoStreams']));
            } else {
              updateCachedDataWithResults(data);
            }
          }

          hubConnection.on('StreamGroupVideoStreamsRefresh', doStreamGroupVideoStreamsGetPagedStreamGroupVideoStreamsUpdate);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
      }
    },
  }
});
