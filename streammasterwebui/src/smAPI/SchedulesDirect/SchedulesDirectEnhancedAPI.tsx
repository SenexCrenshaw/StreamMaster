import { singletonSchedulesDirectListener } from '../../app/createSingletonListener';
import { isEmptyObject } from '../../common/common';
import isPagedTableDto from '../../components/dataSelector/isPagedTableDto';
import type * as iptv from '../../store/iptvApi';
import { iptvApi } from '../../store/iptvApi';

export const enhancedApiSchedulesDirect = iptvApi.enhanceEndpoints({
  endpoints: {
    schedulesDirectGetCountries: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.Countries) => {
            updateCachedData(() => {
              console.log('updateCachedData', data);
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'SchedulesDirect' }])) {
                if (endpointName !== 'schedulesDirectGetCountries') continue;
                dispatch(iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {
                  console.log('updateCachedData', data, draft);
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
              console.log('updateCachedData', data);
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'SchedulesDirect' }])) {
                if (endpointName !== 'schedulesDirectGetLineup') continue;
                dispatch(iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {
                  console.log('updateCachedData', data, draft);
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
            updateCachedData(() => {
              console.log('updateCachedData', data);
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'SchedulesDirect' }])) {
                if (endpointName !== 'schedulesDirectGetLineupPreviews') continue;
                dispatch(
                  iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {
                    if (isEmptyObject(data)) {
                      console.log('empty', data);
                      dispatch(iptvApi.util.invalidateTags(['SchedulesDirect']));
                      return;
                    }

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
              console.log('updateCachedData', data);
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'SchedulesDirect' }])) {
                if (endpointName !== 'schedulesDirectGetLineups') continue;
                dispatch(iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {
                  console.log('updateCachedData', data, draft);
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
            updateCachedData(() => {
              console.log('updateCachedData', data);
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'SchedulesDirect' }])) {
                if (endpointName !== 'schedulesDirectGetStationPreviews') continue;
                dispatch(
                  iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {
                    if (isEmptyObject(data)) {
                      console.log('empty', data);
                      dispatch(iptvApi.util.invalidateTags(['SchedulesDirect']));
                      return;
                    }

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
              console.log('updateCachedData', data);
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'SchedulesDirect' }])) {
                if (endpointName !== 'schedulesDirectGetStatus') continue;
                dispatch(iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {
                  console.log('updateCachedData', data, draft);
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
