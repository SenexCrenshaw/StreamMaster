import { isDev } from '@lib/settings';
import { singletonIconsListener } from '@lib/signalr/singletonListeners';
import { isEmptyObject } from '@lib/common/common';
import isPagedTableDto from '@lib/common/isPagedTableDto';
import { iptvApi } from '@lib/iptvApi';
import type * as iptv from '@lib/iptvApi';

export const enhancedApiIcons = iptvApi.enhanceEndpoints({
  endpoints: {
    iconsGetIcon: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.IconFileDto) => {
            updateCachedData(() => {{
              if (isDev) console.log('updateCachedData', data);
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'Icons' }])) {
                if (endpointName !== 'iconsGetIcon') continue;
                  dispatch(iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {{
                    if (isDev) console.log('updateCachedData', data, draft);
                   }})
                   );
                 }}
            });
          };

          singletonIconsListener.addListener(updateCachedDataWithResults);

          await cacheEntryRemoved;
          singletonIconsListener.removeListener(updateCachedDataWithResults);
        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }
      }
    // eslint-disable-next-line comma-dangle
    },
    iconsGetIconFromSource: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.IconFileDto) => {
            updateCachedData(() => {{
              if (isDev) console.log('updateCachedData', data);
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'Icons' }])) {
                if (endpointName !== 'iconsGetIconFromSource') continue;
                  dispatch(iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {{
                    if (isDev) console.log('updateCachedData', data, draft);
                   }})
                   );
                 }}
            });
          };

          singletonIconsListener.addListener(updateCachedDataWithResults);

          await cacheEntryRemoved;
          singletonIconsListener.removeListener(updateCachedDataWithResults);
        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }
      }
    // eslint-disable-next-line comma-dangle
    },
    iconsGetPagedIcons: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.IconFileDto[]) => {
            if (!data || isEmptyObject(data)) {
              if (isDev) console.log('Icons Full Refresh');
              dispatch(iptvApi.util.invalidateTags(['Icons']));
              return;
            }

            updateCachedData(() => {
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'Icons' }])) {
                if (endpointName === 'iconsGetPagedIcons') {
                  dispatch(
                    iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {
                      if (isPagedTableDto(data)) {
                        for (const item of data) {
                          const index = draft.data.findIndex((existingItem) => existingItem.id === item.id);
                          if (index !== -1) {
                            draft.data[index] = item;
                          }
                        }
                        return draft;
                      }
                      for (const item of data) {
                        const index = draft.data.findIndex((existingItem) => existingItem.id === item.id);
                        if (index !== -1) {
                          draft.data[index] = item;
                        }
                      }
                      return draft;
                    })
                  );
                }
              }
            });
          };

          singletonIconsListener.addListener(updateCachedDataWithResults);

          await cacheEntryRemoved;
          singletonIconsListener.removeListener(updateCachedDataWithResults);
        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }
      }
    // eslint-disable-next-line comma-dangle
    },
    iconsGetIconsSimpleQuery: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.IconFileDto[]) => {
            if (!data || isEmptyObject(data)) {
              if (isDev) console.log('Icons Full Refresh');
              dispatch(iptvApi.util.invalidateTags(['Icons']));
              return;
            }

            updateCachedData(() => {
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'Icons' }])) {
                if (endpointName === 'iconsGetIconsSimpleQuery') {
                  dispatch(
                    iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {
                      if (isPagedTableDto(data)) {
                        for (const item of data) {
                          const index = draft.findIndex((existingItem) => existingItem.id === item.id);
                          if (index !== -1) {
                            draft[index] = item;
                          }
                        }
                        return draft;
                      }
                      for (const item of data) {
                        const index = draft.findIndex((existingItem) => existingItem.id === item.id);
                        if (index !== -1) {
                          draft[index] = item;
                        }
                      }
                      return draft;
                    })
                  );
                }
              }
            });
          };

          singletonIconsListener.addListener(updateCachedDataWithResults);

          await cacheEntryRemoved;
          singletonIconsListener.removeListener(updateCachedDataWithResults);
        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }
      }
    // eslint-disable-next-line comma-dangle
    },
  }
});
