import { hubConnection } from '../../app/signalr';
import { isEmptyObject } from '../../common/common';
import { iptvApi } from '../../store/iptvApi';
import type * as iptv from '../../store/iptvApi';

export const enhancedApiEpgFiles = iptvApi.enhanceEndpoints({
  endpoints: {
    epgFilesGetEpgFile: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.EpgFileDto) => {
            updateCachedData((draft: iptv.EpgFileDto) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('EPGFilesRefresh', (data: iptv.EpgFileDto) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['EPGFiles']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
        hubConnection.off('EPGFilesRefresh');
      }
    },
    epgFilesGetEpgFiles: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.PagedResponseOfEpgFileDto) => {
            updateCachedData((draft: iptv.PagedResponseOfEpgFileDto) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('EPGFilesRefresh', (data: iptv.PagedResponseOfEpgFileDto) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['EPGFiles']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
        hubConnection.off('EPGFilesRefresh');
      }
    },
  }
});
