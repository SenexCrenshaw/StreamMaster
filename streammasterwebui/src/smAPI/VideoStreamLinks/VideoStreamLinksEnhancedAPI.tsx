import { hubConnection } from '../../app/signalr';
import { isEmptyObject } from '../../common/common';
import { iptvApi } from '../../store/iptvApi';
import type * as iptv from '../../store/iptvApi';

export const enhancedApiVideoStreamLinks = iptvApi.enhanceEndpoints({
  endpoints: {
    videoStreamLinksGetVideoStreamVideoStreams: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.PagedResponseOfChildVideoStreamDto) => {
            updateCachedData((draft: iptv.PagedResponseOfChildVideoStreamDto) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('VideoStreamLinksRefresh', (data: iptv.PagedResponseOfChildVideoStreamDto) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['VideoStreamLinks']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
        hubConnection.off('VideoStreamLinksRefresh');
      }
    },
  }
});
