import { hubConnection } from '../../app/signalr';
import { isEmptyObject } from '../../common/common';
import type * as iptv from '../../store/iptvApi';
import { iptvApi } from '../../store/iptvApi';

export const enhancedApiVideoStreams = iptvApi.enhanceEndpoints({
  endpoints: {
    videoStreamsGetChannelLogoDtos: {

      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.ChannelLogoDto[]) => {
            updateCachedData((draft: iptv.ChannelLogoDto[]) => {
              data.forEach(item => {
                const index = draft.findIndex(existingItem => existingItem.id === item.id);
                if (index !== -1) {
                  draft[index] = item;
                }
              });

              return draft;
            });
          };

          hubConnection.off('VideoStreamsRefresh');
          hubConnection.on('VideoStreamsRefresh', (data: iptv.ChannelLogoDto[]) => {
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
    videoStreamsGetVideoStream: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.VideoStreamDto) => {
            updateCachedData((draft: iptv.VideoStreamDto) => {
              draft = data
              return draft;
            });
          };

          hubConnection.off('VideoStreamsRefresh');
          hubConnection.on('VideoStreamsRefresh', (data: iptv.VideoStreamDto) => {
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

          const updateCachedDataWithResults = (data: iptv.VideoStreamDto[]) => {
            updateCachedData((draft: iptv.PagedResponseOfVideoStreamDto) => {
              data.forEach(item => {
                const index = draft.data.findIndex(existingItem => existingItem.id === item.id);
                if (index !== -1) {
                  draft.data[index] = item;
                }
              });

              return draft;
            });
          };

          hubConnection.off('VideoStreamsRefresh');
          hubConnection.on('VideoStreamsRefresh', (data: iptv.VideoStreamDto[]) => {
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
