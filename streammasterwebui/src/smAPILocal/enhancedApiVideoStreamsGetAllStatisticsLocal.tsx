import { singletonStatisticListener } from '../app/createSingletonListener';
import { iptvApi, type StreamStatisticsResult } from '../store/iptvApi';

export const enhancedApiVideoStreamsGetAllStatisticsLocal = iptvApi.enhanceEndpoints({
  endpoints: {
    videoStreamsGetAllStatisticsForAllUrls: {
      async onCacheEntryAdded(arg, { updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: StreamStatisticsResult[]) => {
            updateCachedData(
              () => {
                return data;
              }
            )
          };

          singletonStatisticListener.addListener(updateCachedDataWithResults);

          await cacheEntryRemoved;
          singletonStatisticListener.removeListener(updateCachedDataWithResults);


        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

      }
    },
  }
});
