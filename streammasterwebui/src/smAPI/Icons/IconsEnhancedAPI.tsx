import { hubConnection } from '../../app/signalr';
import { isEmptyObject } from '../../common/common';
import { iptvApi } from '../../store/iptvApi';
import type * as iptv from '../../store/iptvApi';

export const enhancedApiIcons = iptvApi.enhanceEndpoints({
  endpoints: {
    iconsGetIcon: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.IconFileDto) => {
            updateCachedData((draft: iptv.IconFileDto) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('IconsRefresh', (data: iptv.IconFileDto) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['Icons']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
        hubConnection.off('IconsRefresh');
      }
    },
    iconsGetIconFromSource: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.IconFileDto) => {
            updateCachedData((draft: iptv.IconFileDto) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('IconsRefresh', (data: iptv.IconFileDto) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['Icons']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
        hubConnection.off('IconsRefresh');
      }
    },
    iconsGetIcons: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.PagedResponseOfIconFileDto) => {
            updateCachedData((draft: iptv.PagedResponseOfIconFileDto) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('IconsRefresh', (data: iptv.PagedResponseOfIconFileDto) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['Icons']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
        hubConnection.off('IconsRefresh');
      }
    },
    iconsGetIconsSimpleQuery: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.IconFileDto[]) => {
            updateCachedData((draft: iptv.IconFileDto[]) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('IconsRefresh', (data: iptv.IconFileDto[]) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['Icons']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
        hubConnection.off('IconsRefresh');
      }
    },
  }
});
