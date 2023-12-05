import { isDev } from '@lib/settings';
import { singletonSchedulesDirectListener } from '@lib/signalr/singletonListeners';
import { isEmptyObject } from '@lib/common/common';
import isPagedTableDto from '@lib/common/isPagedTableDto';
import { iptvApi } from '@lib/iptvApi';
import type * as iptv from '@lib/iptvApi';

export const enhancedApiSchedulesDirect = iptvApi.enhanceEndpoints({
  endpoints: {
    schedulesDirectGetPagedStationChannelNameSelections: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.StationChannelName[]) => {
            if (!data || isEmptyObject(data)) {
              if (isDev) console.log('SchedulesDirect Full Refresh');
              dispatch(iptvApi.util.invalidateTags(['SchedulesDirect']));
              return;
            }

            updateCachedData(() => {
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'SchedulesDirect' }])) {
                if (endpointName === 'schedulesDirectGetPagedStationChannelNameSelections') {
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

          singletonSchedulesDirectListener.addListener(updateCachedDataWithResults);

          await cacheEntryRemoved;
          singletonSchedulesDirectListener.removeListener(updateCachedDataWithResults);
        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }
      }
    // eslint-disable-next-line comma-dangle
    },
    schedulesDirectGetStationChannelNameFromDisplayName: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.StationChannelName) => {
            updateCachedData(() => {{
              if (isDev) console.log('updateCachedData', data);
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'SchedulesDirect' }])) {
                if (endpointName !== 'schedulesDirectGetStationChannelNameFromDisplayName') continue;
                  dispatch(iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {{
                    if (isDev) console.log('updateCachedData', data, draft);
                   }})
                   );
                 }}
            });
          };

          singletonSchedulesDirectListener.addListener(updateCachedDataWithResults);

          await cacheEntryRemoved;
          singletonSchedulesDirectListener.removeListener(updateCachedDataWithResults);
        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }
      }
    // eslint-disable-next-line comma-dangle
    },
    schedulesDirectGetStationChannelNamesSimpleQuery: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.StationChannelName[]) => {
            if (!data || isEmptyObject(data)) {
              if (isDev) console.log('SchedulesDirect Full Refresh');
              dispatch(iptvApi.util.invalidateTags(['SchedulesDirect']));
              return;
            }

            updateCachedData(() => {
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'SchedulesDirect' }])) {
                if (endpointName === 'schedulesDirectGetStationChannelNamesSimpleQuery') {
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

          singletonSchedulesDirectListener.addListener(updateCachedDataWithResults);

          await cacheEntryRemoved;
          singletonSchedulesDirectListener.removeListener(updateCachedDataWithResults);
        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }
      }
    // eslint-disable-next-line comma-dangle
    },
    schedulesDirectGetStatus: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.UserStatus) => {
            updateCachedData(() => {{
              if (isDev) console.log('updateCachedData', data);
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'SchedulesDirect' }])) {
                if (endpointName !== 'schedulesDirectGetStatus') continue;
                  dispatch(iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {{
                    if (isDev) console.log('updateCachedData', data, draft);
                   }})
                   );
                 }}
            });
          };

          singletonSchedulesDirectListener.addListener(updateCachedDataWithResults);

          await cacheEntryRemoved;
          singletonSchedulesDirectListener.removeListener(updateCachedDataWithResults);
        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }
      }
    // eslint-disable-next-line comma-dangle
    },
  }
});
