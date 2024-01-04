import type * as iptv from '@lib/iptvApi';
import { iptvApi } from '@lib/iptvApi';
import { isDev } from '@lib/settings';
import { singletonMiscListener } from '@lib/signalr/singletonListeners';

export const enhancedApiMisc = iptvApi.enhanceEndpoints({
  endpoints: {
    miscGetDownloadServiceStatus: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.ImageDownloadServiceStatus) => {
            updateCachedData(() => {
              {
                if (isDev) console.log('updateCachedData', data);
                if (!data) {
                  dispatch(iptvApi.util.invalidateTags(['Misc']));
                  return;
                }
                for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'Misc' }])) {
                  if (endpointName !== 'miscGetDownloadServiceStatus') continue;
                  dispatch(
                    iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {
                      {
                        if (isDev) console.log('updateCachedData', data, draft);
                      }
                    })
                  );
                }
              }
            });
          };

          singletonMiscListener.addListener(updateCachedDataWithResults);

          await cacheEntryRemoved;
          singletonMiscListener.removeListener(updateCachedDataWithResults);
        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }
      }
      // eslint-disable-next-line comma-dangle
    }
  }
});
