import { isDebug } from '@/lib/settings';
import { singletonSchedulesDirectListener } from '@/lib/signalr/singletonListeners';
import { isEmptyObject } from '@/lib/common/common';
import isPagedTableDto from '@/lib/common/isPagedTableDto';
import { iptvApi } from '@/lib/iptvApi';
import type * as iptv from '@/lib/iptvApi';

export const enhancedApiSchedulesDirect = iptvApi.enhanceEndpoints({
  endpoints: {
    schedulesDirectGetCountries: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.Countries) => {
            updateCachedData(() => {
              if (isDebug) console.log('updateCachedData', data);
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'SchedulesDirect' }])) {
                if (endpointName !== 'schedulesDirectGetCountries') continue;
                  dispatch(iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {
                    if (isDebug) console.log('updateCachedData', data, draft);
                   })
                   );
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
    },
    schedulesDirectGetLineup: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.LineUpResult) => {
            updateCachedData(() => {
              if (isDebug) console.log('updateCachedData', data);
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'SchedulesDirect' }])) {
                if (endpointName !== 'schedulesDirectGetLineup') continue;
                  dispatch(iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {
                    if (isDebug) console.log('updateCachedData', data, draft);
                   })
                   );
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
    },
    schedulesDirectGetLineupPreviews: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.LineUpPreview[]) => {
            if (!data || isEmptyObject(data)) {
              if (isDebug) console.log('empty', data);
              dispatch(iptvApi.util.invalidateTags(['SchedulesDirect']));
              return;
            }

            updateCachedData(() => {
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'SchedulesDirect' }])) {
                if (endpointName !== 'schedulesDirectGetLineupPreviews') continue;
                  dispatch(
                    iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {

                      if (isPagedTableDto(data)) {
                      data.forEach(item => {
                        const index = draft.findIndex(existingItem => existingItem.id === item.id);
                        if (index !== -1) {
                          draft[index] = item;
                        }
                        });

                        return draft;
                        }

                      data.forEach(item => {
                        const index = draft.findIndex(existingItem => existingItem.id === item.id);
                        if (index !== -1) {
                          draft[index] = item;
                        }
                        });

                      return draft;
                     })
                   )
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
    },
    schedulesDirectGetLineups: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.LineUpsResult) => {
            updateCachedData(() => {
              if (isDebug) console.log('updateCachedData', data);
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'SchedulesDirect' }])) {
                if (endpointName !== 'schedulesDirectGetLineups') continue;
                  dispatch(iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {
                    if (isDebug) console.log('updateCachedData', data, draft);
                   })
                   );
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
    },
    schedulesDirectGetStationPreviews: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.StationPreview[]) => {
            if (!data || isEmptyObject(data)) {
              if (isDebug) console.log('empty', data);
              dispatch(iptvApi.util.invalidateTags(['SchedulesDirect']));
              return;
            }

            updateCachedData(() => {
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'SchedulesDirect' }])) {
                if (endpointName !== 'schedulesDirectGetStationPreviews') continue;
                  dispatch(
                    iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {

                      if (isPagedTableDto(data)) {
                      data.forEach(item => {
                        const index = draft.findIndex(existingItem => existingItem.id === item.id);
                        if (index !== -1) {
                          draft[index] = item;
                        }
                        });

                        return draft;
                        }

                      data.forEach(item => {
                        const index = draft.findIndex(existingItem => existingItem.id === item.id);
                        if (index !== -1) {
                          draft[index] = item;
                        }
                        });

                      return draft;
                     })
                   )
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
    },
    schedulesDirectGetStatus: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.SdStatus) => {
            updateCachedData(() => {
              if (isDebug) console.log('updateCachedData', data);
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'SchedulesDirect' }])) {
                if (endpointName !== 'schedulesDirectGetStatus') continue;
                  dispatch(iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {
                    if (isDebug) console.log('updateCachedData', data, draft);
                   })
                   );
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
    },
  }
});
