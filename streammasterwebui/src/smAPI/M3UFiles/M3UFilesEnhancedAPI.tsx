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

          hubConnection.off('M3UFilesRefresh');
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
      }
    },
    m3UFilesGetM3UFiles: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.M3UFileDto[]) => {
            updateCachedData((draft: iptv.PagedResponseOfM3UFileDto) => {
              data.forEach(item => {
                const index = draft.data.findIndex(existingItem => existingItem.id === item.id);
                if (index !== -1) {
                  draft.data[index] = item;
                }
              });

              return draft;
            });
          };

          hubConnection.off('M3UFilesRefresh');
          hubConnection.on('M3UFilesRefresh', (data: iptv.M3UFileDto[]) => {
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
      }
    },
  }
});
