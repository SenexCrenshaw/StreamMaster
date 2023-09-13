import { hubConnection } from '../../app/signalr';
import { isEmptyObject } from '../../common/common';
import { iptvApi } from '../../store/iptvApi';
import type * as iptv from '../../store/iptvApi';

export const enhancedApiSchedulesDirect = iptvApi.enhanceEndpoints({
  endpoints: {
    schedulesDirectGetCountries: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.Countries) => {
            updateCachedData((draft: iptv.Countries) => {
              draft=data
              return draft;
            });
          };

          hubConnection.off('SchedulesDirectRefresh');
          hubConnection.on('SchedulesDirectRefresh', (data: iptv.Countries) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['SchedulesDirect']));
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
    schedulesDirectGetLineup: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.LineUpResult) => {
            updateCachedData((draft: iptv.LineUpResult) => {
              draft=data
              return draft;
            });
          };

          hubConnection.off('SchedulesDirectRefresh');
          hubConnection.on('SchedulesDirectRefresh', (data: iptv.LineUpResult) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['SchedulesDirect']));
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
    schedulesDirectGetLineupPreviews: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.LineUpPreview[]) => {
            updateCachedData((draft: iptv.LineUpPreview[]) => {
              data.forEach(item => {
                const index = draft.findIndex(existingItem => existingItem.id === item.id);
                if (index !== -1) {
                  draft[index] = item;
                }
              });

              return draft;
            });
          };

          hubConnection.off('SchedulesDirectRefresh');
          hubConnection.on('SchedulesDirectRefresh', (data: iptv.LineUpPreview[]) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['SchedulesDirect']));
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
    schedulesDirectGetLineups: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.LineUpsResult) => {
            updateCachedData((draft: iptv.LineUpsResult) => {
              draft=data
              return draft;
            });
          };

          hubConnection.off('SchedulesDirectRefresh');
          hubConnection.on('SchedulesDirectRefresh', (data: iptv.LineUpsResult) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['SchedulesDirect']));
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
    schedulesDirectGetStationPreviews: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.StationPreview[]) => {
            updateCachedData((draft: iptv.StationPreview[]) => {
              data.forEach(item => {
                const index = draft.findIndex(existingItem => existingItem.id === item.id);
                if (index !== -1) {
                  draft[index] = item;
                }
              });

              return draft;
            });
          };

          hubConnection.off('SchedulesDirectRefresh');
          hubConnection.on('SchedulesDirectRefresh', (data: iptv.StationPreview[]) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['SchedulesDirect']));
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
    schedulesDirectGetStatus: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.SdStatus) => {
            updateCachedData((draft: iptv.SdStatus) => {
              draft=data
              return draft;
            });
          };

          hubConnection.off('SchedulesDirectRefresh');
          hubConnection.on('SchedulesDirectRefresh', (data: iptv.SdStatus) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['SchedulesDirect']));
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
