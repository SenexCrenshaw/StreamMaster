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
            updateCachedData((draft: iptv.IconsGetIconApiResponse) => {
              draft=data
              return draft;
            });
          };

          const doIconsGetIconUpdate = (data: iptv.IconFileDto) => {
            // console.log('doIconsGetIconUpdate')
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['Icons']));
            } else {
              updateCachedDataWithResults(data);
            }
          }

          hubConnection.on('IconsRefresh', doIconsGetIconUpdate);

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
            updateCachedData((draft: iptv.IconsGetIconFromSourceApiResponse) => {
              draft=data
              return draft;
            });
          };

          const doIconsGetIconFromSourceUpdate = (data: iptv.IconFileDto) => {
            // console.log('doIconsGetIconFromSourceUpdate')
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['Icons']));
            } else {
              updateCachedDataWithResults(data);
            }
          }

          hubConnection.on('IconsRefresh', doIconsGetIconFromSourceUpdate);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
      }
    },
    iconsGetPagedIcons: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.IconFileDto[]) => {
            updateCachedData((draft: iptv.IconsGetPagedIconsApiResponse) => {
              data.forEach(item => {
                const index = draft.data.findIndex(existingItem => existingItem.id === item.id);
                if (index !== -1) {
                  draft.data[index] = item;
                }
              });

              return draft;
            });
          };

          const doIconsGetPagedIconsUpdate = (data: iptv.IconFileDto[]) => {
            // console.log('doIconsGetPagedIconsUpdate')
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['Icons']));
            } else {
              updateCachedDataWithResults(data);
            }
          }

          hubConnection.on('IconsRefresh', doIconsGetPagedIconsUpdate);

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
            updateCachedData((draft: iptv.IconsGetIconsSimpleQueryApiResponse) => {
              data.forEach(item => {
                const index = draft.findIndex(existingItem => existingItem.id === item.id);
                if (index !== -1) {
                  draft[index] = item;
                }
              });

              return draft;
            });
          };

          const doIconsGetIconsSimpleQueryUpdate = (data: iptv.IconFileDto[]) => {
            // console.log('doIconsGetIconsSimpleQueryUpdate')
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['Icons']));
            } else {
              updateCachedDataWithResults(data);
            }
          }

          hubConnection.on('IconsRefresh', doIconsGetIconsSimpleQueryUpdate);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
      }
    },
  }
});
