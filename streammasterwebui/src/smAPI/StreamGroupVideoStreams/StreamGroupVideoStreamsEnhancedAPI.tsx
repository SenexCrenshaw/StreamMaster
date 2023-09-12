import { hubConnection } from '../../app/signalr';
import { isEmptyObject } from '../../common/common';
import { iptvApi } from '../../store/iptvApi';
import type * as iptv from '../../store/iptvApi';

export const enhancedApiStreamGroupVideoStreams = iptvApi.enhanceEndpoints({
  endpoints: {
    streamGroupVideoStreamsGetStreamGroupVideoStreamIds: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.VideoStreamIsReadOnly[]) => {
            updateCachedData((draft: iptv.VideoStreamIsReadOnly[]) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('StreamGroupVideoStreamsRefresh', (data: iptv.VideoStreamIsReadOnly[]) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['StreamGroupVideoStreams']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
        hubConnection.off('StreamGroupVideoStreamsRefresh');
      }
    },
    streamGroupVideoStreamsGetStreamGroupVideoStreams: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.PagedResponseOfVideoStreamDto) => {
            updateCachedData((draft: iptv.PagedResponseOfVideoStreamDto) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('StreamGroupVideoStreamsRefresh', (data: iptv.PagedResponseOfVideoStreamDto) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['StreamGroupVideoStreams']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
        hubConnection.off('StreamGroupVideoStreamsRefresh');
      }
    },
  }
});
