import { hubConnection } from '../../app/signalr';
import { isEmptyObject } from '../../common/common';
import { iptvApi } from '../../store/iptvApi';
import type * as iptv from '../../store/iptvApi';

export const enhancedApiM3UFiles = iptvApi.enhanceEndpoints({
  endpoints: {
    m3UFilesGetM3UFile: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.M3UFileDto) => {
            updateCachedData((draft: iptv.M3UFileDto) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('M3UFilesRefresh', (data: iptv.M3UFileDto) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['M3UFiles']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
        hubConnection.off('M3UFilesRefresh');
      }
    },
    m3UFilesGetM3UFiles: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.PagedResponseOfM3UFileDto) => {
            updateCachedData((draft: iptv.PagedResponseOfM3UFileDto) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('M3UFilesRefresh', (data: iptv.PagedResponseOfM3UFileDto) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['M3UFiles']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
        hubConnection.off('M3UFilesRefresh');
      }
    },
  }
});
