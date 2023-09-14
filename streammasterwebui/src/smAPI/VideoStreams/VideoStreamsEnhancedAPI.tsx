import { hubConnection } from '../../app/signalr';
import { isEmptyObject } from '../../common/common';
import { iptvApi } from '../../store/iptvApi';
import type * as iptv from '../../store/iptvApi';

export const enhancedApiVideoStreams = iptvApi.enhanceEndpoints({
  endpoints: {
    videoStreamsGetChannelLogoDtos: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.ChannelLogoDto[]) => {
            updateCachedData((draft: iptv.VideoStreamsGetChannelLogoDtosApiResponse) => {
              data.forEach(item => {
                const index = draft.findIndex(existingItem => existingItem.id === item.id);
                if (index !== -1) {
                  draft[index] = item;
                }
              });

              return draft;
            });
          };

          const doVideoStreamsGetChannelLogoDtosUpdate = (data: iptv.ChannelLogoDto[]) => {
            // console.log('doVideoStreamsGetChannelLogoDtosUpdate')
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['VideoStreams']));
            } else {
              updateCachedDataWithResults(data);
            }
          }

          hubConnection.on('VideoStreamsRefresh', doVideoStreamsGetChannelLogoDtosUpdate);

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
            updateCachedData((draft: iptv.VideoStreamsGetVideoStreamApiResponse) => {
              draft=data
              return draft;
            });
          };

          const doVideoStreamsGetVideoStreamUpdate = (data: iptv.VideoStreamDto) => {
            // console.log('doVideoStreamsGetVideoStreamUpdate')
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['VideoStreams']));
            } else {
              updateCachedDataWithResults(data);
            }
          }

          hubConnection.on('VideoStreamsRefresh', doVideoStreamsGetVideoStreamUpdate);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
      }
    },
    videoStreamsGetPagedVideoStreams: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.VideoStreamDto[]) => {
            updateCachedData((draft: iptv.VideoStreamsGetPagedVideoStreamsApiResponse) => {
              data.forEach(item => {
                const index = draft.data.findIndex(existingItem => existingItem.id === item.id);
                if (index !== -1) {
                  draft.data[index] = item;
                }
              });

              return draft;
            });
          };

          const doVideoStreamsGetPagedVideoStreamsUpdate = (data: iptv.VideoStreamDto[]) => {
            // console.log('doVideoStreamsGetPagedVideoStreamsUpdate')
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['VideoStreams']));
            } else {
              updateCachedDataWithResults(data);
            }
          }

          hubConnection.on('VideoStreamsRefresh', doVideoStreamsGetPagedVideoStreamsUpdate);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
      }
    },
  }
});
