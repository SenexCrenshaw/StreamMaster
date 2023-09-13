import { hubConnection } from '../../app/signalr';
import { isEmptyObject, type IDIsHidden } from '../../common/common';
import type * as iptv from '../../store/iptvApi';
import { iptvApi } from '../../store/iptvApi';

export const enhancedApiVideoStreamsLocal = iptvApi.enhanceEndpoints({
  endpoints: {
    videoStreamsGetVideoStream: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: IDIsHidden[]) => {
            updateCachedData((draft: iptv.VideoStreamDto) => {
              const foundIndex = data.findIndex(existingItem => existingItem.id === draft.id);
              if (foundIndex !== -1) {
                draft.isHidden = data[foundIndex].isHidden;
              }
            });
          };

          hubConnection.off('VideoStreamsVisibilityRefresh');
          hubConnection.on('VideoStreamsVisibilityRefresh', (data: IDIsHidden[]) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['VideoStreams']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
      }
    },
    videoStreamsGetVideoStreams: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: IDIsHidden[]) => {
            updateCachedData((draft: iptv.PagedResponseOfVideoStreamDto) => {
              data.forEach(item => {
                const index = draft.data.findIndex(existingItem => existingItem.id === item.id);
                if (index !== -1) {
                  draft.data[index].isHidden = item.isHidden;
                }
              });

              return draft;
            });
          };

          hubConnection.off('VideoStreamsVisibilityRefresh');
          hubConnection.on('VideoStreamsVisibilityRefresh', (data: IDIsHidden[]) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['VideoStreams']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
      }
    },
  }
});
