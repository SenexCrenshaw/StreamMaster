import { hubConnection } from '../../app/signalr';
import { isEmptyObject } from '../../common/common';
import { iptvApi } from '../../store/iptvApi';
import type * as iptv from '../../store/iptvApi';

export const enhancedApiVideoStreams = iptvApi.enhanceEndpoints({
  endpoints: {
    videoStreamsGetAllStatisticsForAllUrls: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.StreamStatisticsResult[]) => {
            updateCachedData((draft: iptv.StreamStatisticsResult[]) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('VideoStreamsRefresh', (data: iptv.StreamStatisticsResult[]) => {
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
        hubConnection.off('VideoStreamsRefresh');
      }
    },
    videoStreamsGetChannelLogoDtos: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.ChannelLogoDto[]) => {
            updateCachedData((draft: iptv.ChannelLogoDto[]) => {
              draft=data
              return draft;
            });
          };

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
        hubConnection.off('VideoStreamsRefresh');
      }
    },
    videoStreamsGetVideoStream: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.VideoStreamDto) => {
            updateCachedData((draft: iptv.VideoStreamDto) => {
              draft=data
              return draft;
            });
          };

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
        hubConnection.off('VideoStreamsRefresh');
      }
    },
    videoStreamsGetVideoStreams: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.PagedResponseOfVideoStreamDto) => {
            updateCachedData((draft: iptv.PagedResponseOfVideoStreamDto) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('VideoStreamsRefresh', (data: iptv.PagedResponseOfVideoStreamDto) => {
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
        hubConnection.off('VideoStreamsRefresh');
      }
    },
  }
});
