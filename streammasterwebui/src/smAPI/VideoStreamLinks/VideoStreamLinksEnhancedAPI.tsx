import { hubConnection } from '../../app/signalr';
import { isEmptyObject } from '../../common/common';
import { iptvApi } from '../../store/iptvApi';
import type * as iptv from '../../store/iptvApi';

export const enhancedApiVideoStreamLinks = iptvApi.enhanceEndpoints({
  endpoints: {
    videoStreamLinksGetPagedVideoStreamVideoStreams: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.ChildVideoStreamDto[]) => {
            updateCachedData((draft: iptv.VideoStreamLinksGetPagedVideoStreamVideoStreamsApiResponse) => {
              data.forEach(item => {
                const index = draft.data.findIndex(existingItem => existingItem.id === item.id);
                if (index !== -1) {
                  draft.data[index] = item;
                }
              });

              return draft;
            });
          };

          const doVideoStreamLinksGetPagedVideoStreamVideoStreamsUpdate = (data: iptv.ChildVideoStreamDto[]) => {
            // console.log('doVideoStreamLinksGetPagedVideoStreamVideoStreamsUpdate')
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['VideoStreamLinks']));
            } else {
              updateCachedDataWithResults(data);
            }
          }

          hubConnection.on('VideoStreamLinksRefresh', doVideoStreamLinksGetPagedVideoStreamVideoStreamsUpdate);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
      }
    },
  }
});
