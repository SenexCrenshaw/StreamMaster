import { hubConnection } from '../../app/signalr';
import { isEmptyObject } from '../../common/common';
import { iptvApi, type PagedResponseOfVideoStreamDto } from '../iptvApi';

export const enhancedApiVideoStreams = iptvApi.enhanceEndpoints({
  endpoints: {
    videoStreamsGetVideoStreams: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          // Function to update cached data with new results
          const updateCachedDataWithResults = (data: PagedResponseOfVideoStreamDto) => {
            updateCachedData((draft: PagedResponseOfVideoStreamDto) => {
              data.data.forEach(videoStream => {
                const index = draft.data.findIndex(existingStream => existingStream.id === videoStream.id);
                if (index !== -1) {
                  draft.data[index] = videoStream;
                }
              });
              return draft;
            });
          };

          // Event listener for VideoStreamsRefresh
          hubConnection.on('VideoStreamsRefresh', (data: PagedResponseOfVideoStreamDto) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(["VideoStreams"]));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          // Logging the error can help with debugging in the future
          console.error("Error in onCacheEntryAdded:", error);
        }

        await cacheEntryRemoved;

        // Removing the event listener
        hubConnection.off('VideoStreamsRefresh');
      }
    },
  }
});
