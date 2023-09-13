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

          hubConnection.off('IconsRefresh');
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

          hubConnection.off('IconsRefresh');
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
      }
    },
    iconsGetIcons: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.IconFileDto[]) => {
            updateCachedData((draft: iptv.PagedResponseOfIconFileDto) => {
              data.forEach(item => {
                const index = draft.data.findIndex(existingItem => existingItem.id === item.id);
                if (index !== -1) {
                  draft.data[index] = item;
                }
              });

              return draft;
            });
          };

          hubConnection.off('IconsRefresh');
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
      }
    },
    iconsGetIconsSimpleQuery: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.IconFileDto[]) => {
            updateCachedData((draft: iptv.IconFileDto[]) => {
              data.forEach(item => {
                const index = draft.findIndex(existingItem => existingItem.id === item.id);
                if (index !== -1) {
                  draft[index] = item;
                }
              });

              return draft;
            });
          };

          hubConnection.off('IconsRefresh');
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
      }
    },
  }
});
